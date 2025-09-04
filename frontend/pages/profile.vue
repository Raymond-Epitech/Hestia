<template>
    <div>
        <button class="settings" @click="redirect('/settings')">
            <img src="~/public/profile/settings.svg" class="icon">
        </button>
        <button class="add-user" @click="redirect('/add-user')">
            <img src="~/public/profile/addUser.svg" class="icon">
        </button>
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
const user = userStore.user;
const router = useRouter();
const { $bridge } = useNuxtApp();
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const colocationData = ref([])
const list_coloc = ref([]);

api.getColocationById(userStore.user.colocationId).then((response) => {
    colocationData.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
});

api.getUserbyCollocId(userStore.user.colocationId).then((response) => {
    list_coloc.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
});

const redirect = (page) => {
    router.push(page);
}
</script>

<style scoped>

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
    filter: brightness(0%);
}

.dark .icon {
    filter: none;
}

.hestia .icon {
    filter: none;
}

.add-user {
    top: 3%;
    right: 3%;
}

.settings {
    top: 3%;
    left: 3%;
}

.colocation-preview {
    padding: 1rem;
    display: grid;
    position: fixed;
    width: 100%;
    bottom: 4.5rem;
    max-height: (100rem - 4.5rem);
    grid-template-rows: 1fr;
    background-color: var(--main-buttons);
    justify-content: center;
    align-items: center;
    text-align: center;
    border-top-left-radius: 30px;
    border-top-right-radius: 30px;
    box-shadow: var(--button-shadow-light)
}

.header {
    padding: 13px;
    padding-bottom: 16px;
    font-size: 26px;
    font-weight: 600;
    border-radius: 20px;
    background-color: var(--sent-message);
    /* box-shadow: var(--button-shadow-light); */
    color: var(--page-text);
}

sub {
    margin-bottom: 1rem;
    font-size: 18px;
    line-height: 1.4rem;
    font-weight: 600;
    color: var(--page-text);
}

.roommates-list {
    max-height: 20rem;
    overflow-y: auto;
    margin-top: 5px;
    padding: 0 1rem;
    /* text-align: left; */
    font-weight: 600;
    /* width: fit-content; */
    /* max-width: 90%; */
    border-radius: 20px;
    background-color: var(--recieved-message);
    /* box-shadow: var(--button-shadow-light); */
    color: var(--page-text);
    /* align-items: center; */
}

.colocation-name {
    font-size: 22px;
    padding-top: 1rem;
    padding-bottom: 1rem;
    stroke: var(basic-grey);
    stroke-width: 2px;
    background-color: var(--recieved-message);
    /* box-shadow: var(--button-shadow-light); */
    border-radius: 20px;
    line-height: 2.2rem;
    color: var(--page-text);
}

</style>
