<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" @click.stop>
                    <div class="padding-top">
                    </div>
                    <div v-for="(task) in props.tasks" :key="task.id" class="task-list">
                        <Task :key="task.id" :id="task.id" :title="task.title" :description="task.description"
                        :createdBy="task.createdBy" :createdAt="task.createdAt" :dueDate="task.dueDate" :isDone="task.isDone"
                        :enrolledUsers="task.enrolledUsers" :updatedAt="task.updatedAt" @proceed="getall()"></Task>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import useModal from '../composables/useModal';

const props = withDefaults(
    defineProps<{
        modelValue?: boolean
        name?: string
        tasks: Array<any>,
    }>(),
    {
        modelValue: false,
    },
)

const { modelValue } = toRefs(props)
const { open, close, toggle, visible } = useModal(props.name)

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
    emit('update:modelValue', false)
    emit('closed')
}

watch(
  modelValue,
  (value) => {
    toggle(!!value)
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
    min-height: 400px;
    height: fit-content;
    margin-top: 0px;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
    border-bottom-left-radius: 30px;
    border-bottom-right-radius: 30px;
    animation: slideIn 0.4s;
    background-color: #1e1e1eda;
    backdrop-filter: blur(8px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
    display: flex;
    flex-direction: column;
    justify-content: center;
    position: relative;
}

.padding-top {
    height: 25pt;
}

.modal-header {
    padding: 16px 24px;
    font-weight: 600;
    color: #fff;
    border: none;
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

.modal-buttons:deep(button) {
    border-radius: 7px;
}

.modal-no-border {
    border: 0;
}

/* Transition */
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