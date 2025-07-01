import { bridge } from '~/composables/service/bridge'

declare module '#app' {
  interface NuxtApp {
    $bridge: bridge
  }
}

declare module 'vue' {
  interface ComponentCustomProperties {
    $bridge: bridge
  }
}