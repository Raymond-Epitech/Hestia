import { defineNuxtConfig } from 'nuxt/config'

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  modules: [
    '@pinia/nuxt',
    'nuxt-vue3-google-signin',
    '@nuxtjs/i18n'
  ],
  css: [
    '~/assets/styles/main.scss',
  ],
  plugins: [
    { src: '~/plugins/bootstrap.js', mode: 'client' },
  ],
  googleSignIn: {
    clientId: '80772791160-407a5d395k8o0h5v1ao3shvjog0m68ul.apps.googleusercontent.com',
  },
  i18n: {
    locales: [
      { code: 'fr', name: 'Français', file: 'fr.json' },
      { code: 'en', name: 'Anglais', file: 'en.json' },
      { code: 'es', name: 'Español', file: 'es.json' },
      { code: 'de', name: 'Allemand', file: 'de.json' },
      { code: 'zh', name: 'Mandarin', file: 'zh.json' },
      { code: 'ja', name: 'Japonais', file: 'ja.json' },
    ],
    lazy: true,
    langDir: 'locales/',
    defaultLocale: 'fr',
    strategy: 'no_prefix'
  },
})
