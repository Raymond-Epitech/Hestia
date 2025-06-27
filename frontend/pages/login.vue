<template>
    <div class="base">
        <img src="../public/logo-hestia.png" class="logo" />
        <div v-if="registretion" class="register">
            <h2 class="login-font">Register : </h2>
            <h2 class="register-font">Nom d'utilisateur :</h2>
            <input class="input" type="text" placeholder="Nom d'utilisateur" v-model="username" />
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
    if (!username.value) {
        console.log("no username")
        alert.value = true;
        return;
    }
    alert.value = false;
    if (alert.value == false) {
        const res = await SocialLogin.login({
            provider: 'google',
            options: {
                scopes: ['email', 'profile'],
            },
        });
        if (res) {
            const newuser = {
                username: username.value,
                colocationId: colocationID.value
            };
            const data = await $bridge.addUser(newuser, res.result.idToken);
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
}

const login = async () => {
    const res = await SocialLogin.login({
        provider: 'google',
        options: {
            scopes: ['email', 'profile'],
        },
    });
    if (res) {
        const data = await $bridge.login(res.result.idToken);
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
    min-height: 200px;
    min-width: 300px;
    padding: 10px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: #a3d397;
    border-radius: 20px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
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
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
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
    min-width: 68px;
    min-height: 28px;
    margin-top: 14px;
    border-radius: 8px;
    color: #E7FEED;
    background-color: #074338;
    font-weight: 600;
    border: none;
}

.google-button {
    display: flex;
    justify-content: center;
    align-items: center;
    min-width: 200px;
    height: 50px;
    padding: 5px;
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