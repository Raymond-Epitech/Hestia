<template>
    <div class="post" :class="color">
        <button class="delete-button" @click="handleDelete">x</button>
        <p>{{ text }}</p>
        <ProfileIcon class="profile-icon" />
    </div>
</template>

<script setup>
import { useI18n } from '#imports';

const { t } = useI18n();
const props = defineProps({
    id: {
        type: String,
        required: true,
    },
    text: {
        type: String,
        required: true
    },
    color: {
        type: String,
        required: true
    }
})

const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');

const emit = defineEmits(['delete'])
const handleDelete = async () => {
    const confirmed = window.confirm(t('confirm_delete_reminder'));
    if (confirmed) {
        await api.deleteReminder(props.id)
        emit('delete')
    }
}

</script>

<style scoped>
.post {
    background-size: cover;
    background-position: center;
    background-repeat: no-repeat;
    width: 250px;
    height: 250px;
    position: relative;
}

.delete-button {
    display: flex;
    justify-content: center;
    position: absolute;
    top: 10px;
    right: 10px;
    background: #FF6A61;
    color: white;
    border: none;
    border-radius: 50%;
    width: 30px;
    height: 30px;
    cursor: pointer;
    opacity: 1;
    transition: opacity 0.3s ease;
    font-weight: 600;
}

.post h1 {
    position: absolute;
    left: 50%;
    transform: translate(-50%, 0%);
    color: rgb(10, 10, 10);
}

.post p {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-60%, -50%);
    width: 200px;
    max-height: 250px;
    color: rgb(10, 10, 10);
    overflow-wrap: anywhere;
    text-align: center;
}

.profile-icon {
    position: absolute;
    bottom: 10px;
    right: 10px;
}

.blue {
    background-color: #A8CBFF;
}

.yellow {
    background-color: #FFF973;
}

.pink {
    background-color: #FFA3EB;
}

.green {
    background-color: #9CFFB2;
}
</style>