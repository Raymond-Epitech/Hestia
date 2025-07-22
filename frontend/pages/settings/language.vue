<template>
  <NuxtPage />
  <div class="container">
    <Rectangle color="#85AD7B" id="return" :onClick="() => redirect('/settings')">
      <div class="inner-class">
        <img src="/return.png" alt="Return" width="50" height="50" />
        <div class="language-text">
          <Texte_language source="Return" />
        </div>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="fr" @click="setlangue('fr')">
      <div class="inner-class">
        <img src="/flags/france.png" alt="France" width="64" height="64" />
        <div class="language-text">
          <Texte_language source="French" />
        </div>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="ang" @click="setlangue('en')">
      <div class="inner-class">
        <img src="/flags/united-kingdom.png" alt="United Kingdom" width="64" height="64" />
        <div class="language-text">
          <Texte_language source="English" />
        </div>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="es" @click="setlangue('es')">
      <div class="inner-class">
        <img src="/flags/spain.png" alt="Spain" width="64" height="64" />
        <div class="language-text">
          <Texte_language source="Spanish" />
        </div>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="allemand" @click="setlangue('de')">
      <div class="inner-class">
        <img src="/flags/germany.png" alt="Germany" width="64" height="64" />
        <div class="language-text">
          <Texte_language source="German" />
        </div>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="mandarin" @click="setlangue('zh')">
      <div class="inner-class">
        <img src="/flags/china.png" alt="China" width="64" height="64" />
        <div class="language-text">
          <Texte_language source="Chinese" />
        </div>
      </div>
    </Rectangle>
    <Rectangle color="#85AD7B" id="japonais" @click="setlangue('ja')">
      <div class="inner-class">
        <img src="/flags/japan.png" alt="Japan" width="64" height="64" />
        <div class="language-text">
          <Texte_language source="Japanese" />
        </div>
      </div>
    </Rectangle>
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
  gap: 40px;
  width: 80%;
  margin: 40px auto;
}

.inner-class {
  width: 100%;
  display: grid;
  grid-template-columns: 1fr 3fr;
  text-align: center;
  align-items: center;
  gap: 10px;
}

.language-text {
  font-weight: 600;
  font-size: 28px;
}
</style>