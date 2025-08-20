import { defineNuxtPlugin } from '#app'
import { bridge } from '~/composables/service/bridge'

const bridgeInstance = new bridge()

export default defineNuxtPlugin(() => {
  return {
    provide: {
      bridge: bridgeInstance as bridge
    }
  }
})