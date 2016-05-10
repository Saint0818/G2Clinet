//#define PUSHWOOSH
#define APPSFLYER
using UnityEngine;
using System;
using System.Collections;
using GameEnum;

public class LobbyStart : MonoBehaviour {
    public bool OpenTutorial = false;
    public bool IsUseFpsLimiter = true;

    IEnumerator init() {
        yield return new WaitForEndOfFrame();

        GameObject obj = GameObject.Find("AudioMgr");
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

            #if UNITY_ANDROID
            SendHttp.Get.InitIAP();

            #if PUSHWOOSH
            SendHttp.Get.gameObject.AddComponent<Pushwoosh>();
            #endif

            #if APPSFLYER
            SendHttp.Get.gameObject.AddComponent<StartUp>();
            #endif
            #endif
        }
    }

	void Start ()
    {
        GameData.OpenTutorial = OpenTutorial;
        GameData.IsUseFpsLimiter = IsUseFpsLimiter;

		Time.timeScale = 1;
        #if UNITY_EDITOR
        GameObject obj = GameObject.Find("FileManager");
        if (obj) {
            Destroy(obj);
            obj = null;
        }
        #else
        Application.runInBackground = false;
        #endif

        if (!GameData.IsLoaded)
            StartCoroutine(init());
    }
}
