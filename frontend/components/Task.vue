<template>
    <div class="task-container">
        <div class="task" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
            <TaskModal v-model="isModalOpen"></TaskModal>
        </div>
    </div>
</template>

<script setup>
const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)

const task = ref({
    id: '',
    createdBy: '',
    createdAt: '',
    dueDate: "2024-12-18T06:50:22.796Z",
    title: 'Serpillère',
    description: '',
    isDone: false
})
</script>

<style>
.add-post {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    margin: 16px 16px;
    background-color: #FFF973;
    border-radius: 9px;
    border: none;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.plus {
    width: 20px;
    height: 20px;
}

.task-container {
    display: flex;
    justify-content: center;
}

.task {
    width: 320px;
    height: 100px;
    background-color: #FF6A61;
    margin-bottom: 15px;
    border-radius: 20px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}
</style>