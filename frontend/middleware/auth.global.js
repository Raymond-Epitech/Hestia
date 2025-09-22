import { storeToRefs } from 'pinia';
import { useAuthStore } from "../store/auth";
import { useUserStore } from '~/store/user';
import { Capacitor } from '@capacitor/core';
import { Preferences } from '@capacitor/preferences';

export default defineNuxtRouteMiddleware((to) => {
  const { authenticated } = storeToRefs(useAuthStore());
  const token = useCookie('token');
  const userStore = useUserStore();
  const user = userStore.user;

  if (Capacitor.getPlatform() !== 'web') {
    Preferences.get({ key: 'token' }).then((result) => {
      if (result.value) {
        token.value = result.value;
      }
    });
  }
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

  if (token.value && user.colocationId === null && !to?.name?.startsWith('colocation-mandatory') && to?.name !== 'invite') {
    abortNavigation();
    return navigateTo({ path: '/colocation-mandatory', query: to.query });
  }
});