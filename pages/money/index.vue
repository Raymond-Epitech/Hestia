<template>
    <div class="center-container">
        <Rectangle v-for="expense in expenses_list" :key="expense.id" color="#85AD7B" id="rec" class="expense mini_rec"
            :onClick="() => redirectto(expense.name, expense.id)">
            <text class="category">{{ expense.name }}</text>
            <text class="regularize-text number">
                {{ expense.totalAmount }} €
            </text>
        </Rectangle>
        <Rectangle color="#4FA3A6" id="rec" class="expense mini_rec">
            <Texte_language class="category" source="global" />
            <text class="regularize-text number">
                {{ global }} €
            </text>
        </Rectangle>
        <Rectangle color="#FFF973" id="rec" class="regularize-text mini_rec"
            :onClick="() => redirectto('add_category')">
            <Texte_language source="add_category" />
        </Rectangle>
        <Rectangle color="#FFF973" id="rec" class="regularize-text mini_rec" :onClick="() => redirectto('balance')">
            <Texte_language source="regularize" />
        </Rectangle>
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
const food = ref(0);
const health = ref(0);
const partie = ref(0);
const expenses_list = ref<expenses_category_get[]>([]);
const collocid = user.colocationId

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

api.getExpenseByColocationId(collocid).then((response) => {
    expenses_list.value = response;
    global.value = expenses_list.value.reduce((acc, expense) => acc + expense.totalAmount, 0);
}).catch((error) => {
    console.error('Error fetching data:', error);
})
</script>

<style scoped>
.center-container {
    min-height: 700px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
}

.mini_rec {
    width: 80%;
    margin: 10px;
    padding: 10px;
    display: grid;
    align-items: center;
}

.expense {
    grid-template-columns: 1fr 1fr;
    justify-content: space-between;
}

.regularize-text {
    font-size: 24px;
    font-weight: 600;
}

.category {
    margin-left: 5px;
    font-weight: 600;
    text-align: left;
}

.number {
    margin-right: 5px;
    text-align: right;
}
</style>