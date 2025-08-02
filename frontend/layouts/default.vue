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

const userStore = useUserStore();
const { $signalr } = useNuxtApp()

onMounted(() => {
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
  border-top: 1px solid #d2edd9;
  top: 25pt;
  bottom: 58px;
  left: 0;
  right: 0;
  overflow: auto;
}
</style>
