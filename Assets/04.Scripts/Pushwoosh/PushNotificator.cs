using UnityEngine;
using System.Collections;
using System;

public class PushNotificator : MonoBehaviour
{
    string notificationText = "Pushwoosh is not initialized";
    public PushNotificationsAndroid PushNotificationsAndroidMgr = null;

    // Use this for initialization
    void Start()
    {
        Pushwoosh.Instance.OnRegisteredForPushNotifications += onRegisteredForPushNotifications;
        Pushwoosh.Instance.OnFailedToRegisteredForPushNotifications += onFailedToRegisteredForPushNotifications;
        Pushwoosh.Instance.OnPushNotificationsReceived += onPushNotificationsReceived;
        #if UNITY_ANDROID && !UNITY_EDITOR
			PushNotificationsAndroidMgr = Pushwoosh.Instance as PushNotificationsAndroid;
			PushNotificationsAndroidMgr.ClearLocalNotifications();
        #endif
    }

    void onRegisteredForPushNotifications(string token)
    {
        notificationText = "Received token: \n" + token;

        //do handling here
        Debug.Log("onRegisteredForPushNotifications : " + notificationText);
    }

    void onFailedToRegisteredForPushNotifications(string error)
    {
        notificationText = "Error ocurred while registering to push notifications: \n" + error;

        //do handling here
        Debug.Log("onFailedToRegisteredForPushNotifications : " + notificationText);
    }

    void onPushNotificationsReceived(string payload)
    {
        notificationText = "Received push notificaiton: \n" + payload;

        //do handling here
        Debug.Log("onPushNotificationsReceived : " + notificationText);
    }

    void OnApplicationQuit()
    {
        CallPowerFull();	
    }
        
    void OnApplicationFocus(bool focusStatus)
    {
        if (!focusStatus)
        {
            CallPowerFull();
        }
        else
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            PushNotificationsAndroidMgr.ClearLocalNotifications();
            #endif
        }
    }

    public void CallPowerFull()
    {
        if (GameData.Team.Power < GameConst.Max_Power)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                PushNotificationsAndroidMgr.ClearLocalNotifications();
                PushNotificationsAndroidMgr.ScheduleLocalNotification(TextConst.S(802), (int)GetFullPowerTime());
            #endif
        }
    }

    private Double GetFullPowerTime()
    {
//        TimeSpan checktime;
//        Double TotalSeconds = 0;
//        checktime = GameData.Team.PowerCD.ToUniversalTime().Subtract(DateTime.UtcNow);
//        if (checktime.TotalSeconds >= 0)
//            TotalSeconds = ((GameConst.Max_Power - GameData.Team.Power) - 1) * 10 * 60 + checktime.TotalSeconds;
        return (GameConst.Max_Power - GameData.Team.Power) * 10 * 1;
    }
    void Update () 
	{  
        if (Input.GetKeyDown (KeyCode.Escape))
		{
            UIMessage.Get.ShowMessage(TextConst.S(211), TextConst.S(235),OnYes);
        }  
    }

	private void OnYes()
	{
		CallPowerFull();
		Application.Quit();  		
	}
}
