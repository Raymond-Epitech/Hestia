<template>
  <div>
    <AddPostModal v-model="isModalOpen" @proceed="getall()" />
    <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
      <img src="~/public/posts/Post.svg" class="post">
    </button>
    <div v-for="(post, index) in posts" :key="index">
      <Post :id="post.id" :text="post.content" :color="post.color" :createdBy="post.createdBy" :linkToPP="post.linkToPP" :post="post"
        @delete="getall()" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { Reminder, SignalRClient } from '../composables/service/type'

  const isModalOpen = ref(false)
  const openModal = () => (isModalOpen.value = true)

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
signalr.on("reminderdeleted", (ReminderOutput) => {
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