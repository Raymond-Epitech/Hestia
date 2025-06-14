<template>
    <div class="expense" @click="props.onclick && props.onclick()">
        <div class="expense-header">
            <span v-if="!modif" class="expense-name">{{ item.name }} <span>
                    <img src="../public/edit.png" width="20" height="20" @click="modif = !modif" class="edit" />
                </span></span>
            <span v-if="modif" class="expense-name">
                <input v-model="item.name" maxlength="42" required />
                <button @click="modifname">
                    <Texte_language source="confirm" />
                </button></span>
            <div class="check-zone" :class="{ checked: item.isChecked }" @click.stop="toggleCheck"></div>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { shoppinglist_item } from '~/composables/service/type';

const modif = ref(false);
const props = defineProps<{
    item: shoppinglist_item;
    onclick?: () => void;
}>();
const emit = defineEmits<{
    (e: 'update:isChecked', value: boolean): void;
    (e: 'update:name', value: string): void;
}>();

const toggleCheck = () => {
    emit('update:isChecked', !props.item.isChecked);
};
const modifname = () => {
    modif.value = false;
    emit('update:name', props.item.name);
}

</script>

<style scoped>
.expense {
    border-bottom: 2px dotted #dddddd94;
    padding: 10px 0;
}

.expense-header {
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
    background-color: #85AD7B;
    border-color: #85AD7B;
}

.check-zone:not(.checked) {
    border-color: #8D90D6;
}

.expense-name {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-grow: 1;
    margin-right: 10px;
}

.edit {
    margin-bottom: 2px;
}
</style>