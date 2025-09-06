<template>
    <transition name="modal">
        <div v-if="visible">
            <div class="modal-background" @click="handleClose">
                <div class="modal" @click.stop>
                    <div class="modal-header">
                        <Texte_language source="header_add_expense" />
                    </div>
                    <form class="form-container" method="post" action="">
                        <div>
                            <div class="name-expense-container">
                                <input class="name-input" maxlength="40" v-model="expense.name"  :placeholder="$t('expense_name')" required />
                                <div class="expense-container">
                                    <input class="expense-input" type="number" v-model="expense.amount"
                                        @input="filterNumericInput" min="0" required />
                                    <text>€</text>
                                </div>
                            </div>
                            <h3 class="subtext">
                                <Texte_language source="expense_paid_by" /> :
                            </h3>
                            <select v-model="expense.paidBy" class="drop-down-input name">
                                <option v-for="coloc in list_coloc" :key="coloc.id" :value="coloc.id">
                                    {{ coloc.username }}
                                </option>
                            </select>
                            <h3 class="subtext">
                                <Texte_language source="split_type" /> :
                            </h3>
                            <select v-model="expense.splitType" class="drop-down-input type">
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
                                            :value="expense.splitBetween.includes(coloc.id) ? calculatedSplitValue : 0"
                                            readonly />
                                    </label>
                                </div>
                            </div>
                            <div v-if="expense.splitType == 1">
                                <div class="checkbox-list">
                                    <label v-for="coloc in list_coloc" :key="coloc.id" class="checkbox-item">
                                        <input type="checkbox" :value="coloc.id" />
                                        {{ coloc.username }}
                                        <input type="number" class="split-value-input"
                                            v-model.number="expense.splitValues[coloc.id]" placeholder="0.00" min="0" />
                                    </label>
                                </div>
                            </div>
                            <div v-if="expense.splitType == 2">
                                <div class="checkbox-list">
                                    <label v-for="coloc in list_coloc" :key="coloc.id"
                                        class="checkbox-item-pourcentage">
                                        <input class="check-zone" type="checkbox" :value="coloc.id" />
                                        {{ coloc.username }}
                                        <div class="split-value-container">
                                            <input type="number" class="split-value-input"
                                                v-model.number="expense.splitPercentages[coloc.id]" placeholder="0"
                                                min="0" max="100" />
                                            <span class="percentage-symbol">%</span>
                                        </div>
                                        <input type="number" class="split-value-input"
                                            :value="calculateValueFromPercentage(coloc.id)" readonly />
                                    </label>
                                </div>
                            </div>
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
import useModal from '~/composables/useModal';
import { useUserStore } from '~/store/user';
import type { Expense, Coloc } from '~/composables/service/type';

const props = withDefaults(
    defineProps < {
        name?: string,
        modelValue?: boolean,
        header?: boolean,
        buttons?: boolean,
        borders?: boolean,
        categoryId: string,
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
api.setjwt(useCookie('token').value ?? '');
const date = new Date();
const list_coloc = ref<Coloc[]>([]);
const splitTypes = [
  { value: 0, label: 'split_type0' },
  { value: 1, label: 'split_type1' },
  { value: 2, label: 'split_type2' },
];

const expense = ref<Expense>({
  colocationId: user.colocationId,
  expenseCategoryId: props.categoryId,
  createdBy: '',
  name: '',
  description: '',
  amount: 0,
  category: '',
  paidBy: list_coloc.value[0]?.id || '',
  splitType: 0,
  splitBetween: [],
  splitValues: {},
  splitPercentages: {},
  dateOfPayment: date.toISOString(),
})

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

api.getUserbyCollocId(user.colocationId).then((response) => {
  list_coloc.value = response;
  Object.assign(expense.value, {
    colocationId: user.colocationId,
    expenseCategoryId: props.categoryId,
    createdBy: user.id,
    description: '',
    category: name,
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

const calculateValueFromPercentage = (colocId: string) => {
  const percentage = expense.value.splitPercentages[colocId] || 0;
  return ((percentage / 100) * expense.value.amount).toFixed(2);
};

const calculatedSplitValue = computed(() => {
  const numPeople = expense.value.splitBetween.length;
  return numPeople > 0 ? (expense.value.amount / numPeople).toFixed(2) : 0;
});

const handleProceed = async () => {
  api.addExpense(expense.value).then((response) => {
    if (response === true) {
      close()
      emit('proceed')
       Object.assign(expense.value, {
        colocationId: user.colocationId,
        expenseCategoryId: props.categoryId,
        createdBy: user.id,
        description: '',
        category: props.name,
        name: '',
        amount: 0,
        paidBy: list_coloc.value[0]?.id || '',
        splitType: 0,
        splitBetween: [],
        splitValues: {},
        splitPercentages: {},
        dateOfPayment: date.toISOString(),
      });
    }
  }).catch((error) => {
    console.error('Error adding expense:', error);
  });
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
    padding: 0px 5% 10px;
    font-weight: 600;
    font-size: 24px;
    border-bottom: none;
    color: var(--overlay-text);
}

.modal-background {
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 1001;
    position: fixed;
    animation: fadeIn 0.2s;
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
}

h3 {
  font-weight: 600;
  margin-left: 2px;
  font-size: 16px;
}

.form-container {
  width: 90%;
  margin-left: 5%;
  margin-right: 5%;
}

.name-expense-container {
  display: grid;
  grid-template-columns: 7fr 2fr;
  justify-content: center;
  align-content: center;
}

.name-input {
  height: 30px;
  width: 90%;
  margin-top: 8px;
  box-sizing: border-box;
  border: none;
  background-color: inherit;
  border-bottom: 2px dotted var(--list-lines);
  outline: none;
  color: var(--overlay-text);
  font-weight: 600;
}

.expense-container {
  height: 62px;
  margin-left: 10px;
  width: 100%;
  display: grid;
  grid-template-columns: 3fr 1fr;
  align-items: center;
  background-color: var(--main-buttons);
  color: var(--page-text);
  border-radius: 9px;
  font-size: 18px;
  padding-right: 6px;
}

.expense-input {
  height: 62px;
  width: 100%;
  background-color: var(--main-buttons);
  color: var(--page-text);
  outline: none;
  border: none;
  border-radius: 9px;
  text-align: center;
  font-weight: 600;
}

.subtext {
  margin-bottom: 15px;
  text-align: left;
  font-size: 16px;
  color: var(--overlay-text);
}

.drop-down-input {
  width: 100%;
  height: 30px;
  margin-bottom: 15px;
  padding-left: 10px;
  border-radius: 9px;
  border: none;
  font-weight: 600;
  font-size: 12px;
}

.name {
  background-color: var(--sent-message);
  color: var(--page-text);
}

.type {
  background-color: var(--recieved-message);
  color: var(--page-text);
}

.split-value-container {
  display: flex;
  align-items: center;
  gap: 5px;
}

.split-value-input {
  width: 90%;
  height: 30px;
  background-color: var(--recieved-message);
  outline: none;
  border: none;
  line-height: 3ch;
  background-size: 100% 3ch;
  color: var(--page-text);
  font-size: 16px;
  text-align: end;
}

.percentage-symbol {
  color: var(--page-text);
  font-size: 16px;
  font-weight: 600;
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
  font-weight: 600;
  text-align: left;
  font-size: 12px;
}

.checkbox-item {
  width: 100%;
  height: 30px;
  display: grid;
  grid-template-columns: 1fr 7fr 3fr;
  align-items: center;
  padding-left: 4px;
  border-radius: 9px;
  background-color: var(--recieved-message);
  color: var(--page-text);
}

.checkbox-item-pourcentage {
  width: 100%;
  height: 30px;
  display: grid;
  grid-template-columns: 2fr 11fr 4fr 6fr;
  align-items: center;
  justify-content: space-between;
  padding-left: 4px;
  border-radius: 9px;
  background-color: var(--recieved-message);
  color: var(--page-text);
}

input[type="checkbox"] {
  /* Add if not using autoprefixer */
  -webkit-appearance: none;
  appearance: none;
  /* Not removed via appearance */
  margin: 0;
  width: 20px;
  height: 20px;
  border: 3px solid;
  border-color: var(--basic-purple);
  border-radius: 50%;
  cursor: pointer;
  transition: background-color 0.3s;
}

input[type="checkbox"]:checked {
  background-color: var(--basic-green);
  border-color: var(--basic-green);
}

.modal-buttons {
  height: 48px;
  padding: 12px 4px;
  border-top: 0px;
  border-bottom-left-radius: 20px;
  border-bottom-right-radius: 20px;
  display: flex;
  justify-content: right;
  gap: 1em;
}

/** Fallback Buttons */
.button-proceed {
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