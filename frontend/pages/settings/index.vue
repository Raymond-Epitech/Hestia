<template>
    <div class="conteneur">
        <Rectangle color="#85AD7B" :onClick="() => redirect('/settings/language')" id="language">
            <h1>
                <Texte_language source="Language" />
            </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => redirect('/settings/colocation')" id="colocation">
            <h1>
                <Texte_language source="Colocation" />
            </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('notif')" id="notif">
            <h1>
                <Texte_language source="Notifications" />
            </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('acces')" id="acces">
            <h1>
                <Texte_language source="Accessibility" />
            </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('regle')" id="regle">
            <h1>
                <Texte_language source="Rules" />
            </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('conf')" id="conf">
            <h1>
                <Texte_language source="Confidentiality" />
            </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => handleComposantClick('info')" id="info">
            <h1>
                <Texte_language source="Informations" />
            </h1>
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
    align-items: center;
    gap: 40px;
    width: 80%;
    margin: 40px auto;
}

h1 {
    align-items: center;
    font-weight: 600;
    margin: 0;
}
</style>