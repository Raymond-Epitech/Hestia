<template>
    <div>
        <button @click="selectImage">Choisir une image</button>
        <input type="file" accept="image/*" @change="handleFile" ref="fileInput" style="display:none" />
        <img v-if="imageSrc" :src="imageSrc" alt="Image sélectionnée" style="max-width: 100%; margin-top: 1em;" />
    </div>
    <button @click="chargeImage">Charger l'image</button>
    <img :src="imageget" alt="Image" />
</template>

<script setup>
import { useUserStore } from '~/store/user';
import { Capacitor } from '@capacitor/core'
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera'

const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const fileInput = ref(null);
const imageget = ref('');

function isNative() {
  return Capacitor.getPlatform() !== 'web'
}

async function selectImage() {
  if (isNative()) {
    try {
      const photo = await Camera.getPhoto({
        quality: 90,
        allowEditing: false,
        resultType: CameraResultType.DataUrl,
        source: CameraSource.Photos
      })
      imageSrc.value = photo.dataUrl
    } catch (error) {
      console.error('Erreur accès galerie mobile:', error)
    }
  } else {
    fileInput.value.click()
  }
}
function handleFile(event) {
  const file = event.target.files[0]
  if (!file) return

  const reader = new FileReader()
  reader.onload = (e) => {
    imageSrc.value = e.target.result
  }
  reader.readAsDataURL(file)
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
