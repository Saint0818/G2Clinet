using GameEnum;
using GamePlayEnum;
using UnityEngine;

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
	public bool IsAutoReplay = false;
	public bool IsShowPlayerInfo = false;
	public bool IsShowShootRate = false;
	public float AlleyoopPassTime = 0.3f;
	public float PlayerShineTime = 0.5f;
	public int PlayerShineCount = 3;
	public int FriendNumber = 3;
	public int GameWinValue = 13;
	public float CrossTimeX = 0.5f;
	public float CrossTimeZ = 0.8f;
	public EPlayerState SelectAniState = EPlayerState.Dunk6;
	public EBasketAnimationTest SelectBasketState = EBasketAnimationTest.Basket0;

	void Start() {
		Time.timeScale = 1;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		SceneMgr.Get.SetDontDestory (gameObject);

		switch(SceneMode) {
		case ESceneTest.Single:
			SceneMgr.Get.ChangeLevel (ESceneName.Court_0);
			break;
		case ESceneTest.Release:
			SendHttp.Get.CheckServerData(ConnectToServer);
			break;
		}

		TextConst.Init();
		GameData.Init();
		Application.runInBackground = IsDebugAnimation;
	}
}
