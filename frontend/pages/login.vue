<template>
    <div class="body-container">
        <div class="base">
            <img src="../public/logo-hestia.png" class="logo" />
            <div v-if="registretion" class="register">
                <h2 class="login-font">{{ $t('register') }}</h2>
                <h2 class="register-font">{{ $t('user_name') }} :</h2>
                <input class="input" type="text" :placeholder="$t('user_name')" maxlength="12" v-model="username" />
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
import { PushNotifications } from '@capacitor/push-notifications';
import { addListeners, registerNotifications } from '~/store/push';

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
const fcmToken = ref('');

onMounted(() => {
    SocialLogin.initialize({
        google: {
            webClientId: '80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com', // the web client id for Android and Web
        }
    })
    registerNotifications();
    colocationID.value = route.query.collocID;
    if (colocationID.value) {
        registretion.value = true;
    }

    PushNotifications.addListener('registration', (token) => {
        fcmToken.value = token.value;
    });
})

addListeners();

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
            const data = await $bridge.addUser(newuser, res.result.idToken, fcmToken.value);
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
        const data = await $bridge.login(res.result.idToken, fcmToken.value);
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
        background-color: var(--background-light);
        border-radius: 20px;
        text-align: center;
        box-shadow: var(--rectangle-shadow-light);
    }

    .dark .login {
        background-color: var(--main-buttons-dark);
        box-shadow: var(--rectangle-shadow-light);
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
        background-color: var(--background-light);
        border-radius: 20px;
        box-shadow: var(--rectangle-shadow-light);
    }

    .dark .register {
        background-color: var(--main-buttons-dark);
        box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
    }

    h2 {
        color: var(--page-text-light);
        font-weight: 600;
    }

    .dark h2 {
        color: var(--page-text-dark);
    }

    .input {
        margin-bottom: 10px;
        outline: none;
        background-color: var(--main-buttons-light);
        border-radius: 8px;
        border: none;
        text-align: center;
        color: var(--basic-grey);
        font-weight: 600;
    }

    .dark .input {
        background-color: var(--background-dark);
        color: var(--page-text-dark);
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
        color: var(--background-light);
        background-color: var(--main-buttons-dark);
        font-weight: 600;
        border: none;
        text-align: center;
    }

    .dark .register-button {
        background-color: var(--background-light);
        color: var(--background-dark);
    }

    .google-button {
        display: flex;
        justify-content: center;
        align-items: center;
        min-width: 200px;
        height: fit-content;
        padding: 5px;
        background-color: var(--background-light);
        border-radius: 14px;
        color: var(--page-text-light);
        font-weight: 600;
        font-size: 20px;
        text-decoration: none;
        box-shadow: var(--button-shadow-light);
    }

    .dark .google-button {
        background-color: var(--background-dark);
        color: var(--page-text-dark);
    }

    .alert {
        padding: 0;
        margin: 0;
        margin-top: -6px;
        margin-bottom: 8px;
        font-size: 15px;
        color: var(--basic-red);
    }
</style>
