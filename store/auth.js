import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    authenticated: false,
    loading: false,
  }),
  actions: {
    async authenticateUser(credential) {
      console.log("trying to authenticate User");
      console.log(credential)
      if (credential) {
        console.log("passed the credential check");
        const token = useCookie('token');
        token.value = credential;
        this.authenticated = true;
      }
    },
    logUserOut() {
      const token = useCookie('token');
      this.authenticated = false;
      token.value = null;
    },
  },
});