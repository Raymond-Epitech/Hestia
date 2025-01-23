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
    clientId: '80772791160-407a5d395k8o0h5v1ao3shvjog0m68ul.apps.googleusercontent.com',
  }
})
