<template>
  <transition name="modal">
    <div v-if="visible">
      <div class="modal-background" @click="handleClose">
        <div class="modal" @click.stop>
          <div class="modal-header left">
            <h1 class="modal-header-text">{{ $t('shopping-list-name') }} :</h1>
          </div>
          <form method="post" action="">
            <div class="modal-body left">
              <input class="modal-body-input" rows="1" maxlength="50" v-model="post.shoppinglistName"></input>
            </div>
            <div ref="itemList">
              <div v-for="(item, index) in item_list" :key="index">
                <shopping-item :item="item" @update:isChecked="updateIsChecked(item.id ?? '', $event)"
                  @update:name="updateName(item.id ?? '', $event)" @delete="deleteItem(item.id ?? '')" />
              </div>
            </div>
            <div @click.prevent="handleAddItem">
              <input v-model="newitemList.name" type="text" placeholder="Message" class="body-input" />
              <button type="submit" class="button">
                <img src="/Submit.svg" alt="Submit Icon" class="svg-icon" />
              </button>
            </div>
            <div v-if="post.shoppinglistName" class="modal-buttons">
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
import type { Reminder, ReminderItem } from '~/composables/service/type';
import useModal from '~/composables/useModal';
import { useUserStore } from '~/store/user';

const props = withDefaults(
  defineProps<{
    name?: string
    modelValue?: boolean
    header?: boolean
    buttons?: boolean
    borders?: boolean
    post?: Reminder
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
const prewiew = ref('');
const item_list = ref<ReminderItem[]>([]);
const Id = ref('');
const modify = ref(false);
const newitemList = ref<ReminderItem>({
  name: '',
  reminderId: '',
  createdBy: userStore.user.id,
  isChecked: false
})


const post = ref({
  colocationId: userStore.user.colocationId,
  createdBy: userStore.user.id,
  coordX: 0,
  coordY: 0,
  coordZ: 0,
  reminderType: 2,
  content: '',
  color: '',
  image: new File([], 'test.jpg'),
  shoppinglistName: '',
  pollInput: {
    title: '',
    description: '',
    expirationdate: '',
    isanonymous: false,
    allowmultiplechoice: false,
  }
})
const { modelValue } = toRefs(props)

const { open, close, toggle, visible } = useModal(props.name)

const emit = defineEmits<{
  closed: [],
  proceed: [],
  'update:modelValue': [value: boolean]
}>()

defineExpose({
  open,
  close,
  toggle,
  visible,
})

const resetPost = () => {
  prewiew.value = '';
  post.value = {
    colocationId: userStore.user.colocationId,
    createdBy: userStore.user.id,
    coordX: 0,
    coordY: 0,
    coordZ: 0,
    reminderType: 0,
    content: '',
    color: '',
    image: new File([], 'test.jpg'),
    shoppinglistName: '',
    pollInput: {
      title: '',
      description: '',
      expirationdate: '',
      isanonymous: false,
      allowmultiplechoice: false,
    }
  }
}

const handleClose = () => {
  resetPost()
  close()
  emit('closed')
}

const handleAddItem = () => {
  if (newitemList.value.name.trim() !== '') {
    item_list.value.push({ ...newitemList.value });
    newitemList.value.name = '';
  }
}

const handleProceed = async () => {
  if (modify) {
    api.updateReminder(post.value, Id.value).then((response) => {
      if (!response) {
        console.error(`Failed to update reminder ${Id.value}`);
        return;
      }
    });
    resetPost()
    close()
    emit('closed')
    return
  }
  const response = await api.addReminder(post.value)
  if (response != '') {
    item_list.value.forEach(async (item) => {
      item.reminderId = response;
      await api.addReminderShoppingListItem(item);
    });
    resetPost()
    close()
    emit('closed')
  }
}


watch(
  modelValue,
  (value, oldValue) => {
    if (value !== oldValue) {
      toggle(value)
    }
    emit('proceed')
  }
)

watch(visible, (value) => {
  emit('update:modelValue', value)
})

watch(() => props.post, (newPost, oldPost) => {
  if (newPost) {
    if (newPost) {
      console.log("post to modify", newPost);
      console.log("post avant modif", post.value);
      post.value.shoppinglistName = newPost.shoppingListName;
      Id.value = newPost.id;
      item_list.value = newPost.items;
      modify.value = true;
      console.log("id", Id.value)
    }
  }
});

const updateIsChecked = (id: string, value: boolean) => {
  const item = item_list.value?.find((item) => item.id === id);
  if (!modify && item) {
    item.isChecked = value;
  }
  if (item && modify) {
    item.isChecked = value;
    api.updateReminderShoppingListItem(item).then((response) => {
      if (!response) {
        console.error(`Failed to update item ${id}`);
        item.isChecked = !value;
        return;
      }
    }).catch((error) => {
      console.error(`Error updating item ${id}:`, error);
    });
  }
};

const updateName = (id: string, value: string) => {
  const item = item_list.value?.find((item) => item.id === id);
  if (!modify && item) {
    item.name = value;
  }
  if (item && modify) {
    item.name = value;
    api.updateReminderShoppingListItem(item).then((response) => {
      if (!response) {
        console.error(`Failed to update item ${id}`);
        window.location.reload();
        return;
      }
    }).catch((error) => {
      console.error(`Error updating item ${id}:`, error);
    });
  }
};

const deleteItem = (id: string) => {
  if (!modify) {
    item_list.value = item_list.value?.filter((item) => item.id !== id);
    return;
  } else {
    api.deleteReminderShoppingListItem(id).then((response) => {
      if (!response) {
        console.error(`Failed to delete item ${id}`);
        return;
      }
      item_list.value = item_list.value?.filter((item) => item.id !== id);
    });
  }
};

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
  color: var(--overlay-text);
  font-size: 18px;
  margin-bottom: 12px;
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
  color: var(--overlay-text);
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