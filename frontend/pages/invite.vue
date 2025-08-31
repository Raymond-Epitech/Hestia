<template>
  <div>
    <p>Redirection en cours...</p>
  </div>
</template>

<script setup lang="ts">
import { useRouter, useRoute } from 'vue-router';
import { useUserStore } from '~/store/user';

const router = useRouter();
const route = useRoute();
const userStore = useUserStore();

const collocID = route.query.collocID as string;
const islogin = useCookie('token').value ?? ''

if (!islogin) {
  router.push({ path: '/login', query: { collocID: collocID } });
} else if (userStore.user.colocationId) {
  router.push({ path: '/settings/change-colocation', query: { collocID: collocID } });
} else {
  router.push({ path: '/colocation-mandatory', query: { collocID: collocID } });
}
</script>
