<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
            <h1>
                <TexteLanguage source="header_refund" />
            </h1>
            <p></p>
        </div>
        <div>
            <h2>
                <TexteLanguage source="refund_with_me" />
            </h2>
            <ul>
                <li v-for="refund in userRefunds" :key="`${refund.from}-${refund.to}-${refund.amount}}`">
                    <span v-if="refund.from === user.id">De vous à {{ getUsername(refund.to) }}</span>
                    <span v-else>De {{ getUsername(refund.from) }} à vous</span>
                    <span> Montant : </span>
                    {{ refund.amount }}€
                    <button @click=refund_prosess(refund) class="button">
                        <TexteLanguage source="refund_button" />
                    </button>
                </li>
            </ul>
        </div>
        <div>
            <h2>
                <TexteLanguage source="other_refund" />
            </h2>
            <ul>
                <li v-for="refund in otherRefunds" :key="`${refund.from}-${refund.to}-${refund.amount}}`">
                    <span>De {{ getUsername(refund.from) }} à {{ getUsername(refund.to) }}</span>
                    <span> Montant : </span>
                    {{ refund.amount }}€
                </li>
            </ul>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { Coloc, refund, Expense } from '~/composables/service/type';

useHead({
    bodyAttrs: {
        style: 'background-color: #1E1E1E;'
    }
})

const route = useRoute();
const listColoc = ref<Coloc[]>([]);
const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
const refund_list = ref<refund[]>([]);
api.setjwt(useCookie('token').value ?? '');
let rufendcategoryId = '';

onMounted(() => {
    const queryListColoc = route.query.list_coloc as string;
    if (queryListColoc) {
        listColoc.value = JSON.parse(queryListColoc);
    }
    api.getRefund(user.colocationId).then((response) => {
        refund_list.value = response;
    }).catch((error) => {
        console.error('Error fetching data:', error);
    });
    api.getExpenseByColocationId(user.colocationId).then((response) => {
        response.forEach((expense) => {
            if (expense.name === "Refund") {
                rufendcategoryId = expense.id;
            }
        });
    }).catch((error) => {
        console.error('Error fetching expenses:', error);
    });

});

const refund_prosess = (refund: refund) => {
    const data: Expense = {
        colocationId: user.colocationId,
        createdBy: user.id,
        name: "Refund",
        description: "Remboursement de " + refund.amount + "€",
        amount: refund.amount,
        category: "Refund",
        paidBy: refund.from,
        splitType: 0,
        splitBetween: [refund.to],
        splitValues: {},
        splitPercentages: {},
        dateOfPayment: new Date().toISOString(),
        expenseCategoryId: rufendcategoryId,
    }
    api.addExpense(data).then((response) => {
        if (response === true) {
            window.location.reload();
        }
    }).catch((error) => {
        console.error('Error adding expense:', error);
    });
}

const userRefunds = computed(() => {
    return refund_list.value.filter(refund => refund.from === user.id || refund.to === user.id);
});

const otherRefunds = computed(() => {
    return refund_list.value.filter(refund => refund.from !== user.id && refund.to !== user.id);
});

const getUsername = (id: string): string => {
    const user = listColoc.value.find(coloc => coloc.id === id);
    return user ? user.username : id;
};
</script>

<style scoped>
.background {
    height: 100%;
    background-color: #1E1E1E;
    color: white;
    padding: 20px;
}
</style>