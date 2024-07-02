<template>
    <div type="background">
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

</style>