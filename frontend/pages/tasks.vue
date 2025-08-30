<template>
    <AddTaskModal v-model="isModalOpen" @proceed="getall()" />
    <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
        <img src="~/public/plus.png" class="plus">
    </button>
    <button class="calendar-view" @click="triggerCalendar()">
        <img v-if="calendar_view === true" src="~/public/order.svg" class="calendar">
        <img v-else src="~public/calendar.svg" class="calendar"/>
    </button>
    <div v-if="calendar_view === false" class="tasks">
        <div v-for="(task, index) in task_list" :key="index" class="task-list">
            <Task :id="task.id" :title="task.title" :description="task.description" :createdBy="task.createdBy"
                :createdAt="task.createdAt" :dueDate="task.dueDate" :isDone="task.isDone"
                :enrolledUsers="task.enrolledUsers" @proceed="getall()"></Task>
        </div>
    </div>
    <div v-else>
      <CalendarView class="calendar-container" :task_list="task_list" @proceed="getall()"/>
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
const calendar_view = ref(false);

const getall = async () => {
    const data = await api.getAllChore(userStore.user.colocationId);
    task_list.value = data;
};

function triggerCalendar() {
  calendar_view.value = !calendar_view.value;
}

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

.calendar-view {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    margin: 8px 16%;
    background-color: var(--main-buttons-light);
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
    position: fixed;
    z-index: 10;
}

.dark .calendar-view {
    background-color: var(--main-buttons-dark);
}

.calendar {
   filter: invert(1) opacity(1);
}

.dark .calendar {
   filter: invert(0) opacity(1);
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

.calendar-container {
    padding: 0% 1%;
    overflow-y: visible;
    max-height: calc(100vh - 7.5rem);
    margin-top: 3rem;
}
</style>