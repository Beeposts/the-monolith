/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly OIDC_REDIRECT_URI: string
    readonly OIDC_CLIENT_ID: string
    readonly OIDC_CLIENT_SECRET: string
    readonly OIDC_ISSUER: string
}

interface ImportMeta {
    readonly env: ImportMetaEnv
}
