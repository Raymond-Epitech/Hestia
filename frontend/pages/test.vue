<template>
    <input type="file" accept="image/*" @change="handleFile" />
    <img :src="imageget" alt="Image" />
</template>

<script setup>
import { useUserStore } from '~/store/user';

const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const fileInput = ref(null);
const imageget = ref('');

function handleFile(event) {
    fileInput.value = event.target.files[0]
    api.uploadImage(fileInput.value);
    if (fileInput.value) {
        const file = fileInput.value;
        console.log('Image sélectionnée :', file)
    }
}
onMounted(async () => {
  const test = await api.getImage('test2.png');
    console.log(test);
    imageget.value = test;
});
</script>
