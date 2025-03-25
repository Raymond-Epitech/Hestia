<template>
    <div class="background">
        <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()"/>
        <div>
            l'historique de {{ name }} arrive bientôt
        </div>
    </div>
</template>

<script setup>
const route = useRoute();
const name = route.query.name; 
</script>

<style scoped>
.background {
    height: 100vh;
    background-color: #1E1E1E;
    color: white;
    padding: 20px;
}
</style>
