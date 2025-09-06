<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
            <div class="header-name">{{ name }}</div>
            <div class="square" :onClick="() => redirectto()">
                <img src="/plus.png" alt="Return" width="20" height="20" />
            </div>
        </div>
        <div>
            <ShoppingItem v-for="item in shopping_list.shoppingItems" :key="item.id ?? item.name" :item="item"
                @update:isChecked="updateIsChecked(item.id ?? '', $event)"
                @update:name="updateName(item.id ?? '', $event)"
                @delete="deleteItem(item.id ?? '')" />
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
        api.updateShoppingListItem(item).then((response) => {
            if (!response) {
                console.error(`Failed to update item ${id}`);
                item.isChecked = !value;
                return;
            }
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
        api.updateShoppingListItem(item).then((response) => {
            if (!response) {
                console.error(`Failed to update item ${id}`);
                window.location.reload();
                return;
            }
        }).catch((error) => {
            console.error(`Error updating item ${id}:`, error);
        });
    }
};

const deleteItem = (id: string) => {
    api.deleteShoppingListItem(id).then((response) => {
        if (!response) {
            console.error(`Failed to delete item ${id}`);
            return;
        }
        shopping_list.value.shoppingItems = shopping_list.value.shoppingItems?.filter((item) => item.id !== id);
    }).catch((error) => {
        console.error(`Error deleting item ${id}:`, error);
    });
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
    width: 30px;
    height: 30px;
    border-radius: 9px;
    margin-left: 10px;
    box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.header {
    width: 100%;
    display: grid;
    grid-template-columns: 1fr 6fr 1fr;
    align-items: center;
    text-align: center;
    font-size: 30px;
}

.header-name {
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
}
</style>
