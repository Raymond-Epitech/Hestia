<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
            <h1>{{ name }}</h1>
            <div class="square">
                <Rectangle color="#FFF973" id="add" :onClick="() => redirectto()">
                    <img src="/plus.png" alt="Return" width="30" height="30" />
                </Rectangle>
            </div>
        </div>
        <div>
            <ExpenseItem v-for="expense in expenses_list" :key="expense.id" :expense="expense" :onclick="() => redirecttomodify(expense.id)"
                :paidBy="getUsername(expense.paidBy)" />
        </div>
    </div>
</template>

<script setup lang="ts">
import type { Expenseget, Coloc } from '~/composables/service/type';
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const user = userStore.user;
const route = useRoute();
const router = useRouter();
const name = route.query.name;
const expenses_list = ref<Expenseget[]>([]);
const list_coloc = ref<Coloc[]>([]);
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const collocid = user.colocationId;

api.getExpenseByColocationId(collocid).then((response) => {
    const matchingCategory = response.find(expenseList => expenseList.category === name);
    expenses_list.value = matchingCategory ? matchingCategory.expenses : [];
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
    router.push({ path: '/money/addExpense', query: { name } });
}

const redirecttomodify = (id: string) => {
    router.push({ path: '/money/ModifyExpense', query: { id } });
}
</script>

<style scoped>
.background {
    height: 100vh;
    background-color: #1E1E1E;
    color: white;
    padding: 20px;
}

.square {
    width: 50px;
    height: 64px;
}

.header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
}
</style>
