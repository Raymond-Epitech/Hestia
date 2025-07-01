# Nuxt 3 Minimal Starter

Look at the [Nuxt 3 documentation](https://nuxt.com/docs/getting-started/introduction) to learn more.

## Setup

Make sure to install the dependencies:

```bash
# npm
npm install

# pnpm
pnpm install

# yarn
yarn install

# bun
bun install
```

## Development Server

Start the development server on `http://localhost:3000`:

```bash
# npm
npm run dev

# pnpm
pnpm run dev

# yarn
yarn dev

# bun
bun run dev
```

## Production

Build the application for production:

```bash
# npm
npm run build

# pnpm
pnpm run build

# yarn
yarn build

# bun
bun run build
```

Locally preview production build:

```bash
# npm
npm run preview

# pnpm
pnpm run preview

# yarn
yarn preview

# bun
bun run preview
```

Check out the [deployment documentation](https://nuxt.com/docs/getting-started/deployment) for more information.

# Build APK

Build the apk for release:
### **In /android/gradle.properties, the last line specifies a custom path for aapt2 the apk builder. This is to enable build on arm64 system. If you are not building on arm64, comment that line. If you are, download [this](https://objects.githubusercontent.com/github-production-release-asset-2e65be/122611158/b1d62f57-94cb-4921-97e5-8e9bcb7d1990?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=releaseassetproduction%2F20241217%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20241217T204939Z&X-Amz-Expires=300&X-Amz-Signature=958c6fd9a6296bacb0a34eba964961a71d724cebfe325116c38dc5197baf6eb9&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3Dandroid-sdk-tools-static-aarch64.zip&response-content-type=application%2Foctet-stream) android SDK build and move the aapt2 binary to the `/usr/bin` dir and give execution rights with `chmod +x /usr/bin/aapt2`. This should let you build hestia on an arm64 system.**

```bash
./build-app.sh
```
