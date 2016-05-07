//#define PUSHWOOSH
#define APPSFLYER
using UnityEngine;
using System;
using GameEnum;

public class LobbyStart : MonoBehaviour {
    private static LobbyStart instance;

    public bool OpenTutorial = false;
    public bool IsUseFpsLimiter = true;

    public static LobbyStart Get {
        get {
            return instance;
        }
    }

    public static bool Visible {
        get {
            return instance && instance.gameObject.activeInHierarchy;
        }
    }

	void Awake ()
    {
        instance = gameObject.GetComponent<LobbyStart>();
		Time.timeScale = 1;
        GameObject obj = null;
        #if UNITY_EDITOR
        obj = GameObject.Find("FileManager");
        if (obj) {
            Destroy(obj);
            obj = null;
        }
        #else
        Application.runInBackground = false;
        #endif

        obj = GameObject.Find("AudioMgr");
        if (!obj) {
            obj = Resources.Load("Prefab/AudioMgr") as GameObject;
            obj = Instantiate(obj) as GameObject;
            obj.name = "AudioMgr";
        }

        if (GameData.Init()) {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            UILoading.UIShow(true, ELoading.Login);
            SendHttp.Get.CheckVersion();
            if (IsUseFpsLimiter) {
                FpsLimiter fl = gameObject.GetComponent<FpsLimiter>();
                if (!fl)
                    SendHttp.Get.gameObject.AddComponent<FpsLimiter>();
            }

			SendHttp.Get.InitIAP();
            #if UNITY_ANDROID
                #if PUSHWOOSH
                SendHttp.Get.gameObject.AddComponent<Pushwoosh>();
                #endif

                #if APPSFLYER
                SendHttp.Get.gameObject.AddComponent<StartUp>();
                #endif
            #endif
        }
    }

    public void EnterLobby()
    {
		try
        {
			UILoading.UIShow(false);
			UIMainLobby.Get.Show();
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
}
