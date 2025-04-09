<template>
    <div class="rectangle" :style="{ backgroundColor: color }" @click="handleClick">
        <slot></slot>
    </div>
</template>

<script setup>
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
    height: 70px;
    padding: 10px;
    border-radius: 20px;
    display: flex;
    justify-content: center;
    align-items: center;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}
</style>