<template>
    <div  class="center-container">
        <Rectangle color="#85AD7B" id="rec" class="center mini_rec" :onClick="() => redirectto($t('food_expenses'))">
            <Texte_language source="food_expenses" /> 
            <p class="regularize-text">
                {{ food }} €
            </p>
        </Rectangle>
        <Rectangle color="#85AD7B" id="rec" class="center mini_rec" :onClick="() => redirectto($t('Health_expenses'))">
            <Texte_language source="Health_expenses" /> 
            <p class="regularize-text">
                {{ health }} €
            </p>
        </Rectangle>
        <Rectangle color="#85AD7B" id="rec" class="center mini_rec" :onClick="() => redirectto($t('partie_expenses'))">
            <Texte_language source="partie_expenses" /> 
            <p class="regularize-text">
                {{ partie }} €
            </p>
        </Rectangle>
        <Rectangle color="#4FA3A6" id="rec" class="center mini_rec">
            <Texte_language source="global" />
            <p class="regularize-text">
                {{ global }} €
            </p>
        </Rectangle>
        <Rectangle color="#FFF973" id="rec" class="center regularize-text mini_rec">
            <Texte_language source="regularize" />
        </Rectangle>
    </div>
</template>

<script setup lang="ts">
import type  { ExpenseList } from '~/composables/service/type';
import { useI18n } from 'vue-i18n';

const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const { t } = useI18n();
const router = useRouter();
const global = ref(0);
const food = ref(0);
const health = ref(0);
const partie = ref(0);
const expenses_list = ref<ExpenseList[]>([]);
//a changer par la vrai colocid
const collocid = "164cb6e7-b8dd-4391-828d-e5ba7be45039"
const redirectto = (name: string) => {
    console.log(name);
    router.push({ path: '/money/historical', query: { name } });
}
onMounted(() => {
api.getExpenseByColocationId(collocid).then((response) => {
    expenses_list.value = response;
    global.value = expenses_list.value.reduce((acc, expense) => acc + expense.totalAmount, 0);
}).catch((error) => {
    console.error('Error fetching data:', error);
})
});
</script>

<style scoped>
.center-container {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    height: 100vh;
    text-align: center;
}

.mini_rec {
    width: 50%;
    margin: 10px;
    padding: 10px;
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 10px;
}

.center {
    text-align: center;
}

.regularize-text {
    font-size: 24px;
    font-weight: bold;
}
</style>