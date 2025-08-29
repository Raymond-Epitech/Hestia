<template>
    <div>
        <AddTaskModal v-model="isModalOpen" @proceed="getall()" />
        <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
            <img src="~/public/plus.png" class="plus">
        </button>
        <div v-for="(task, index) in task_list" :key="index" class="task-list">
            <Task :id="task.id" :title="task.title" :description="task.description" :createdBy="task.createdBy"
                :createdAt="task.createdAt" :dueDate="task.dueDate" :isDone="task.isDone"
                :enrolledUsers="task.enrolledUsers" @proceed="getall()"></Task>
        </div>
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
  task_list.value = task_list.value.map(task => {
    if (task.id === ChoreOutput.choreId) {
      const newEnrolled = {...task.enrolledUsers};
      if (!newEnrolled[ChoreOutput.userId]) {
        newEnrolled[ChoreOutput.userId] = ChoreOutput.pathToPP;
      }
      return {
        ...task,
        enrolledUsers: newEnrolled
      }
    }
    return task
  })
})

// signalr.on("ChoreEnrollmentRemoved", async (ChoreOutput) => {
//   console.log("test2")
//   console.log(ChoreOutput)
//   if (!task_list.value.some(task => task.id === ChoreOutput.choreId)) {
//     task_list.value.push(ChoreOutput.chore)
//     console.log(task_list.value)
//   }
// })

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
</style>