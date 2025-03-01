<template>
    <div class="conteneur">
        <Rectangle color="#85AD7B" :onClick="() => redirect('/settings/createColocation')" id="createColocation">
            <h1>
                <Texte_language source="CreateColocation" />
            </h1>
        </Rectangle>
        <Rectangle color="#85AD7B" :onClick="() => redirect('/settings/joinColocation')" id="joinColocation">
            <h1>
                <Texte_language source="JoinColocation" />
            </h1>
        </Rectangle>
    </div>
</template>

<script setup>
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