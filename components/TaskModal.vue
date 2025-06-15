<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" :class="props.color" @click.stop>
                    <div class="modal-header">
                        <div class="modal-header-text">
                            <text class="title">{{ title }}</text>
                            <text class="description">{{ description }}</text>
                        </div>
                        <div class=" due-date">
                            <div class="number">{{ getDayNumber() }}</div>
                            <div class="month">{{ getMonthAbbreviation() }}</div>
                        </div>
                    </div>

                    <div class="modal-body">
                        <task-message-box></task-message-box>
                        <slot name="content"></slot>
                    </div>

                    <!-- Buttons -->
                    <div class="modal-buttons">
                        <div class="enrollees-icon-container">
                            <div v-for="enroll in enrollees" :key="enroll.id" :value="enroll.id" class="enrollees-icon">
                                <profile-icon :height="33" :width="33"></profile-icon>
                            </div>
                        </div>
                        <slot name="buttons">
                            <div v-if="isEnrolled">
                                <button class="button button-proceed" @click="handleQuit">
                                    <Texte_language source="quit" /> :c
                                </button>
                            </div>
                            <div v-else>
                                <button class="button button-proceed" @click="handleEnroll">
                                    <Texte_language source="Enroll" /> !
                                </button>
                            </div>
                            <div v-if="done">
                                <text>
                                    <Texte_language source="isDone" />
                                </text>
                            </div>
                            <div v-else>
                                <button class="button button-proceed" @click="handleDone">
                                    <Texte_language source="Done" /> !
                                </button>
                            </div>
                        </slot>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useUserStore } from '../store/user';
import useModal from '../composables/useModal';
import type { User } from '../composables/service/type'

const props = defineProps<{
    id: string
    title: string
    modelValue?: boolean
    description: string
    color: string
    dueDate: string
    isDone: boolean
}>();

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const isEnrolled = ref(false);
const enrollees = ref<User[]>([]);
var done = props.isDone;

const { modelValue } = toRefs(props)
const { open, close, toggle, visible } = useModal(props.title)

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

const getEnroll = async () => {
    const data = await api.getUserEnrollChore(props.id);
    enrollees.value = data;
    isEnrolled.value = data.some(obj => obj.id === userStore.user.id);
};

const handleClose = () => {
    close()
    emit('closed')
}

const handleQuit = async () => {
    api.deleteChoreUser(props.id, userStore.user.id).then(() => {
        isEnrolled.value = false;
    }).catch((error) => {
        console.error('Error delete chore user:', error);
    });
    close()
    emit('proceed')
}

const handleEnroll = async () => {
    api.addChoreUser(props.id, userStore.user.id).then(() => {
        isEnrolled.value = true;
    }).catch((error) => {
        console.error('Error add chore user:', error);
    });
    close()
    emit('proceed')
}
const handleDone = async () => {
    const updateChore = {
        id: props.id,
        colocationId: userStore.user.colocationId,
        dueDate: props.dueDate,
        title: props.title,
        description: props.description,
        isDone: true,
        enrolled: enrollees.value.map(user => user.id),
    }
    api.updateChore(updateChore).then(() => {
        done = true;
    }).catch((error) => {
        console.error('Error update chore:', error);
    });
    close()
    emit('proceed')
}

function getDayNumber() {
    const date = new Date(props.dueDate);
    return date.getDate();
}

function getMonthAbbreviation() {
    const date = new Date(props.dueDate);
    return date.toLocaleString('en-US', { month: 'short' });
}

watch(
    modelValue,
    (value, oldValue) => {
        if (value !== oldValue) {
            toggle(value)
            if (value === true) {
                getEnroll();
            }
        }
    },
    { immediate: true }
)

watch(visible, (value) => {
    emit('update:modelValue', value)
})
</script>

<style scoped>
.red {
    background-color: #FF6A61;
}

.orange {
    background-color: #FFC93D;
}

.green {
    background-color: #85AD7B;
}

.modal {
    width: 100%;
    height: 360px;
    overflow-y: auto;
    margin-top: 0px;
    padding-top: 20pt;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
    border-bottom-left-radius: 30px;
    border-bottom-right-radius: 30px;
    animation: slideIn 0.4s;
    backdrop-filter: blur(8px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    position: relative;
}

.modal-header {
    color: #000000;
    display: grid;
    grid-template-columns: 4fr 1fr;
    align-items: center;
    justify-content: space-between;
    width: 90%;
    height: 80px;
    padding-left: 5%;
    margin-top: 5px;
    margin-bottom: 15px;
    border-bottom: none;
}

.modal-header-text {
    display: flex;
    flex-direction: column;
}

.title {
    font-weight: 700;
    font-size: 18px;
    margin-bottom: 5px;
}

.description {
    font-weight: 500;
}

.due-date {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    padding-bottom: 6px;
    width: 62px;
    height: 62px;
}

.number {
    display: flex;
    justify-content: center;
    align-content: center;
    text-align: center;
    height: 40px;
    font-size: 38px;
    margin-bottom: 4px;
    font-weight: 600;
}

.month {
    font-size: 16px;
    font-weight: 600;
    text-transform: uppercase;
}

.modal-body {
    width: 95%;
    padding: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    overflow: auto;
    gap: 16px;
}

.modal-body:deep(p) {
    margin: 0;
    font-size: 18px;
    line-height: 23px;
}

.modal-body-input {
    width: 100%;
    background-color: #1e1e1e00;
    outline: none;
    border: none;
    line-height: 3ch;
    background-image: linear-gradient(transparent, transparent calc(3ch - 1px), #E7EFF8 0px);
    background-size: 100% 3ch;
    color: #fff;
    font-size: 18px;
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
    width: 100%;
    padding: 14px;
    border-top: 0px;
    border-bottom-left-radius: 20px;
    border-bottom-right-radius: 20px;
    margin-top: auto;
    display: grid;
    grid-template-columns: 1fr 3fr 3fr;
    align-items: center;
    gap: 1em;
}


.modal-buttons:deep(button) {
    border-radius: 7px;
}

.modal-no-border {
    border: 0;
}

.enrollees-icon-container {
    display: flex;
}

.enrollees-icon {
    margin-right: -14px;
}

/** Fallback Buttons */
.button {
    min-width: 50%;
    max-width: fit-content;
    margin-left: 20px;
    padding: 10px 10px;
    border-radius: 15px;
    border: 0;
    cursor: pointer;
}

.button-proceed {
    background: #00000088;
    color: #fff;
}

.button-proceed:hover {
    opacity: 0.7;
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
        transform: translateY(-400px);
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
        transform: translateY(-400px);
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
        border-bottom-left-radius: 50px;
        border-bottom-right-radius: 50px;
    }
}
</style>