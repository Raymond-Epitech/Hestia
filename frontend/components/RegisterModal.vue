<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" @click.stop>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import useModal from '~/composables/useModal';
import { SocialLogin } from '@capgo/capacitor-social-login';
import { useRouter, useRoute } from 'vue-router';
import { storeToRefs } from 'pinia';
import { useAuthStore } from '~/store/auth';
import { useUserStore } from '~/store/user';

// definePageMeta({
//     layout: false
// })

const props = withDefaults(
    defineProps<{
        name?: string,
        modelValue?: boolean,
        providerJWT: string,
    }>(),
    {
        providerJWT: '',
    }
);

const { setLocale } = useI18n();
const { $locally } = useNuxtApp();
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


const { modelValue } = toRefs(props);
const { open, close, toggle, visible } = useModal(props.name);


const emit = defineEmits<{
    closed: [], // named tuple syntax
    proceed: [],
    'update:modelValue': [value: boolean]
}>()

defineExpose({
    open,
    close,
    toggle,
    visible,
})

const handleClose = () => {
    close()
    emit('closed')
}

watch(
    modelValue,
    (value, oldValue) => {
        if (value !== oldValue) {
            toggle(value)
        }
    },
    { immediate: true }
)

watch(visible, (value) => {
    emit('update:modelValue', value)
})

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

<style scoped>

.modal-background {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: var(--overlay-background);
    backdrop-filter: var(--overlay-blur);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

</style>