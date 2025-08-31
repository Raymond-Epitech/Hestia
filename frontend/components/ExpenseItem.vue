<template>
  <ModifyExpenseModal v-model="isModalOpen" :key="expense.id" :expense="expense" @open="emitOpen()" @proceed="emitProceed()"/>
  <div class="expense" @click="openModal">
    <div class="expense-header">
      <div class="dot-container">
        <div class="dot"/>
      </div>
      <span class="expense-name">{{ expense.name }}</span>
      <span class="expense-amount">{{ expense.amount }} €</span>
    </div>
    <div class="payer">
      <span>
        <TexteLanguage source="paidby" /> {{ paidBy }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Expenseget } from '~/composables/service/type';

const props = defineProps<{
  expense: Expenseget;
  paidBy: string;
  onclick?: () => void;
}>();

const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)
const emit = defineEmits(['proceed', 'get', 'open'])

function emitProceed() {
    emit('proceed')
}

function emitOpen() {
  emit('open')
}

</script>

<style scoped>
.expense {
  color: var(--overlay-text);
  text-align: left;
  border-bottom: 1px solid var(--list-lines-light);
  padding: 4px 0;
}

.dark .expense {
  border-bottom: 1px solid var(--list-lines-dark);
}

.expense-header {
  display: grid;
  grid-template-columns: 1fr 9fr 2fr;
  font-weight: bold;
  font-size: 20px;
}

.dot-container {
  display: flex;
  justify-content: center;
  align-items: center;
}

.dot {
  width: 6px;
  background-color: var(--overlay-text);
  height: 6px;
  border-radius: 50%;
}

.expense-amount {
  text-align: right;
}
</style>