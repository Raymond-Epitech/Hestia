<template>
    <RefundModal v-model="isRefundModalOpen" :refund="refund" @proceed="handleProceed()" />
    <div class="refund-rectangle to" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openRefundModal">
        <text>{{ $t('Due_to') }} {{ refund.toUsername }} </text>
        <text class="number">
            {{ refund.amount}} €
        </text>
    </div>
</template>

<script setup>
const props = defineProps({
    refund: {
        type: Object,
        required: true,
    },
})
const isRefundModalOpen = ref(false)
const openRefundModal = () => (isRefundModalOpen.value = true)

const emit = defineEmits(["proceed"])

const handleProceed = () => {
    close()
    emit('proceed')
}
</script>

<style scoped>
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

    .to {
        background-color: var(--recieved-message-light);
    }

    .dark .to {
        background-color: var(--recieved-message-dark);
        color: var(--page-text-dark);
    }

    .number {
        display: flex;
        justify-content: right;
        font-size: 22px;
    }
</style>