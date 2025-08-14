<template>
  <div>
    <AddPostModal v-model="isModalOpen" @proceed="getall()" />
    <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
      <img src="~/public/plus.png" class="plus">
    </button>
    <div v-for="(post, index) in posts" :key="index">
      <Post :id="post.id" :text="post.content" :color="post.color" :isImage="post.isImage" @delete="getall()" />
    </div>
  </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';

const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');

const posts = ref([]);

const getall = async () => {
  const data = await api.getAllReminders(userStore.user.colocationId);
  for (const post of data) {
    if (post.isImage) {
      await api.getImagetocache(post.content);
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
  display: flex;
  justify-content: center;
  align-items: center;
  width: 30px;
  height: 30px;
  margin: 16px 16px;
  background-color: #FFF973;
  border-radius: 9px;
  border: none;
  box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.plus {
  width: 20px;
  height: 20px;
}
</style>