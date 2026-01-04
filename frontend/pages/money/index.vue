<template>
    <div class="body-container">
        <AddCategoryModal v-model="isCategoryModalOpen" @proceed="getall()" />
        <AddBalanceModal v-model="isBalanceModalOpen" @proceed="getall()" />
        <div class="top-bar">
            <button data-toggle="modal" data-target=".bd-example-modal-sm" @click="openCategoryModal">
                <img src="~/public/plus.png" class="plus">
            </button>
            <button class="balance" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openBalanceModal">
                {{$t('balance')}}
            </button>
        </div>
        <div class="center-container">
            <div v-for="expense in expenses_list" :key="expense.id" class="center-container">
                <ExpenseCategoryBox :expense="expense" @proceed="getall()" @delete="getall()" />
            </div>
            <div class="global">
                <Texte_language class="category" source="global" />
                <text class="regularize-text number">
                    {{ global }} €
                </text>
            </div>
        </div>
        <div v-if="errview">
            <Errorpopup :status="err.status" :body="err.body" @close="errview = false" />
        </div>
    </div>
</template>

<script setup lang="ts">
import type { expenses_category_get, SignalRClient } from '~/composables/service/type';
import { useI18n } from 'vue-i18n';
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
const err = ref<{ status: number; body: any }>({ status: 0, body: null });
const errview = ref(false);
api.setjwt(useCookie('token').value ?? '');
const { t } = useI18n();
const router = useRouter();
const global = ref(0);
const expenses_list = ref<expenses_category_get[]>([]);
const collocid = user.colocationId;
const { $signalr } = useNuxtApp();
const signalr = $signalr as SignalRClient;

const isCategoryModalOpen = ref(false)
const openCategoryModal = () => (isCategoryModalOpen.value = true)

const isBalanceModalOpen = ref(false)
const openBalanceModal = () => (isBalanceModalOpen.value = true)

signalr.on("NewExpenseCategoryAdded", async (CategoryOutput) => {
    if (!expenses_list.value.some(expense => expense.id === CategoryOutput.id)) {
        expenses_list.value.push(CategoryOutput)
    }
})

signalr.on("ExpenseCategoryUpdated", (CategoryOutput) => {
    expenses_list.value = expenses_list.value.filter(expense => expense.id !== CategoryOutput)
})

signalr.on("NewExpenseAdded", (CategoryOutput) => {
    getall();
})

signalr.on("expensecategorydeleted", (CategoryOutput) => {
    getall();
})

signalr.on("ExpenseUpdated", (CategoryOutput) => {
    getall();
})

signalr.on("ExpenseDeleted", (CategoryOutput) => {
    getall();
})

const getall = async () => {
    api.getExpenseByColocationId(collocid).then((response) => {
        expenses_list.value = response.filter(item => item.name !== "Refund");
        global.value = expenses_list.value.reduce((acc, expense) => acc + expense.totalAmount, 0);
    }).catch((error) => {
        console.error(error);
        err.value = error;
        errview.value = true;
    });
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

button {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    margin: 8px 5%;
    background-color: var(--main-buttons);
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
}

.balance {
    padding: 5px 10px;
    font-size: 14px;
    font-weight: 600;
    color: var(--page-text);
    width: auto;
}

.plus {
    width: 20px;
    height: 20px;
}

.dark .plus {
    filter: invert(1) opacity(1);
}

.hestia .plus {
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
    background-color: var(--recieved-message);
    border-radius: 20px;
    box-shadow: var(--rectangle-shadow-light);
    color: var(--page-text);
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
    background-color: var(--main-buttons);
    border-radius: 20px 20px 0px 0px;
    box-shadow: var(--button-shadow-light);
    color: var(--page-text);
}
</style>