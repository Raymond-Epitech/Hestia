<template>
  <transition name="modal">
    <div v-if="visible">
      <div class="modal-background" @click="handleClose">
        <div class="modal" @click.stop>
          <div class="modal-header left">
            <input class="modal-body-input" rows="1" maxlength="50" v-model="post.shoppinglistName"
              :placeholder="$t('shopping-list-name')"></input>
            <button v-if="modify" src="/Trash.svg" alt="Delete Icon" @click="showPopup">
              <img src="/Trash.svg" alt="Delete Icon" class="svg-icon" />
            </button>
          </div>
          <form method="post" action="" @submit.prevent="handleSubmit">
            <div ref="itemList" class="item-list">
              <div v-for="(item, index) in item_list" :key="index">
                <shopping-item :item="item" @update:isChecked="updateIsChecked(item.id ?? '', $event)"
                  @update:name="updateName(item.id ?? '', $event)" @delete="deleteItem(item.id ?? '')" />
              </div>
            </div>
            <div @click.prevent="handleAddItem" class="form-add-item">
              <input v-model="newitemList.name" type="text" placeholder="Item" maxlength="18"
                class="modal-body-input" /><!-- !!!! add locale !!! -->
              <button v-if="newitemList.name == ''" :disabled="true" type="submit">
                <img src="/Submit.svg" alt="Submit Icon" class="svg-icon submit" />
              </button>
              <button v-if="newitemList.name != ''" type="submit">
                <img src="/Submit.svg" alt="Submit Icon" class="svg-icon submit" />
              </button>
            </div>
          </form>
        </div>
      </div>
      <popup v-if="popup_vue" :text="$t('confirm_delete_shoppinglist')" @confirm="confirmDelete"
        @close="cancelDelete" />
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
const popup_vue = ref(false)
const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const prewiew = ref('');
const item_list = ref<ReminderItem[]>([]);
const Id = ref('');
const modify = ref(false);
const oldshoppinglistName = ref('');
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
  }
  item_list.value = [];
  Id.value = '';
  modify.value = false;
}

const createList = async () => {
  const response = await api.addReminder(post.value)
  if (response != '') {
    Id.value = response;
    modify.value = true;
    return true;
  }
  return false;
}

const handleClose = async () => {
  if (modify.value && oldshoppinglistName.value != post.value.shoppinglistName) {
    api.updateReminder(post.value, Id.value).then((response) => {
      if (!response) {
        console.error(`Failed to update reminder ${Id.value}`);
        return;
      }
    });
  }
  if (!modify.value && post.value.shoppinglistName != '') {
    const response = await createList();
    if (!response) {
      console.error('Failed to create list');
      return;
    }
  }
  resetPost()
  close()
  emit('closed')
}

const handleAddItem = async () => {
  if (newitemList.value.name.trim() !== '') {
    if (!modify.value) {
      const response = await createList();
      if (!response) {
        console.error('Failed to create list');
        return;
      }
    }
    newitemList.value.reminderId = Id.value;
    const newID = await api.addReminderShoppingListItem(newitemList.value);
    if (newID) {
      newitemList.value.id = newID;
    }
    item_list.value.push({ ...newitemList.value });
    newitemList.value.name = '';
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
    console.log("post to modify", newPost);
    console.log("post avant modif", post.value);
    post.value.shoppinglistName = newPost.shoppingListName;
    oldshoppinglistName.value = newPost.shoppingListName;
    Id.value = newPost.id;
    item_list.value = newPost.items;
    modify.value = true;
    console.log("id", Id.value)
  }
});

const updateIsChecked = (id: string, value: boolean) => {
  const item = item_list.value?.find((item) => item.id === id);
  if (!modify.value && item) {
    item.isChecked = value;
  }
  if (item && modify.value) {
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
  if (!item) return;
  item.name = value;
  console.log("update name", item);
  api.updateReminderShoppingListItem(item).then((response) => {
    if (!response) {
      console.error(`Failed to update item ${id}`);
      return;
    }
  }).catch((error) => {
    console.error(`Error updating item ${id}:`, error);
  });
};

const deleteItem = (id: string) => {
  if (!modify.value) {
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

const handleSubmit = () => {
  return;
};

const showPopup = () => {
  popup_vue.value = true;
};

const confirmDelete = async () => {
  popup_vue.value = false;
  api.deleteReminder(Id.value).then((response) => {
    if (!response) {
      console.error(`Failed to delete reminder ${Id.value}`);
      return;
    }
    resetPost()
    close()
    emit('closed')
  });
};

const cancelDelete = () => {
  popup_vue.value = false;
};

</script>

<style scoped>
.modal {
  width: 100%;
  font-weight: 600;
  height: fit-content;
  max-height: 30rem;
  overflow-y: auto;
  padding-top: 1.5rem;
  padding-bottom: 1.2rem;
  border-top-left-radius: 20px;
  border-top-right-radius: 20px;
  animation: slideIn 0.4s;
  background-color: var(--main-buttons);
  box-shadow: var(--rectangle-shadow-light);
  display: flex;
  flex-direction: column;
  position: relative;
  top: -4.5rem;
}

.modal-header {
  font-size: 10px;
  padding: 0;
  border-bottom: none;
  color: var(--page-text);
}

.modal-body {
  padding: 0px;
  display: flex;
  flex-direction: column;
  overflow: auto;
  gap: 16px;
}

.modal-body-input {
  background-color: var(--main-buttons);
  width: 13rem;
  height: 2.2rem;
  border-radius: 9px;
  border-style: hidden;
  padding: 0.5rem;
  margin: 0.5rem;
  color: var(--page-text);
  outline: none;
  border: none;
  line-height: 3ch;
  background-image: linear-gradient(transparent, transparent calc(3ch - 1px), var(--list-lines) 0px);
  background-size: 100% 3ch;
  font-size: 14px;
}

.modal-background {
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 11;
  display: flex;
  position: fixed;
  align-items: flex-end;
  flex-direction: column-reverse;

}

.item-list {
  max-height: 13rem;
  overflow-y: scroll;
  padding-left: 0.5rem;
  padding-right: 0.5rem;
}

.form-add-item {
  display: flex;
  align-items: center;
  padding: 0.5rem 0;
}

.svg-icon {
  width: 25px;
  height: 25px;
}

.svg-icon.submit {
  filter: var(--icon-filter);
}


button {
  padding: 10px 20px;
  border-radius: 15px;
  border: 0;
  cursor: pointer;
  display: flex;
  justify-content: center;
  padding: 14px;
  border-top: 0px;
  gap: 1em;
  margin: 0.5rem;
  background: var(--main-buttons);
  color: var(--page-text);
  font-weight: 600;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
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
    transform: translateY(600px);
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
    transform: translateY(600px);
  }
}

@media screen and (max-width: 768px) {

  /** Slide Out Transition (mobile only) */
  .modal-enter-from:deep(.modal),
  .modal-leave-to:deep(.modal) {
    animation: slideOut 0.4s linear;
  }
}
</style>