<template>
    <div class="background">
        <TexteLanguage source="header_refund" />
        <div>
            <h2><TexteLanguage source="refund_with_me"/></h2>
            <ul>
                <li v-for="refund in userRefunds" :key="`${refund.from}-${refund.to}-${refund.amount}}`">
                    <span v-if="refund.from === user.id">De vous à {{ getUsername(refund.to) }}</span>
                    <span v-else>De {{ getUsername(refund.from) }} à vous</span>
                    <span> Montant : </span>
                    {{ refund.amount }}€
                </li>
            </ul>
        </div>
        <div>
            <h2><TexteLanguage source="other_refund"/></h2>
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
import type { Coloc, refund } from '~/composables/service/type';

const route = useRoute();
const listColoc = ref<Coloc[]>([]);
const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
const refund_list = ref<refund[]>([]);
api.setjwt(useCookie('token').value ?? '');

onMounted(() => {
    const queryListColoc = route.query.list_coloc as string;
    if (queryListColoc) {
        listColoc.value = JSON.parse(queryListColoc);
    }
    api.getRefund(user.colocationId).then((response) => {
        refund_list.value = response;
        console.log(refund_list.value);
    }).catch((error) => {
        console.error('Error fetching data:', error);
    });
});

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
  height: 100vh;
  background-color: #1E1E1E;
  color: white;
  padding: 20px;
}
</style>