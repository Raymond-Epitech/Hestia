<template>
    <div class="conteneur">
        <Rectangle color="#85AD7B" :onClick="() => redirect('/settings/language')" id="language">   
            <h1> Language </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('notif')"  id="notif">
            <h1> Notifications </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('acces')" id="acces">
            <h1> Accessibilité</h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('regle')" id="regle">
            <h1> Regles de vie</h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('conf')" id="conf">
            <h1> Confidentialitée</h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('info')" id="info">
            <h1> Informations</h1>
        </Rectangle>
    </div>
</template>

<script setup>
const handleComposantClick = (id) => {
  console.log(`ID du composant cliqué: ${id}`);
  alert(`ID du composant cliqué: ${id}`);
}
const router = useRouter();
const redirect = (page) => {
  router.push(page);
}
</script>

<style scoped>
.conteneur {
    display: grid;
    gap: 40px;
    width: 80%;
    margin: 40px auto;
}
</style>