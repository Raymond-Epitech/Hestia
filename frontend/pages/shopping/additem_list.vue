<template>
    <div class="background">
        <div>
            <h1 class="header">
                <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
                <Texte_language source="header_add_shoppingitem" />
                <p></p>
            </h1>
        </div>
        <form method="post" action="">
            <div>
                <h3>
                    <Texte_language source="shoppingitem_name" />
                </h3>
                <input class="body-input" maxlength="42" v-model="item.name" required />
            </div>
            <div class="modal-buttons">
                <button class="button" @click.prevent="handleProceed">
                    <Texte_language source="poster" />
                </button>
            </div>
        </form>
    </div>
</template>

<script setup lang="ts">
import type { Expense, Coloc, shoppinglist_item } from '~/composables/service/type';
import { useUserStore } from '~/store/user';

useHead({
    bodyAttrs: {
        style: 'background-color: #1E1E1E;'
    }
})

const userStore = useUserStore();
const user = userStore.user;
const route = useRoute();
const router = useRouter();
const name = route.query.name as string;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const date = new Date();
const collocid = user.colocationId
const myid = user.id;

const item = ref<shoppinglist_item>({
    shoppinglistId: route.query.listId as string,
    createdBy: myid,
    name: '',
    isChecked: false,
})

const handleProceed = async () => {
    console.log(item.value);
    api.addShoppingListItem(item.value).then((response) => {
        if (!response) {
            console.error('Failed to add item');
            return;
        }
        console.log('item added successfully');
        router.back()
    }).catch((error) => {
        console.error('Error adding expense:', error);
    });
}

</script>

<style scoped>
.background {
    height: 100%;
    background-color: #1E1E1E;
    color: white;
    padding: 20px;
}

.header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
}

.body-input {
    width: 70%;
    background-color: #1e1e1e00;
    outline: none;
    border: none;
    line-height: 3ch;
    background-image: linear-gradient(transparent, transparent calc(3ch - 1px), #E7EFF8 0px);
    background-size: 100% 3ch;
    color: #fff;
    font-size: 18px;
}

.button {
    margin-top: 16px;
    padding: 10px 20px;
    border-radius: 15px;
    border: 0;
    cursor: pointer;
    background: #00000088;
    color: #fff;
}
</style>