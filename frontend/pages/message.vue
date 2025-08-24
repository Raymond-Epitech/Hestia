<template>
    <div class="background">
        <div class="messages-box">
            <MessageBox
                v-for="message in messages"
                :key="message.id"
                :content="message.content"
                :sendBy="getUsername(message.sendBy)"
            />
        </div>
        <form @submit.prevent="handleSendMessage" class="form">
            <input v-model="newMessage.content" type="text" placeholder="Message" class="body-input" required />
            <button type="submit" class="button">
                <img src="/Submit.svg" alt="Submit Icon" class="svg-icon" />
            </button>
        </form>
    </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { Coloc, message, SignalRClient } from '~/composables/service/type';

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const colocationId = userStore.user.colocationId;
const messages = ref<message[]>([]);
const list_coloc = ref<Coloc[]>([]);
const { $signalr } = useNuxtApp()
const signalr = $signalr as SignalRClient;

signalr.on("NewMessageAdded", (messageOutput) => {
    if (!messages.value.some(msg => msg.id === messageOutput.id)) {
        messages.value.push(messageOutput);
    }
});
signalr.on("MessageDeleted", (messageId) => {
    console.log("Message deleted:", messageId);
    messages.value = messages.value.filter(msg => msg.id !== messageId);
});
signalr.on("MessageUpdated", (messageOutput) => {
    for (let i = 0; i < messages.value.length; i++) {
        if (messages.value[i].id === messageOutput.id) {
            messages.value[i] = messageOutput;
            break;
        }
    }
});
const fetchMessages = async () => {
    try {
        messages.value = await api.getMessageByColocationId(colocationId);
    } catch (error) {
        console.error('Error fetching messages:', error);
    }
};

onMounted(() => {
    fetchMessages();
    api.getUserbyCollocId(colocationId)
        .then((colocs: Coloc[]) => {
            list_coloc.value = colocs;
        });
});
const newMessage = ref<message>({
    colocationId: userStore.user.colocationId,
    content: '',
    sendBy: userStore.user.id,
});
const handleSendMessage = async () => {
    try {
        await api.addMessage(newMessage.value);
        newMessage.value.content = '';
        fetchMessages();
    } catch (error) {
        console.error('Error sending message:', error);
    }
};
const getUsername = (sendById: string): string => {
    if (!list_coloc.value.length) return 'Unknown';
    if (sendById === userStore.user.id) return 'me';
    const user = list_coloc.value.find(coloc => coloc.id === sendById);
    return user ? user.username : 'Unknown';
};
</script>

<style scoped>
.svg-icon {
    width: 24px;
    height: 24px;
    display: inline-block;
    vertical-align: middle;
}

.body-input {
    width: 80%;
    padding-right: 40px;
    border: none;
    border-bottom: 2px solid #BDD4F6;
}

form {
    position: fixed;
    bottom: 10%;
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 10px;
    width: 100%;
}

.messages-box {
    padding: 0.2rem;
    max-height: 46rem;
    overflow-y: scroll;
    line-height: 1rem;
}
</style>
