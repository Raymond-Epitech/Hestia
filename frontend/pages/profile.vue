<template>
    <button class="settings" @click="redirect('/settings')">
        <img src="~/public/profile/settings.svg" class="button-icon">
    </button>
    <button class="add-user" @click="redirect('/add-user')">
        <img src="~/public/profile/addUser.svg" class="button-icon">
    </button>
    <div class="page-container">
        <div class="user">
            <div v-if="user" class="username">{{ user.username }}</div>
            <ProfileIcon v-if="user" :key="user.id" :height="200" :width="200" :linkToPP="user.profilePictureUrl" />
        </div>
        <div class="colocation-preview">
            <text class="header">
                <Texte_language source="flatInfo" />
            </text>
            <div class="colocation-name">{{ colocationData.name }}<br>{{ colocationData.address }}</div>
            <text class="header">
                <Texte_language source="roomates" />
            </text>
            <div class="roommates-list">
                <div v-for="coloc in list_coloc" :key="coloc.id" :index="coloc.id">
                    {{ coloc.username }}
                </div>
            </div>
        </div>
        <div v-if="errview">
            <Errorpopup :status="err.status" :body="err.body" @close="errview = false" />
        </div>
    </div>
</template>

<script setup lang="ts">
import type { Coloc } from '~/composables/service/type';
import { useUserStore } from '~/store/user';
const userStore = useUserStore();
const router = useRouter();
const { $bridge } = useNuxtApp();
const api = $bridge;
const err = ref < { status: number, body: any } > ({ status: 0, body: null });
const errview = ref(false);
api.setjwt(useCookie('token').value ?? '');
const colocationData = ref([]);
const list_coloc = ref<Coloc[]>([]);
const user = ref<Coloc | null>(null);

api.getColocationById(userStore.user.colocationId).then((response) => {
    colocationData.value = response;
}).catch((error) => {
    console.error(error);
    err.valueOf = error;
    errview.value = true;
  });

api.getUserbyCollocId(userStore.user.colocationId).then((response) => {
    list_coloc.value = response;
    user.value = list_coloc.value.find(user => user.id === userStore.user.id) || null;
    console.log(user.value)
}).catch((error) => {
    console.error(error);
    err.valueOf = error;
    errview.value = true;
  });


const redirect = (page: any) => {
    router.push(page);
}
</script>

<style scoped>
.page-container {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    margin-bottom: 4.5rem;
    height: calc(100vh - 9rem);
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

.button-icon {
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
    margin-top: 200px;
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
    margin-bottom: 20px;
    padding: 30px;
    display: flex;
    height: fit-content;
    width: 80%;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: var(--login-box-bg);
    border-radius: 20px;
    box-shadow: var(--rectangle-shadow-light);
    gap: 10px;
    font-size: 16px;
    font-weight: 600;
    text-align: center;
    color: var(--page-text);
}

.header {
    width: 90%;
    padding: 8px 12px;
    font-weight: 600;
    border-radius: 18px;
    background-color: var(--sent-message);
    color: var(--page-text);
}

sub {
    padding: 8px 12px;
    font-weight: 600;
    border-radius: 18px;
    background-color: var(--sent-message);
    color: var(--page-text);
}

.roommates-list {
    width: 90%;
    padding: 8px 12px;
    font-weight: 600;
    border-radius: 18px;
    background-color: var(--secondary-button);
    color: var(--secondary-page-text);
}

.colocation-name {
    width: 90%;
    padding: 8px 12px;
    font-weight: 600;
    border-radius: 18px;
    background-color: var(--secondary-button);
    color: var(--secondary-page-text);
    line-height: 24px;
    font-size: 16px;
}
</style>
