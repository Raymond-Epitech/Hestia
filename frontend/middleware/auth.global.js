import { storeToRefs } from 'pinia';
import { useAuthStore } from "../store/auth";

export default defineNuxtRouteMiddleware((to) => {
  const { authenticated } = storeToRefs(useAuthStore());
  const token = useCookie('token');

  if (token.value) {
    authenticated.value = true;
  }

  if (token.value && to?.name === 'login') {
    return navigateTo('/');
  }
  if (!token.value && to.path !== '/login' && to.path !== '/auth/google-callback') {
    abortNavigation();
    return navigateTo('/login');
  }
});