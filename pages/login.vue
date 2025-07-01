<template>
    <div class="base">
        <img src="../public/logo-hestia.png" class="logo" />
        <div v-if="registretion" class="register">
            <h2 class="login-font">Register : </h2>
            <h2 class="register-font">Nom d'utilisateur :</h2>
            <input class="input" type="text" placeholder="Nom d'utilisateur" v-model="username" required />
            <h2 v-if="alert" class="alert">*Veuillez indiqué votre nom d'utilisateur*</h2>
            <h2 class="register-font">Id de colocation :</h2>
            <input class="input" type="text" placeholder="Optionel" v-model="colocationID" />
            <h2 class="register-font">Créer un compte :</h2>
            <a type="submit" @click.prevent="register()" class="google-button"> Register with Google</a>
            <button class="register-button" @click="login()">Login</button>
        </div>
        <div v-else class="login">
            <h2 class="login-font">Login : </h2>
            <a @click="login()" class="google-button">
                Login with Google</a>
            <button class="register-button" @click="goRegister()">Register</button>
        </div>
    </div>
</template>

<script setup>
import { Browser } from '@capacitor/browser';
import { SocialLogin } from '@capgo/capacitor-social-login'
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
const username = ref('');
const colocationID = ref('');
const registretion = ref(false);
const alert = ref(false);

onMounted(() => {
    SocialLogin.initialize({
        google: {
            webClientId: '80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com', // the web client id for Android and Web
        }
    })
})

function goRegister() {
    registretion.value = true;
}

const register = async () => {
    if (!username) {
        alert.value = true;
        return;
    }
    alert.value = false;
    const register_token = useCookie('register_token');
    register_token.value = username.value;
    console.log(`registe ${register_token.value}`)
    const googleAuthURL = "https://accounts.google.com/o/oauth2/v2/auth?client_id=80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com&redirect_uri=http://localhost:3000/auth/google-callback&response_type=code&scope=openid%20email%20profile&access_type=offline&prompt=consent";
    await Browser.open({ url: googleAuthURL });
}

const login = async () => {
    const res = await SocialLogin.login({
        provider: 'google',
        options: {
            scopes: ['email', 'profile'],
        },
    });
    console.log(res.result.idToken)
    if (res) {
        const data = await $bridge.login(res.result.idToken);
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
    // registretion.value = false;
    // const register_token = useCookie('register_token');
    // register_token.value = "";
    // const googleAuthURL = "https://accounts.google.com/o/oauth2/v2/auth?client_id=80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com&redirect_uri=http://localhost:3000/auth/google-callback&response_type=code&scope=openid%20email%20profile&access_type=offline&prompt=consent";
    // await Browser.open({ url: googleAuthURL });
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
    padding-bottom: 20px;
    font-size: 50px;
}

.register-button {
    width: 68px;
    height: 28px;
    margin-top: 14px;
    border-radius: 8px;
    color: #E7FEED;
    background-color: #074338;
    font-weight: 600;
}

.google-button {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 200px;
    height: 50px;
    background-color: #E7FEED;
    border-radius: 14px;
    color: #074338;
    font-weight: 600;
    font-size: 20px;
    text-decoration: none;
}

.alert {
    padding: 0;
    margin: 0;
    margin-top: -6px;
    margin-bottom: 8px;
    font-size: 15px;
    color: #FF6A61;
}
</style>