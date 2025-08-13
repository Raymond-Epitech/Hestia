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
    z-index: 1000;
    backdrop-filter: blur(6px);
}

.popup {
    position: fixed;
    display: flex;
    flex-direction: column;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background-color: #1D1B20;
    
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
    font-size: 16px;
    text-align: center;
}

.popup button {
    padding: 10px 20px;
    border: none;
    border-radius: 7px;
}

.button {
    display: flex;
    justify-content: center;
    gap: 16px
}

.text {
    text-align: center;
    font-size: 16px;
    color: white;
    font-weight: bold;
}
.popup .cancel-button {
    background-color: #000000;
    color: white;
    width: 120px;
}

.popup .confirm-button {
    background-color: #526074;
    color: white;
    width: 120px;
}
</style>