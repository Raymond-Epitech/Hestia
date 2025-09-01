<template>
  <div>
    <AddPostModal v-model="isModalOpen" @proceed="getall()" />
    <AddImageModal v-model="isModalImageOpen" @proceed="getall()" />
    <AddListModal v-model="isModalListOpen" @proceed="getall()" />
    <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
      <img src="~/public/posts/Post.svg" class="post">
    </button>
    <button class="add-image" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openImageModal">
      <img src="~/public/posts/Camera.svg" class="post">
    </button>
    <button class="add-list" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openListModal">
      <img src="~/public/posts/List.svg" class="post">
    </button>
    <div class="post-list">
      <div v-for="(post) in posts" :key="post.id">
        <Post :post="post"
          @delete="getall()" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { Reminder, SignalRClient } from '../composables/service/type';

  const isModalOpen = ref(false)
  const openModal = () => (isModalOpen.value = true)
  const isModalImageOpen = ref(false)
  const openImageModal = () => (isModalImageOpen.value = true)
  const isModalListOpen = ref(false)
  const openListModal = () => (isModalListOpen.value = true)

  const userStore = useUserStore();
  const { $bridge } = useNuxtApp()
  const api = $bridge;
  api.setjwt(useCookie('token').value ?? '');

const posts = ref<Reminder[]>([]);

const { $signalr } = useNuxtApp()
const signalr = $signalr as SignalRClient;
signalr.on("NewReminderAdded", async (ReminderOutput) => {
  if (!posts.value.some(post => post.id === ReminderOutput.id)) {
    if (ReminderOutput.reminderType === 1) {
      await api.getImagetocache(ReminderOutput.imageUrl);
    }
    posts.value.push(ReminderOutput)
  }
})
signalr.on("ReminderDeleted", (ReminderOutput) => {
  posts.value = posts.value.filter(post => post.id !== ReminderOutput)
})

signalr.on("ReminderUpdated", (ReminderOutput) => {
  for (let i = 0; i < posts.value.length; i++) {
    if (posts.value[i].id === ReminderOutput.id) {
      posts.value[i] = ReminderOutput;
      break;
    }
  }
})


const getall = async () => {
  const data = await api.getAllReminders(userStore.user.colocationId);
  for (const post of data) {
    if (post.reminderType === 1) {
      console.log('load image')
      await api.getImagetocache(post.imageUrl);
    }
  }
  posts.value = data;
};

onMounted(async () => {
  await getall();
});
</script>

<style scoped>
  .post-list {
    overflow-y: auto;
    max-height: calc(100vh - 4.5rem);
  }

  .add-post {
    position: fixed;
    top: 6%;
    right: 3%;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 40px;
    height: 40px;
    background-color: #FFFFFF;
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
  }

  .add-image {
    position: fixed;
    top: 15%;
    right: 3%;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 40px;
    height: 40px;
    background-color: #FFFFFF;
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
  }

  .add-list {
    position: fixed;
    top: 24%;
    right: 3%;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 40px;
    height: 40px;
    background-color: #FFFFFF;
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
  }

  .dark .add-post {
    background-color: #000000;
  }

  .post {
    width: 72%;
    padding-top: 5%;
    filter: brightness(29%);
  }

  .dark .post {
    filter: none;
  }
</style>