<template>
    <div class="rectangle" :style="{ backgroundColor: color }" @click="handleClick">
        <slot></slot>
    </div>
</template>

<script>
export default {
    props: {
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
    },
    methods: {
        handleClick() {
            if (this.onClick) {
                this.onClick();
            }
        }
    }
}
</script>

<style scoped>
.rectangle {
    padding: 10px;
    border-radius: 20px;
}
</style>