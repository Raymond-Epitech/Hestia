<template>
{{ $t(source) }} 
</template>

<script setup>
const props = defineProps({
    source: {
        type: String,
        required: true
    }
});
</script>
