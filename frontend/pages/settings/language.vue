<template>
  <NuxtPage />
  <div class="container">
        <Rectangle color="#85AD7B" id="fr">
          <div class="inner-class">
            <h1> Français </h1>
            <img src="/flags/france.png" alt="France" width="64" height="64"/>
          </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="ang">
          <div class="inner-class">
              <h1> Anglais </h1>
              <img src="/flags/united-kingdom.png" alt="United Kingdom" width="64" height="64"/>
            </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="es">
          <div class="inner-class">
            <h1> Espagnol</h1>
            <img src="/flags/spain.png" alt="Spain" width="64" height="64"/>
          </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="allemand">
          <div class="inner-class">
            <h1> Allemand</h1>
            <img src="/flags/germany.png" alt="Germany" width="64" height="64"/>
          </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="mandarin">
          <div class="inner-class">
            <h1> Mandarin</h1>
            <img src="/flags/china.png" alt="China" width="64" height="64"/>
          </div>
        </Rectangle>
        <Rectangle color="#85AD7B" id="japonais">
          <div class="inner-class">
            <h1> Japonais</h1>
            <img src="/flags/japan.png" alt="Japan" width="64" height="64"/>
          </div>
        </Rectangle>
    </div>
</template>

<script setup lang="ts">

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