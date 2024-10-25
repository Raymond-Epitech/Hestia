<template>
    <transition name="modal">
      <div v-if="visible">
        <div class="modal-background" @click="handleClose">
          <div class="modal" @click.stop>
            <div class="modal-header left">
              <h1>Contenu du post : </h1>
            </div>

            <div class="modal-body left">
              <form>
                <textarea class="modal-body-input" rows="5" required></textarea>
              </form>
            </div>

            <!-- Buttons -->
            <div class="modal-buttons">
                <!-- <button class="button button-cancel" @click="handleClose">Cancel</button> -->
                <button class="button button-proceed" @click="handleProceed">Yes, Proceed</button>
            </div>
          </div>
        </div>
      </div>
    </transition>
  </template>

  <script setup lang="ts">
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
    close()
    emit('closed')
  }

  const handleProceed = () => {
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
  </script>

  <style>
  .modal {
    width: 100%;
    height: 334px;
    overflow-y: auto;
    margin-top: 0px;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
    border-bottom-left-radius: 20px;
    border-bottom-right-radius: 20px;
    animation: slideIn 0.2s;
    background-color: #1E1E1E;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
    display: flex;
    flex-direction: column;
    position: relative;
  }

  .modal-header {
    padding: 20px 16px;
    font-weight: 600;
    border-bottom: 1px solid lightgrey;
    color: #fff;
  }

  .modal-body {
    padding: 16px;
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
    background-color: #1E1E1E;
    outline: none;
    border: none;
    line-height: 4ch;
    background-image: linear-gradient(transparent, transparent calc(4ch - 1px), #E7EFF8 0px);
    background-size: 100% 4ch;
    color: #fff;
    font-size: 20px;
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
    padding: 16px;
    border-top: 0px;
    border-bottom-left-radius: 20px;
    border-bottom-right-radius: 20px;
    margin-top: auto;
    display: flex;
    justify-content: space-between;
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
    border-radius: 4px;
    border: 0;
    cursor: pointer;
  }

  .button-cancel {
    background: transparent;
  }

  .button-cancel:hover {
    background: #fafafa;
  }

  .button-proceed {
    background: #000;
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
  </style>