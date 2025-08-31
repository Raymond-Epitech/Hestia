<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"></meta>
<template>
  <div>
    <div class="body-container">
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

StatusBar.setOverlaysWebView({ overlay: true });

onMounted(async () => {
  await $signalrReady
  if ((await StatusBar.getInfo()).visible) {
    await StatusBar.hide();
  }
  await StatusBar.hide();
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
  left: 0;
  right: 0;
  overflow: auto;
}

.dark .body-container {
  border-top: 1px solid #333333;
}
</style>
