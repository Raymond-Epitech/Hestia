<template>
    <div>
        <div class="messages-box" ref="messagesBox">
            <MessageBox
                v-for="message in messages"
                :key="message.id"
                :content="message.content"
                :sendBy="getUsername(message.sendBy)"
            />
        </div>
        <form @submit.prevent="handleSendMessage" class="form">
            <input v-model="newMessage.content" type="text" placeholder="Message" maxlength="6000" class="body-input" required />
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
const messagesBox = ref<HTMLDivElement | null>(null);

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
    fetchMessages().then(() => {
        nextTick(() => {
            messagesBox.value?.scrollTo(0, messagesBox.value.scrollHeight);
        });
    });
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
    background: transparent;
}

.dark .svg-icon {
    filter: invert(1);
}


.body-input {
    width: 80%;
    padding-right: 40px;
    border: none;
    border-bottom: 2px solid var(--list-lines-light);
    height: 2.8rem;
    background: transparent;
    color: var(--page-text-light);
    font-size: 1.1rem;
    border-radius: 8px;
    padding-left: 12px;
    outline: none;
}

.dark .body-input {
    border-bottom: 2px solid var(--list-lines-dark);
    color: var(--page-text-dark);
}

form {
    position: fixed;
    bottom: 4.5rem;
    display: flex;
    justify-content: center;
    align-items: center;
    border-top-left-radius: 8px;
    border-top-right-radius: 8px;
    gap: 10px;
    width: 100%;
    background: var(--main-buttons-light);
    padding: 0.8rem 0;
    box-shadow: var(--button-shadow-light);
    padding-bottom: 1.2rem;
}

.dark form {
    background: var(--main-buttons-dark);
}

.messages-box {
    padding: 0.2rem;
    padding-bottom: 6rem;
    max-height: calc(100vh - 7rem);
    overflow-y: auto;
    line-height: 1rem;
}

button {
    background: transparent;
    border: none;
}
</style>
