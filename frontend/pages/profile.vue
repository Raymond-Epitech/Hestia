<template>
    <button class="settings" @click="redirect('/settings')">
        <img src="~/public/profile/settings.svg" class="icon">
    </button>
    <button class="add-user" @click="redirect('/add-user')">
        <img src="~/public/profile/addUser.svg" class="icon">
    </button>
    <div class="page-container">
        <div class="user">
            <div v-if="user" class="username">{{ user.username }}</div>
            <ProfileIcon v-if="user" :key="user.id" :height="200" :width="200" :linkToPP="user.profilePictureUrl"/>
        </div>
        <div class="colocation-preview">
            <text class="header">
                <Texte_language source="flatInfo" />
            </text>
            <sub class="colocation-name">{{ colocationData.name }}<br>{{ colocationData.address }}</sub>
            <text class="header">
                <Texte_language source="roomates" /> :
            </text>
            <div class="roommates-list">
                <div v-for="coloc in list_coloc" :key="coloc.id" :index="coloc.id">
                    {{ coloc.username }}
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';
const userStore = useUserStore();
const router = useRouter();
const { $bridge } = useNuxtApp();
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const colocationData = ref([]);
const list_coloc = ref([]);
const user = ref(null);

api.getColocationById(userStore.user.colocationId).then((response) => {
    colocationData.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
});

api.getUserbyCollocId(userStore.user.colocationId).then((response) => {
    list_coloc.value = response;
    user.value = list_coloc.value.find(user => user.id === userStore.user.id) || null;
}).catch((error) => {
    console.error('Error fetching data:', error);
});


const redirect = (page) => {
    router.push(page);
}
</script>

<style scoped>
.page-container {
    margin-bottom: 4.5rem;
    height: calc(100vh - 4.5rem);
    display: grid;
}

button {
    background-color: var(--main-buttons);
    position: fixed;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 40px;
    height: 40px;
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
}

.icon {
    filter: var(--icon-filter);
}

.add-user {
    top: 3%;
    right: 3%;
}

.settings {
    top: 3%;
    left: 3%;
}

.user {
    margin-top: 80px;
    margin-bottom: 20px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: none;
}

.username {
    font-size: 24px;
    color: var(--secondary-page-text);
    font-weight: 600;
}

.colocation-preview {
    bottom: 4.5rem;
    padding: 1rem;
    display: grid;
    grid-template-rows: repeat(4,1fr);
    gap: 10px;
    background-color: var(--main-buttons);
    align-items: center;
    text-align: center;
    border-top-left-radius: 30px;
    border-top-right-radius: 30px;
    box-shadow: var(--button-shadow-light);
    overflow: scroll;
    scrollbar-width: none;
}

.header {
    padding: 13px;
    font-size: 26px;
    font-weight: 600;
    border-radius: 20px;
    background-color: var(--sent-message);
    /* box-shadow: var(--button-shadow-light); */
    color: var(--page-text);
}

sub {
    font-size: 18px;
    font-weight: 600;
    color: var(--page-text);
}

.roommates-list {
    margin-top: 6px;
    height: fit-content;
    padding: 13px;
    font-weight: 600;
    border-radius: 20px;
    background-color: var(--recieved-message);
    color: var(--page-text);
}

.colocation-name {
    margin-bottom: 12px;
    font-size: 22px;
    padding: 13px;
    stroke: var(basic-grey);
    stroke-width: 2px;
    background-color: var(--recieved-message);
    /* box-shadow: var(--button-shadow-light); */
    border-radius: 20px;
    line-height: 2.2rem;
    color: var(--page-text);
}

</style>
