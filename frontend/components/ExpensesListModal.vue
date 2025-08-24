<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" @click.stop>
                    <div class="modal-top-bar">
                        <div id="return" @click="handleClose">
                            <img src="/Retour.svg" class="icon" alt="Return" width="32" height="32" />
                        </div>
                    </div>
                    <Texte_language class="modal-header" source="regularize" />
                     <div>
                        <ExpenseItem v-for="expense in expenses_list" :key="expense.id" :expense="expense" :paidBy="getUsername(expense.paidBy)" />
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import useModal from '~/composables/useModal';
import { useUserStore } from '~/store/user';
import type { Expenseget, Coloc } from '~/composables/service/type';

const props = withDefaults(
    defineProps < {
        name?: string,
        modelValue?: boolean,
        header?: boolean,
        buttons?: boolean,
        borders?: boolean,
        expense: string,
    } > (),
    {
        header: true,
        buttons: true,
        borders: true,
    }
)

const userStore = useUserStore();
const user = userStore.user;
const expenses_list = ref<Expenseget[]>([]);
const list_coloc = ref<Coloc[]>([]);
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');

const { modelValue } = toRefs(props)
const { open, close, toggle, visible } = useModal(props.name)

const emit = defineEmits < {
    closed: [], // named tuple syntax
    proceed: [],
    'update:modelValue': [value: boolean]
} > ()

api.getExpensebycategoryId(props.expense).then((response) => {
    expenses_list.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
})
api.getUserbyCollocId(user.colocationId).then((response) => {
    list_coloc.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
})

const getUsername = (id: string): string => {
    const user = list_coloc.value.find(coloc => coloc.id === id);
    return user ? user.username : 'Unknown';
};

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
</script>

<style scoped>
    .modal {
        width: 100%;
        height: 100vh;
        overflow-y: auto;
        margin-top: 0px;
        padding: 25pt 6%;
        animation: slideIn 0.4s;
        background-color: var(--overlay-background-light);
        backdrop-filter: blur(8px);
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
        display: flex;
        flex-direction: column;
        position: relative;
    }

    .modal-top-bar {
        display: flex;
        justify-content: space-between;
    }

    .dark .modal {
        background-color: var(--overlay-background-dark);
    }

    .icon {
        display: flex;
        align-items: center;
        justify-content: center;
        filter: invert(0);
    }

    .icon-inverse {
        display: flex;
        align-items: center;
        justify-content: center;
        filter: invert(1);
    }


    .modal-header {
        display: flex;
        justify-content: center;
        padding: 6%;
        font-weight: 600;
        border-bottom: none;
        color: var(--overlay-text);
        font-size: 32px;
    }

    .modal-background {
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 1000;
        position: fixed;
        animation: fadeIn 0.2s;
        display: flex;
        flex-direction: column;
        justify-content: flex-start;
    }

    .refund-rectangle {
        width: 100%;
        height: 100px;
        display: grid;
        grid-template-columns: 2fr 1fr;
        justify-content: space-between;
        align-items: center;
        padding: 0% 6%;
        color: var(--page-text-light);
        font-size: 20px;
        font-weight: 600;
        border-radius: 20px;
        margin: 10px 0px;
    }

    .from {
        background-color: var(--sent-message-light);
    }

    .dark .from {
        background-color: var(--sent-message-dark);
        color: var(--page-text-dark);
    }

    .to {
        background-color: var(--recieved-message-light);
    }

    .dark .to {
        background-color: var(--recieved-message-dark);
        color: var(--page-text-dark);
    }

    .others {
        background-color: var(--recieved-message-light);
    }

    .dark .others {
        background-color: var(--recieved-message-dark);
        color: var(--page-text-dark);
    }

    .number {
        display: flex;
        justify-content: center;
        font-size: 32px;
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


    button:disabled {
        opacity: 0.5;
    }

    /* Transition */
    .modal-enter-active,
    .modal-leave-active {
        transition: 0.2s ease;
    }

    .modal-enter-from,
    .modal-leave-to {
        opacity: 1;
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
            transform: translateX(600px);
        }

        100% {
            transform: translateX(0px);
        }
    }

    @keyframes slideOut {
        0% {
            transform: translateX(0px);
        }

        100% {
            transform: translateX(600px);
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