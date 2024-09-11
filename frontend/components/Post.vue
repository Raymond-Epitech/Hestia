<template>
    <div class="post">
        <button class="delete-button" @click="handleDelete">X</button>
        <h1>{{ title }}</h1>
        <p>{{ text }}</p>
        <ProfileIcon class="profile-icon"/>
    </div>
</template>

<script setup>
const props = defineProps({
title: {
    type: String,
    required: true
},
text: {
    type: String,
    required: true
},
})

const emit = defineEmits(['delete'])
const handleDelete = () => {
    emit('delete')
}

</script>

<style scoped>
.post {
    background-image: url('../public/carre_jaune.png');
    background-size: cover;
    background-position: center; 
    background-repeat: no-repeat; 
    width: 500px; 
    height: 500px; 
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
</style>