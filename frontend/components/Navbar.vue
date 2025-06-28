<template>
    <div class="navbar">
        <a class="money-button" :onClick="() => redirect('/settings')">
            <img src="../public/settings.png" class="settings">
        </a>
        <a class="task-button" :onClick="() => redirect('/tasks')">
            <div class="task-button-top-bar"></div>
            <div class="task-button-middle-bar"></div>
            <div class="task-button-bottom-bar"></div>
        </a>
        <a class="wall-button" :onClick="() => redirect('/')">
            <img src="../public/frigo.png" class="fridge">
        </a>
        <a class="money-button" :onClick="() => redirect('/money')">
            <img src="../public/money-button.png" class="money">
        </a>
        <a class="shopping-button" :onClick="() => redirect('/shopping')">
            <img src="../public/shopping.png" class="shopping">
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
    bottom: 0px;
    width: 100vw;
    height: 58px;
    background-color: #074338;
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
    justify-content: center;
    align-items: center;
    padding: 0%;
}

.task-button {
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
    width: 100%;
    height: 35px;
}

.task-button-top-bar {
    width: 39px;
    height: 7px;
    background-color: #85AD7B;
    border-radius: 8px;
}

.task-button-middle-bar {
    width: 39px;
    height: 7px;
    background-color: #FFC93D;
    border-radius: 8px;
}

.task-button-bottom-bar {
    width: 39px;
    height: 7px;
    background-color: #FF6A61;
    border-radius: 8px;
}

.wall-button {
    max-width: 100%;
    height: 35px;
    display: flex;
    justify-content: center;
}

.fridge {
    display: flex;
    max-width: 125px;
    height: 200px;
    margin-top: -30px;
}

.money-button {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 58px;
    width: 100%;
}

.money {
    height: 58px;
    width: 58px;
}

.shopping-button {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 30px;
    width: 100%;
}

.shopping {
    height: 38px;
    width: 38px;
}

.shopping-button {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 30px;
    width: 30px;
}

.shopping {
    height: 38px;
    width: 48px;
}
</style>