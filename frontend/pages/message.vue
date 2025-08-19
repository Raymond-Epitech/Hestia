<template>
    
</template>

<script setup>
import { useUserStore } from '~/store/user';
const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
</script>

<style scoped></style>
