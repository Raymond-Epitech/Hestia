<template>
    <div>
      <AddPostModal v-model="isModalOpen">
      </AddPostModal>
      <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
        <img src="~/public/plus.png" class="plus">
      </button>
      <h1>Welcome to the homepage</h1>
        <div v-for="(post, index) in posts" :key="index">
          <Post :id="post.id" :text="post.content" :color="post.color"/>
        </div>
    </div>
</template>

<script setup>
import { bridge } from '~/service/bridge.ts';

const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)

const api = new bridge();
const posts = ref([]);

const getall = async () => {
  const data = await api.getAllReminders();
  posts.value = data;
  console.log("post:"+posts.value);
};

onMounted(async () => {
  await getall();
});

</script>

<style scoped>
.add-post{
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