<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" class="icon" alt="Return" width="30" height="30" @click="$router.back()" />
        </div>
        <div class="page-container">
            <div class="header-name">
                {{ $t('refund_history') }}
            </div>
            <div v-for="refund in refund_list" :key="refund.id" class="refund-rectangle">
                <div class="refund-text">
                    <text>{{ $t('from') }} {{ getUsername(refund.paidBy) }} {{ $t('to') }} {{getUsername(Object.keys(refund.splitBetween)[0])}}</text>
                    <text class="date"> {{ getDayNumber(refund.dateOfPayment) }}</text>
                </div>
                <text class="number"> {{ refund.amount }} € </text>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { expenses_category_get, Expenseget, Coloc } from '~/composables/service/type';
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
const refund = ref < expenses_category_get > ();
const refund_list = ref<Expenseget[]> ([]);
api.setjwt(useCookie('token').value ?? '');
const list_coloc = ref<Coloc[]>([]);

await api.getExpenseByColocationId(user.colocationId).then((response) => {
            refund.value = response.find(item => item.name === "refund");
            if (refund.value) {
                api.getExpensebycategoryId(refund.value.id).then((response) => {
                refund_list.value = response;
                }).catch((error) => {
                    console.error('Error fetching data:', error);
                })
            }
        }).catch((error) => {
            console.error('Error fetching data:', error);
        })

api.getUserbyCollocId(user.colocationId).then((response) => {
    list_coloc.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
})

const getUsername = (id: string): string => {
    const user = list_coloc.value.find(coloc => coloc.id === id);
    return user ? user.username : 'Unknown';
};

const getDayNumber = (timecode: string) => {
    const date = new Date(timecode);
    const yyyy = date.getFullYear();
    let mm = date.getMonth() + 1; // Months start at 0!
    let dd = date.getDate();
    if (dd < 10) dd = '0' + dd;
    if (mm < 10) mm = '0' + mm;
    const formattedToday = dd + '/' + mm + '/' + yyyy;

    return formattedToday;
}

</script>

<style scoped>
.background {
    padding: 20px;
}

.icon {
    display: flex;
    align-items: center;
    justify-content: center;
    filter: invert(1);
}

.dark .icon {
    filter: invert(0);
}

.page-container {
    width: 100%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    color: var(--page-text);
    font-weight: 600;
    text-align: center;
}

.header-name {
    width: 100%;
    font-size: 32px;
    margin: 10px 0px;
}

.refund-rectangle {
    width: 100%;
    height: 100px;
    display: grid;
    grid-template-columns: 2fr 1fr;
    justify-content: space-between;
    align-items: center;
    padding: 0% 6%;
    color: var(--page-text);
    font-size: 20px;
    font-weight: 600;
    border-radius: 20px;
    margin: 10px 0px;
    background-color: var(--sent-message);
}

.refund-text {
    display: flex;
    flex-direction: column;
    text-align: left;
    padding-top: 20px;
}

.date {
    font-size: 14px;
    margin-left: 6%;
}

.number {
    display: flex;
    justify-content: right;
    font-size: 28px;
}
</style>
