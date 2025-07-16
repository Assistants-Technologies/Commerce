import { loadEnv, defineConfig } from '@medusajs/framework/utils'
import { join } from 'path'

loadEnv(process.env.ENVIRONMENT!.toLowerCase(), process.cwd())

const certDir = join(__dirname, "certs")

module.exports = defineConfig({
  projectConfig: {
    databaseUrl: process.env.COMMERCE_DATABASE_URL,
    redisUrl: process.env.COMMERCE_REDIS_URL,
    http: {
      storeCors: process.env.COMMERCE_STORE_CORS!,
      adminCors: process.env.COMMERCE_ADMIN_CORS!,
      authCors: process.env.COMMERCE_AUTH_CORS!,
      jwtSecret: process.env.COMMERCE_JWT_SECRET!,
      cookieSecret: process.env.COMMERCE_COOKIE_SECRET!,
    }
  },
  modules: [
    {
      resolve: "@medusajs/payment",
      options: {
        providers: [
          {
            resolve: "@medusajs/payment-stripe",
            id: "stripe",
            options: {
              apiKey: process.env.COMMERCE_STRIPE_API_KEY,
            },
          },
        ],
      }
    },
    {
      resolve: "@medusajs/medusa/auth",
      options: {
        providers: [
          {
            resolve: "@medusajs/medusa/auth-emailpass",
            id: "emailpass",
          },
          {
            resolve: join(__dirname, "./src/modules/openid"),
            id: "openid",
            options: {
              issuer_url: process.env.IDP_ISSUER,
              client_id: process.env.COMMERCE_CLIENT_ID,
              client_secret: process.env.COMMERCE_CLIENT_SECRET,
              redirect_uri: process.env.COMMERCE_REDIRECT_URI,
            },
          },
        ],
      },
    }
  ],
  admin: {
    vite: () => {
      return {
        server: {
          allowedHosts: process.env.COMMERCE_ALLOWED_HOSTS.split(','),
        },
      }
    },
  },
})
