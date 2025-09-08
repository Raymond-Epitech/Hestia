<template>
  <button class="back" @click="redirect('/settings')">
    <img src="~/public/Retour.svg" class="icon">
  </button>
  <div class="container">
    <div class="setting-button" id="fr" @click="setlangue('fr')">
      <div class="flag">
        <img src="/flags/france.png" alt="France" />
      </div>
      <Texte_language class="language-text" source="French" />
    </div>
    <div class="setting-button" id="ang" @click="setlangue('en')">
      <div class="flag">
        <img src="/flags/united-kingdom.png" alt="United Kingdom" />
      </div>
      <Texte_language class="language-text" source="English" />
    </div>
    <div class="setting-button" id="es" @click="setlangue('es')">
      <div class="flag">
        <img src="/flags/spain.png" alt="Spain" />
      </div>
      <Texte_language class="language-text" source="Spanish" />
    </div>
    <div class="setting-button" id="allemand" @click="setlangue('de')">
      <div class="flag">
        <img src="/flags/germany.png" alt="Germany" />
      </div>
      <Texte_language class="language-text" source="German" />
    </div>
    <div class="setting-button" id="mandarin" @click="setlangue('zh')">
      <div class="flag">
        <img src="/flags/china.png" alt="China" />
      </div>
      <Texte_language class="language-text" source="Chinese" />
    </div>
    <div class="setting-button" id="japonais" @click="setlangue('ja')">
      <div class="flag">
        <img src="/flags/japan.png" alt="Japan" />
      </div>
      <Texte_language class="language-text" source="Japanese" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import Texte_language from '~/components/texte_language.vue';
import type { Locale } from '~/composables/service/type';
import { useUserStore } from '~/store/user';
const { setLocale } = useI18n();
const { $locally } = useNuxtApp()
const userStore = useUserStore();
const router = useRouter();
const { $bridge } = useNuxtApp()
const api = $bridge;
const redirect = (page: string) => {
  router.push(page);
}

const setlangue = (lang: Locale) => {
  setLocale(lang);
  $locally.setItem('locale', lang);
  api.updateLanguage(lang, userStore.user.id);
}
</script>

<style scoped>
.container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  margin: 40px auto;
  margin-top: 5rem;
  max-height: calc(100vh - 5.5rem);
}

.icon {
  display: flex;
  align-items: center;
  justify-content: center;
  filter: invert(1);
}

.dark .icon {
  filter: invert(0);
}

.setting-button {
  width: 20.2rem;
  height: 80px;
  display: grid;
  grid-template-columns: 1fr 4fr;
  align-items: center;
  border-radius: 20px;
  background-color: var(--main-buttons-light);
  box-shadow: var(--rectangle-shadow-light);
}

.dark .setting-button {
  background-color: var(--main-buttons-dark);
  color: var(--page-text-dark);
}

.flag {
  margin-left: 30%;
  height: 60px;
  width: 40px;
  display: flex;
  align-items: center;
  justify-content: center;

}

.language-text {
  margin-left: 8%;
  font-weight: 600;
  font-size: 24px;
}

.back {
  background-color: var(--main-buttons-light);
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
  filter: invert(1);
  width: 25px;
}

.dark .back {
  background-color: var(--main-buttons-dark);
}

.dark .back .icon {
  filter: none;
}
</style>