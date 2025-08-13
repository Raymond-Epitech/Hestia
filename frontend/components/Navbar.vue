<template>
    <div class="navbar">
        <a class="button" :onClick="() => redirect('/settings')"> <!--change to profile page-->
            <img src="../public/navbar/Profile.svg" class="icon">
            <br>
            <p class="icon-subtext">Profile</p>
        </a>
        <a class="button" :onClick="() => redirect('/tasks')">
            <img src="../public/navbar/Tasks.svg" class="icon">
            <br>
            <p class="icon-subtext">Tasks</p>
        </a>
        <a class="wall-button" :onClick="() => redirect('/')">
            <img src="../public/navbar/Fridge.png" class="fridge">
        </a>
        <a class="button" :onClick="() => redirect('/shopping')">
            <img src="../public/navbar/Message.svg" class="icon">
            <br>
            <p class="icon-subtext">Messages</p>
        </a>
        <a class="button" :onClick="() => redirect('/money')">
            <img src="../public/navbar/Money.svg" class="icon">
            <br>
            <p class="icon-subtext">Budget</p>
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
    width: 100%;
    height: 8%;
    background-color: #FFFFFF;
    display: grid;
    grid-template-columns: repeat(5, 1fr);
    justify-content: center;
    align-items: center;
    padding: 0%;
}

.dark .navbar{
    background-color: #000000;
}

.icon-subtext {
    font-size: 65%;
    margin-top: 1%;
    font-weight: bold;
    list-style: none;
    text-align: center;
    display: block;
    color: #4B4B4B;
}

.dark .icon-subtext {
    color: #D6D6D6;
}

.button {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    height: 100%;
    width: 100%;
    padding-top: 7%;
    margin-top: 5%;
    text-decoration: none;
}

.icon {
    height: 60%;
    filter: brightness(29%);
}

.dark .icon {
    color: #FFFFFF;
    filter: none;
}

.wall-button {
    max-width: 100%;
    height: 35px;
    display: flex;
    justify-content: center;
}

.fridge {
    display: flex;
    max-width: 175%;
    height: 400%;
    margin-top: -40%;
}

</style>