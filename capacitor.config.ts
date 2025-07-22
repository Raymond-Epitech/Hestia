import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'io.ionic.starter',
  appName: 'Hestia',
  server: {
    "allowNavigation": ["https://91.134.48.124:8080"]
  },
  webDir: 'dist',
  "plugins": {
    "CapacitorHttp": {
      "enabled": true
    },
    SocialLogin: {
      providers: {
        google: {
          clientId: "80772791160-169jnnnnm5o18mg1h0uc7jm4s2epaj5d.apps.googleusercontent.com"
        }
      }
    }
  }
};

export default config;
