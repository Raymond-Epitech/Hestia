<template>
    <div class="conteneur">
        <form class="conteneur form">
            <h1>
                <Texte_language source="ColocationID" />:
            </h1>
            <input type="text" class="input" v-model="new_data.colocationId" required />
            <button class="button" @click.prevent="handleProceed()">
                <Texte_language source="JoinColocation" />
            </button>
        </form>
    </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');

const new_data = ref({
    username: user.username,
    email: user.email,
    colocationId: '',
    pathToProfilePicture: 'exempledetest',
    id: user.id,
})
const handleProceed = async () => {
    const data = await api.updateUser(new_data.value)
    if (data) {
        userStore.setColocation(new_data.colocationId);
    }
}
</script>

<style scoped>
.conteneur {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
}

.form {
    margin-top: 40%;
    height: 300px;
    width: 300px;
    background-color: #85AD7B;
    border-radius: 15px;
}

.input {
    height: 30px;
    border: none;
    border-radius: 8px;
    outline: none;
    background-color: #E7FEED;
}

.button {
    margin-top: 10px;
    padding: 10px 20px;
    border-radius: 15px;
    border: 0;
}
</style>