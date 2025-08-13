<template>
  <div>
    <AddPostModal v-model="isModalOpen" @proceed="getall()" />
    <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
      <img src="~/public/posts/Post.svg" class="post">
    </button>
    <div v-for="(post, index) in posts" :key="index">
      <Post :id="post.id" :text="post.content" :color="post.color" :createdBy="post.createdBy" @delete="getall()" />
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
  box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
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