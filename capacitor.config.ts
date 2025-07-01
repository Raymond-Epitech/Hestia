import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'io.ionic.starter',
  appName: 'Hestia',
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
