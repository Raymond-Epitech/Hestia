<template>
    <div class="background">
        <div class="header">
            <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()"/>
            <div class="square">
                <Rectangle color="#FFF973" id="add" >
                    <img src="/plus.png" alt="Return" width="30" height="30" />
                </Rectangle>
            </div>
        </div>
        <div>
            <ExpenseItem v-for="expense in expenses_list" :key="expense.id" :expense="expense" />
        </div>
    </div>
</template>

<script setup>
const route = useRoute();
const name = route.query.name; 
const expenses_list = ref([]);
//en attendant de pouvoir récupérer les données
expenses_list.value = [
    {"name": "food", "amount": 100, "paidBy": "Tibo"},
    {"name": "sucrerie", "amount": 200, "paidBy": "sugarDaddy"},
    {"name": "miamiam", "amount": 300, "paidBy": "Benji"},
    {"name": "viande", "amount": 600, "paidBy": "Adé"},
    {"name": "bon shit sa mère", "amount": 420.50, "paidBy": "Raimon"}
];
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
</style>
