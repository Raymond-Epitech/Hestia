<template>
    <button class="back" @click="redirect('/settings')">
        <img src="~/public/Retour.svg" class="icon">
    </button>
    <div class="container">
        <h1>{{ $t('theme') }} : {{ $t($colorMode.preference) }}</h1>
        <div class="themes-container">
            <div class="theme-box" :class="{ selected: $colorMode.preference === 'system' }"
                @click="$colorMode.preference = 'system'">
                <img src="~/public/settings/system.svg" class="icon"></img>
            </div>
            <div class="theme-box" :class="{ selected: $colorMode.preference === 'light' }"
                @click="$colorMode.preference = 'light'">
                <img src="~/public/settings/sun.svg" class="icon"></img>
            </div>
            <div class="theme-box" :class="{ selected: $colorMode.preference === 'dark' }"
                @click="$colorMode.preference = 'dark'">
                <img src="~/public/settings/moon.svg" class="icon"></img>
            </div>
            <div class="theme-box" :class="{ selected: $colorMode.preference === 'vegane' }"
                @click="$colorMode.preference = 'vegane'">
                <img src="~/public/settings/fridge.svg" class="icon"></img>
            </div>
        </div>
    </div>
    <div>
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
.themes-container {
    display: grid;
    grid-template-columns: repeat(4, 50px);
    margin-top: 20px;
    grid-gap: 22px;
}

.vegane .container {
    color: var(--main-buttons);
}

.theme-box {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 62px;
    height: 62px;
    border-radius: 10px;
    background-color: var(--main-buttons);
    box-shadow: var(--button-shadow-light);
}

.icon {
    width: 48px;
    height: 48px;
    filter: var(--icon-filter);
}

.selected {
    transform: scale(1.18);
}

.container {
    background-color: var(--background);
    color: var(--page-text);
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
    margin: 40px auto;
    margin-top: 5rem;
    max-height: calc(100vh - 5.5rem);
}

.select-theme {
    background-color: var(--main-buttons);
    width: 13rem;
    height: 2.2rem;
    border-radius: 9px;
    border-style: hidden;
    padding-left: 0.5rem;
    color: var(--page-text);
    box-shadow: var(--rectangle-shadow-light);
}

.back {
    background-color: var(--main-buttons);
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
    width: 25px;
}
</style>