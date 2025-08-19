<template>
    <div class="background">
        <div v-for="msg in messages" :key="msg.id">
            <MessageBox :content="msg.content" :sendBy="getUsername(msg.sendBy)" />
        </div>
        <form @submit.prevent="handleSendMessage">
            <input v-model="newMessage.content" type="text" placeholder="Message" class="body-input"
                required />
            <button type="submit" class="button">
                <img src="/Submit.svg" alt="Submit Icon" class="svg-icon" />
            </button>
        </form>
    </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { Coloc, message } from '~/composables/service/type';

const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const colocationId = userStore.user.colocationId;
const messages = ref<message[]>([]);
const list_coloc = ref<Coloc[]>([]);

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
    width: 24px; /* Ajustez la largeur */
    height: 24px; /* Ajustez la hauteur */
    display: inline-block;
    vertical-align: middle;
}
</style>
