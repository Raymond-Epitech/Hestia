<template>
    <ExpensesListModal v-model="isExpenseModalOpen" :expense="expense.id" @proceed="handleProceed('proceed')" />
    <div class="expense" data-toggle="modal" data-target=".bd-example-modal-sm" @click="openExpenseModal">
        <text class="category">{{ expense.name }}</text>
        <text class="regularize-text number">
            {{ expense.totalAmount }} €
            <button src="/Trash.svg" alt="Delete Icon" class="svg-icon" @click="showPopup">
                <img src="/Trash.svg" alt="Delete Icon" class="svg-icon" />
            </button>
        </text>
    </div>
    <popup v-if="popup_vue" :text="$t('confirm_delete_category')" @confirm="handleProceed('delete')"
        @close="cancelDelete">
    </popup>
    <div v-if="errview">
        <Errorpopup :status="err.status" :body="err.body" @close="errview = false" />
    </div>
</template>

<script setup lang="ts">
const props = defineProps({
    expense: {
        type: Object,
        required: true,
    },
})
const isExpenseModalOpen = ref(false)
const openExpenseModal = () => (isExpenseModalOpen.value = true)
const err = ref<{ status: number, body: any }>({ status: 0, body: null });
const errview = ref(false);
const popup_vue = ref(false);
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const emit = defineEmits(["proceed", "delete"]);

const deleteExpense = async () => {
    console.log('Deleting expense category with id:', props.expense.id);
    api.deleteexpensecategory(props.expense.id).then(() => {
        emit('delete');
    }).catch((error) => {
        console.error(error);
        err.value = error;
        errview.value = true;
    });
};

const showPopup = (event: Event) => {
    event.stopPropagation();
    popup_vue.value = true;
};

const cancelDelete = () => {
    popup_vue.value = false;
}


const handleProceed = async (action: "proceed" | "delete") => {
    if (action === "delete") {
        await deleteExpense();
        popup_vue.value = false;
        return;
    }
    emit(action);
}
</script>

<style scoped>
.expense {
    width: 90%;
    height: 88px;
    margin: 10px;
    padding: 10px;
    display: grid;
    align-items: center;
    grid-template-columns: 1fr 1fr;
    justify-content: space-between;
    background-color: var(--recieved-message);
    border-radius: 20px;
    box-shadow: var(--rectangle-shadow-light);
    color: var(--page-text);
}

.regularize-text {
    font-size: 24px;
    font-weight: 600;
}

.category {
    font-size: 20px;
    margin-left: 5px;
    font-weight: 600;
    text-align: left;
}

.number {
    font-size: 28px;
    margin-right: 5px;
    text-align: right;
}

.svg-icon {
  background-color: var(--recieved-message);
}
</style>