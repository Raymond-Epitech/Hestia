<template>
    <div class="overlay" @click.self="emit('close')">
        <div class="popup">
            <h1>{{ title }}</h1>
            <p class="text">{{ text }}</p>
            <div class="button">
                <button class="confirm-button" @click="emit('confirm')">
                    <Texte_language source="confirm" />
                </button>
                <button class="cancel-button" @click="emit('close')">
                    <Texte_language source="cancel" />
                </button>
            </div>
        </div>
    </div>
</template>

<script setup>
    const props = defineProps({
        text: {
            type: String,
            required: true
        },
        title: {
            type: String,
            required: false
        }
    })
    const emit = defineEmits([
        'confirm',
        'close'
    ]);
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
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        background-color: var(--background-light);
        padding: 4%;
        border-radius: 8px;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        z-index: 1000;
    }

    .dark .popup {
        background-color: var(--background-dark);
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
        border-radius: 7px;
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
        color: var(--page-text-light);
    }

    .dark .text {
        color: var(--page-text-dark);
    }

    .cancel-button {
        background-color: var(--main-buttons-light);
        color: var(--page-text-light);
        width: 120px;
        box-shadow: var(--button-shadow-light);
    }

    .confirm-button {
        background-color: var(--basic-red);
        color: var(--page-text-light);
        width: 120px;
        box-shadow: var(--button-shadow-light);
    }

    .dark .cancel-button {
        background-color: #000000;
        color: var(--page-text-dark);
        width: 120px;
    }

    .dark .confirm-button {
        background-color: var(--sent-message-dark);
        color: var(--page-text-dark);
        width: 120px;
    }
</style>