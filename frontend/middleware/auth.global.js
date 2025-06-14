import { storeToRefs } from 'pinia';
import { useAuthStore } from "../store/auth";
import { useUserStore } from '~/store/user';

export default defineNuxtRouteMiddleware((to) => {
  const { authenticated } = storeToRefs(useAuthStore());
  const token = useCookie('token');
  const userStore = useUserStore();
  const user = userStore.user;

  if (token.value) {
    authenticated.value = true;
  }

  if (token.value && to?.name === 'login') {
    console.log(token.value)
    console.log("navitage to /")
    return navigateTo('/');
  }
  if (!token.value && to?.name !== 'login') {
    console.log(token.value)
    console.log("navitage to /login")
    abortNavigation();
    return navigateTo('/login');
  }

  if (token.value && user.colocationId === null && to?.name !== 'colocation-mandatory') {
    console.log(token.value)
    console.log(user)
    console.log("navitage to /colocation")
    abortNavigation();
    return navigateTo('/colocation-mandatory');
  }
});