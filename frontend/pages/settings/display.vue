<template>
    <div>
        <h1>Color mode: {{ $colorMode.value }}</h1>
        <select v-model="$colorMode.preference" class="select-theme">
            <option value="system">
                <Texte_language source="system" />
            </option>
            <option value="light">
                <Texte_language source="light" />
            </option>
            <option value="dark">
                <Texte_language source="dark" />
            </option>
            <!-- <option value="sepia">Sepia</option> -->
        </select>
    </div>
</template>

<script setup>
    const colorMode = useColorMode()

</script>

<style scoped>
    body {
        background-color: #fff;
        color: rgba(0, 0, 0, 0.8);
    }

    .dark-mode body {
        background-color: #091a28;
        color: #ebf4f1;
    }

    .select-theme {
        background-color: #fff;
        width: 60%;
        height: 30px;
        border-radius: 9px;
        border-color: #091a28;
        color: #091a28;
    }

    .dark .select-theme {
        background-color: #1D1B20;
        width: 60%;
        height: 30px;
        border-radius: 9px;
        border-color: #fff;
        color: #fff;
    }
</style>