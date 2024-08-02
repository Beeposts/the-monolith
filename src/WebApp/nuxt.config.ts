// https://nuxt.com/docs/api/configuration/nuxt-config

import { defineNuxtConfig } from 'nuxt/config'
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  sourcemap: true,
  devtools: {enabled: true},
  devServer:{
    https:{
      cert: 'localhost.pem',
      key: 'localhost-key.pem'
    }
  },
  modules: ['@nuxt/eslint', '']
})