<template>
    <div v-if="showImage" class="profile">
        <img class="profile" :src="ppurl" alt="profile icon"
            :style="{ height: `${props.height}px`, width: `${props.width}px` }" @error="onImgError">
    </div>
    <div v-else class="icon">
        <img class="icon_image" src="../public/navbar/Profile.svg" alt="profile icon"
            :style="{ height: `${props.height}px`, width: `${props.width}px` }">
    </div>
</template>

<script setup lang="ts">
const props = defineProps({
    linkToPP: {
        type: String,
        required: false,
    },
    height: {
        type: Number,
        default: 50,
    },
    width: {
        type: Number,
        default: 50,
    }
})
const { $bridge } = useNuxtApp();
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const ppurl = ref<string>('');
const showImage = ref(true);

const loadImage = async (imageUrl: string) => {
    if (!imageUrl) {
        showImage.value = false;
        return;
    }
    api.getImagetocache(props.linkToPP ?? '').then((response) => {
    }).catch((error) => {
        console.error(error);
        showImage.value = false;
    });

    api.getImagefromcache(props.linkToPP ?? '').then((response) => {
        if (response !== null) {
            ppurl.value = response;
            console.log('Profile picture loaded from cache in Profile_icon.vue: ', ppurl.value);
        } else {
            showImage.value = false;
        }
    }).catch((error) => {
        console.error(error);
        showImage.value = false;
    });
};

watch(() => props.linkToPP, (newVal) => {
    if (newVal) {
        loadImage(newVal);
    }
});

onMounted(() => {
    if (props.linkToPP) {
        loadImage(props.linkToPP);
    } else {
        showImage.value = false;
    }
});

const hasError = ref(false);

const onImgError = () => {
    hasError.value = true;
};
</script>

<style scoped>
.profile {
    border-radius: 50%;
}

.icon {
    border-radius: 50%;
    background-color: var(--icon-background);
}

.icon_image {
    filter: var(--icon-filter);
}
</style>