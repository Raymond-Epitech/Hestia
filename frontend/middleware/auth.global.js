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
    return navigateTo('/');
  }
  if (!token.value && to?.name !== 'login' && to?.name !== 'invite') {
    abortNavigation();
    return navigateTo('/login');
  }

  if (token.value && user.colocationId === null && to?.name !== 'colocation-mandatory') {
    abortNavigation();
    return navigateTo('/colocation-mandatory');
  }
});