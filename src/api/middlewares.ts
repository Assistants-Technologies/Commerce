import {
    defineMiddlewares,
    MedusaNextFunction,
    MedusaRequest,
    MedusaResponse,
} from "@medusajs/framework/http"
import jwksClient from "jwks-rsa"


/// 1) Skonfiguruj klienta JWKS
const client = jwksClient({
    jwksUri: `${process.env.IDP_ISSUER}/.well-known/openid-configuration/jwks`,
    cache: true,
    rateLimit: true,
    jwksRequestsPerMinute: 5,
})
const getKey: jwt.SecretCallback = (header, cb) => {
    client.getSigningKey(header.kid, (err, key) => {
        if (err) return cb(err as any, null as any)
        const pub = (key as any).getPublicKey()
        cb(null, pub)
    })
}

export default defineMiddlewares({
    routes: [
        {
            matcher: "**",
            middlewares: [
                (
                    req: MedusaRequest,
                    res: MedusaResponse,
                    next: MedusaNextFunction
                ) => {
                    const auth = req.headers.authorization
                    console.log(auth)
                    req.session.user = { id: 123 }
                    next()
                },
            ],
        },
    ],
})