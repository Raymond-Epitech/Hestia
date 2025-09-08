<template>
    <div v-if="props.linkToPP && props.linkToPP!= 'deleted.jpg' && props.linkToPP!= 'exempledetest' && props.linkToPP!= 'default.jpg'"
        class="profile">
        <img class="profile" :src="$props.linkToPP" alt="profile icon"
           :style="{ height: `${props.height}px`, width: `${props.width}px` }" @error="onImgError" >
    </div>
    <div v-else class="icon">
        <img class="icon_image" src="../public/navbar/Profile.svg" alt="profile icon"
            :style="{height: `${props.height}px`, width: `${props.width}px`}">
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

const onImgError = (event: Event) => {
  (event.target as HTMLImageElement).src = '../public/navbar/Profile.svg';
}
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