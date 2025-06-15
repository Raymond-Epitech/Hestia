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
  top: 20px;
  bottom: 58px;
  left: 0;
  right: 0;
  overflow: auto;
}
</style>
