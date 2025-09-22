import { defineStore } from 'pinia'
import { Preferences } from '@capacitor/preferences';
import { Capacitor } from '@capacitor/core';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    authenticated: false,
    loading: false,
  }),
  actions: {
    async authenticateUser(credential) {

      if (credential) {
        const platform = Capacitor.getPlatform();
        if (platform !== 'web') {
          await Preferences.set({
            key: 'token',
            value: credential,
          });
        } else {
          const token = useCookie('token');
          token.value = credential;
        }
        this.authenticated = true;
      }
    },
    logUserOut() {
      const platform = Capacitor.getPlatform();
      if (platform !== 'web') {
        Preferences.remove({ key: 'token' });
      } else {
        const token = useCookie('token');
        this.authenticated = false;
        token.value = null;
      }
    },
  },
});