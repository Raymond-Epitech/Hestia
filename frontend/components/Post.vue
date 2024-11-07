<template>
    <div class="post" :class="color">
        <button class="delete-button" @click="handleDelete">X</button>
        <p>{{ text }}</p>
        <ProfileIcon class="profile-icon"/>
    </div>
</template>

<script setup>
const props = defineProps({
text: {
    type: String,
    required: true
},
color: {
    type: String,
    required: true
}
})

const emit = defineEmits(['delete'])
const handleDelete = () => {
    emit('delete')
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

.post:hover .delete-button {
    opacity: 1;
}

.delete-button {
    position: absolute;
    top: 10px;
    right: 10px;
    background: red;
    color: white;
    border: none;
    border-radius: 50%;
    width: 30px;
    height: 30px;
    cursor: pointer;
    opacity: 0;
    transition: opacity 0.3s ease;
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
    transform: translate(-50%, -50%);
    color: rgb(10, 10, 10);
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