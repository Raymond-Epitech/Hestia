<template>
    <div class="background">
        <div>
            <h1 class="header">
                <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
                <Texte_language source="header_add_shopping" />
                <p></p>
            </h1>
        </div>
        <form method="post" action="">
            <div>
                <h3>
                    <Texte_language source="shopping_list_name" />
                </h3>
                <input class="body-input" maxlength="35" v-model="newshopping.name" required />
            </div>
            <div class="modal-buttons">
                <button class="button button-proceed" @click.prevent="handleProceed">
                    <Texte_language source="poster" />
                </button>
            </div>
        </form>
    </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { shoppinglist } from '~/composables/service/type';

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const newshopping = ref<shoppinglist>({
    colocationId: user.colocationId,
    createdBy: user.id,
    name: '',
});
const handleProceed = async () => {
    console.log(newshopping.value);
    api.addShoppingList(newshopping.value).then(() => {
        console.log('Expense category added successfully');
        router.back()
    }).catch((error) => {
        console.error('Error adding expense category:', error);
    });
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

.button {
    padding: 10px 20px;
    border-radius: 15px;
    border: 0;
    cursor: pointer;
}

.button-proceed {
    background: #00000088;
    color: #fff;
}
</style>