import { defineNuxtConfig } from 'nuxt/config'

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  modules: [
    '@pinia/nuxt',
    'nuxt-vue3-google-signin'
  ],
  css: [
    '~/assets/styles/main.scss',
  ],
  plugins: [
    { src: '~/plugins/bootstrap.js', mode: 'client' },
  ],
  googleSignIn: {
    clientId: "80772791160-i0v9t46ic6cuqj38u6g5gcs0kh7c6opk.apps.googleusercontent.com",
  }
})
