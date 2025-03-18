<template>
    <div class="conteneur">
        <form class="conteneur form">
            <h1>
                <Texte_language source="ColocationName" />:
            </h1>
            <input type="text" class="input" v-model="colocation.name" required />
            <h1>
                <Texte_language source="ColocationAdress" />:
            </h1>
            <input type="text" class="input" v-model="colocation.adress" required />
            <button class="button" @click.prevent="handleProceed()">
                <Texte_language source="CreateColocation" />
            </button>
        </form>
    </div>
</template>

<script setup>
import { useUserStore } from '~/store/user';

const { $bridge } = useNuxtApp()
const api = $bridge;
const userStore = useUserStore();
const user = userStore.user;

console.log(userStore.user)
const colocation = ref({
    name: '',
    adress: '',
    createdBy: user.id,
})
const handleProceed = async () => {
    await api.addColocation(colocation.value)
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