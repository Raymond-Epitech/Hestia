<template>
    <button class="back" @click="redirect('/settings')">
        <img src="~/public/Retour.svg" class="icon">
    </button>
    <div class="page-conteneur">
        <div class="colocation-container">
            <form class="create-colocation">
                <Texte_language source="Feedback-subtext"/>
                <select v-model="feedback.BugType" class="input-task-assignee" required>
                    <option :value=0> {{ $t("issue") }} </option>
                    <option :value=1> {{ $t("error") }} </option>
                    <option :value=2> {{ $t("suggestion") }} </option>
                    <option :value=3> {{ $t("other") }} </option>
                </select>
                <input type="text" class="input" v-model="feedback.Title" :placeholder="$t('title')"
                    maxlength="50" required />
                <textarea type="text" class="input" v-model="feedback.Description" :placeholder="$t('description')"
                    rows="4" maxlength="1500" required />
                <button class="button" @click.prevent="sendFeedback">
                    <Texte_language source="SendFeedback" />
                </button>
            </form>
        </div>
        <div v-if="errview">
            <Errorpopup :status="err.status" :body="err.body" @close="errview = false" />
        </div>
    </div>
</template>


<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { Feedback } from '~/composables/service/type';

const router = useRouter();
const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const err = ref<{ status: number; body: any }>({ status: 0, body: null });
const errview = ref(false);
const version = await api.getVersion();
const feedback : Feedback ={
    CreatedBy: user.id,
    VersionHash: version,
    Title: "",
    Description: "",
    BugType: 3,
}

const redirect = (page) => {
    router.push(page);
}

const sendFeedback = async () => {
    console.log(feedback)
     api.addFeedback(feedback).catch((error) => {
        console.error(error);
        err.value = error;
        errview.value = true;
    });
}
</script>

<style scoped>

.page-conteneur {
    width: 100%;
    height: calc(100vh - 4.5rem);
    display: flex;
    justify-content: center;
    overflow: scroll;
}

.back {
    background-color: var(--main-buttons);
    position: fixed;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 40px;
    height: 40px;
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
    top: 3%;
    left: 3%;
}

.back .icon {
    filter: var(--icon-filter);
    width: 25px;
}

.colocation-container {
    max-width: 90%;
    margin: 100px 0px 20px 0px;
    padding: 30px;
    display: flex;
    height: fit-content;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background-color: var(--login-box-bg);
    border-radius: 20px;
    box-shadow: var(--rectangle-shadow-light);
    gap: 40px;
    font-size: 20px;
    font-weight: 600;
    text-align: center;
    color: var(--page-text);
}

.create-colocation {
    display: grid;
    gap: 10px;
}

.input {
    outline: none;
    border-radius: 18px;
    border: none;
    font-size: 16px;
    text-align: start;
    padding: 8px 12px;
    background-color: var(--secondary-button);
    color: var(--secondary-page-text);
}

.input-task-assignee {
    outline: none;
    border-radius: 18px;
    border: none;
    font-size: 16px;
    padding: 8px 12px;
    background-color: #26272D;
    color: #FFFFFF;
}

.alert {
    padding: 0;
    margin: 0;
    margin-top: -6px;
    margin-bottom: 8px;
    font-size: 15px;
    color: var(--basic-red);
}

.button {
    padding: 10px 20px;
    font-weight: 600;
    border-radius: 16px;
    background-color: var(--secondary-button);
    color: var(--secondary-page-text);
}
</style>