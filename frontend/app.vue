<template>
  <div>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<script setup>
const { $push } = useNuxtApp();

onMounted(() => {
  $push.registerNotifications()
    .then(()  => console.log("Notifications authorised"));
    .catch(() => console.log("Notifications refused or authorise failure/unavailable", err));
})

</script>