<template>
    <transition name="modal">
      <div v-if="visible">
        <div class="modal-background" @click="handleClose">
          <div class="modal" @click.stop>
            <div class="modal-header left">
              <h1 class="modal-header-text"><Texte_language source="header_add_expense" /></h1>
            </div>
            <form method="post" action="">
              <div class="modal-body left">
                <h3><Texte_language source="expense_name" /></h3>
                <input class="modal-body-input" rows="3" v-model="expense.name" required/>
                <h3><Texte_language source="expense_amount" /></h3>
                <input
                class="modal-body-input"
                type="number"
                v-model="expense.amount"
                placeholder="0.00"
                @input="filterNumericInput"
                min="0"
                required
              />
              </div>
              <div class="modal-buttons">
                <button class="button button-proceed" @click.prevent="handleProceed">Poster</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </transition>
  </template>
  
  <script setup lang="ts">
  import useModal from '~/composables/useModal';
  
  const props = withDefaults(
    defineProps<{
      name?: string
      modelValue?: boolean
      header?: boolean
      buttons?: boolean
      borders?: boolean
    }>(),
    {
      header: true,
      buttons: true,
      borders: true,
    }
  )
  
  const { $bridge } = useNuxtApp()
  const api = $bridge;
  
  const expense = ref({
    colocationId: "d6c34b10-e6dc-472e-8047-da3a89d44eae",
    createdBy: '',
    name: '',
    amount: 0,
    paidBy: '',
    splitType: 0,
    splitBetween: [],
    splitValues: {},
    splitPercentages: {},
    dateOfPayment: new Date(),
  })
  
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
    Object.assign(expense.value, {
        colocationId: "d6c34b10-e6dc-472e-8047-da3a89d44eae",
        createdBy: '',
        name: '',
        amount: 0,
        paidBy: '',
        splitType: 0,
        splitBetween: [],
        splitValues: {},
        splitPercentages: {},
        dateOfPayment: new Date(),
    });
    close()
    emit('closed')
  }
  
  const handleProceed = async () => {
    close()
    emit('proceed')
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
  .modal {
    width: 100%;
    height: 600px;
    overflow-y: auto;
    margin-top: 0px;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
    border-bottom-left-radius: 20px;
    border-bottom-right-radius: 20px;
    animation: slideIn 0.2s;
    background-color: #1e1e1eda;
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
  
  .post-colors-buttons {
    padding: 8px;
    display: flex;
    justify-content: space-evenly;
  }
  
  .color-choice {
    width: 24px;
    height: 24px;
    border: none;
  }
  
  .color-choice:focus {
    box-shadow: none;
  }
  
  .blue {
    background-color: #A8CBFF;
  }
  
  .blue:checked {
    background-color: #A8CBFF;
  }
  
  .yellow {
    background-color: #FFF973;
  }
  
  .yellow:checked {
    background-color: #FFF973;
  }
  
  .pink {
    background-color: #FFA3EB;
  }
  
  .pink:checked {
    background-color: #FFA3EB;
  }
  
  .green {
    background-color: #9CFFB2;
  }
  
  .green:checked {
    background-color: #9CFFB2;
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
      border-bottom-left-radius: 20px;
      border-bottom-right-radius: 20px;
    }
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
  </style>