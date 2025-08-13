<template>
  <transition name="modal">
    <div v-if="visible">
      <div class="modal-background" @click="handleClose">
        <div class="modal" @click.stop>
          <div class="modal-header left">
            <h1 class="modal-header-text">{{ $t('post-content') }} :</h1>
          </div>
          <form method="post" action="">
            <div class="modal-body left">
              <textarea class="modal-body-input" rows="3" maxlength="150" v-model="post.content" required></textarea>
            </div>
            <div class="modal-header left">
              <h1 class="modal-header-text">{{ $t('post-color') }} :</h1>
            </div>
            <div class="post-colors-buttons">
              <input class="form-check-input color-choice blue" v-model="post.color" type="radio" name="gridRadios"
                id="gridRadios1" value="blue" required>
              <input class="form-check-input color-choice yellow" v-model="post.color" type="radio" name="gridRadios"
                id="gridRadios2" value="yellow">
              <input class="form-check-input color-choice pink" v-model="post.color" type="radio" name="gridRadios"
                id="gridRadios3" value="pink">
              <input class="form-check-input color-choice green" v-model="post.color" type="radio" name="gridRadios"
                id="gridRadios4" value="green">
            </div>
            <div v-if="post.color && post.content" class="modal-buttons">
              <button class="button button-proceed" @click.prevent="handleProceed">{{ $t('poster') }}</button>
            </div>
            <div v-else class="modal-buttons">
              <button class="button button-proceed" @click.prevent="handleProceed" disabled>{{ $t('poster') }}</button>
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

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');

const post = ref({
  createdBy: userStore.user.id,
  content: '',
  color: '',
  coordX: 0,
  coordY: 0,
  coordZ: 0,
  colocationId: userStore.user.colocationId,
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

const resetPost = () => {
  post.value = {
    createdBy: userStore.user.id,
    content: '',
    color: '',
    coordX: 0,
    coordY: 0,
    coordZ: 0,
    colocationId: userStore.user.colocationId,
  }
}

const handleClose = () => {
  resetPost()
  close()
  emit('closed')
}

const handleProceed = async () => {
  const response = await api.addReminder(post.value)
  if (response) {
    resetPost()
    close()
  }
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
  color: #fff;
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

.modal-body:deep(p) {
  margin: 0;
  font-size: 18px;
  line-height: 23px;
}

.modal-body-input {
  width: 100%;
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