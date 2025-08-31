<template>
    <div :class="[post.reminderType == 1 ? 'post_image' : 'post', , post.reminderType != 1 && color]">
        <ProfileIcon class="profile-icon" :height="30" :width="30" :linkToPP="props.linkToPP" />
        <button class="delete-button" @click="showPopup" v-if="createdBy == user.id">
            <div class="close"></div>
        </button>
        <p v-if="post.reminderType == 0">{{ text }}</p>
        <img v-if="post.reminderType == 1" :src="imageget" alt="Post Image" class="image" />
    </div>
    <popup v-if="popup_vue" :text="$t('confirm_delete_reminder')" @confirm="confirmDelete" @close="cancelDelete">
    </popup>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import { useI18n } from '#imports';
import type { Reminder } from '../composables/service/type'

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
    },
    createdBy: {
        type: String,
        required: true
    },
    linkToPP: {
        type: String,
        required: false,
    },
    post: {
        type: Object as PropType<Reminder>,
        required: true
    }
})
const popup_vue = ref(false)
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const userStore = useUserStore();
const user = userStore.user;
const imageget = ref('');
const emit = defineEmits(['delete'])

const showPopup = () => {
    popup_vue.value = true;
};

const confirmDelete = async () => {
    popup_vue.value = false;
    try {
        await api.deleteReminder(props.id);
        emit('delete');
    } catch (error) {
        console.error('Failed to delete the post:', error);
    }
};

const cancelDelete = () => {
    popup_vue.value = false;
};

onMounted(() => {
    if (props.post.reminderType == 1) {
        console.log('Image URL:', props.text);
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
}

.delete-button {
    display: flex;
    justify-content: center;
    align-items: center;
    position: absolute;
    top: 10px;
    right: 14px;
    background: #FF6A61;
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
    width: 200px;
    max-height: 250px;
    color: rgb(10, 10, 10);
    overflow-wrap: anywhere;
    text-align: center;
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
</style>