<template>
    <div>
        <div class="conteneur">
            <div class="header">
                <h1 class="header">
                    <Texte_language source="ColocationMenu" />
                </h1>
            </div>
            <div class="colocation-container">
                <form class="create-colocation">
                    <h2>
                        <Texte_language source="ColocationName" />:
                    </h2>
                    <input type="text" class="input" v-model="colocation.name" required />
                    <h2>
                        <Texte_language source="ColocationAdress" />:
                    </h2>
                    <input type="text" class="input" v-model="colocation.address" required />
                    <button class="button" @click.prevent="createColocation()">
                        <Texte_language source="CreateColocation" />
                    </button>
                </form>
                <form class="create-colocation">
                    <h2>
                        <Texte_language source="ColocationID" />:
                    </h2>
                    <input type="text" class="input" v-model="new_data.colocationId" required />
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

const colocation = ref({
    name: '',
    address: '',
    createdBy: user.id,
})
const new_data = ref({
    username: user.username,
    email: user.email,
    colocationId: '',
    pathToProfilePicture: 'exempledetest',
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
    width: 100%;
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

.colocation-container {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    align-items: center;
}

h2 {
    margin-top: 12px;
    font-weight: 600;
}

.create-colocation {
    width: 100%;
    margin-top: 40px;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    text-align: center;
}

.input {
    width: 80%;
    height: 46px;
    margin-top: 12px;
    padding-left: 20px;
    border: none;
    border-radius: 20px;
    outline: none;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
    font-weight: 500;
    font-size: 20px;
}

.button {
    width: 80%;
    height: 70px;
    margin-top: 12px;
    padding: 10px 20px;
    border-radius: 15px;
    border: 0;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
    background-color: #85AD7B;
    font-size: 32px;
    font-weight: 600;
    text-align: center;
}
</style>