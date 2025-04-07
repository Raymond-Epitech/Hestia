<template>
    <div>
        <div class="polygone"></div>
        <div class="conteneur">
            <h1>
                <Texte_language source="ColocationMenu" />
            </h1>
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

.polygone {
    position: absolute;
    margin-top: -150px;
    height: 300px;
    width: 100vw;
    clip-path: ellipse(87% 35% at 33% 59%);
    background-color: #BAE5C6;
    z-index: 0;
}

h1 {
    margin-top: -20px;
    align-items: center;
    text-align: center;
    font-size: 32px;
    font-weight: 600;
    z-index: 1;
}
</style>