<template>
    <div class="navbar">
        <a class="button" @click="select('profile'); redirect('/profile')">
            <img src="../public/navbar/Profile.svg" class="icon" :class="{ selected: selected === 'profile' }">
            <br>
            <p class="icon-subtext">{{ $t('nav_profile') }}</p>
        </a>
        <a class="button" @click="select('tasks'); redirect('/tasks')">
            <img src="../public/navbar/Tasks.svg" class="icon" :class="{ selected: selected === 'tasks' }">
            <br>
            <p class="icon-subtext">{{ $t('nav_tasks') }}</p>
        </a>
        <a class="wall-button" @click="select('fridge'); redirect('/')">
            <img src="../public/navbar/Fridge.png" class="fridge" :class="{ selected: selected === 'fridge' }">
        </a>
        <a class="button" @click="select('message'); redirect('/message')">
            <img src="../public/navbar/Message.svg" class="icon" :class="{ selected: selected === 'message' }">
            <br>
            <p class="icon-subtext">{{ $t('nav_messages') }}</p>
        </a>
        <a class="button" @click="select('money'); redirect('/money')">
            <img src="../public/navbar/Money.svg" class="icon" :class="{ selected: selected === 'money' }">
            <br>
            <p class="icon-subtext">{{ $t('nav_expenses') }}</p>
        </a>
    </div>
</template>

<script setup>
import { ref } from 'vue'
const router = useRouter();
const selected = ref(null);

const select = (name) => {
    selected.value = name;
};

const redirect = (page) => {
    router.push(page);
}
</script>

<style scoped>
.navbar {
    position: fixed;
    bottom: 0;
    width: 100%;
    height: 4.5rem;
    background-color: var(--main-buttons);
    display: grid;
    grid-template-columns: repeat(5, 1fr);
    justify-content: center;
    align-items: center;
    padding: 0;
    line-height: 0rem;
    z-index: 100;
}


.icon-subtext {
    font-size: 0.65rem;
    margin-top: 0.8rem;
    font-weight: bold;
    list-style: none;
    text-align: center;
    display: block;
    color: var(--basic-grey);
}

.dark .icon-subtext {
    color: var(--light-grey);
}

.hestia .icon-subtext {
    color: var(--light-grey);
}

.button {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    max-height: 3rem;
    width: 100%;
    padding-top: 0.3rem;
    margin-top: 0.2rem;
    text-decoration: none;
}

.icon {
    height: 3rem;
    filter: brightness(29%);
    transition: transform 0.2s cubic-bezier(.4,2,.3,1);
}

.icon.selected {
    transform: translateY(-0.3rem) scale(1.08);
}

.dark .icon {
    color: var(--page-text);
    filter: none;
}

.hestia .icon {
    color: var(--page-text);
    filter: none;
}

.wall-button {
    height: 3.5rem;
    display: flex;
    justify-content: center;
}

.fridge {
    display: block;
    max-width: 5.8rem;
    height: 10rem;
    margin-top: -1.5rem;
    transition: transform 0.2s cubic-bezier(.4,2,.3,1);
}

.fridge.selected {
    transform: translateY(-12px) scale(1.08);
}
</style>
