<template>
    <span class="text">{{ $t(source) }}</span>
</template>

<script setup>
const props = defineProps({
    source: {
        type: String,
        required: true
    }
});
</script>

<style scoped>
.text {
    white-space: pre-line;
}
</style>