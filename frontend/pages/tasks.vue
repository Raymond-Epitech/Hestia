<template>
    <AddTaskModal v-model="isModalOpen" @proceed="getall()" />
    <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
        <img src="~/public/plus.png" class="plus">
    </button>
    <div class="tasks"> 
        <div v-for="(task, index) in task_list" :key="index" class="task-list">
            <Task :id="task.id" :title="task.title" :description="task.description" :createdBy="task.createdBy"
                :createdAt="task.createdAt" :dueDate="task.dueDate" :isDone="task.isDone"
                :enrolledUsers="task.enrolledUsers" @proceed="getall()"></Task>
        </div>
    </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';

const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');

const task_list = ref([]);

const getall = async () => {
    const data = await api.getAllChore(userStore.user.colocationId);
    task_list.value = data;
};

onMounted(async () => {
    await getall();
});
</script>

<style scoped>
.add-post {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    margin: 8px 5%;
    background-color: var(--main-buttons-light);
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
    position: fixed;
    z-index: 10;
}

.dark .add-post {
    background-color: var(--main-buttons-dark);
}

.plus {
    width: 20px;
    height: 20px;
}

.dark .plus {
    filter: invert(1) opacity(1);
}

.task-list {
    display: flex;
    justify-content: center;
    flex-direction: column;
    align-content: center;
}

.tasks {
    overflow-y: visible;
    margin-top: 3rem;
    max-height: calc(100vh - 7.5rem);
}
</style>