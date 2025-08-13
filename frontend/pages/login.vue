<template>
    <div class="body-container">
        <div class="base">
            <img src="../public/logo-hestia.png" class="logo" />
            <div v-if="registretion" class="register">
                <h2 class="login-font">{{ $t('register') }}</h2>
                <h2 class="register-font">{{ $t('user_name') }} :</h2>
                <input class="input" type="text" :placeholder="$t('user_name')" v-model="username" />
                <h2 v-if="alert" class="alert">{{ $t('error_register') }}</h2>
                <h2 class="register-font">{{ $t('colocation_id') }} :</h2>
                <input class="input" type="text" :placeholder="$t('optional')" v-model="colocationID" />
                <h2 class="register-font">{{ $t('create_account') }} :</h2>
                <a type="submit" @click.prevent="register()" class="google-button">
                    {{ $t('register_with_google') }}
                </a>
                <button class="register-button" @click="goLogin()">{{ $t('login') }}</button>
            </div>
            <div v-else class="login">
                <h2 class="login-font">{{ $t('login') }}</h2>
                <a @click="login()" class="google-button">
                    {{ $t('login_with_google') }}
                </a>
                <button class="register-button" @click="goRegister()">
                    {{ $t('register') }}
                </button>
            </div>
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

function goLogin() {
    registretion.value = false;
}

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
.body-container {
    position: absolute;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    overflow: auto;
}

.base {
    display: flex;
    justify-content: space-evenly;
    flex-direction: column;
    align-items: center;
}

.logo {
    margin: 30px;
    width: 280px;
    border-radius: 15px;
}

.login {
    min-height: 200px;
    min-width: 85%;
    padding: 30px;
    margin: 30px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: white;
    border-radius: 20px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
    text-align: center;
}

.dark .login {
    background-color: black;
}

.register {
    height: 400px;
    width: 300px;
    padding: 30px;
    margin: 30px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: white;
    border-radius: 20px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.dark .register {
    background-color: black;
}

h2 {
    color: #000000;
    font-weight: 600;
}

.dark h2 {
    color: #FFFFFF;
}

.input {
    margin-bottom: 10px;
    outline: none;
    background-color: #FFFCF7;
    border-radius: 8px;
    border: none;
    text-align: center;
    color: #4B4B4B;
    font-weight: 600;
}

.dark .input {
    background-color: #1D1B20;
    color: #D6D6D6;
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
    padding: 0px 5px;
    border-radius: 8px;
    color: white;
    background-color: black;
    font-weight: 600;
    border: none;
    text-align: center;
}

.dark .register-button {
    background-color: white;
    color: black;
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
    color: black;
    font-weight: 600;
    font-size: 20px;
    text-decoration: none;
}

.dark .google-button {
    background-color: #1D1B20;
    color: #E7FEED;
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