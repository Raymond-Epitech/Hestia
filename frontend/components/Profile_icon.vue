<template>
    <div v-if="props.linkToPP && props.linkToPP!= 'deleted.jpg' && props.linkToPP!= 'exempledetest' && props.linkToPP!= 'default.jpg'"
        class="profile">
        <img class="profile" :src="$props.linkToPP" alt="profile icon"
           :style="{ height: `${props.height}px`, width: `${props.width}px` }" @error="onImgError" >
    </div>
    <div v-else class="icon">
        <img class="icon_image" src="../public/profile-icon.svg" alt="profile icon"
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
  (event.target as HTMLImageElement).src = '/profile-icon.svg';
}
</script>

<style scoped>
    .profile {
        border-radius: 50px;
    }

    .icon {
        border-radius: 50px;
        background-color: white;
    }

    .dark .icon {
        background-color: black;
    }

    .icon_image {
        filter: brightness(29%);
    }

    .dark .icon_image {
        filter: none;
    }
</style>