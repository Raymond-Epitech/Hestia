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
        <input class="body-input" rows="3" v-model="newcategory.name" required />
      </div>
      <div class="modal-buttons">
        <button class="button button-proceed" @click.prevent="handleProceed">
          <Texte_language source="poster" />
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '~/store/user';
import type { expenses_category } from '~/composables/service/type';

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