<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" @click.stop>
                    <div class="modal-header left">
                        <h1 class="modal-header-text">Add a new task: </h1>
                    </div>
                    <form method="post" action="">
                        <div class="modal-body left">
                            <input class="modal-body-input" v-model="task.title" placeholder="Set task name" required />
                            <input class="modal-body-input" v-model="task.description"
                                placeholder="Set task description (optional)" />
                        </div>
                        <div class="task-assignee">
                            <text class="text-task-assignee">Assignee:</text>
                            <select v-model="task.enroll" class="input-task-assignee">
                                <option v-for="coloc in list_coloc" :key="coloc.id" :value="coloc.id">
                                    {{ coloc.username }}
                                </option>
                            </select>
                        </div>
                        <div class="modal-buttons">
                            <button class="button-proceed" @click.prevent="handleProceed">
                                <img src="../public/submit.png" class="submit">
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import type { Coloc } from '../composables/service/type';
import useModal from '../composables/useModal';
import { useUserStore } from '../store/user';

const props = withDefaults(
    defineProps<{
        name?: string
        modelValue?: boolean
        header?: boolean
        buttons?: boolean
        borders?: boolean
    }>(),
    {
        header: true,
        buttons: true,
        borders: true,
    }
)

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');

const list_coloc = ref<Coloc[]>([]);

const task = ref({
    colocationId: userStore.user.colocationId,
    createdBy: userStore.user.id,
    dueDate: '',
    title: '',
    description: '',
    enroll: '',
    isDone: false,
})

const { modelValue } = toRefs(props)

const { open, close, toggle, visible } = useModal(props.name)

api.getUserbyCollocId(userStore.user.colocationId).then((response) => {
    list_coloc.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
});

const emit = defineEmits<{
    closed: [] // named tuple syntax
    proceed: []
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

const handleProceed = async () => {
    await api.addReminder(task.value)
    close()
    emit('proceed')
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
    height: 300px;
    overflow-y: auto;
    margin-top: 0px;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
    border-bottom-left-radius: 20px;
    border-bottom-right-radius: 20px;
    animation: slideIn 0.2s;
    background-color: #1e1e1eda;
    backdrop-filter: blur(8px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
    display: flex;
    flex-direction: column;
    justify-content: center;
    position: relative;
}

.modal-header {
    padding: 16px 24px;
    font-weight: 600;
    color: #fff;
}

.modal-header-text {
    margin: 0px;
    font-size: 28px;
}

.modal-body {
    padding: 12px 24px;
    display: flex;
    flex-direction: column;
    overflow: auto;
    gap: 12px;
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

.task-assignee {
    display: flex;
    flex-direction: column;
    padding: 8px 24px;
}

.text-task-assignee {
    color: white;
    font-size: 18px;
    margin-left: 2px;
}

.input-task-assignee {
    width: 60%;
    height: 30px;
    background-color: #26272D;
    border-radius: 9px;
    outline: none;
    border: none;
    color: #FFFFFF;
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
    height: 30px;
    padding: 12px 24px;
    border-top: 0px;
    border-bottom-left-radius: 20px;
    border-bottom-right-radius: 20px;
    display: flex;
    justify-content: right;
    gap: 1em;
}

.modal-buttons:deep(button) {
    border-radius: 7px;
}

.modal-no-border {
    border: 0;
}

/** Fallback Buttons */

.button-proceed {
    padding-right: 14px;
    display: flex;
    justify-content: center;
    align-items: center;
    border: none;
    background: none;
    height: 22px;
    width: 22px;
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
        transform: translateY(100px);
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
        transform: translateY(100px);
    }
}

@media screen and (max-width: 768px) {

    /** Slide Out Transition (mobile only) */
    .modal-enter-from:deep(.modal),
    .modal-leave-to:deep(.modal) {
        animation: slideOut 0.2s linear;
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