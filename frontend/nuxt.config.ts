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
    '@nuxtjs/color-mode',
  ],

  css: [
    '~/assets/styles/main.scss',
    '@/assets/styles/fonts.scss'
  ],

  plugins: [
    { src: '~/plugins/bootstrap.js', mode: 'client' },
    { src: '~/plugins/vue-datepicker', ssr: false },
  ],

  googleSignIn: {
    clientId: "80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com",
  },

  colorMode: {
    preference: 'system', // default value of $colorMode.preference
    fallback: 'light', // fallback value if not system preference found
    hid: 'nuxt-color-mode-script',
    globalName: '__NUXT_COLOR_MODE__',
    componentName: 'ColorScheme',
    classPrefix: '',
    classSuffix: '',
    storage: 'localStorage', // or 'sessionStorage' or 'cookie'
    storageKey: 'nuxt-color-mode',
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
  ssr: false,

  runtimeConfig: {
    public: {
      signalrUrl: 'https://hestiaapp.org/hestiaHub'
    }
  },

  compatibilityDate: '2025-06-27',
})