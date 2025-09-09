<template>
    <div class="box">
        <div v-if="sendBy != 'me'" class="header">
            <ProfileIcon :height="15" :width="15" />
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
    color: var(--basic-grey);
    border-radius: 8px;
    margin-bottom: 0.2rem;
    margin-right: 5%;
    margin-left: 5%;
}

.dark .box {
    color: var(--light-grey);
}

.hestia .box {
    color: var(--light-grey);
}

.header {
    display: flex;
    line-height: 0rem;
    align-items: center;
    margin-top: 0.4rem;
    margin-bottom: 0.5rem;
}

.header p {
    margin-bottom: 0;
    margin-left: 10px;
    font-weight: bold;
}

.content {
    color: var(--page-text);
    align-content: center;
    max-width: 80%;
    width: fit-content;
    height: fit-content;
    padding: 10px;
}

.content p {
    margin-bottom: 0;
}

.content-me {
    font-size: 14px;
    border-radius: 8px 8px 0 8px;
    background-color: var(--sent-message);
    margin-left: auto;
}

.content-other {
    font-size: 14px;
    border-radius: 8px 8px 8px 0;
    background-color: var(--recieved-message);
    margin-right: auto;
}
</style>
