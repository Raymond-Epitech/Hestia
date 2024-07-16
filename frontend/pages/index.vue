<template>
    <div>
      <h1>Welcome to the homepage</h1>
      <AppAlert>
        This is an auto-imported component
      </AppAlert>
      <h1>{{data}}</h1>
    </div>
</template>

<script setup>
const {data} = await useFetch('http://localhost:8080/api/Test')

</script>
 