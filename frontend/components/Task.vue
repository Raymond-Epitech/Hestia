<template>
    <div class="task-container">
        <TaskModal v-model="isModalOpen" :key="id" :id="id" :title="title" :description="description"
        :color="getColor()" :dueDate="dueDate" :isDone="isDone" :enrolledUsers="enrolledUsers"
        @proceed="emitProceed()"></TaskModal>
        <div class="task" :class="[getColor(), { 'done': isDone }]" data-toggle="modal" data-target=".bd-example-modal-sm"
        @click="openModal">
            <h1>{{ title }}</h1>
            <div class="due-date">
                <div class="number">{{ getDayNumber() }}</div>
                <div class="month">{{ getMonthAbbreviation() }}</div>
                <div class="year">{{ getYearNumber() }}</div>
            </div>
            <div class="enrolles-list">
                <div v-for="(userObj, index) in enrolledUsers" :key="index" class="enrolles-pictures">
                    <profile-icon :linkToPP="userObj" :height="33" :width="33"></profile-icon>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)
const emit = defineEmits(['proceed', 'get'])
const props = defineProps({
    id: {
        type: String,
        required: true,
    },
    title: {
        type: String,
        required: false,
    },
    description: {
        type: String,
        required: false,
    },
    createdBy: {
        type: String,
        required: true,
    },
    createdAt: {
        type: String,
        required: true,
    },
    dueDate: {
        type: String,
        required: true,
    },
    isDone: {
        type: Boolean,
        required: true,
    },
    enrolledUsers: {
        type: Object,
        required: false,
    },
})

function emitProceed() {
    emit('proceed')
}

function getDayNumber() {
    const date = new Date(props.dueDate);
    return date.getDate();
}

function getMonthAbbreviation() {
    const date = new Date(props.dueDate);
    return date.toLocaleString('en-US', { month: 'short' });
}

function getYearNumber() {
    const date = new Date(props.dueDate);
    return date.getFullYear();
}

function getColor() {
    const date = new Date(props.dueDate);
    const today = new Date();
    const daysDifference = Math.ceil((date - today) / (1000 * 3600 * 24));
    if (daysDifference < 1) {
        return "red"
    } else if (daysDifference < 3) {
        return "orange"
    } else if (daysDifference >= 3) {
        return "green"
    } else {
        return "red"
    }
}
</script>

<style scoped>
.task-container {
    display: flex;
    justify-content: center;
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

.task {
    position: relative;
    display: grid;
    grid-template-columns: 80% 20%;
    padding-top: 10px;
    justify-content: space-between;
    width: 90%;
    height: 80px;
    padding-left: 5%;
    padding-right: 5%;
    margin-bottom: 15px;
    border-radius: 20px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.done {
    opacity: 0.5;
}

h1 {
    font-weight: 700;
    font-size: 16px;
    padding-top: 6px;
}

.due-date {
    height: fit-content;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    padding-left: 10px;
    padding-bottom: 6px;
    width: 62px;
    height: 62px;
}

.number {
    display: flex;
    justify-content: center;
    align-content: center;
    text-align: center;
    height: 32px;
    font-size: 34px;
    margin-bottom: 10%;
    font-weight: 600;
    -webkit-text-size-adjust: none;
    text-size-adjust: none;
}

.month {
    font-size: 16px;
    font-weight: 600;
    text-transform: uppercase;
    -webkit-text-size-adjust: none;
    text-size-adjust: none;
}
.year {
    font-size: 12px;
    font-weight: 500;
    -webkit-text-size-adjust: none;
    text-size-adjust: none;
}

.enrolles-list {
    width: 90%;
    position: absolute;
    float: left;
    display: flex;
    justify-content: left;
    margin-top: 58px;
    margin-left: 28px;
}

.enrolles-pictures {
    margin-left: -12px;
    z-index: 1;
}
</style>
