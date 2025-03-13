<template>
    <Rectangle color="#4FA3A6" id="rec_gdepense"><Texte_language source="global"/> {{ global }} €</Rectangle>
    <Rectangle color="#FFF973" id="rec_regularize"><Texte_language source="regularize"/></Rectangle>
</template>

<script setup>
const global = ref(0);
</script>