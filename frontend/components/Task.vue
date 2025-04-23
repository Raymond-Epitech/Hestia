<template>
    <div class="task-container">
        <TaskModal v-model="isModalOpen" :title="title" :description="description" :color="getColor()"
            :dueDate="dueDate"></TaskModal>
        <div class="task" :class="[getColor()]" data-toggle="modal" data-target=".bd-example-modal-sm"
            @click="openModal">
            <h1>{{ title }}</h1>
            <div class="due-date">
                <div class="number">{{ getDayNumber() }}</div>
                <div class="month">{{ getMonthAbbreviation() }}</div>
            </div>
        </div>
    </div>
</template>

<script setup>
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
    }
})
const isModalOpen = ref(false)
const openModal = () => (isModalOpen.value = true)

function getDayNumber() {
    const date = new Date(props.dueDate);
    return date.getDate();
}

function getMonthAbbreviation() {
    const date = new Date(props.dueDate);
    return date.toLocaleString('en-US', { month: 'short' });
}

function getColor() {
    const date = new Date(props.dueDate);
    const today = new Date();
    const daysDifference = Math.ceil((date - today) / (1000 * 3600 * 24));
    if (daysDifference < 1) {
        return "red"
    } else if (daysDifference < 3) {
        return "orange"
    } else if (daysDifference > 3) {
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
    background-color: #FF6A61;
}

.orange {
    background-color: #FFC93D;
}

.green {
    background-color: #85AD7B;
}

.task {
    display: grid;
    grid-template-columns: 4fr 1fr;
    align-items: center;
    justify-content: space-between;
    width: 90%;
    height: 80px;
    padding-left: 5%;
    padding-right: 5%;
    margin-bottom: 15px;
    border-radius: 20px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

h1 {
    font-weight: 700;
    font-size: 18px;
    margin-bottom: 24px;
}

.due-date {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    padding-bottom: 6px;
    width: 62px;
    height: 62px;
}

.number {
    display: flex;
    justify-content: center;
    align-content: center;
    text-align: center;
    height: 40px;
    font-size: 38px;
    margin-bottom: 4px;
    font-weight: 600;
}

.month {
    font-size: 16px;
    font-weight: 600;
    text-transform: uppercase;
}
</style>