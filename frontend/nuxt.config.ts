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
    clientId: "80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com",
  }
})
