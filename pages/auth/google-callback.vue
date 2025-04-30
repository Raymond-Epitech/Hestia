<template>
    <div>Connexion en cours...</div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { storeToRefs } from 'pinia';
import { useAuthStore } from '~/store/auth';
import { useUserStore } from '~/store/user';

definePageMeta({
    layout: false
})

const { authenticateUser } = useAuthStore();
const { authenticated } = storeToRefs(useAuthStore());
const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const registretion = ref(false);
const router = useRouter();
const route = useRoute()

onMounted(async () => {
    const code = route.query.code
    console.log(code)

    if (code) {
        const data = await $bridge.login(code);
        if (data.error) {
            console.error(data.error);
            registretion.value = true;
            return;
        }
        if (data) {
            $bridge.setjwt(data.jwt);
            userStore.setUser(data.user);
            await authenticateUser(data.jwt);
        }
        if (authenticated) {
            router.push('/');
        }
    }

})

</script>
