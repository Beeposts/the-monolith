// https://nuxt.com/docs/api/configuration/nuxt-config



import { defineNuxtConfig } from 'nuxt/config'
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  sourcemap: true,
  devtools: {enabled: true},
  devServer:{
    https:{
      cert: './dev-certs/localhost.pem',
      key: './dev-certs/localhost.key'
    }
  },
  modules: ['@nuxt/eslint', 'nuxt-oidc-auth'],
  oidc: {
    
    defaultProvider: 'oidc',
    enabled: true,
    
    middleware: {
      globalMiddlewareEnabled: false,
    },
    providers: {
      oidc: {
        redirectUri: 'https://localhost:3000/auth/oidc/callback',
        clientId: 'interactive',
        clientSecret: '49C1A7E1-0C79-4A89-A3D6-A37998FB86B0',
        audience: 'api',
        scope: ['openid', 'profile', 'api'],
        authorizationUrl: 'https://localhost:5001/connect/authorize',
        tokenUrl: 'https://localhost:5001/connect/token',
        userinfoUrl: 'https://localhost:5001/connect/userinfo',
        logoutUrl: 'https://localhost:5001/connect/endsession',
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
  }  
})