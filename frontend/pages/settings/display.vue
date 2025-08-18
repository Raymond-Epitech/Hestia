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
        background-color: var(--primary-light);
        color: var(--font-color-dark);
    }

    .dark-mode body {
        background-color: var(--primary-dark);
        color: var(--font-color-light);
    }

    .select-theme {
        background-color: var(--primary-light);
        width: 60%;
        height: 30px;
        border-radius: 9px;
        border-color: var(--font-color-dark);
        color: var(--font-color-dark);
    }

    .dark .select-theme {
        background-color: var(--primary-dark);
        width: 60%;
        height: 30px;
        border-radius: 9px;
        border-color: var(--font-color-light);
        color: var(--font-color-light);
    }
</style>