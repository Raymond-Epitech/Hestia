<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" @click.stop>
                    <div class="modal-header left">
                        <h1 class="modal-header-text">{{ $t('expense_category_name') }} :</h1>
                    </div>
                    <form method="post" action="">
                        <div class="modal-body">
                            <input class="body-input" maxlength="35" v-model="newcategory.name" required />
                        </div>
                        <div v-if="newcategory.name" class="modal-buttons">
                            <button class="button button-proceed" @click.prevent="handleProceed">{{ $t('poster')
                            }}</button>
                        </div>
                        <div v-else class="modal-buttons">
                            <button class="button button-proceed" @click.prevent="handleProceed" disabled>{{
                                $t('poster') }}</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import useModal from '~/composables/useModal';
import { useUserStore } from '~/store/user';
import type { expenses_category } from '~/composables/service/type';


const route = useRoute();
const router = useRouter();
const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const newcategory = ref<expenses_category>({
    colocationId: user.colocationId,
    name: '',
});

const props = withDefaults(
    defineProps<{
        name?: string,
        modelValue?: boolean,
        header?: boolean,
        buttons?: boolean,
        borders?: boolean,
    }>(),
    {
        header: true,
        buttons: true,
        borders: true,
    }
)
const { modelValue } = toRefs(props)
const { open, close, toggle, visible } = useModal(props.name)

const emit = defineEmits<{
    closed: [], // named tuple syntax
    proceed: [],
    'update:modelValue': [value: boolean]
}>()

const handleProceed = async () => {
    api.addexpensecategory(newcategory.value).then(() => {
        resetPost()
        close()
        emit('proceed')
    }).catch((error) => {
        console.error('Error adding expense category:', error);
    });
}

defineExpose({
    open,
    close,
    toggle,
    visible,
})

const resetPost = () => {
    newcategory.value = {
        colocationId: user.colocationId,
        name: '',
    }
}

const handleClose = () => {
    resetPost()
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
</script>

<style scoped>
.modal {
    width: 100%;
    height: fit-content;
    overflow-y: auto;
    margin-top: 0px;
    padding-top: 25pt;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
    border-bottom-left-radius: 20px;
    border-bottom-right-radius: 20px;
    animation: slideIn 0.4s;
    background-color: var(--overlay-background);
    backdrop-filter: blur(8px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
    display: flex;
    flex-direction: column;
    justify-content: center;
    position: relative;
}

.modal-header {
    padding: 0px 6%;
    font-weight: 600;
    border-bottom: none;
    color: var(--overlay-text);
}

.modal-header-text {
    font-size: 20px;
}

.modal-body {
    padding: 0px 6%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    overflow: auto;
    gap: 16px;
}

.modal-body:deep(p) {
    margin: 0;
    font-size: 18px;
    line-height: 23px;
}

.body-input {
    width: 100%;
    background-color: #1e1e1e00;
    outline: none;
    border: none;
    line-height: 3ch;
    background-image: linear-gradient(transparent, transparent calc(3ch - 1px), var(--list-lines) 0px);
    background-size: 100% 3ch;
    color: var(--overlay-text);
    font-size: 18px;
    margin-bottom: 12px;
}

.modal-background {
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 100;
    position: fixed;
    animation: fadeIn 0.2s;
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
}

.modal-buttons {
    padding: 14px;
    border-top: 0px;
    margin-top: auto;
    margin-bottom: 12px;
    display: flex;
    justify-content: center;
}

.modal-no-border {
    border: 0;
}

/** Fallback Buttons */
.button {
    width: 136px;
    height: 66px;
    border-radius: 16px;
    border: 0;
    cursor: pointer;
    font-size: 20px;
}

.button-proceed {
    background: var(--main-buttons);
    color: var(--page-text);
    font-weight: 600;
}

button:disabled {
    opacity: 0.5;
}

/* Transition */
.modal-enter-active,
.modal-leave-active {
    transition: opacity 0.2s ease;
}

.modal-enter-from,
.modal-leave-to {
    opacity: 0;
}

@keyframes fadeIn {
    0% {
        opacity: 0;
    }

    100% {
        opacity: 1;
    }
}

@keyframes slideIn {
    0% {
        transform: translateY(-600px);
    }

    100% {
        transform: translateY(0px);
    }
}

@keyframes slideOut {
    0% {
        transform: translateY(0px);
    }

    100% {
        transform: translateY(-600px);
    }
}

@media screen and (max-width: 768px) {

    /** Slide Out Transition (mobile only) */
    .modal-enter-from:deep(.modal),
    .modal-leave-to:deep(.modal) {
        animation: slideOut 0.4s linear;
    }
}

@media screen and (min-width: 768px) {
    .modal-background {
        justify-content: flex-start;
    }

    .modal {
        width: 100%;
        margin: 0 0 0 0;
        max-height: calc(100dvh - 120px);
        border-bottom-left-radius: 20px;
        border-bottom-right-radius: 20px;
    }
}
</style>