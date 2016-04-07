﻿//#define PUSHWOOSH
#define APPSFLYER
using UnityEngine;
using System;
using System.Collections;
using Newtonsoft.Json;
using GameStruct;
using GameEnum;

public class LobbyStart : MonoBehaviour {
    private static LobbyStart instance;

    public EGameTest TestMode = EGameTest.None;
    public EModelTest TestModel = EModelTest.None;
    public ECameraTest TestCameraMode = ECameraTest.None;
    public int CourtMode = ECourtMode.Full;
    public bool ConnectToServer = false;
    public bool OpenTutorial = false;
    public bool IsDebugAnimation = false;
    public bool IsAutoReplay = false;
    public bool IsShowShootRate = false;
    public bool IsShowPlayerInfo = false;
    public bool IsUseFpsLimiter = true;
    public bool IsOpenColorfulFX = true;
    public int FriendNumber = 3;
    public int GameWinValue = 13;
    public int GameWinTimeValue = 0;
    public EPlayerState SelectAniState = EPlayerState.Dunk6;
    public EBasketAnimationTest SelectBasketState = EBasketAnimationTest.BasketballAction_0;
    public ETestActive TestID = ETestActive.Dunk20;
    public int TestLv = 2;
    public float GameSpeed = 1f;

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
        Application.runInBackground = IsDebugAnimation;
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
            SendHttp.Get.CheckServerData(ConnectToServer);
            if (IsUseFpsLimiter) {
                FpsLimiter fl = gameObject.GetComponent<FpsLimiter>();
                if (!fl)
                    SendHttp.Get.gameObject.AddComponent<FpsLimiter>();
            }

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
