<template>
  <div>
    <AddPostModal v-model="isModalOpen" @proceed="getall()" />
    <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
      <img src="~/public/plus.png" class="plus">
    </button>
    <div v-for="(post, index) in posts" :key="index">
      <Post :id="post.id" :text="post.content" :color="post.color" @delete="getall()" />
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
api.setjwt(useCookie('token').value ?? '');

const getall = async () => {
  console.log(userStore.user)
  const data = await api.getAllReminders(userStore.user.colocationId);
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
  width: 42px;
  height: 42px;
  margin: 16px 16px;
  background-color: #FFF973;
  border-radius: 9px;
  border: none;
  box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.plus {
  width: 24px;
  height: 24px;
}
</style>