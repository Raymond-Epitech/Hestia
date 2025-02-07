<template>
  <NuxtPage />
  <div class="container">
    <Rectangle color="#85AD7B" id="return" @click="$router.back()">
      <div class="inner-class">
        <img src="/return.png" alt="Return" width="64" height="64" />
        <h1>
          <Texte_language source="Return" />
        </h1>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="fr" @click="setlangue('fr')">
      <div class="inner-class">
        <img src="/flags/france.png" alt="France" width="64" height="64" />
        <h1>
          <Texte_language source="French" />
        </h1>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="ang" @click="setlangue('en')">
      <div class="inner-class">
        <img src="/flags/united-kingdom.png" alt="United Kingdom" width="64" height="64" />
        <h1>
          <Texte_language source="English" />
        </h1>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="es" @click="setlangue('es')">
      <div class="inner-class">
        <img src="/flags/spain.png" alt="Spain" width="64" height="64" />
        <h1>
          <Texte_language source="Spanish" />
        </h1>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="allemand" @click="setlangue('de')">
      <div class="inner-class">
        <img src="/flags/germany.png" alt="Germany" width="64" height="64" />
        <h1>
          <Texte_language source="German" />
        </h1>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="mandarin" @click="setlangue('zh')">
      <div class="inner-class">
        <img src="/flags/china.png" alt="China" width="64" height="64" />
        <h1>
          <Texte_language source="Chinese" />
        </h1>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="japonais" @click="setlangue('ja')">
      <div class="inner-class">
        <img src="/flags/japan.png" alt="Japan" width="64" height="64" />
        <h1>
          <Texte_language source="Japanese" />
        </h1>
      </div>
    </Rectangle>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import Texte_language from '~/components/texte_language.vue';
import type { Locale } from '~/service/type';
const { setLocale } = useI18n();
const { $locally } = useNuxtApp()

const setlangue = (lang: Locale) => {
  setLocale(lang);
  $locally.setItem('locale', lang);
}
</script>

<style scoped>
.container {
  display: grid;
  gap: 40px;
  width: 80%;
  margin: 40px auto;
}

.inner-class {
  display: flex;
  align-items: center;
  gap: 10px;
}
</style>