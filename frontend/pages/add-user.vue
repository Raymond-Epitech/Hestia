<template>
    <button class="back" @click="redirect('/profile')">
        <img src="~/public/Retour.svg" class="icon">
    </button>
    <div class="container">
        <h2>{{ $t('add_user_header') }}</h2>
        <div class="link-section">
            <p>{{ inviteLink }}</p>
            <button class="copy" @click="copyContent">
                <img src="~/public/add-user/Copy.svg" class="icon"/>
            </button>
        </div>
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

const copyContent = async () => {
    try {
        await navigator.clipboard.writeText(inviteLink);
    } catch (err) {
        console.error('Failed to copy: ', err);
    }
}

onMounted(() => {
    generateQRCode();
});
</script>

<style scoped>
.container {
    margin-top: 6rem;
    text-align: justify;
    font-weight: 600;
    padding: 15px;
    border-radius: 5px;
    background-color: var(--main-buttons);
    color: var(--page-text);
}

.container canvas {
    margin: auto;
    margin-top: 1rem;
    border: 1px solid #ccc;
    border-radius: 8px;
    display: block;
}

.icon {
    display: flex;
    align-items: center;
    justify-content: center;
    filter: var(--icon-filter);
}

button {
    background-color: var(--main-buttons);
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 9px;
    border: none;
    box-shadow: var(--small-button-shadow);
}

.back {
    top: 3%;
    left: 3%;
    position: fixed;
    width: 40px;
    height: 40px;
}

.back .icon {
    width: 25px;
}

.link-section {
    margin-top: 10px;
    display: grid;
    grid-template-columns: 8fr 1.5fr;
}

p {
    background-color: var(--background);
    color: var(--secondary-page-text);
    padding: 10px;
    border-radius: 5px;
    width: fit-content;
    margin: 0 auto;
    font-size: 14px;
    word-break: break-all;
    box-shadow: var(--small-button-shadow);
    user-select: all;
    font-weight: 500;
}

.copy {
    margin: 0 auto;
    word-break: break-all;
    width: 40px;
    height: 40px;
    margin-left: 10px;
    padding: 10px;
}
</style>