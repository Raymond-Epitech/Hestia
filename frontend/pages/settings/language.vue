<template>
  <div class="container">
    <div id="return" :onClick="() => redirect('/settings')">
      <img src="/Retour.svg" class="icon" alt="Return" width="32" height="32" />
    </div>

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
  const { setLocale } = useI18n();
  const { $locally } = useNuxtApp()
  const router = useRouter();
  const redirect = (page) => {
    router.push(page);
  }


  const setlangue = (lang: Locale) => {
    setLocale(lang);
    $locally.setItem('locale', lang);
  }
</script>

<style scoped>
  .container {
    display: grid;
    gap: 10px;
    width: 90%;
    margin: 40px auto;
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
    width: 100%;
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
</style>