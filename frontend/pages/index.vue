<template>
    <div>
      <h1>Welcome to the homepage</h1>
      <AppAlert>
        This is an auto-imported component
      </AppAlert>
      <h1>{{data}}</h1>
      <Post title="test-title" text="test-text" @delete="handlePostDelete"/>
    </div>
</template>

<script setup>
const {data} = await useFetch('http://localhost:8080/api/Test')

const handlePostDelete = () => {
  console.log('Post supprimé')
  // Ajoutez ici la logique pour supprimer le post
}
</script>
 
