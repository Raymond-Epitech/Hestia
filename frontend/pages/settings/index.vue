<template>
    <div class="conteneur">
        <div class="setting-button" :onClick="() => redirect('/settings/language')" id="language">
            <div class="icon">
                <img src="~/public/settings/language.svg">
            </div>
            <div class="setting-text">
                <Texte_language class="header" source="Language" />
                <Texte_language class="sub-header" source="Language-subtext" />
            </div>
        </div>
        <div class="setting-button" :onClick="() => redirect('/settings/display')" id="display">
            <div class="icon">
                <img src="~/public/settings/Affichage.svg">
            </div>
            <div class="setting-text">
                <Texte_language class="header" source="Display" />
                <Texte_language class="sub-header" source="Display-subtext" />
            </div>
        </div>
        <div class="setting-button" :onClick="() => redirect('/settings/colocation')" id="colocation">
            <img src>
            <div class="setting-text">
                <Texte_language class="header" source="Colocation" />
                <Texte_language class="sub-header" source="Colocation-subtext" />
            </div>
        </div>
        <!-- <div class="setting-button" :onClick="() => handleComposantClick('notif')" id="notif">
            <img src>
            <Texte_language class="header" source="Notifications" />
        </div>
        <div class="setting-button" :onClick="() => handleComposantClick('acces')" id="acces">
            <img src>
            <Texte_language  class="header" source="Accessibility" />
        </div>
        <div class="setting-button" :onClick="() => handleComposantClick('regle')" id="regle">
            <img src>
            <Texte_language class="header" source="Rules" />
        </div>
        <div class="setting-button" :onClick="() => handleComposantClick('conf')" id="conf">
            <img src>
            <Texte_language class="header" source="Confidentiality" />
        </div>
        <div class="setting-button" :onClick="() => handleComposantClick('info')" id="info">
            <img src>
            <Texte_language class="header" source="Informations" />
        </div> -->
        <div class="setting-button" color="#FF6A61" @click.passive="logout()" id="logout">
            <img src>
            <Texte_language class="header" source="logout" />
        </div>
    </div>
</template>

<script setup>
    import { useAuthStore } from '~/store/auth';

    const authenticateUser = useAuthStore();

    const handleComposantClick = (id) => {
        console.log(`ID du composant cliqué: ${id}`);
        alert(`ID du composant cliqué: ${id}`);
    }
    const router = useRouter();
    const redirect = (page) => {
        router.push(page);
    }

    function logout() {
        authenticateUser.logUserOut();
        router.push('/');
    }
</script>

<style scoped>
    .conteneur {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 10px;
        margin: 40px auto;
    }

    .setting-button {
        width: 90%;
        height: 80px;
        display: grid;
        grid-template-columns: 1fr 4fr;
        align-items: center;
        border-radius: 20px;
        background-color: var(--main-buttons-light);
        box-shadow: var(--rectangle-shadow-light);
    }

    .dark .setting-button {
        background-color: var(--main-buttons-dark);
        color: var(--page-text-dark);
    }

    .setting-text {
        display: flex;
        flex-direction: column;
    }

    .header {
        font-size: 24px;
        font-weight: 600;
    }

    .sub-header {
        font-size: 12px;
        font-weight: 600;
    }

    .icon {
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .dark .icon {
        filter: invert(1);
    }
</style>