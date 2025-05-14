<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
            <h1>
                <Texte_language source="regularize" />
            </h1>
            <p></p>
        </div>
        <div class="center-container">
            <Rectangle v-for="coloc in list_coloc" :key="coloc.id"
                :color="getBalance(coloc.id) < 0 ? '#FF6A61' : '#85AD7B'" class="center">
                <h3>{{ coloc.username }}</h3>
                <h4>
                    {{ getBalance(coloc.id) }} €
                </h4>
            </Rectangle>
            <rectangle color="#4FA3A6" id="rec" class="center regularize-text mini_rec"
                :onClick="() => redirectto('balance')">
                <Texte_language source="refund" />
            </rectangle>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { Coloc, UserBalance } from '~/composables/service/type';
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const user = userStore.user;
const list_balance = ref<UserBalance>();
const list_coloc = ref<Coloc[]>([]);
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const collocid = user.colocationId
const myid = user.id

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
    return list_balance.value?.[id] ?? 0;
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