<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" @click.stop>
                    <div class="modal-header left">
                        <h1 class="modal-header-text">{{ $t('refund') }} :</h1>
                    </div>
                    <div class="modal-buttons">
                        <button class="button" @click.prevent="handleRefund"> {{ $t('refund') }}</button>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import useModal from '~/composables/useModal';
import { useUserStore } from '~/store/user';
import type { refund, Expense } from '~/composables/service/type';

const props = withDefaults(
    defineProps < {
        name?: string,
        modelValue?: boolean,
        header?: boolean,
        buttons?: boolean,
        borders?: boolean,
        refund: refund,
    } > (),
    {
        header: true,
        buttons: true,
        borders: true,
    }
)
const { $bridge } = useNuxtApp()
const api = $bridge;
const userStore = useUserStore();
const user = userStore.user;
let rufendcategoryId = '';

const { modelValue } = toRefs(props)
const { open, close, toggle, visible } = useModal(props.name)

const emit = defineEmits < {
    closed: [], // named tuple syntax
    proceed: [],
    'update:modelValue': [value: boolean]
} > ()

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

const handleRefund = async () => {
    await api.getExpenseByColocationId(user.colocationId).then((response) => {
    response.forEach((expense) => {
        if (expense.name === "Refund") {
            rufendcategoryId = expense.id;
        }
    });
    }).catch((error) => {
        console.error('Error fetching expenses:', error);
    });
    const data: Expense = {
        colocationId: user.colocationId,
        createdBy: user.id,
        name: "Refund",
        description: "Remboursement de " + props.refund.amount + "€",
        amount: props.refund.amount,
        category: "Refund",
        paidBy: props.refund.from,
        splitType: 0,
        splitBetween: [props.refund.to],
        splitValues: {},
        splitPercentages: {},
        dateOfPayment: new Date().toISOString(),
        expenseCategoryId: rufendcategoryId,
    }
    api.addExpense(data).then((response) => {
        close()
        emit('proceed')
    }).catch((error) => {
        console.error('Error adding expense:', error);
    });
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
        background-color: #1e1e1eda;
        backdrop-filter: blur(8px);
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
        display: flex;
        flex-direction: column;
        justify-content: center;
        position: relative;
    }

    .modal-header {
        padding: 0px 16px;
        font-weight: 600;
        border-bottom: none;
        color: var(--overlay-text);
    }

    .modal-header-text {
        font-size: 20px;
    }

    .modal-body {
        padding: 0px 12px 12px 12px;
        display: flex;
        flex-direction: column;
        overflow: auto;
        gap: 16px;
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