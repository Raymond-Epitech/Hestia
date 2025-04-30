<template>
    <div class="base">
        <img src="../public/logo-hestia.png" class="logo" />
        <div v-if="registretion" class="register">
            <h2 class="login-font">Register : </h2>
            <h2 class="register-font">Nom d'utilisateur :</h2>
            <input class="input" type="text" placeholder="Nom d'utilisateur" v-model="username" />
            <h2 class="register-font">Créer un compte :</h2>
            <a
                href="https://accounts.google.com/o/oauth2/v2/auth?client_id=80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com&redirect_uri=http://localhost:3000/auth/google-callback&response_type=code&scope=openid%20email%20profile&access_type=offline&prompt=consent">
                Register with Google</a>

        </div>
        <div v-else class="login">
            <h2 class="login-font">Login : </h2>
            <a
                href="https://accounts.google.com/o/oauth2/v2/auth?client_id=80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com&redirect_uri=http://localhost:3000/auth/google-callback&response_type=code&scope=openid%20email%20profile&access_type=offline&prompt=consent">
                Login with Google</a>
            <button class="register-button" @click="register()">Register</button>
        </div>
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

function register() {
    registretion.value = true;
}

</script>

<style scoped>
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

.login {
    height: 200px;
    width: 300px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: #a3d397;
    border-radius: 20px;
}

.register {
    height: 400px;
    width: 300px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: #a3d397;
    border-radius: 20px;
}

h2 {
    color: #E7FEED;
    font-weight: 600;
}

.input {
    margin-bottom: 10px;
    outline: none;
    background-color: #E7FEED;
    border-radius: 8px;
    border: none;
    text-align: center;
    color: #85AD7B;
    font-weight: 600;
}

.register-font {
    font-size: 20px;
}

.login-font {
    padding-bottom: 25px;
    font-size: 50px;
}

.register-button {
    width: 68px;
    height: 28px;
    margin-top: 16px;
    border-radius: 8px;
    color: #E7FEED;
    background-color: #074338;
    font-weight: 600;
}
</style>