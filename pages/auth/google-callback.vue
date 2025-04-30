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
const router = useRouter();
const route = useRoute()

onMounted(async () => {
    const code = route.query.code
    console.log(code)
    const register_token = useCookie('register_token');

    if (code) {
        if (register_token.value) {
            const newuser = {
                username: register_token.value
            };
            const data = await $bridge.addUser(newuser, code);
            console.log("user registerd")
            if (data) {
                $bridge.setjwt(data.jwt);
                userStore.setUser(data.user);
                await authenticateUser(data.jwt);
            }
            if (authenticated) {
                router.push('/');
            }
        } else {
            const data = await $bridge.login(code);
            console.log("user logged in")
            if (data) {
                $bridge.setjwt(data.jwt);
                userStore.setUser(data.user);
                await authenticateUser(data.jwt);
            }
            if (authenticated) {
                router.push('/');
            }
        }
    }

})

</script>
