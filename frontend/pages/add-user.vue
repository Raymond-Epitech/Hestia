<template>
    <button class="back" @click="redirect('/profile')">
        <img src="~/public/Retour.svg" class="icon">
    </button>
    <div class="qr-code-container">
        <h2>{{ $t('add_user_header') }}</h2>
        <p>{{ inviteLink }}</p>
        <canvas ref="qrCanvas"></canvas>
    </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import QRCode from 'qrcode';

const userStore = useUserStore();
const qrCanvas = ref<HTMLCanvasElement | null>(null);
const inviteLink = `https://hestiaapp.org/invite?collocID=${userStore.user.colocationId}`;
const router = useRouter();
const redirect = (page: string) => {
  router.push(page);
}
const generateQRCode = async () => {
    if (qrCanvas.value) {
        await QRCode.toCanvas(qrCanvas.value, inviteLink, {
            width: 200,
        });
    }
};

onMounted(() => {
    generateQRCode();
});
</script>

<style scoped>
.qr-code-container {
    margin-top: 20px;
    text-align: center;
    color: var(--secondary-page-text);
}

.qr-code-container canvas {
    margin-top: 10px;
    border: 1px solid #ccc;
    border-radius: 8px;
}

.icon {
  display: flex;
  align-items: center;
  justify-content: center;
  filter: var(--icon-filter);
}

.back {
  background-color: var(--main-buttons);
  position: fixed;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 40px;
  height: 40px;
  border-radius: 9px;
  border: none;
  box-shadow: var(--button-shadow-light);
  top: 3%;
  left: 3%;
}

.back .icon {
  width: 25px;
}
</style>