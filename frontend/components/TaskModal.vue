<template>
    <teleport to="body">
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

                        <!-- Buttons -->
                        <div class="modal-buttons">
                            <div class="enrollees-icon-container">
                                <div v-for="(link, index) in linkToPP" :key="index" :value="linkToPP"
                                    class="enrollees-icon">
                                    <profile-icon :linkToPP="link" :height="33" :width="33"></profile-icon>
                                </div>
                            </div>
                            <slot name="buttons">
                                <div v-if="isEnrolled">
                                    <button class="button button-proceed" @click="handleQuit" :disabled="loading">
                                        <Texte_language source="quit" /> :c
                                    </button>
                                </div>
                                <div v-else>
                                    <button class="button button-proceed" @click="handleEnroll" :disabled="loading">
                                        <Texte_language source="Enroll" /> !
                                    </button>
                                </div>
                                <div v-if="isDone">
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
                <div v-if="errview">
                    <Errorpopup :status="err.status" :body="err.body" @close="errview = false" />
                </div>
            </div>
        </transition>
    </teleport>
</template>

<script setup lang="ts">
import { useUserStore } from '../store/user';
import useModal from '../composables/useModal';
import type { UpdateChore, User } from '../composables/service/type'

const props = defineProps<{
    id: string,
    title: string,
    modelValue?: boolean,
    description: string,
    color: string,
    dueDate: string,
    isDone: boolean,
    enrolledUsers?: Object,
}>();

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
const err = ref<{ status: number; body: any }>({ status: 0, body: null });
const errview = ref(false);
api.setjwt(useCookie('token').value ?? '');
const isEnrolled = ref(false);
const enrollees = computed(() =>
    props.enrolledUsers ? Object.keys(props.enrolledUsers) : []
)
const linkToPP = props.enrolledUsers ? Object.values(props.enrolledUsers) : [];
const loading = ref(false)

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

const getEnroll = () => {
    isEnrolled.value = enrollees.value.includes(userStore.user.id)
};

const handleClose = () => {
    close()
    emit('closed')
}

const handleQuit = async () => {
    if (loading.value) return
    loading.value = true
    try {
        await api.deleteChoreUser(props.id, userStore.user.id);
        emit('proceed');
        close();
    } catch (error) {
        console.error(error);
        err.value = error;
        errview.value = true;
    } finally {
        loading.value = false
    }
}

const handleEnroll = async () => {
    if (loading.value) return
    loading.value = true
    try {
        await api.addChoreUser(props.id, userStore.user.id);
        emit('proceed');
        close();
    } catch (error) {
        console.error(error);
        err.value = error;
        errview.value = true;
    } finally {
        loading.value = false
    }
}

const handleDone = async () => {
    const updateChore: UpdateChore = {
        id: props.id,
        colocationId: userStore.user.colocationId,
        dueDate: props.dueDate,
        title: props.title,
        description: props.description,
        isDone: true,
        enrolled: enrollees.map(userId => userId),
    }
    api.updateChore(updateChore).then(() => {
        // done = true;
    }).catch((error) => {
        console.error(error);
        err.value = error;
        errview.value = true;
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
    background-color: var(--basic-red);
}

.orange {
    background-color: var(--basic-yellow);
}

.green {
    background-color: var(--basic-green);
}

.modal {
    width: 100%;
    height: fit-content;
    overflow-y: auto;
    margin-top: 0px;
    padding-top: 25pt;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
    border-bottom-left-radius: 30px;
    border-bottom-right-radius: 30px;
    backdrop-filter: blur(8px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    position: relative;
}

.modal-header {
    height: fit-content;
    color: var(--page-text);
    display: grid;
    grid-template-columns: 80% 20%;
    align-items: start;
    justify-content: space-between;
    width: 90%;
    padding: 0;
    border-bottom: none;
}

.modal-header-text {
    padding-top: 4px;
    padding-left: 6%;
    display: flex;
    flex-direction: column;
    line-height: normal;
    text-align: left;
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
    height: fit-content;
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
    font-size: 32px;
    margin-bottom: 4px;
    font-weight: 600;
    -webkit-text-size-adjust: none;
    text-size-adjust: none;
}

.month {
    font-size: 16px;
    font-weight: 600;
    text-transform: uppercase;
    -webkit-text-size-adjust: none;
    text-size-adjust: none;
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
    grid-template-columns: 2fr 3fr 2fr;
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
    margin-left: 28px;
}

.enrollees-icon {
    margin-left: -12px;
    z-index: 1;
}

/** Fallback Buttons */
.button {
    min-width: 50%;
    width: fit-content;
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

/* Modal background fade */
.modal-enter-active,
.modal-leave-active {
    transition: opacity 0.25s ease;
}

.modal-enter-from,
.modal-leave-to {
    opacity: 0;
}

/* Modal slide */
.modal-enter-active .modal,
.modal-leave-active .modal {
    transition: transform 0.35s cubic-bezier(0.22, 1, 0.36, 1);
    will-change: transform;
}

.modal-enter-from .modal {
    transform: translateY(-60px);
}

.modal-leave-to .modal {
    transform: translateY(-60px);
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