// https://nuxt.com/docs/api/configuration/nuxt-config

import { defineNuxtConfig } from 'nuxt/config'
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  sourcemap: true,
  devtools: { enabled: true },
  devServer: {
    https: {
      cert: './dev-certs/localhost.pem',
      key: './dev-certs/localhost.key',
    },
  },
  modules: ['@nuxt/eslint', 'nuxt-oidc-auth', '@nuxtjs/tailwindcss', '@nuxtjs/google-fonts'],
  oidc: {
    defaultProvider: 'oidc',
    enabled: true,

    middleware: {
      globalMiddlewareEnabled: false,
    },
    providers: {
      oidc: {
        redirectUri: `${process.env.OIDC_REDIRECT_URI}`,
        clientId: process.env.OIDC_CLIENT_ID!,
        clientSecret: process.env.OIDC_CLIENT_SECRET!,
        audience: 'api',
        scope: ['openid', 'profile', 'api'],
        authorizationUrl: `${process.env.OIDC_ISSUER}/connect/authorize`,
        tokenUrl: `${process.env.OIDC_ISSUER}/connect/token`,
        userinfoUrl: `${process.env.OIDC_ISSUER}/connect/userinfo`,
        logoutUrl: `${process.env.OIDC_ISSUER}/connect/endsession`,
        authenticationScheme: 'body',
        grantType: 'authorization_code',
        pkce: true,
        state: true,
        nonce: true,
        tokenRequestType: 'form-urlencoded',
        validateAccessToken: false,
        validateIdToken: false,
        exposeAccessToken: true,
      },
    },
  },
})
