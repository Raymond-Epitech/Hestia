<template>
    <div>
    </div>
    <div>
        <slot />
    </div>
    <div>
      <Navbar/>
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
