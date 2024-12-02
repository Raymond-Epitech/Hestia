<template>
  <NuxtPage />
  <div class="container">
        <Rectangle color="#85AD7B" id="fr" @click="setLocale('fr')">
          <div class="inner-class">
            <img src="/flags/france.png" alt="France" width="64" height="64"/>
            <h1> {{ $t('French') }} </h1>
          </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="ang" @click="setLocale('en')">
          <div class="inner-class">
            <img src="/flags/united-kingdom.png" alt="United Kingdom" width="64" height="64"/>
              <h1> {{ $t('English') }} </h1>
            </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="es" @click="setLocale('es')">
          <div class="inner-class">
            <img src="/flags/spain.png" alt="Spain" width="64" height="64"/>
            <h1> {{ $t('Spanish') }} </h1>
          </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="allemand" @click="setLocale('de')">
          <div class="inner-class">
            <img src="/flags/germany.png" alt="Germany" width="64" height="64"/>
            <h1> {{ $t('German') }} </h1>
          </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="mandarin" @click="setLocale('zh')">
          <div class="inner-class">
            <img src="/flags/china.png" alt="China" width="64" height="64"/>
            <h1> {{ $t('Chinese') }} </h1>
          </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="japonais" @click="setLocale('ja')">
          <div class="inner-class">
            <img src="/flags/japan.png" alt="Japan" width="64" height="64"/>
            <h1> {{ $t('Japanese') }} </h1>
          </div>
        </Rectangle>
    </div>
</template>

<script setup lang="ts">
const { setLocale } = useI18n();
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