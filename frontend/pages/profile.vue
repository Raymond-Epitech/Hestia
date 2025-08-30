<template>
    <div>
        <button class="settings" @click="redirect('/settings')">
            <img src="~/public/profile/settings.svg" class="icon">
        </button>
        <button class="add-user" @click="redirect('/add-user')">
            <img src="~/public/profile/addUser.svg" class="icon">
        </button>
    </div>
</template>

<script setup>
const router = useRouter();

const redirect = (page) => {
    router.push(page);
}
</script>

<style scoped>

button {
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
}

.dark button {
    background-color: var(--main-buttons-dark);
}

.icon {
    filter: brightness(0%);
}

.dark .icon {
    filter: none;
}

.add-user {
    top: 6%;
    right: 3%;
}

.settings {
    top: 6%;
    left: 3%;
}


</style>
