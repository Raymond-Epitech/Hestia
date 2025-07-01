<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
            <h1 class="header-name">{{ name }}</h1>
            <div class="square">
                <div id="add" :onClick="() => redirectto()">
                    <img src="/plus.png" alt="Return" width="30" height="30" />
                </div>
            </div>
        </div>
        <div>
            <ShoppingItem v-for="item in shopping_list.shoppingItems" :key="item.id ?? item.name" :item="item"
                @update:isChecked="updateIsChecked(item.id ?? '', $event)"
                @update:name="updateName(item.id ?? '', $event)" />
        </div>
    </div>
</template>
<script setup lang="ts">
import type { shoppinglist, shoppinglist_item } from '~/composables/service/type';
import { useI18n } from 'vue-i18n';
import { useUserStore } from '~/store/user';
import { ShoppingItem } from '#components';

useHead({
    bodyAttrs: {
        style: 'background-color: #1E1E1E;'
    }
})

const route = useRoute();
const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const { t } = useI18n();
const router = useRouter();
const shopping_list = ref<shoppinglist>({} as shoppinglist);
const collocid = user.colocationId
const name = route.query.name;
const listId = route.query.id as string;

api.getShoppingListById(listId).then((response) => {
    shopping_list.value = response;
}).catch((error) => {
    console.error('Error fetching shopping list items:', error);
});

const redirectto = () => {
    router.push({ path: '/shopping/additem_list', query: { name, listId } });
}
const updateIsChecked = (id: string, value: boolean) => {
    const item = shopping_list.value.shoppingItems?.find((item) => item.id === id);
    if (item) {
        item.isChecked = value;
        item.shoppinglistId = listId;
        console.log(item);
        api.updateShoppingListItem(item).then((response) => {
            if (!response) {
                console.error(`Failed to update item ${id}`);
                item.isChecked = !value;
                return;
            }
            console.log(`Item ${id} updated successfully`);
        }).catch((error) => {
            console.error(`Error updating item ${id}:`, error);
        });
    }
};
const updateName = (id: string, value: string) => {
    const item = shopping_list.value.shoppingItems?.find((item) => item.id === id);
    if (item) {
        item.name = value;
        item.shoppinglistId = listId;
        console.log(item);
        api.updateShoppingListItem(item).then((response) => {
            if (!response) {
                console.error(`Failed to update item ${id}`);
                window.location.reload();
                return;
            }
            console.log(`Item ${id} updated successfully`);
        }).catch((error) => {
            console.error(`Error updating item ${id}:`, error);
        });
    }
};
</script>

<style scoped>
.background {
    height: 100%;
    background-color: #1E1E1E;
    color: white;
    padding: 20px;
}

.square {
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: #8D90D6;
    width: 40px;
    height: 40px;
    border-radius: 10px;
    margin-left: 10px;
}

.header {
    width: 100%;
    display: grid;
    grid-template-columns: 1fr 6fr 1fr;
    align-items: center;
    margin-bottom: 20px;
    text-align: center;
}

.header-name {
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
}
</style>
