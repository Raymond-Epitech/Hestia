set -e
set -x

npx nuxi generate # Create a web build
if [ ! -d ./dist ]
then
	npx nuxt generate # Create 'dist'
fi
if [ ! -d ./android ]
then
	npx cap add android
fi
npx cap sync # update capacitor project directories
cd android
rm -f app/build/outputs/apk/realease/*
./gradlew clean
./gradlew --stop
./gradlew assembleDebug --scan --no-daemon --parallel # Create apk

cp app/build/outputs/apk/debug/*.apk ../Hestia.apk
