<template>
    <div class="navbar">
        <a class="button" :onClick="() => redirect('/settings')"> <!--change to profile page-->
            <img src="../public/navbar/Profile.svg" class="icon">
            <br>
            <p class="icon-subtext">{{ $t('nav_profile') }}</p>
        </a>
        <a class="button" :onClick="() => redirect('/tasks')">
            <img src="../public/navbar/Tasks.svg" class="icon">
            <br>
            <p class="icon-subtext">{{ $t('nav_tasks') }}</p>
        </a>
        <a class="wall-button" :onClick="() => redirect('/')">
            <img src="../public/navbar/Fridge.png" class="fridge">
        </a>
        <a class="button" :onClick="() => redirect('/shopping')">
            <img src="../public/navbar/Message.svg" class="icon">
            <br>
            <p class="icon-subtext">{{ $t('nav_messages') }}</p>
        </a>
        <a class="button" :onClick="() => redirect('/money')">
            <img src="../public/navbar/Money.svg" class="icon">
            <br>
            <p class="icon-subtext">{{ $t('nav_expenses') }}</p>
        </a>
    </div>
</template>

<script setup>
const router = useRouter();
const redirect = (page) => {
    router.push(page);
}
</script>

<style scoped>
.navbar {
    position: fixed;
    bottom: 0;
    width: 100%;
    height: 4.5rem; /* ~8% of a typical 56rem tall viewport */
    background-color: var(--main-buttons-light);
    display: grid;
    grid-template-columns: repeat(5, 1fr);
    justify-content: center;
    align-items: center;
    padding: 0;
}

.dark .navbar {
    background-color: var(--main-buttons-dark);
}

.icon-subtext {
    font-size: 0.7rem; /* ~65% of base font size */
    margin-top: 0.2rem;
    font-weight: bold;
    list-style: none;
    text-align: center;
    display: block;
    color: var(--basic-grey);
}

.dark .icon-subtext {
    color: var(--light-grey);
}

.button {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    height: 100%;
    width: 100%;
    padding-top: 0.3rem; /* ~7% of 4.5rem */
    margin-top: 0.2rem;  /* ~5% of 4.5rem */
    text-decoration: none;
}

.icon {
    height: 2rem; /* ~60% of 4.5rem */
    filter: brightness(29%);
}

.dark .icon {
    color: var(--page-text-dark);
    filter: none;
}

.wall-button {
    max-width: 100%;
    height: 3.5rem; /* ~78% of 4.5rem */
    display: flex;
    justify-content: center;
}

.fridge {
    display: block;
    max-width: 100%;
    height: 10rem;
    margin-top: -7rem;
}
</style>