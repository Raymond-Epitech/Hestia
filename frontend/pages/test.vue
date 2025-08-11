<template>
    <input type="file" accept="image/*" @change="handleFile" />
    <button @click="chargeImage">Charger l'image</button>
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
onMounted(async () => {
    await api.getImagetocache('test.jpg');
});
</script>
