<template>
    <div class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full flex justify-center items-center">
        <div class="relative bg-white rounded-lg shadow p-6 w-full max-w-md">
            <h2 class="text-2xl font-bold mb-4 text-center">Register</h2>
            <form @submit.prevent="register">
                <div class="mb-4">
                    <label for="username" class="block text-gray-700 font-semibold mb-2">Username</label>
                    <input v-model="username" type="text" id="username" class="w-full px-3 py-2 border rounded focus:outline-none focus:ring focus:border-blue-300" required>
                </div>
                <div class="mb-4">
                    <label for="colocationID" class="block text-gray-700 font-semibold mb-2">Colocation ID (optional)</label>
                    <input v-model="colocationID" type="text" id="colocationID" class="w-full px-3 py-2 border rounded focus:outline-none focus:ring focus:border-blue-300">
                </div>
                <div v-if="alert" class="text-red-500 mb-4 text-center">Username is required.</div>
                <button type="submit" class="w-full bg-blue-500 text-white font-bold py-2 px-4 rounded hover:bg-blue-600 transition duration-300">Register with Google</button>
            </form>
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

const { setLocale } = useI18n();
const { $locally } = useNuxtApp()
const { authenticateUser } = useAuthStore();
const { authenticated } = storeToRefs(useAuthStore());
const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const router = useRouter();
const route = useRoute()
const username = ref('');
const colocationID = ref('');
const alert = ref(false);
const fcmToken = ref('');

const register = async () => {
    if (!username.value) {
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
                $bridge.getLanguage(userStore.user.id).then((lang) => {
                    if (lang != '') {
                        setLocale(lang);
                        $locally.setItem('locale', lang);
                    }
                })
                await authenticateUser(data.jwt);
            }
            if (authenticated) {
                router.push('/');
            }
        }
    }
}

</script>