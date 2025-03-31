import { defineStore } from 'pinia';
import type { User } from "~/composables/service/type";

export const useUserStore = defineStore('user', {
    state: () => ({
        user: {
            username: '',
            email: '',
            colocationId: '',
            id: '',
        },
    }),
    actions: {
        setUser(data: User) {
            this.user = data;
            console.log(this.user)
        },
        setColocation(data: string) {
            this.user.colocationId = data;
        }
    },
    persist: {
        storage: piniaPluginPersistedstate.localStorage(),
    },
});
