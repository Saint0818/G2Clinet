using UnityEngine;
using System;
using System.Collections;
using GameStruct;
using GameEnum;
using GamePlayEnum;

public class GameStart : KnightSingleton<GameStart> {
	public ESceneTest  SceneMode = ESceneTest.Single;
	public EGameTest TestMode = EGameTest.None;
	public EModelTest TestModel = EModelTest.None;
	public ECameraTest TestCameraMode = ECameraTest.None;
	public ECourtMode CourtMode = ECourtMode.Full;
	public EWinMode WinMode = EWinMode.Score;
	public bool ConnectToServer = false;
	public bool OpenGameMode = false;
	public bool IsDebugAnimation = false;
	public bool IsShowPlayerInfo = false;
	public int FriendNumber = 3;
	public int GameWinValue = 13;
	public float CrossTimeX = 0.5f;
	public float CrossTimeZ = 0.8f;
	public float LayupBallSpeed = 0.4f;
	public EPlayerState SelectAniState = EPlayerState.Dunk6;
	public EBasketAnimationTest SelectBasketState = EBasketAnimationTest.Basket0;

	void Start() {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		SceneMgr.Get.SetDontDestory (gameObject);

		switch(SceneMode) {
		case ESceneTest.Single:
			SceneMgr.Get.ChangeLevel (SceneName.Court_0);
			break;
		case ESceneTest.Release:
			SendHttp.Get.CheckServerData(ConnectToServer);
			break;
		}

		TextConst.Init();
		GameData.Init();
	}
	
	void OnGUI() {
		if (SceneMode == ESceneTest.Multi) {
			if (GUI.Button (new Rect (100, 0, 100, 50), "Lobby"))
				SceneMgr.Get.ChangeLevel(SceneName.Lobby);
				
			if (GUI.Button (new Rect (200 , 0, 100, 50), "InGame"))
				SceneMgr.Get.ChangeLevel(SceneName.Court_0);
		}
	}
}
