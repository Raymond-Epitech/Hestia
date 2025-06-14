import { defineNuxtConfig } from 'nuxt/config'

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },

  modules: [
    '@pinia/nuxt',
    'nuxt-vue3-google-signin',
    '@nuxtjs/i18n',
    'pinia-plugin-persistedstate/nuxt',
    '@nuxtjs/ionic',
  ],

  css: [
    '~/assets/styles/main.scss',
    '@/assets/styles/fonts.scss'
  ],

  plugins: [
    { src: '~/plugins/bootstrap.js', mode: 'client' },
  ],

  googleSignIn: {
    // clientId: "80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com",
    clientId: "80772791160-mlnmccfg78eafb49as0bfkjgqmijjmcc.apps.googleusercontent.com",
  },

  extends: ['nuxt-modal'],

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

  compatibilityDate: '2025-02-21',
})