<template>
  <div class="background">
    <div>
      <h1 class="header">
        <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
        <Texte_language source="header_modify_expense" />
        <p></p>
      </h1>
    </div>
    <form method="post" action="">
      <div>
        <h3>
          <Texte_language source="expense_name" />
        </h3>
        <input class="body-input" rows="3" v-model="expense.name" required />
        <h3>
          <Texte_language source="expense_amount" />
        </h3>
        <input class="body-input" type="number" v-model="expense.amount" placeholder="0.00" @input="filterNumericInput"
          min="0" required />
        <h3>
          <Texte_language source="expense_paid_by" />
        </h3>
        <select v-model="expense.paidBy" class="body-input">
          <option v-for="coloc in list_coloc" :key="coloc.id" :value="coloc.id">
            {{ coloc.username }}
          </option>
        </select>
        <h3>
          <Texte_language source="split_type" />
        </h3>
        <select v-model="expense.splitType" class="body-input">
          <option v-for="type in splitTypes" :key="type.value" :value="type.value">
            <Texte_language :source=type.label />
          </option>
        </select>
        <div v-if="expense.splitType == 0">
          <div class="checkbox-list">
            <label v-for="coloc in list_coloc" :key="coloc.id" class="checkbox-item">
              <input type="checkbox" :value="coloc.id" v-model="expense.splitBetween" />
              {{ coloc.username }}
              <input type="number" class="split-value-input"
                :value="expense.splitBetween.includes(coloc.id) ? calculatedSplitValue : 0" readonly />
            </label>
          </div>
        </div>
        <div v-if="expense.splitType == 1">
          <div class="checkbox-list">
            <label v-for="coloc in list_coloc" :key="coloc.id" class="checkbox-item">
              <input type="checkbox" :value="coloc.id" />
              {{ coloc.username }}
              <input type="number" class="split-value-input" v-model.number="expense.splitValues[coloc.id]"
                placeholder="0.00" min="0" />
            </label>
          </div>
        </div>
        <div v-if="expense.splitType == 2">
          <div class="checkbox-list">
            <label v-for="coloc in list_coloc" :key="coloc.id" class="checkbox-item">
              <input type="checkbox" :value="coloc.id" />
              {{ coloc.username }}
              <input type="number" class="split-value-input" v-model.number="expense.splitPercentages[coloc.id]"
                placeholder="0" min="0" max="100" />
              <input type="number" class="split-value-input" :value="calculateValueFromPercentage(coloc.id)" readonly />
            </label>
          </div>
        </div>
      </div>
      <div class="modal-buttons">
        <button class="button button-proceed" @click.prevent="handleProceed('modify')">
          <Texte_language source="modify" />
        </button>
        <button class="button button-proceed" @click.prevent="handleProceed('delete')">
          <Texte_language source="delete" />
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import type { Expense_Modif, Coloc } from '~/composables/service/type';
import { useUserStore } from '~/store/user';
import { useI18n } from '#imports';

const { t } = useI18n();
const userStore = useUserStore();
const user = userStore.user;
const route = useRoute();
const router = useRouter();
const id = route.query.id as string;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const date = new Date();
//a changer par les vrais valeur
const collocid = user.colocationId
const myid = user.id

const list_coloc = ref<Coloc[]>([]);
const splitTypes = [
  { value: 0, label: 'split_type0' },
  { value: 1, label: 'split_type1' },
  { value: 2, label: 'split_type2' },
];

const expense = ref<Expense_Modif>({
  id: id,
  colocationId: collocid,
  description: '',
  category: '',
  name: '',
  amount: 0,
  paidBy: list_coloc.value[0]?.id || '',
  splitType: 0,
  splitBetween: [],
  splitValues: {},
  splitPercentages: {},
  dateOfPayment: date.toISOString(),
})

api.getUserbyCollocId(collocid).then((response) => {
  list_coloc.value = response;
  Object.assign(expense.value, {
    colocationId: collocid,
    createdBy: myid,
    description: '',
    category: '',
    name: '',
    amount: 0,
    paidBy: list_coloc.value[0]?.id || '',
    splitType: 0,
    splitBetween: [],
    splitValues: {},
    splitPercentages: {},
    dateOfPayment: date.toISOString(),
  });
}).catch((error) => {
  console.error('Error fetching data:', error);
});

api.getExpenseById(id).then((response) => {
  const expenseData = response;
  Object.assign(expense.value, {
    id: expenseData.id,
    colocationId: expenseData.colocationId,
    createdBy: expenseData.createdBy,
    description: expenseData.description,
    category: expenseData.category,
    name: expenseData.name,
    amount: expenseData.amount,
    paidBy: expenseData.paidBy,
    splitType: expenseData.splitType,
    dateOfPayment: date.toISOString(),
  });
  if (expenseData.splitType === 0) {
    expense.value.splitBetween = Object.keys(expenseData.splitBetween);;
  } else if (expenseData.splitType === 1) {
    expense.value.splitValues = expenseData.splitBetween;
  } else if (expenseData.splitType === 2) {
    expense.value.splitPercentages = expenseData.splitBetween;
  }
}).catch((error) => {
  console.error('Error fetching data:', error);
});

const calculateValueFromPercentage = (colocId: string) => {
  const percentage = expense.value.splitPercentages[colocId] || 0;
  return ((percentage / 100) * expense.value.amount).toFixed(2);
};

const calculatedSplitValue = computed(() => {
  const numPeople = expense.value.splitBetween.length;
  return numPeople > 0 ? (expense.value.amount / numPeople).toFixed(2) : 0;
});

const handleProceed = async (action: string) => {
  if (action === 'modify') {
    await api.updateExpense(expense.value);
    router.back()
  } else if (action === 'delete') {
    const confirmed = window.confirm(t('confirm_delete_expense'));
    if (confirmed) {
      await api.deleteExpense(id);
      router.back();
    }
  }
}

const filterNumericInput = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const value = target.value;
  const filteredValue = value.replace(/[^0-9.]/g, '').replace(/(\..*?)\..*/g, '$1');

  if (filteredValue !== value) {
    target.value = filteredValue;
  }
  if (!isNaN(parseFloat(filteredValue))) {
    expense.value.amount = parseFloat(filteredValue);
  } else {
    expense.value.amount = 0;
  }
};
</script>

<style>
.background {
  height: 100vh;
  background-color: #1E1E1E;
  color: white;
  padding: 20px;
}

.body-input {
  width: 50%;
  background-color: #1e1e1e00;
  outline: none;
  border: none;
  line-height: 3ch;
  background-image: linear-gradient(transparent, transparent calc(3ch - 1px), #E7EFF8 0px);
  background-size: 100% 3ch;
  color: #fff;
  font-size: 18px;
}

.split-value-input {
  width: 25%;
  background-color: #1e1e1e00;
  outline: none;
  border: none;
  line-height: 3ch;
  background-image: linear-gradient(transparent, transparent calc(3ch - 1px), #E7EFF8 0px);
  background-size: 100% 3ch;
  color: #fff;
  font-size: 18px;
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

/* Masquer les boutons de type "spinner" pour les navigateurs Webkit (Chrome, Safari, Edge) */
input[type="number"]::-webkit-inner-spin-button,
input[type="number"]::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

/* Masquer les boutons de type "spinner" pour Firefox */
input[type="number"] {
  -moz-appearance: textfield;
}

.checkbox-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.checkbox-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
</style>