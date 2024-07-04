<template>
    <div class="base">
        <img src="../public/logo-hestia.png" class="logo"/>
        <GoogleSignInButton @success="handleLoginSuccess" @error="handleLoginError"></GoogleSignInButton>
    </div>
</template>

<script setup>
import { storeToRefs } from 'pinia';
import { useAuthStore } from '~/store/auth';

const { authenticateUser } = useAuthStore();
const { authenticated } = storeToRefs(useAuthStore());

const router = useRouter();
const handleLoginSuccess = async (response) => {
  const { credential } = response;
  console.log("Access Token", credential);
  await authenticateUser(credential);
    if (authenticated) {
        router.push('/');
    }
};

const handleLoginError = () => {
  console.error("Login failed");
};
</script>

<style>
body {
    background-color: #E7FEED;
}

.base {
    display: flex;
    justify-content: space-evenly;
    flex-direction: column;
    align-items: center;
    height: 100vh;
}

.logo {
    width: 280px;
    border-radius: 15px;
}

</style>