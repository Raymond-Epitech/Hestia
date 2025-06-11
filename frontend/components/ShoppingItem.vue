<template>
    <div class="expense" @click="props.onclick && props.onclick()">
        <div class="expense-header">
            <span v-if="!modif" class="expense-name">{{ item.name }} <span><img src="../public/edit.png" width="20"
                        height="20" @click="modif = !modif" /> </span></span>
            <span v-if="modif" class="expense-name"><input v-model="item.name" required /><button @click="modifname">
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
    border-bottom: 2px solid #ddd;
    padding: 10px 0;
}

.expense-header {
    display: flex;
    justify-content: space-between;
    font-weight: bold;
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
    background-color: #8D90D6;
    border-color: #8D90D6;
}

.expense-name {
    flex-grow: 1;
    margin-right: 10px;
}
</style>