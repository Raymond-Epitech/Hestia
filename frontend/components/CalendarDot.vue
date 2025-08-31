<template>
    <TaskModal class="task-modal" v-model="isModalOpen" :key="id" :id="id" :title="title" :description="description"
    :color="color" :dueDate="dueDate" :isDone="isDone" :enrolledUsers="enrolledUsers"
    @proceed="emitProceed()"></TaskModal>
    <div class="task-dot" :class="[color, { 'done': isDone }]" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
    </div>
</template>

<script setup lang="ts">

const props = defineProps < {
    id: string,
    title: string,
    modelValue?: boolean,
    description: string,
    color: string,
    dueDate: string,
    isDone: boolean,
    enrolledUsers: Object,
} > ();

const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)
const emit = defineEmits(['proceed', 'get'])

function emitProceed() {
    emit('proceed')
}

</script>

<style scoped>
.task-dot {
    height: 12px;
    width: 12px;
    border-radius: 50%;
}

.done {
    opacity: 0.5;
}

.red {
    background-color: var(--basic-red);
}

.orange {
    background-color: var(--basic-yellow);
}

.green {
    background-color: var(--basic-green);
}
</style>