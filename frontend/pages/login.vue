<template>
    <div class="base">
        <img src="../public/logo-hestia.png" class="logo" />
        <div v-if="registretion">
            <h2>Pour vous enregistrer, veuillez entrer un nom d'utilisateur et relancer en sélectionnant le compte
                Google</h2>
            <input type="text" placeholder="Nom d'utilisateur" v-model="username" />
        </div>
        <GoogleSignInButton @success="handleLoginSuccess" @error="handleLoginError"></GoogleSignInButton>
    </div>
</template>

<script setup>
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
const username = ref('');
const registretion = ref(false);
const router = useRouter();

const handleregistration = async (response) => {
    const { credential } = response;
    console.log("Access Token", credential);
    const newuser = {
        username: username.value
    };

    const data = await $bridge.addUser(newuser, credential);
    console.log("Data", data);
    if (data) {
        $bridge.setjwt(data.jwt);
        await authenticateUser(data.jwt);
    }
    if (authenticated) {
        router.push('/');
    }
};

const handleLoginSuccess = async (response) => {
    if (registretion.value) {
        handleregistration(response);
        return;
    }
    const { credential } = response;
    console.log("Access Token", credential);
    const data = await $bridge.login(credential);
    console.log("Data", data);
    if (data.error) {
        console.error(data.error);
        registretion.value = true;
        return;
    }
    if (data) {
        $bridge.setjwt(data.jwt);
        await userStore.setUser(data.user);
        await authenticateUser(data.jwt);
    }
    if (authenticated) {
        router.push('/');
    }
};

const handleLoginError = () => {
    console.error("Login failed");
};

</script>

<style>
body {
    background-color: #E7FEED;
}

.base {
    display: flex;
    justify-content: space-evenly;
    flex-direction: column;
    align-items: center;
    height: 100vh;
}

.logo {
    width: 280px;
    border-radius: 15px;
}
</style>