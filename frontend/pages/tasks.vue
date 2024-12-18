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
.add-post {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    margin: 16px 16px;
    background-color: #525461;
    border-radius: 9px;
    border: none;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.plus {
    width: 20px;
    height: 20px;
    filter: brightness(0) invert(1);
}

.task-list {
    display: flex;
    justify-content: center;
    flex-direction: column;
    align-content: center;
}
</style>