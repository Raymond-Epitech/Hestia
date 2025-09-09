<template>
    <div
        :class="[post.reminderType == 1 ? 'post_image' : post.reminderType == 2 ? 'shopping-container' : 'post', , post.reminderType != 1 && post.color]">
        <ProfileIcon v-if="post.reminderType !== 2" class="profile-icon" :height="30" :width="30"
            :linkToPP="post.linkToPP" />
        <ReactModal :postId="post.id" v-model="isModalOpen" />
        <button class="react-button" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openModal">
            <div class="heart">❤️</div>
        </button>
        <div v-if="reactions.length > 0" class="reaction-list">
            <div v-for="reaction in reactions" :key="reaction.id" class="reaction">
                {{ reaction.type }}
            </div>
        </div>
        <button class="delete-button" @click="showPopup" v-if="post.createdBy == user.id && post.reminderType != 2">
            <div class="close"></div>
        </button>
        <button class="edit-button" @click="handleModify" v-if="post.reminderType == 2">
            <img src="/edit.svg" class="edit-icon" />
        </button>
        <p v-if="post.reminderType == 0">{{ post.content }}</p>
        <img v-if="post.reminderType == 1" :src="imageget" alt="Post Image" class="image" />
        <div v-if="post.reminderType == 2">
            <h3 v-if="post.shoppingListName" class="shopping-title">{{ post.shoppingListName }}</h3>
            <div class="shopping-items">
                <div v-for="item in post.items" :key="item.id" class="shopping-header">
                    <span class="shopping-name">
                        {{ item.name }}
                    </span>
                    <div class="check-zone" :class="{ checked: item.isChecked }" @click.stop="toggleCheck(item)">
                    </div>
                </div>
            </div>
        </div>
    </div>
    <popup v-if="popup_vue" :text="$t('confirm_delete_reminder')" @confirm="confirmDelete" @close="cancelDelete">
    </popup>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { Reminder, SignalRClient, Reaction } from '../composables/service/type';

const isModalOpen = ref(false);
const openModal = () => (isModalOpen.value = true);
const props = defineProps({
    post: {
        type: Object as PropType<Reminder>,
        required: true
    }
})
const popup_vue = ref(false);
const { $bridge } = useNuxtApp();
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const userStore = useUserStore();
const user = userStore.user;
const imageget = ref('');
const emit = defineEmits(['delete', 'modify']);
const reactions = ref<Reaction[]>([]);
const { $signalr } = useNuxtApp();
const signalr = $signalr as SignalRClient;

const showPopup = () => {
    popup_vue.value = true;
};

const confirmDelete = async () => {
    popup_vue.value = false;
    try {
        await api.deleteReminder(props.post.id);
        emit('delete');
    } catch (error) {
        console.error('Failed to delete the post:', error);
    }
};

const handleModify = () => {
    emit('modify');
};

const cancelDelete = () => {
    popup_vue.value = false;
};

signalr.on("NewReaction", async (ReactionOutput) => {
    const reaction = ReactionOutput as Reaction;
    if (reaction.reminderId == props.post.id) {
        reactions.value.push(reaction);
    }
})

signalr.on("DeleteReaction", async (GUID) => {
    const input = GUID as string;
    reactions.value = reactions.value.filter(reaction => reaction.id !== input);
})

signalr.on("UpdateReaction", async (ReactionOutput) => {
    const reaction = ReactionOutput as Reaction;
    if (reaction.reminderId == props.post.id) {
        for (let i = 0; i < reactions.value.length; i++) {
            if (reactions.value[i].id == reaction.id) {
                reactions.value[i] = reaction;
            }
        }
    }
})

onMounted(async () => {
    reactions.value = [];
    await getReactions();
    if (props.post.reminderType == 1) {
        api.getImagefromcache(props.post.imageUrl).then((image) => {
            if (image) {
                imageget.value = image;
            } else {
                console.error('Image non trouvée dans le cache');
            }
        }).catch((error) => {
            console.error('Erreur lors de la récupération de l\'image :', error);
        });
    }
});

const getReactions = async () => {
    try {
        const data = await api.getReactionsReminder(props.post.id);
        if (data && Array.isArray(data)) {
            reactions.value = data;
        } else {
            console.error('Données de réaction invalides reçues:', data);
        }
    } catch (error) {
        console.error('Erreur lors de la récupération des réactions:', error);
    }
}

const toggleCheck = (item: any) => {
    item.isChecked = !item.isChecked;
    item.createdBy = user.id;
    api.updateReminderShoppingListItem(item).then(() => {
    }).catch((error) => {
        console.error('Error updating item:', error);
    });
}
</script>

<style scoped>
.post {
    width: 250px;
    height: 250px;
    margin: 20px;
    position: relative;
    border-radius: 96% 4% 92% 8% / 0% 100% 6% 100%;
    box-shadow: -9px 16px 12px 0px rgba(0, 0, 0, 0.33);
}

.post_image {
    width: 250px;
    margin: 20px;
    position: relative;
    background-color: transparent;
    box-shadow: var(--rectangle-shadow-light);
}

.edit-button {
    display: flex;
    justify-content: center;
    align-items: center;
    position: absolute;
    top: -20px;
    right: -20px;
    background: var(--main-buttons);
    border: none;
    border-radius: 9px;
    width: 40px;
    height: 40px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
}

.edit-icon {
    height: 33px;
    width: 33px;
    filter: var(--icon-filter);
}

.delete-button {
    display: flex;
    justify-content: center;
    align-items: center;
    position: absolute;
    top: 10px;
    right: 14px;
    background: var(--basic-red);
    border: none;
    border-radius: 50%;
    width: 30px;
    height: 30px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
}

.close {
    height: 12px;
    width: 12px;
    clip-path: polygon(20% 0%, 0% 20%, 30% 50%, 0% 80%, 20% 100%, 50% 70%, 80% 100%, 100% 80%, 70% 50%, 100% 20%, 80% 0%, 50% 30%);
    background-color: white;
}

.dark .close {
    filter: brightness(0);
}

.post p {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 200px;
    max-height: 250px;
    color: rgb(10, 10, 10);
    overflow-wrap: anywhere;
    text-align: center;
}

.post h3 {
    text-align: center;
    margin: 0 auto;
    width: 100%;
}

.profile-icon {
    position: absolute;
    top: 10px;
    left: 10px;
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

.image {
    max-width: 100%;

}

.react-button {
    display: flex;
    justify-content: center;
    align-items: center;
    position: absolute;
    bottom: 10px;
    right: 14px;
    background: var(--main-buttons);
    border: none;
    border-radius: 50%;
    width: 30px;
    height: 30px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
}

.heart {
    height: 30px;
    width: 30px;
    padding-top: 2px;
    text-align: center;
}

.reaction {
    font-size: 22px;
    text-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
    width: 14px;
    overflow-x: visible;
}

.reaction-list {
    position: absolute;
    bottom: -8px;
    width: 70%;
    display: flex;
    white-space: nowrap;
    /* overflow-x: scroll; */
    justify-content: left;
}

.check-zone {
    width: 20px;
    height: 20px;
    border: 2px solid #ddd;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;
}

.check-zone.checked {
    background-color: #85AD7B;
    border-color: #85AD7B;
}

.check-zone:not(.checked) {
    border-color: #8D90D6;
}

.modify-input {
    border: none;
    border-radius: 5px;
    background-color: #393a40;
    outline: none;
}

.shopping-container {
    background-color: var(--main-buttons);
    color: var(--page-text);
    width: 310px;
    padding: 20px 20px 50px 20px;
    border-radius: 0px;
    margin: 40px 20px 20px 20px;
    position: relative;
    box-shadow: var(--rectangle-shadow-light);
    font-weight: 600;
}

.shopping-title {
    font-weight: 600;
    font-size: 20px;
    border-bottom: 2px dashed var(--page-text);
}

.shopping-items {
    max-height: 310px;
    overflow-y: scroll;
}

.shopping-header {
    display: grid;
    grid-template-columns: 9fr 1fr;
    font-weight: bold;
    align-items: center;
    border-bottom: 1px solid var(--sent-message);
    margin-top: 10px;
}

.shopping-name {
    display: grid;
    align-items: center;
    margin-right: 10px;
    padding-left: 2px;
}
</style>
