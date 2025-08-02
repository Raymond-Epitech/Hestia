<template>
  <div>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const { $signalr } = useNuxtApp()

onMounted(() => {
  $signalr.invoke("JoinColocationGroup", userStore.user.colocationId)
    .then(() => console.log("Demande envoyée au hub"))
    .catch(err => console.error("Erreur lors de l'envoi", err));
})

</script>