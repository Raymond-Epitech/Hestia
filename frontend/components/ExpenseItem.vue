<template>
    <div class="expense">
      <div class="expense-header">
        <span class="expense-name">{{ expense.name }}</span>
        <span class="expense-amount">{{ expense.amount }} €</span>
      </div>
      <div class="payer">
        <span>Payé par {{ paidBy }}</span>
      </div>
    </div>
  </template>
  
  <script setup lang="ts">
  import type { Expenseget } from '~/composables/service/type';

  const props = defineProps<{
  expense: Expenseget;
  paidBy: string;
  }>();
  </script>
  
  <style scoped>
  .expense {
    border-bottom: 2px solid #ddd;
    padding: 10px 0;
  }
  
  .expense-header {
    display: flex;
    justify-content: space-between;
    font-weight: bold;
  }
  
  .payer {
    color: rgb(214, 207, 207);
  }
  </style>