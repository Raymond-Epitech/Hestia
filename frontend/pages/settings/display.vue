<template>
    <button class="back" @click="redirect('/settings')">
        <img src="~/public/Retour.svg" class="icon">
    </button>
    <div class="container">
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
  const router = useRouter();
  const redirect = (page) => {
    router.push(page);
  }

</script>

<style scoped>
.container {
    background-color: var(--background-light);
    color: var(--page-text-light);
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
    margin: 40px auto;
    margin-top: 5rem;
    max-height: calc(100vh - 5.5rem);
}

.dark .container {
    background-color: var(--background-dark);
    color: var(--page-text-dark);
}

.select-theme {
    background-color: var(--main-buttons-light);
    width: 13rem;
    height: 2.2rem;
    border-radius: 9px;
    border-style: hidden;
    padding-left: 0.5rem;
    color: var(--page-text-light);
    box-shadow: var(--rectangle-shadow-light);
}

.dark .select-theme {
    background-color: var(--main-buttons-dark);
    color: var(--page-text-dark);
}

.back {
    background-color: var(--main-buttons-light);
    position: fixed;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 40px;
    height: 40px;
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
    top: 3%;
    left: 3%;
}

.back .icon {
    filter: invert(1);
    width: 25px;
}

.dark .back {
    background-color: var(--main-buttons-dark);
}

.dark .back .icon {
    filter: none;
}
</style>