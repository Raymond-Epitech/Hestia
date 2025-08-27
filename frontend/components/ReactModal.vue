<template>
  <transition name="modal">
    <div v-if="visible">
      <div class="modal-background" @click="handleClose">
        <div class="modal" @click.stop>
          <div v-for="emoji in ['❤️', '😂', '😮', '😢', '😡', '👍', '👎', '🎉', '💡', '🔥']" :key="emoji">
            <span class="emoticon" @click="handleReaction(emoji)">{{ emoji }}</span>
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

.modal {
    position: relative;
    top: 0.15rem;
    left: 0;
    width: 98%;
    height: 98%;
    display: flex;
    align-items: center;
    z-index: 1000;
    white-space: nowrap;
    overflow-x: scroll;
    border-radius: 20px 20px 20px 20px;
}

.modal-background {
    position: relative;
    top: 13rem;
    left: 0.25rem;
    width: 100%;
    height: 3rem;
    background-clip:content-box;
    background-color: var(--main-buttons-light);
    box-shadow: var(--box-shadow-light);
    border-radius: 20px 20px 20px 20px;
    display: flex;
    justify-content: center;
    z-index: 1000;
}

.dark .modal-background {
    background-color: var(--main-buttons-dark);
    /* box-shadow: var(--box-shadow-dark); */
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
</style>