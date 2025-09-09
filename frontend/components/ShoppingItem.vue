<template>
    <div class="shopping" @click="props.onclick && props.onclick()">
        <div class="shopping-header">
            <span v-if="!modif" class="shopping-name">{{ item.name }} <span>
                    <img src="../public/edit.svg" width="20" height="20" @click="modif = !modif" class="icon" />
                </span></span>
            <span v-if="modif" class="shopping-name">
                <input class="modify-input" v-model="item.name" maxlength="42" required />
                <button class="ok-button" @click="modifname">
                    <img src="../public/Submit.svg" width="20" height="20" class="icon" />
                </button>
                <button class="delete-button" @click="showPopup">
                    <img src="../public/Trash.svg" width="20" height="20" class="icon" />
                </button>
            </span>
            <div v-if="!modif" class="check-zone" :class="{ checked: item.isChecked }" @click.stop="toggleCheck"></div>
        </div>
    </div>
    <popup v-if="popup_vue" :text="$t('confirm_delete_shoppingitem')" @confirm="confirmDelete" @close="cancelDelete" />
</template>

<script setup lang="ts">
import type { ReminderItem, } from '~/composables/service/type';

const popup_vue = ref(false)
const modif = ref(false);
const props = defineProps<{
    item: ReminderItem;
    onclick?: () => void;
}>();
const emit = defineEmits<{
    (e: 'update:isChecked', value: boolean): void;
    (e: 'update:name', value: string): void;
    (e: 'delete'): void;
}>();


const showPopup = () => {
    popup_vue.value = true;
};

const confirmDelete = async () => {
    popup_vue.value = false;
    emit('delete');
};

const cancelDelete = () => {
    popup_vue.value = false;
};
const toggleCheck = () => {
    emit('update:isChecked', !props.item.isChecked);
};
const modifname = () => {
    modif.value = false;
    emit('update:name', props.item.name);
}

</script>

<style scoped>
.shopping {
    border-bottom: 2px dotted var(--list-lines);
    color: var(--page-text);
    word-wrap: break-word;
    word-break: break-word;
    padding: 10px 0;
}

.shopping-header {
    display: grid;
    grid-template-columns: 10fr 1fr;
    font-weight: bold;
    align-items: center;
}

.check-zone {
    width: 20px;
    height: 20px;
    border: 2px solid #ddd;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;
}

.check-zone.checked {
    background-color: var(--basic-green);
    border-color: var(--recieved-message);
}

.check-zone:not(.checked) {
    border-color: var(--basic-purple);
}

.shopping-name {
    display: grid;
    grid-template-columns: 8fr 1fr 1fr;
    align-items: center;
    margin-right: 10px;
    padding-left: 2px;
}

.icon {
    margin-bottom: 2px;
    filter: var(--icon-filter);
}

.modify-input {
    border: none;
    border-radius: 5px;
    background-color: var(--sent-message);
    box-shadow: var(--small-button-shadow);
    outline: none;
}

.ok-button {
    width: fit-content;
    height: fit-content;
    padding: 2px 4px;
    margin-left: 30%;
    border-radius: 4px;
    color: var(--page-text);
    background-color: var(--sent-message);
    box-shadow: var(--small-button-shadow);
    font-weight: 600;
}

.delete-button {
    width: fit-content;
    height: fit-content;
    padding: 2px 4px;
    margin-left: 30%;
    border-radius: 4px;
    background-color: var(--main-buttons);
    box-shadow: var(--small-button-shadow);
    font-weight: 600;
}
</style>