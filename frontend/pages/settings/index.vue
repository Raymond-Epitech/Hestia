<template>
    <button class="back" @click="redirect('/profile')">
        <img src="~/public/Retour.svg" class="icon">
    </button>
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
            <div class="icon">
                <img src="~/public/settings/Users.svg">
            </div>
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
            <div class="icon">
                <img src="~/public/settings/Quitter.svg">
            </div>
            <Texte_language class="header" source="logout" />
        </div>
    </div>
</template>

<script setup>
    import { useAuthStore } from '~/store/auth';

    const authenticateUser = useAuthStore();

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
        margin-top: 5rem;
        max-height: calc(100vh - 5.5rem);
    }

    .setting-button {
        width: 20.2rem;
        height: 80px;
        display: grid;
        grid-template-columns: 1fr 4fr;
        align-items: center;
        border-radius: 20px;
        background-color: var(--main-buttons);
        box-shadow: var(--rectangle-shadow-light);
        color: var(--page-text);
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
        filter: var(--icon-filter);
    }

    img {
        width: 30px;
    }

.back {
    background-color: var(--main-buttons);
    position: fixed;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 40px;
    height: 40px;
    border-radius: 9px;
    border: none;
    box-shadow: var(--button-shadow-light);
    top: 3%;
    left: 3%;
}

.back .icon {
    filter: var(--icon-filter);
    width: 25px;
}
</style>