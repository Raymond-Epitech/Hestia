import { PushNotifications } from '@capacitor/push-notifications';

export const addListeners = async () => {
  await PushNotifications.addListener('registration', token => {
    console.info('Registration token: ', token.value);
  });

  await PushNotifications.addListener('registrationError', err => {
    console.error('Registration error: ', err.error);
  });

  await PushNotifications.addListener('pushNotificationReceived', notification => {
    console.log('Push notification received: ', notification);
  });

  await PushNotifications.addListener('pushNotificationActionPerformed', notification => {
    console.log('Push notification action performed', notification.actionId, notification.inputValue);
  });
}

export const registerNotifications = async () => {
  let permStatus = await PushNotifications.checkPermissions();

  if (permStatus.receive === 'prompt') {
    permStatus = await PushNotifications.requestPermissions();
  }

  if (permStatus.receive !== 'granted') {
    throw new Error('User denied permissions!');
  }

  await PushNotifications.register();
}

// export class push {
//     constructor() {
//         console.log('Push notifications instance created')
//     }
//     push_token: string = "";

//     async addListeners () {
//       console.log('Adding listeners for push notifications');
//       await PushNotifications.addListener('registration', token => {
//         console.info('Registration token: ', token.value);
//       });

//       await PushNotifications.addListener('registrationError', err => {
//         console.error('Registration error: ', err.error);
//       });

//       await PushNotifications.addListener('pushNotificationReceived', notification => {
//         console.log('Push notification received: ', notification);
//       });

//       await PushNotifications.addListener('pushNotificationActionPerformed', notification => {
//         console.log('Push notification action performed', notification.actionId, notification.inputValue);
//       });
//     }

//     async registerNotifications () {
//       console.log('Trying to register notifications')
//       let permStatus = await PushNotifications.checkPermissions();

//       if (permStatus.receive === 'prompt') {
//         permStatus = await PushNotifications.requestPermissions();
//       }

//       if (permStatus.receive !== 'granted') {
//         throw new Error('User denied permissions!');
//       }

//       await PushNotifications.register();
//     }

//     async getDeliveredNotifications () {
//       const notificationList = await PushNotifications.getDeliveredNotifications();
//       console.log('delivered notifications', notificationList);
//     }
// }