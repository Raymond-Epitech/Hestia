<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background">
                <button class="modal-close" @click="handleClose"><img class="icon" src="../public/Retour.svg"/></button>
                <div class="modal" @click.stop>
                    <h2 class="login-font">{{ $t('register') }}</h2>
                    <h2 class="register-font">{{ $t('user_name') }} :</h2>
                    <input class="input" type="text" :placeholder="$t('user_name')" maxlength="12" v-model="username" />
                    <h2 v-if="alert" class="alert">{{ $t('error_register') }}</h2>
                    <h2 class="register-font">{{ $t('colocation_id') }} :</h2>
                    <input class="input" type="text" :placeholder="$t('optional')" v-model="colocationID" />
                    <a type="submit" @click.prevent="register()" class="register-button"> 
                        {{ $t('register') }}
                    </a>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import useModal from '~/composables/useModal';
import { useRouter } from 'vue-router';
import { storeToRefs } from 'pinia';
import { useAuthStore } from '~/store/auth';
import { useUserStore } from '~/store/user';

const props = withDefaults(
    defineProps<{
        name?: string,
        modelValue?: boolean,
        providerJWT?: string,
        colocationID?: string,
    }>(),{}
);

const { setLocale } = useI18n();
const { $locally } = useNuxtApp();
const { authenticateUser } = useAuthStore();
const { authenticated } = storeToRefs(useAuthStore());
const userStore = useUserStore();
const { $bridge } = useNuxtApp();
const router = useRouter();
const username = ref('');
const colocationID = ref('');
const alert = ref(false);
const fcmToken = ref('');


const { modelValue } = toRefs(props);
const { open, close, toggle, visible } = useModal(props.name);


const emit = defineEmits<{
    closed: [],
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
        if (props.providerJWT) {
            const newuser = {
                username: username.value,
                colocationId: colocationID.value
            };
            const data = await $bridge.addUser(newuser, props.providerJWT, fcmToken.value);
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

button {
    background-color: var(--main-buttons);
    position: fixed;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 40px;
    height: 40px;
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
}

.icon {
    filter: var(--icon-filter);
}

.modal-close {
    top: 3%;
    left: 3%;
}

.modal-background {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: var(--overlay-background);
    backdrop-filter: var(--overlay-blur);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

.modal {
    color: var(--overlay-text);
    background-color: var(--overlay-background);
    height: fit-content;
    width: 360px;
    padding: 50px;
    margin: 30px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    border-radius: 20px;
    position: relative;
    box-shadow: var(--rectangle-shadow-light);
}

.register-font {
    font-size: 20px;
}

.login-font {
    padding-bottom: 20px;
    font-size: 50px;
}

.input {
    margin-bottom: 10px;
    outline: none;
    background-color: var(--background);
    border-radius: 8px;
    border: none;
    text-align: center;
    color: var(--basic-grey);
    font-weight: 600;
}

.dark .input {
    color: var(--page-text);
}

.hestia .input {
    color: var(--main-buttons);
}

.register-button {
    display: flex;
    justify-content: center;
    align-items: center;
    min-width: 200px;
    height: fit-content;
    padding: 5px;
    margin-top: 20px;
    background-color: var(--background);
    border-radius: 14px;
    color: var(--page-text);
    font-weight: 600;
    font-size: 20px;
    text-decoration: none;
    box-shadow: var(--button-shadow-light);
}

.hestia .register-button {
    background-color: var(--main-buttons);
}

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.2s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}


</style>