<template>
    <div class="navbar">
        <a class="settings-button" href="/settings">
            <img src="../public/settings.png" class="settings">
        </a>
        <a class="task-button" href="/tasks">
            <div class="task-button-top-bar"></div>
            <div class="task-button-middle-bar"></div>
            <div class="task-button-bottom-bar"></div>
        </a>
        <a class="wall-button" href="/">
            <img src="../public/frigo.png" class="fridge">
        </a>
        <a class="money-button" href="/money">
            <img src="../public/money-button.png" class="money">
        </a>

    </div>
</template>

<script setup>

</script>

<style>
.navbar {
    position: fixed;
    bottom: 0px;
    width: 100vw;
    height: 58px;
    background-color: #074338;
    display: flex;
    justify-content: space-around;
    align-items: center;
    padding: 0%;
}

.task-button {
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
    width: 49px;
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
    width: 49px;
    height: 35px;
    display: flex;
    justify-content: center;
}

.fridge {
    display: flex;
    width: 200px;
    height: 200px;
    margin-top: -30px;
}

.money-button {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 40px;
    width: 40px;
}

.money {
    height: 58px;
    width: 68px;
}
</style>