<template>
    <div class="rectangle" :style="{ backgroundColor: color }">
        <slot></slot>
    </div>
</template>

<script>
export default {
    name: "Rectangle",
    props: {
        color: {
            type: String,
            default: "white"
        }
    }
};
</script>

<style scoped>
.rectangle {
    padding: 10px;
    border-radius: 20px;
}
</style>