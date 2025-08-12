package com.hestiaapp.org;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import androidx.core.app.NotificationCompat;
import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;

public class HestiaFirebaseMessagingService
        extends FirebaseMessagingService {
            @Override
            public void onMessageReceived(RemoteMessage remoteMessage) {
                // Handle the received message
                if (remoteMessage.getNotification() != null) {
                    // Get the message body
                    String messageBody = remoteMessage.getNotification().getBody();
                    // Send a notification
                    sendNotification(messageBody);
                }
            }
        
            private void sendNotification(String messageBody) {
                Intent intent = new Intent(this, MainActivity.class);
                intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
                PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, intent, PendingIntent.FLAG_IMMUTABLE);
        
                // String channelId = getString(R.string.default_notification_channel_id);
                NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this)
                        .setSmallIcon(R.drawable.ic_hestia_push)
                        .setContentTitle(getString(R.string.app_name))
                        .setContentText(messageBody)
                        .setAutoCancel(true)
                        .setContentIntent(pendingIntent);
        
                NotificationManager notificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
        
                //if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                    //NotificationChannel channel = new NotificationChannel(channelId, "Channel human readable title", NotificationManager.IMPORTANCE_DEFAULT);
                    //notificationManager.createNotificationChannel(channel);
                //}
        
                notificationManager.notify(0, notificationBuilder.build());
            }
}
