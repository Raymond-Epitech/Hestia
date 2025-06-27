<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
            <h1 class="header-name">{{ name }}</h1>
            <div class="square">
                <div id="add" :onClick="() => redirectto()">
                    <img src="/plus.png" alt="Return" width="30" height="30" />
                </div>
            </div>
        </div>
        <div>
            <ExpenseItem v-for="expense in expenses_list" :key="expense.id" :expense="expense"
                :onclick="() => redirecttomodify(expense.id)" :paidBy="getUsername(expense.paidBy)" />
        </div>
    </div>
</template>

<script setup lang="ts">
import type { Expenseget, Coloc } from '~/composables/service/type';
import { useUserStore } from '~/store/user';

useHead({
    bodyAttrs: {
        style: 'background-color: #1E1E1E;'
    }
})

const userStore = useUserStore();
const user = userStore.user;
const route = useRoute();
const router = useRouter();
const name = route.query.name;
const categoryId = route.query.id as string;
const expenses_list = ref<Expenseget[]>([]);
const list_coloc = ref<Coloc[]>([]);
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const collocid = user.colocationId;

api.getExpensebycategoryId(categoryId).then((response) => {
    expenses_list.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
})
api.getUserbyCollocId(collocid).then((response) => {
    list_coloc.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
})

const getUsername = (id: string): string => {
    const user = list_coloc.value.find(coloc => coloc.id === id);
    return user ? user.username : 'Unknown';
};

const redirectto = () => {
    console.log(name);
    router.push({ path: '/money/addExpense', query: { name, categoryId } });
}

const redirecttomodify = (id: string) => {
    router.push({ path: '/money/ModifyExpense', query: { id } });
}
</script>

<style scoped>
.background {
    height: 100%;
    background-color: #1E1E1E;
    color: white;
    padding: 20px;
}

.square {
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: #FFF973;
    width: 40px;
    height: 40px;
    border-radius: 10px;
    margin-left: 10px;
}

.header {
    width: 100%;
    display: grid;
    grid-template-columns: 1fr 6fr 1fr;
    align-items: center;
    margin-bottom: 20px;
    text-align: center;
}

.header-name {
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
}
</style>
