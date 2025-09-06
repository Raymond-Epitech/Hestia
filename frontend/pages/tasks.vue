<template>
    <AddTaskModal v-model="isModalOpen" @proceed="getall()" />
    <div class="buttons-list">
        <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
            <img src="~/public/tasks/Plus.svg" class="icon">
        </button>
        <button class="calendar-view" @click="triggerCalendar()">
            <img v-if="calendar_view === true" src="~/public/tasks/Order.svg" class="icon">
            <img v-else src="~/public/tasks/Calendar.svg" class="icon"/>
        </button>
    </div>
    <div v-if="calendar_view === false" class="tasks">
        <div v-for="(task) in task_list" :key="task.id" class="task-list">
            <div v-if="shouldDisplay(task)">
                <Task :key="task.id" :id="task.id" :title="task.title" :description="task.description" :createdBy="task.createdBy"
                :createdAt="task.createdAt" :dueDate="task.dueDate" :isDone="task.isDone"
                :enrolledUsers="task.enrolledUsers" :updatedAt="task.updatedAt" @proceed="getall()"></Task>
            </div>
        </div>
    </div>
    <div v-else>
      <CalendarView class="calendar-container" :task_list="task_list" @proceed="getall()"></CalendarView>
    </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { Chore, SignalRClient } from '../composables/service/type';

const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const { $signalr } = useNuxtApp()
const task_list = ref<Chore[]>([]);
const calendar_view = ref(false);
const signalr = $signalr as SignalRClient;

signalr.on("NewChoreAdded", async (ChoreOutput) => {
  if (!task_list.value.some(task => task.id === ChoreOutput.id)) {
    task_list.value.push(ChoreOutput)
  }
})

signalr.on("ChoreDeleted", (ChoreOutput) => {
  task_list.value = task_list.value.filter(task => task.id !== ChoreOutput)
})

signalr.on("ChoreEnrollmentAdded", async (ChoreOutput) => {
  // const index = task_list.value.findIndex(task => task.id === ChoreOutput.id);
  //   if (index !== -1) {
  //     task_list.value[index] = ChoreOutput;
  //   }
  getall()
})

signalr.on("ChoreEnrollmentRemoved", async (ChoreOutput) => {
  // const index = task_list.value.findIndex(task => task.id === ChoreOutput.id);
  // if (index !== -1) {
    // task_list.value[index] = ChoreOutput;
  // }
  getall()
});


signalr.on("ChoreUpdated", async (ChoreOutput) => {
  const index = task_list.value.findIndex(task => task.id === ChoreOutput.id);
    if (index !== -1) {
      task_list.value[index] = ChoreOutput;
    }
})

const getall = async () => {
    const data = await api.getAllChore(userStore.user.colocationId);
    task_list.value = data;
};

function triggerCalendar() {
  calendar_view.value = !calendar_view.value;
}

function shouldDisplay(task) {
    if (task.isDone) {
        const date = new Date(task.dueDate);
        const today = new Date();
        const daysDifference = Math.ceil((date - today) / (1000 * 3600 * 24));
        if (daysDifference <= 0) {
            return false;
        }
    }
    return true;
}

onMounted(async () => {
    await getall();
});
</script>

<style scoped>
    .icon {
        width: 20px;
        height: 20px;
        filter: var(--icon-filter);
    }

button {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    margin: 8px 5%;
    background-color: var(--main-buttons);
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
}

.buttons-list{
    display: flex;
    justify-content: flex-start;
    align-items: center;
    width: 30%;
    position: fixed;
    padding-left: 0.8rem;
    top: 0;
    background-color: var(--page-background-light);
    z-index: 1;
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