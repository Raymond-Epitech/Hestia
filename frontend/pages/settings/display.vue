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
        background-color: var(--background-light);
        color: var(--page-text-light);
    }

    .dark-mode body {
        background-color: var(--background-dark);
        color: var(--page-text-dark);
    }

    .select-theme {
        background-color: var(--background-light);
        width: 60%;
        height: 30px;
        border-radius: 9px;
        border-color: var(--page-text-light);
        color: var(--page-text-light);
    }

    .dark .select-theme {
        background-color: var(--background-dark);
        width: 60%;
        height: 30px;
        border-radius: 9px;
        border-color: var(--page-text-dark);
        color: var(--page-text-dark);
    }
</style>