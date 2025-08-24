<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"></meta>
<template>
  <div>
    <div class="body-container" style="padding: env(safe-area-inset-top) env(safe-area-inset-right) env(safe-area-inset-bottom) env(safe-area-inset-left);">
      <slot />
    </div>
    <footer>
      <Navbar />
    </footer>
  </div>

</template>

<script setup>
import { storeToRefs } from 'pinia';
import { useUserStore } from '~/store/user';
import { StatusBar } from '@capacitor/status-bar';

const userStore = useUserStore();
const { $signalr, $signalrReady } = useNuxtApp()

StatusBar.setOverlaysWebView({ overlay: false });

onMounted(async () => {
  await $signalrReady
  if (userStore.user?.colocationId) {
    $signalr.invoke("JoinColocationGroup", userStore.user.colocationId)
      .then(() => console.log("Demande envoyée au hub"))
      .catch(err => console.error("Erreur lors de l'envoi", err))
  }
})
</script>

<style scoped>
.body-container {
  position: absolute;
  /* border-top: 1px solid #f3f3f3;
  top: 25pt;
  bottom: 58px; */
  left: 0;
  right: 0;
  overflow: auto;
}

.dark .body-container {
  border-top: 1px solid #333333;
}
</style>
