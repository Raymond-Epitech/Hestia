<template>
    <button class="back" @click="redirect('/settings')">
        <img src="~/public/Retour.svg" class="icon">
    </button>
    <div class="page-conteneur">
        <div class="colocation-container">
            <form class="create-colocation">
                <Texte_language source="newFlateshare"/>
                <input type="text" class="input" v-model="colocation.name" :placeholder="$t('ColocationName')" required />
                <input type="text" class="input" v-model="colocation.address" :placeholder="$t('ColocationAdress')" required />
                <button class="button" @click.prevent="createColocation()">
                    <Texte_language source="CreateColocation" />
                </button>
            </form>
            <form class="create-colocation">
                <Texte_language source="ChangeColocation" />
                <input type="text" class="input" v-model="new_data.colocationId" :placeholder="$t('ColocationID')" required />
                <button class="button" @click.prevent="joinColocation()">
                    <Texte_language source="JoinColocation" />
                </button>
            </form>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';

const router = useRouter();
const route = useRoute();
const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const collocID = route.query.collocID as string;
const redirect = (page) => {
    router.push(page);
}

const colocation = ref({
    name: '',
    address: '',
    createdBy: user.id,
})
const new_data = ref({
    username: user.username,
    email: user.email,
    colocationId: collocID ? collocID : '',
    pathToProfilePicture: null,
    id: user.id,
})
const joinColocation = async () => {
    const data = await api.updateUser(new_data.value)
    if (data) {
        userStore.setColocation(new_data.value.colocationId);
        router.push('/');
    }
}
const createColocation = async () => {
    const data = await api.addColocation(colocation.value);
    if (data) {
        userStore.setColocation(data);
        router.push('/');
    }
}

</script>

<style scoped>
.page-conteneur {
    width: 100%;
    height: calc(100vh - 4.5rem);
    display: flex;
    justify-content: center;
    overflow: scroll;
}

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
    filter: invert(1);
    width: 25px;
}

.dark .back .icon {
    filter: none;
}

.hestia .back .icon {
    filter: none;
}

.colocation-container {
    margin: 100px 0px 20px 0px;
    padding: 30px;
    display: flex;
    height: fit-content;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: var(--login-box-bg);
    border-radius: 20px;
    box-shadow: var(--rectangle-shadow-light);
    gap: 40px;
    font-size: 20px;
    font-weight: 600;
    text-align: center;
    color: var(--page-text);
}

.create-colocation {
    display: grid;
    gap: 10px;
}

.input {
    outline: none;
    border-radius: 18px;
    border: none;
    font-size: 16px;
    text-align: start;
    padding-left: 12px;
    background-color: var(--secondary-button);
    color: var(--secondary-page-text);
}

.button {
    padding: 10px 20px;
    font-weight: 600;
    border-radius: 16px;
    background-color: var(--secondary-button);
    color: var(--secondary-page-text);
}

</style>