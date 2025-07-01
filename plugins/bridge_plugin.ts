import { defineNuxtPlugin } from '#app'
import { bridge } from '~/composables/service/bridge'

const bridgeInstance = new bridge()

export default defineNuxtPlugin(() => {
  console.log('Bridge Plugin')
  return {
    provide: {
      bridge: bridgeInstance as bridge
    }
  }
})