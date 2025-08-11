<template>
  <div>
    <button @click="chargeImage">Charger l'image</button>
    <img :src="imageget" alt="Image" />
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

const getall = async () => {
  const data = await api.getAllReminders(userStore.user.colocationId);
  posts.value = data;
};

onMounted(async () => {
  await getall();
});

const imageget = ref('');
function chargeImage() {
    api.getImagefromcache('test.jpg').then((image) => {
        if (image) {
            imageget.value = image;
        } else {
            console.error('Image non trouvée dans le cache');
        }
    }).catch((error) => {
        console.error('Erreur lors de la récupération de l\'image :', error);
    });
}
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