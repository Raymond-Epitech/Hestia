<template>
    <div class="icon">
        <img class="icon_image" src="../public/profile-icon.svg" alt="profile icon" :style="{height: `${props.height}px`, width: `${props.width}px`}">
    </div>
</template>

<script setup>
const props = defineProps({
    // icon: {
        // type: String,
        // required: true,
    // }
    height: {
        type: Number,
        default: 50,
    },
    width: {
        type: Number,
        default: 50,
    }
})
</script>

<style scoped>
.icon {
    border-radius: 50px;
    background-color: white
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