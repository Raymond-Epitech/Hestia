import { defineNuxtPlugin } from '#app'
import { bridge } from '~/composables/service/bridge' // Chemin correct vers bridge.ts

export default defineNuxtPlugin(() => {
  const bridgeInstance = new bridge()

  return {
    provide: {
      bridge: bridgeInstance
    }
  }
})