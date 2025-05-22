<template>
    <div >
        <img src="../public/profile-icon.png" alt="profile icon" :style="{height: `${props.height}px`, width: `${props.width}px`}">
    </div>
</template>

<script setup>
import { computed } from 'vue'
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

</style>