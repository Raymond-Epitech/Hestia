<template>
    <div class="task-container">
        <TaskModal v-model="isModalOpen"></TaskModal>
        <div class="task" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
            <h1>Serpillère</h1>
            <div class="due-date">
                <h1 class="number">24</h1>
                <h1 class="month">DEC</h1>
            </div>
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

<style scoped>
.task-container {
    display: flex;
    justify-content: center;
}

.task {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 380px;
    height: 100px;
    background-color: #FF6A61;
    margin-bottom: 15px;
    border-radius: 20px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

h1 {
    font-weight: 600;
}

.due-date {
    position: absolute;
    right: 30px;
    width: 62px;
    height: 62px;
}
</style>