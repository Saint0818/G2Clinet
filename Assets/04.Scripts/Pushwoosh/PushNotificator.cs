using UnityEngine;
using System.Collections;
using System;

public class PushNotificator : MonoBehaviour
{
    string notificationText = "Pushwoosh is not initialized";
    public PushNotificationsAndroid PushNotificationsAndroidMgr = null;

	#if UNITY_ANDROID && !UNITY_EDITOR
		private static AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		private static AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
	#endif

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
        TimeSpan checktime;
        Double TotalSeconds = 0;
        checktime = GameData.Team.PowerCD.ToUniversalTime().Subtract(DateTime.UtcNow);
        if (checktime.TotalSeconds >= 0)
            TotalSeconds = ((GameConst.Max_Power - GameData.Team.Power) - 1) * 10 * 60 + checktime.TotalSeconds;
        return TotalSeconds;
    }

    private float mPressTimes = 0;
    void Update () {  
        if (Input.GetKeyDown (KeyCode.Escape)) {//KeyCode.Escape表示键盘ESC,手机的返回键
            mPressTimes++;  
            StartCoroutine ("ResetMPressTimes", 1.0f);//若过了1秒都没有按第2次则重置mPressTimes  
            if (mPressTimes == 2) {  
                Application.Quit();  
            }  
			else
			{
				AndroidShowToast("再按一次返回键退出程序");
			}
								
        }  
    }  

    IEnumerator ResetMPressTimes (float sec) {  
        yield return new WaitForSeconds(sec);  
        mPressTimes = 0;  
    } 

	public void AndroidShowToast(string totas)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR				
		jo.Call("makeToast", totas);
		#endif
	}
}
