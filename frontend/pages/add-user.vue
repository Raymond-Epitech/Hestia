<template>
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
    color: var(--page-text-light);
}

.dark .qr-code-container {
    color: var(--page-text-dark)
}

.qr-code-container canvas {
    margin-top: 10px;
    border: 1px solid #ccc;
    border-radius: 8px;
}
</style>