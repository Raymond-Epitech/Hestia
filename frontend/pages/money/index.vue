<template>
    <div class="body-container">
        <AddCategoryModal v-model="isCategoryModalOpen" @proceed="getall()" />
        <AddBalanceModal v-model="isBalanceModalOpen" @proceed="getall()" />
        <div class="top-bar">
            <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openCategoryModal">
                <img src="~/public/plus.png" class="plus">
            </button>
            <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openBalanceModal">
                <img src="~/public/dollar-sign.svg" class="plus">
            </button>
        </div>
        <div class="center-container">
            <div v-for="expense in expenses_list" :key="expense.id" class="center-container" >
                <ExpenseCategoryBox :expense="expense" @proceed="getall()"/>
            </div>
            <div class="global">
                <Texte_language class="category" source="global" />
                <text class="regularize-text number">
                    {{ global }} €
                </text>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { expenses_category_get } from '~/composables/service/type';
import { useI18n } from 'vue-i18n';
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const { t } = useI18n();
const router = useRouter();
const global = ref(0);
const expenses_list = ref < expenses_category_get[] > ([]);
const collocid = user.colocationId

const isCategoryModalOpen = ref(false)
const openCategoryModal = () => (isCategoryModalOpen.value = true)

const isBalanceModalOpen = ref(false)
const openBalanceModal = () => (isBalanceModalOpen.value = true)

const redirectto = (name: string, id?: string) => {
    if (name === 'balance') {
        router.push({ path: '/money/balance' });
        return;
    }
    if (name === 'add_category') {
        router.push({ path: '/money/addCategory' });
        return;
    }
    router.push({ path: '/money/historical', query: { name, id } });
}

const getall = async () => {
    api.getExpenseByColocationId(collocid).then((response) => {
        expenses_list.value = response.filter(item => item.name !== "refund");
        console.log(expenses_list.value)
        global.value = expenses_list.value.reduce((acc, expense) => acc + expense.totalAmount, 0);
    }).catch((error) => {
        console.error('Error fetching data:', error);
    })
}

onMounted(async () => {
    await getall();
});
</script>

<style scoped>
.center-container {
    height: 100%;
    width: 100%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
}

.top-bar {
    display: flex;
    justify-content: space-between;
}

.add-post {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    margin: 8px 5%;
    background-color: var(--main-buttons-light);
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
}

.dark .add-post {
    background-color: var(--main-buttons-dark);
}

.plus {
    width: 20px;
    height: 20px;
}

.dark .plus {
    filter: invert(1) opacity(1);
}

.expense {
    width: 90%;
    height: 88px;
    margin: 10px;
    padding: 10px;
    display: grid;
    align-items: center;
    grid-template-columns: 1fr 1fr;
    justify-content: space-between;
    background-color: var(--recieved-message-light);
    border-radius: 20px;
    box-shadow: var(--rectangle-shadow-light);
    color: var(--page-text-light);
}

.dark .expense {
    background-color: var(--recieved-message-dark);
    color: var(--page-text-dark);
}

.regularize-text {
    font-size: 24px;
    font-weight: 600;
}

.category {
    font-size: 20px;
    margin-left: 5px;
    font-weight: 600;
    text-align: left;
}

.number {
    font-size: 28px;
    margin-right: 5px;
    text-align: right;
}

.global {
    position: fixed;
    bottom: 62px;
    width: 100%;
    height: 88px;
    padding: 10px;
    display: grid;
    align-items: center;
    grid-template-columns: 1fr 1fr;
    justify-content: space-between;
    background-color: var(--main-buttons-light);
    border-radius: 20px 20px 0px 0px;
    box-shadow: var(--button-shadow-light);
    color: var(--page-text-light);
}

.dark .global {
    background-color: var(--main-buttons-dark);
    color: var(--page-text-dark);
}
</style>