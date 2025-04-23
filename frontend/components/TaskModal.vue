<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" :class="props.color" @click.stop>
                    <div class="modal-header left" :class="{ 'modal-no-border': !borders }">
                        <h1>{{ title }}</h1>
                        <div class="due-date">
                            <div class="number">{{ getDayNumber() }}</div>
                            <div class="month">{{ getMonthAbbreviation() }}</div>
                        </div>
                    </div>

                    <div class="modal-body left">
                        <slot name="content"></slot>
                        <h1>{{ description }}</h1>
                    </div>

                    <!-- Buttons -->
                    <div class="modal-buttons" :class="{ 'modal-no-border': !borders }">
                        <slot name="buttons">
                            <button class="button button-cancel" @click="handleClose">Cancel</button>
                            <button class="button button-proceed" @click="handleProceed">Yes, Proceed</button>
                        </slot>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">

const props = defineProps<{
    title: string
    modelValue?: boolean
    description: string
    color: string
    dueDate: string
}>();

const { modelValue } = toRefs(props)
const { open, close, toggle, visible } = useModal(props.name)

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

const handleProceed = async () => {
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
        }
    },
    { immediate: true }
)

watch(visible, (value) => {
    emit('update:modelValue', value)
})
</script>

<style>
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
    height: 300px;
    overflow-y: auto;
    margin-top: 0px;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
    border-bottom-left-radius: 50px;
    border-bottom-right-radius: 50px;
    animation: slideIn 0.2s;
    backdrop-filter: blur(8px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
    display: flex;
    flex-direction: column;
    justify-content: center;
    position: relative;
}

.modal-header {
    padding: 12px 16px;
    font-weight: 600;
    border-bottom: 1px dotted lightgrey;
    color: #fff;
}

.modal-header-text {
    font-size: 20px;
}

.modal-body {
    padding: 12px;
    display: flex;
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

.post-colors-buttons {
    padding: 8px;
    display: flex;
    justify-content: space-evenly;
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
    border-bottom-left-radius: 20px;
    border-bottom-right-radius: 20px;
    margin-top: auto;
    display: flex;
    justify-content: center;
    gap: 1em;
}

.modal-buttons:deep(button) {
    border-radius: 7px;
}

.modal-no-border {
    border: 0;
}

/** Fallback Buttons */
.button {
    padding: 10px 20px;
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
        border-bottom-left-radius: 50px;
        border-bottom-right-radius: 50px;
    }
}
</style>