<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
            <h1><Texte_language source="regularize" /></h1>
            <p></p>
        </div>
        <div class="center-container">
            <Rectangle v-for="coloc in list_coloc" :key="coloc.id" :color="getBalance(coloc.id) < 0 ? '#FF6A61' : '#85AD7B'" class="center">
                <h3>{{ coloc.username }}</h3>
                <h4>
                    {{ getBalance(coloc.id) }} €
                </h4>
            </Rectangle>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { Coloc, UserBalance } from '~/composables/service/type';

const list_balance = ref<UserBalance[]>([]);
const list_coloc = ref<Coloc[]>([]);
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
//a changer par la vrai colocid
const collocid = "164cb6e7-b8dd-4391-828d-e5ba7be45039"
const myid = "939da183-4c1e-4be6-8c64-fa4c012c7a02"

api.getBalance(collocid).then((response) => {
    list_balance.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
})
api.getUserbyCollocId(collocid).then((response) => {
    list_coloc.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
})

const getBalance = (id: string): number => {
    const balance = list_balance.value.find(userBalance => userBalance.userId === id);
    return balance ? balance.personalBalance : 0;
};
</script>


<style scoped>
.background {
    height: 100vh;
    background-color: #1E1E1E;
    color: white;
    padding: 20px;
}

.header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
}

.center {
    width: 50%;
    margin: 10px;
    padding: 10px;
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 10px;
    flex-direction: column;
}

.center-container {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
}

</style>