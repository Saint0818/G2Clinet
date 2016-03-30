//#define PUSHWOOSH
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
        DontDestroyOnLoad(gameObject);
		Time.timeScale = 1;

        #if UNITY_EDITOR
        Application.runInBackground = IsDebugAnimation;
        GameObject obj = GameObject.Find("FileManager");
        if (obj) {
            Destroy(obj);
            obj = null;
        }
        #else
        Application.runInBackground = false;
        #endif

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        GameData.InitGameSetting ();
        GameData.Init();
        AnimatorMgr.Get.InitAnimtorStatesType();
        UILoading.UIShow(true, ELoading.Login);
        SendHttp.Get.CheckServerData(ConnectToServer);

        if (IsUseFpsLimiter) {
            FpsLimiter fl = gameObject.GetComponent<FpsLimiter>();
            if (!fl)
                gameObject.AddComponent<FpsLimiter>();
        }

        #if PUSHWOOSH
        #if UNITY_ANDROID
        gameObject.AddComponent<Pushwoosh>();
        #endif
        #endif
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
