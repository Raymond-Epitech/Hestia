<template>
    <div v-if="imageget" class="profile">
        <img class="profile" :src="imageget" alt="profile icon"
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
const imageget = ref('');

const hasError = ref(false);

const onImgError = () => {
    hasError.value = true;
};

onMounted(async () => {
    if (props.linkToPP) {
        api.getImagefromcache(props.linkToPP).then((image) => {
            if (image) {
                imageget.value = image;
                console.log("got pp from cache")
            } else {
                console.error('Image non trouvée dans le cache');
            }
        }).catch((error) => {
            console.error('Erreur lors de la récupération de l\'image :', error);
        });
    }
});
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