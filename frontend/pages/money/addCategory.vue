<template>
  <div class="background">
    <div>
      <h1 class="header">
        <img src="/return.png" alt="Return" width="30" height="30" @click="$router.back()" />
        <Texte_language source="header_add_category" />
        <p></p>
      </h1>
    </div>
    <form method="post" action="">
      <div>
        <h3>
          <Texte_language source="expense_category_name" />
        </h3>
        <input class="body-input" maxlength="35" v-model="newcategory.name" required />
      </div>
      <div class="modal-buttons">
        <button class="button" @click.prevent="handleProceed">
          <Texte_language source="poster" />
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { expenses_category } from '~/composables/service/type';

useHead({
  bodyAttrs: {
    style: 'background-color: #1E1E1E;'
  }
})

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();
const user = userStore.user;
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const newcategory = ref<expenses_category>({
  colocationId: user.colocationId,
  name: '',
});
const handleProceed = async () => {
  console.log(newcategory.value);
  api.addexpensecategory(newcategory.value).then(() => {
    console.log('Expense category added successfully');
    router.back()
  }).catch((error) => {
    console.error('Error adding expense category:', error);
  });
}
</script>

<style scoped>
.background {
  height: 100%;
  background-color: #1E1E1E;
  color: white;
  padding: 20px;
}

.body-input {
  width: 70%;
  background-color: #1e1e1e00;
  outline: none;
  border: none;
  line-height: 3ch;
  background-image: linear-gradient(transparent, transparent calc(3ch - 1px), #E7EFF8 0px);
  background-size: 100% 3ch;
  color: #fff;
  font-size: 18px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.button {
  margin-top: 16px;
  padding: 10px 20px;
  border-radius: 15px;
  border: 0;
  cursor: pointer;
  background: #00000088;
  color: #fff;
}
</style>