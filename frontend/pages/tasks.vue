<template>
    <div>
        <button class="add-post">
            <img src="~/public/plus.png" class="plus">
        </button>
        <div class="task-list">
            <Task></Task>
            <Task></Task>
        </div>
    </div>
</template>

<script setup>

</script>

<style scoped>
.task-list {
    display: flex;
    justify-content: center;
    flex-direction: column;
    align-content: center;
}
</style>