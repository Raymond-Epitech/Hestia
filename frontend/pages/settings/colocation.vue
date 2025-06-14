<template>
    <div class="page-container">
        <div class="colocation-preview-container">
            <div class="colocation-preview">
                <text class="header">
                    <Texte_language source="flatInfo" /> :
                </text>
                <text class="subtext">{{ colocationData.name }}</text>
                <text class="subtext">{{ colocationData.address }}</text>
                <text class="subtext">
                    <Texte_language source="ColocationID" /> :
                </text>
                <text class="colocation-id">{{ colocationData.id }}</text>
                <text class="subtext">
                    <Texte_language source="roomates" /> :
                </text>
                <div class="roommates-list">
                    <div v-for="coloc in list_coloc" :key="coloc.id" :index="coloc.id">
                        {{ coloc.username }}
                    </div>
                </div>
            </div>
            <button class="button" :onClick="() => redirect('/settings/change-colocation')">
                <Texte_language source="changeFlat" />
            </button>
        </div>
    </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const colocationData = ref([])
const list_coloc = ref([]);
const router = useRouter();

api.getColocationById(userStore.user.colocationId).then((response) => {
    colocationData.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
});

api.getUserbyCollocId(userStore.user.colocationId).then((response) => {
    list_coloc.value = response;
    console.log(list_coloc)
}).catch((error) => {
    console.error('Error fetching data:', error);
});

const redirect = (page) => {
    router.push(page);
}
</script>

<style scoped>
.page-container {
    position: fixed;
    inset: 0px;
    width: fit-content;
    height: fit-content;
    max-width: 100vw;
    max-height: 100dvh;
    margin: 20% auto auto auto;
}

.colocation-preview-container {
    display: flex;
    flex-direction: column;
    align-items: center;
}

.colocation-preview {
    padding: 30px 5%;
    display: grid;
    grid-template-rows: 1fr;
    background-color: #a3d297;
    justify-content: center;
    align-items: center;
    text-align: center;
    border-radius: 30px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.header {
    padding-bottom: 20px;
    font-size: 26px;
    font-weight: 600;
}

.subtext {
    margin-bottom: 10px;
    font-size: 18px;
    font-weight: 600;
}

.colocation-id {
    margin-bottom: 12px;
    background-color: #E7FEED;
    border-radius: 6px;
    font-size: 14px;
    user-select: all;
}

.button {
    width: fit-content;
    margin-top: 20%;
    background-color: #85AD7B;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
    font-weight: 600;
}
</style>