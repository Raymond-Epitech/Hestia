<template>
  <transition name="modal">
    <div v-if="visible">
      <div class="modal-background" @click="handleClose">
        <div class="modal-container">
          <div class="modal" @click.stop>
            <span class="delete" @click="deleteReaction()">❌</span>
            <div v-for="emoji in ['❤️', '😂', '😮', '😢', '😡', '👍', '👎', '🎉', '💡', '🔥']" :key="emoji">
              <span class="emoticon" @click="handleReaction(emoji)">{{ emoji }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup lang="ts">
import useModal from '~/composables/useModal';
import { useUserStore } from '~/store/user';

const props = withDefaults(
  defineProps < {
  name?: string
  postId?: string
  reaction?: string
  modelValue?: boolean
  } > (), {
    postId: '',
  })

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');

const { modelValue } = toRefs(props)

const { open, close, toggle, visible } = useModal(props.name)

const emit = defineEmits < {
    closed: []
    'update:modelValue': [value: boolean]
} > ()

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

const handleReaction = async (emoji: string) => {
    if (!props.postId) return;
    await api.addReactionReminder(props.postId, userStore.user.id, emoji).then(() => {
        close()
        emit('closed')
    }).catch((error: any) => {
        console.error('Error adding reaction:', error);
    });
}

const deleteReaction = async () => {
    await api.deleteReactionReminder(props.postId, userStore.user.id);
    close()
    emit('closed')
}

watch(visible, (value) => {
    emit('update:modelValue', value)
})

watch(
    modelValue,
    (value, oldValue) => {
        if (value !== oldValue) {
            toggle(value)
        }
    },
    { immediate: true }
)

</script>

<style scoped>

.emoticon {
    font-size: 26px;
    /* transition: transform 0.2s; */
}

.delete {
  text-align: center;
    min-width: 30px;
    height: 30px;
    font-size: 20px;
    border-radius: 50%;
    background-color: var(--basic-grey);
}

.modal {
    position: relative;
    margin: 0px 6px;
    display: flex;
    align-items: center;
    z-index: 1000;
    white-space: nowrap;
    overflow-x: scroll;
    border-radius: 20px 20px 20px 20px;
}

.modal-background {
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 1000;
    position: absolute;
    animation: fadeIn 0.2s;
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
}

.modal-container {
    position: absolute;
    padding: 4px 0px;
    bottom: 0px;
    right: 0px;
    width: 98%;
    height: 3rem;
    background-clip:content-box;
    background-color: var(--main-buttons);
    box-shadow: var(--rectangle-shadow-light);
    border-radius: 20px 20px 20px 20px;
    display: flex;
    justify-content: center;
    z-index: 1000;
}

.modal-enter-active,
.modal-leave-active {
    transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter,
.modal-leave-to {
    opacity: 0;
    /* transform: translateY(10px) scale(0.95); */
}

.modal-behind {
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
}
</style>