export default defineNuxtPlugin((nuxtApp) => {
  if (process.client) {
    const savedLocale = localStorage.getItem('locale')
    console.log('savedLocale', savedLocale)
    if (savedLocale) {
      nuxtApp.$i18n.setLocale(savedLocale)
    } else {
      localStorage.setItem('locale', nuxtApp.$i18n.locale)
    }
  }
})