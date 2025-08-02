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
import { useAuthStore } from '../store/auth';
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const { $signalr } = useNuxtApp()

onMounted(() => {
  $signalr.invoke("JoinColocationGroup", userStore.user.colocationId)
    .then(() => console.log("Demande envoyée au hub"))
    .catch(err => console.error("Erreur lors de l'envoi", err));
})

const router = useRouter();

const { logUserOut } = useAuthStore();
const { authenticated } = storeToRefs(useAuthStore());

const logout = () => {
  logUserOut();
  router.push('/login');
};
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
