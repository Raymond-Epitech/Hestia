<template>
    <div class="center-container">
        <Rectangle v-for="shopping in shopping_list" :key="shopping.id" color="#85AD7B" id="rec" class="center mini_rec"
            :onClick="() => redirectto(shopping.name, shopping.id)">
            <p>{{ shopping.name }}</p>
        </Rectangle>
        <Rectangle color="#FFF973" id="rec" class="center regularize-text mini_rec"
            :onClick="() => redirectto('add_shopping')">
            <Texte_language source="add_shopping" />
        </Rectangle>
    </div>
</template>

<script setup lang="ts">
import type { shoppinglist } from '~/composables/service/type';
import { useI18n } from 'vue-i18n';
import { useUserStore } from '~/store/user';

const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const { t } = useI18n();
const router = useRouter();
const shopping_list = ref<shoppinglist[]>([]);
const collocid = user.colocationId

const redirectto = (name: string, id?: string) => {
    if (name === 'add_shopping') {
        router.push({ path: '/shopping/addShopping' });
        return;
    }
    router.push({ path: '/shopping/item_list', query: { name, id } });
}

api.getShoppingListByColocationId(collocid).then((response) => {
    shopping_list.value = response;
}).catch((error) => {
    console.error('Error fetching data:', error);
})
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