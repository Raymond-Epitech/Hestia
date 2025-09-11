<template>
    <div>
        <div class="conteneur">
            <!-- <div class="header">
                <h1 class="header">
                    <Texte_language source="ColocationMenu" />
                </h1>
            </div> -->
            <div class="colocation-container">
                <form class="create-colocation">
                    <Texte_language source="newFlateshare" />
                    <input type="text" class="input" v-model="colocation.name" :placeholder="$t('ColocationName')"
                        required />
                    <input type="text" class="input" v-model="colocation.address" :placeholder="$t('ColocationAdress')"
                        required />
                    <button class="button" @click.prevent="createColocation()">
                        <Texte_language source="CreateColocation" />
                    </button>
                </form>
                <form class="create-colocation">
                    <Texte_language source="ChangeColocation" />
                    <input type="text" class="input" v-model="new_data.colocationId" :placeholder="$t('ColocationID')"
                        required />
                    <button class="button" @click.prevent="joinColocation()">
                        <Texte_language source="JoinColocation" />
                    </button>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';

definePageMeta({
    layout: false
})

const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const router = useRouter();
const route = useRoute();
const collocID = route.query.collocID

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
.conteneur {
    display: flex;
    align-items: center;
    justify-content: center;
}

.header {
    height: 200px;
    display: flex;
    justify-content: center;
    background-color: #BAE5C6;
    border-radius: 0px 0px 400px 191px / 0px 0px 71px 12px;
}

h1 {
    width: 80%;
    align-items: center;
    text-align: center;
    font-size: 32px;
    font-weight: 600;
    z-index: 1;
}

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
    filter: var(--icon-filter);
    width: 25px;
}

.colocation-container {
    margin-top: 100px;
    padding: 30px;
    width: 90%;
    height: fit-content;
    display: flex;
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