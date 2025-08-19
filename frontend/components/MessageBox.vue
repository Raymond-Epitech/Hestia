<template>
    <div class="box">
        <div v-if="sendBy != 'me'" class="header">
            <ProfileIcon :height="33" :width="33"/>
            <p>{{ sendBy }}</p>
        </div>
        <div :class="['content', sendBy === 'me' ? 'content-me' : 'content-other']">
            <p>{{ content }}</p>
        </div>
    </div>
</template>

<script setup>
const props = defineProps({
    content: {
        type: String,
        required: true
    },
    sendBy: {
        type: String,
        default: ''
    },


})
</script>

<style scoped>
.box {
    color: #4B4B4B;
    padding: 10px;
    border-radius: 8px;
    margin-bottom: 10px;
}
.header {
    display: flex;
    align-items: center;
    margin-bottom: 5px;
}

.header p {
    margin-left: 10px;
    font-weight: bold;
}

.content {
    color: black;
    align-content: center;
}

.content-me {
    font-size: 14px;
    border-radius: 8px 8px 0 8px;
    background-color: #BDD4F6
}
.content-other {
    font-size: 14px;
    border-radius: 8px 8px 8px 0;
    background-color: #C1E0B9
}
</style>