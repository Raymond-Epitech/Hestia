<template>
    <div class="rectangle" :style="{ backgroundColor: color }" @click="handleClick">
        <slot></slot>
    </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue';

const props = defineProps({
    color: {
        type: String,
        default: 'lightgray'
    },
    onClick: {
        type: Function,
        default: null
    },
    id: {
        type: String,
        required: true
    }
});

const emit = defineEmits(['click']);

function handleClick() {
    if (props.onClick) {
        props.onClick();
    }
    emit('click');
}
</script>

<style scoped>
.rectangle {
    padding: 10px;
    border-radius: 20px;
}
</style>