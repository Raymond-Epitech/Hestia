<template>
    <div class="overlay" @click.self="emit('close')">
        <div class="popup">
            <div class="modal-body left">
              <input type="file" class="modal-body-input" @change="handleImageUpload" accept="image/*" required />
              <img v-if="prewiew" :src="prewiew" alt="Image sélectionnée" class="image-preview" />
            </div>
            <div v-if="prewiew" class="modal-buttons">
              <button class="button button-proceed" @click.prevent="handleProceed">{{ $t('poster') }}</button>
            </div>
            <div v-else class="modal-buttons">
              <button class="button button-proceed" @click.prevent="handleProceed" disabled>{{ $t('poster') }}</button>
            </div>
        </div>
    </div>
    <div v-if="errview">
        <Errorpopup :status="err.status" :body="err.body" @close="errview = false" />
    </div>
</template>

<script setup lang="ts">
import type {UserInfo} from '~/composables/service/type';

const props = defineProps({
    user: {
        type: Object as PropType<UserInfo>,
        required: true
    }
})

const { $bridge } = useNuxtApp()
const api = $bridge;
const err = ref<{ status: number; body: any }>({ status: 0, body: null });
const errview = ref(false);
api.setjwt(useCookie('token').value ?? '');
const prewiew = ref('');
const selectedFile = ref<File | null>(null);

const emit = defineEmits([
    'close'
]);

const handleImageUpload = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0];
  if (file) {
    selectedFile.value = file;
    prewiew.value = URL.createObjectURL(file);
  }
};

const handleProceed = async () => {
if (!selectedFile.value) return;

  try {
    const updatedUser = ref({
        id: props.user.id,
        username: props.user.username,
        email: props.user.email,
        colocationId: props.user.colocationId,
        pathToProfilePicture: selectedFile.value,
    })
    console.log(updatedUser)
    const response = await api.updateUser(updatedUser.value);
    if (response) emit('close');
  } catch (error: any) {
    console.error(error);
    err.value = { status: error.status || 500, body: error.body || error.message };
    errview.value = true;
  }
};

</script>

<style scoped>
.overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 2000;
    backdrop-filter: blur(6px);
}


.popup {
    position: fixed;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-evenly;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background-color: var(--list-overlay-bg);
    padding: 4%;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    z-index: 1000;
}

.popup h1 {
    margin: 0 0 10px;
    font-size: 24px;
}

.popup p {
    margin: 0 0 20px;
    font-size: 20px;
    text-align: center;
}

.popup button {
    padding: 10px 20px;
    border: none;
    border-radius: 16px;
}

.button {
    display: flex;
    justify-content: center;
    gap: 16px
}

.text {
    text-align: center;
    font-size: 16px;
    font-weight: bold;
    color: var(--page-text);
}

.cancel-button {
    background-color: var(--main-buttons);
    color: var(--page-text);
    padding: 10px 20px;
    box-shadow: var(--button-shadow-light);
}

.confirm-button {
    background-color: var(--basic-red);
    color: var(--page-text);
    padding: 10px 20px;
    box-shadow: var(--button-shadow-light);
}

.dark .confirm-button {
    background-color: var(--sent-message);
}
</style>