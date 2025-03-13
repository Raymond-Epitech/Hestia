import { defineNuxtPlugin } from '#app'
import { bridge } from '~/composables/service/bridge'

export default defineNuxtPlugin(() => {
  const bridgeInstance = new bridge()

  return {
    provide: {
      bridge: bridgeInstance as bridge
    }
  }
})