# Démarrer avec le back

Pour commencer, assurez vous d'avoir bien clone le repository

# Fichier de configuration

Avoir récupérer le fichier "appsettings.Development 2.json" et le renommer en "appsettings.Development.json". Vous allez le copier dans le dossier *Hestia/backend/Api* (vous devriez avoir un appsettings.json deja présent)

## Lancement avec docker

Si tout est bien configurer, lancer le docker (**docker compose up** dans le folder *Hestia/docker)*
Si tout vas bien vous devriez voir apparaitre dans les logs : "**Connection successful!**"
L'API se situe a la route : ip(localhost):8080/api/

Pour acceder au swagger : ip(localhost):8080/swagger

## Lancement en local avec Visual studio

Ouvrez le .sln situé dans *Hestia/backend/backend.sln*
Sélectionner **Api** comme projet de startup et lancer avec **F5** (ou le bouton vert en haut)
 
## Lancement avec le terminal

Utiliser "dotnet run" dans le dossier *Hestia/backendApi*

