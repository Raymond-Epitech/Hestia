<template>
    <div>
      <button class="add-post" data-toggle="modal" data-target=".bd-example-modal-sm">
        <img src="~/public/plus.png" class="plus">
      </button>
      <h1>Welcome to the homepage</h1>
      <AppAlert>
        This is an auto-imported component
      </AppAlert>
      <h1>{{data}}</h1>
      <Post title="test-title" text="test-text" @delete="handlePostDelete"/>
    </div>
</template>

<script setup>
const {data} = await useFetch('http://localhost:8080/api/Version')

const handlePostDelete = () => {
  console.log('Post supprimé')
  // Ajoutez ici la logique pour supprimer le post
}
</script>

<style scoped>
.add-post{
  display: flex;
  justify-content: center;
  align-items: center;
  width: 42px;
  height: 42px;
  margin: 16px 16px;
  background-color: #FFF973;
  border-radius: 9px;
  border: none;
  box-shadow: -5px 5px 10px 0px rgba(0, 0, 0, 0.28);
}

.plus {
  width: 24px;
  height: 24px;
}
</style>