<template>
  <div>
    <div>
    </div>
    <div>
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
