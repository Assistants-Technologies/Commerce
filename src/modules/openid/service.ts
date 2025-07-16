import { MedusaError } from "@medusajs/framework"
import { AbstractAuthModuleProvider } from "@medusajs/framework/utils"
import { CustomerService } from "@medusajs/medusa/dist/services/customer"
import { jwtDecode } from "jwt-decode";
import * as client from "openid-client"
import {
    AuthIdentityProviderService,
    AuthenticationInput,
    AuthenticationResponse,
} from "@medusajs/types"
declare global {
    interface String {
        segment(pos: number): string | undefined
    }
}
String.prototype.segment = function (pos: number): string | undefined {
    const path = this.split("?")[0]
    const parts = path.split("/")
    return parts[pos]
}
export default class OpenIdProviderService extends AbstractAuthModuleProvider {
    static identifier = "openid"
    static DISPLAY_NAME = process.env.COMMERCE_CLIENT_DISPLAY_NAME || "SSO Login"
    protected client
    private config: any
    private states = new Map<string, string>()
    constructor(
        container: any,
        protected options_: {
            issuer_url: string
            client_id: string
            client_secret: string
            redirect_uri: string
            scope?: string
        }
    ) {
        super(container, options_)

        const { issuer_url, client_id, client_secret, redirect_uri } = options_
        if (!issuer_url || !client_id || !client_secret || !redirect_uri) {
            throw new MedusaError(
                MedusaError.Types.INVALID_DATA,
                "Missing required OpenID options: issuer_url, client_id, client_secret, redirect_uri"
            )
        }
    }
    private async getConfig(): Promise<client.Configuration> {
        if (!this.config) {
            this.config = await client.discovery(
                new URL(this.options_.issuer_url),
                this.options_.client_id,
                this.options_.client_secret
            )
        }
        return this.config
    }

    async authenticate(
        data: AuthenticationInput
    ): Promise<AuthenticationResponse> {
        const cfg = await this.getConfig()
        const state = crypto.randomUUID()
        const actorId = data.url.segment(2)
        this.states.set(actorId, state)

        setTimeout(() => this.states.delete(actorId), 60000);
        const redirectUrl = client.buildAuthorizationUrl(cfg, {
            redirect_uri: this.options_.redirect_uri,
            scope: this.options_.scope || "openid email profile",
            state,
        })
        return {
            success: true,
            location: redirectUrl,
        }
    }

    async validateCallback(
        data: AuthenticationInput,
        authIdentityProviderService: AuthIdentityProviderService
    ): Promise<AuthenticationResponse> {
        const cfg = await this.getConfig()
        const { code, state: returnedState } = data.query
        const actorId = data.url.segment(2)
        const saved = this.states.get(actorId)
        if (!saved || saved !== returnedState) {
            return { success: false, error: "Invalid state or missing state parameter." }
        }
        this.states.delete(actorId)

        const redirect_uri = this.options_.redirect_uri

        let tokenSet: any;
        try {
            tokenSet = await client.authorizationCodeGrant(
                cfg,
                new URL(`${redirect_uri}?code=${code}&state=${returnedState}&iss=${encodeURI("https://identity.assts.tech/")}`),
                {
                    expectedState: returnedState
                }
            )
        } catch (error) {
            console.error("Token exchange error:", error)
            return {
                success: false,
                error: "Failed to exchange authorization code for tokens"
            }
        }
        const idToken = jwtDecode(tokenSet.id_token);
        const expectedSubject = idToken.sub as string;

        const userinfo = await client.fetchUserInfo(cfg, tokenSet.access_token, expectedSubject)
        const sub = userinfo.sub
        const email = userinfo.email || `${sub}@no-mail.local`
        const name = userinfo.name || ""
        if (!sub) {
            return { success: false, error: "Invalid token payload: missing sub." }
        }
        let authIdentity
        try {
            authIdentity = await authIdentityProviderService.retrieve({
                entity_id: sub,
                provider: this.identifier,
            })
        } catch {
            authIdentity = await authIdentityProviderService.create({
                entity_id: sub,
                provider: this.identifier,
                provider_metadata: { email },
                user_metadata: { email, name },
            })
        }

        // Pobieramy customerService z kontenera
        const customerService = this.container_.resolve<CustomerService>("customerService")

        try {
            // Sprawdź czy klient już istnieje
            await customerService.retrieve(authIdentity.user_id)
        } catch (error) {
            // Jeśli nie istnieje, utwórz nowego klienta
            const nameParts = name ? name.split(" ") : []
            await customerService.create({
                id: authIdentity.user_id,
                email: email,
                first_name: nameParts[0] || undefined,
                last_name: nameParts.length > 1 ? nameParts.slice(1).join(" ") : undefined,
            })
        }
        return {
            success: true,
            authIdentity,
        }
    }
}