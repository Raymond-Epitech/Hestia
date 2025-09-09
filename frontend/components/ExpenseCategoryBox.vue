<template>
    <ExpensesListModal v-model="isExpenseModalOpen" :expense="expense.id" @proceed="handleProceed()" />
    <div class="expense" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openExpenseModal">
        <text class="category">{{ expense.name }}</text>
        <text class="regularize-text number">
            {{ expense.totalAmount }} €
        </text>
    </div>
</template>

<script setup>
const props = defineProps({
    expense: {
        type: Object,
        required: true,
    },
})
const isExpenseModalOpen = ref(false)
const openExpenseModal = () => (isExpenseModalOpen.value = true)

const emit = defineEmits(["proceed"])

const handleProceed = () => {
    close()
    emit('proceed')
}
</script>

<style scoped>
.expense {
    width: 90%;
    height: 88px;
    margin: 10px;
    padding: 10px;
    display: grid;
    align-items: center;
    grid-template-columns: 1fr 1fr;
    justify-content: space-between;
    background-color: var(--recieved-message);
    border-radius: 20px;
    box-shadow: var(--rectangle-shadow-light);
    color: var(--page-text);
}

.regularize-text {
    font-size: 24px;
    font-weight: 600;
}

.category {
    font-size: 20px;
    margin-left: 5px;
    font-weight: 600;
    text-align: left;
}

.number {
    font-size: 28px;
    margin-right: 5px;
    text-align: right;
}
</style>