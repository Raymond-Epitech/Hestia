<template>
    <div v-if="isModalImageOpen">
        <ChangeProfilePic v-model="isModalImageOpen" :user="user" @close="isModalImageOpen = false"/>
    </div>
    <button class="back" @click="redirect('/settings')">
        <img src="~/public/Retour.svg" class="icon">
    </button>
    <div class="page-container">
        <div class="user">
            <div  @click="openImageModal">
                <ProfileIcon v-if="user" :key="user.id" :height="200" :width="200" :linkToPP="user.profilePictureUrl"/>
            </div>
            <div v-if="user" class="username">
                <input class="username-input" v-model="user.username"></input>
                <div v-if="user.username !== userStore.user.username">
                    <button class="button" @click="handleProceed">{{ $t('modify') }}</button>
                </div>
                <div v-else>
                    <button class="button" @click="handleProceed" disabled>{{ $t('modify') }}</button>
                </div>
            </div>
        </div>
    </div>

    <div v-if="errview">
        <Errorpopup :status="err.status" :body="err.body" @close="errview = false" />
    </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';
const userStore = useUserStore();
const { $bridge } = useNuxtApp();
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const errview = ref(false);
const isModalImageOpen = ref(false)
const user = ref([]);
const router = useRouter();
const redirect = (page) => {
  router.push(page);
}
const openImageModal = () => (isModalImageOpen.value = true)

api.getUserbyId(userStore.user.id).then((response) => {
    user.value = response;
}).catch((error) => {
    console.error(error);
    err.valueOf = error;
    errview.value = true;
});

const handleProceed = async () => {
  if (user.username !== "") {
    const data = await api.updateUser(user.value).catch((error) => {
        console.error(error);
        err.value = error;
        errview.value = true;
    });
    if (data) {
        userStore.setUser(user.value)
    }
  }
}

</script>

<style scoped>
.back {
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
  top: 3%;
  left: 3%;
}

.back .icon {
  width: 25px;
}

.icon {
  display: flex;
  align-items: center;
  justify-content: center;
  filter: var(--icon-filter);
}

.page-container {
    margin-bottom: 4.5rem;
    height: calc(100vh - 4.5rem);
    display: grid;
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
    padding: 20px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-evenly;
    font-size: 24px;
    font-weight: 600;
}

.username-input {
    width: 80%;
    padding: 5px 0px;
    outline: none;
    background-color: var(--main-buttons);
    color: var(--page-text);
    border: none;
    border-radius: 16px;
    text-align: center;
}

.button {
    padding: 10px 20px;
    margin: 10px;
    border: none;
    border-radius: 16px;
    display: flex;
    justify-content: center;
    gap: 16px;
    color: var(--page-text);
}

.button:disabled {
    opacity: 0.5;
}
</style>