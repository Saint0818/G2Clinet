using GameEnum;
using GamePlayEnum;
using UnityEngine;

public class GameStart : KnightSingleton<GameStart> {
	public ESceneTest  SceneMode = ESceneTest.Single;
	public EGameTest TestMode = EGameTest.None;
	public EModelTest TestModel = EModelTest.None;
	public ECameraTest TestCameraMode = ECameraTest.None;
	public ECourtMode CourtMode = ECourtMode.Full;
	public EWinMode WinMode = EWinMode.NoTimeScore;
	public bool ConnectToServer = false;
	public bool OpenTutorial = false;
	public bool IsDebugAnimation = false;
	public bool IsAutoReplay = false;
	public bool IsShowPlayerInfo = false;
	public bool IsShowShootRate = false;
	public int FriendNumber = 3;
	public int GameWinValue = 13;
	public int GameWinTimeValue = 0;
	public EPlayerState SelectAniState = EPlayerState.Dunk6;
	public EBasketAnimationTest SelectBasketState = EBasketAnimationTest.Basket0;
	public ETestActive TestID = ETestActive.Dunk20;
	public int TestLv = 2;

	void Start() {
		#if UNITY_EDITOR
		Application.runInBackground = IsDebugAnimation;
		#else
		Application.runInBackground = false;
		#endif

		Time.timeScale = 1;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		SceneMgr.Get.SetDontDestory (gameObject);

		TextConst.Init();
		GameData.Init();

		switch(SceneMode) {
		case ESceneTest.Single:
			SceneMgr.Get.ChangeLevel (ESceneName.Court_0);
			break;
		case ESceneTest.Release:
			SendHttp.Get.CheckServerData(ConnectToServer);
			break;
		}
	}
}
