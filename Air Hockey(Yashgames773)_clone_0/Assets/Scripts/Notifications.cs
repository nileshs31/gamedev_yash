
using Unity.Notifications.Android;


using UnityEngine;

public class Notifications : MonoBehaviour
{

    [SerializeField]  private string NotificationTitle;
    [SerializeField] private string NotificationText;
    void Start()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Notification Channel",
            Importance = Importance.Default,
            Description = "Reminder notifications",

        };

  
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = NotificationTitle;
        notification.Text = NotificationText;
        //   notification.FireTime = System.DateTime.Now.AddHours(23);
        notification.FireTime = System.DateTime.Now.AddSeconds(7);
        notification.SmallIcon = "icon_small";
        notification.LargeIcon = "icon_large";
        notification.ShowTimestamp = true;


        var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }


       

    }


}

 

