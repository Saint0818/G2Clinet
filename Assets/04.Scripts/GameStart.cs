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
//	public bool OpenGameMode = false;
	public bool IsDebugAnimation = false;
	public bool IsAutoReplay = false;
	public bool IsShowPlayerInfo = false;
	public bool IsShowShootRate = false;
	public float AlleyoopPassTime = 0.3f;
	public float PlayerShineTime = 0.5f;
	public int PlayerShineCount = 3;
	public int FriendNumber = 3;
	public int GameWinValue = 13;
	public int GameWinTimeValue = 0;
	public float CrossTimeX = 0.5f;
	public float CrossTimeZ = 0.8f;
	public EPlayerState SelectAniState = EPlayerState.Dunk6;
	public EBasketAnimationTest SelectBasketState = EBasketAnimationTest.Basket0;
	[HideInInspector]
	public int[] StageHint;

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
		#if UNITY_EDITOR
		Application.runInBackground = IsDebugAnimation;
		#else
		Application.runInBackground = false;
		#endif

		StageJoin(1);
		GameData.StageID = -1;
	}

	public void StageJoin (int id) {
		if(GameData.DStageData.ContainsKey(id)) {
//			StageHint = AI.BitConverter.Convert(GameData.DStageData[id].Hint);
			StageHint = GameData.DStageData[id].HintBit;
			GameData.StageID = id;
			
//			GameStart.Get.CourtMode =  (ECourtMode)GameData.DStageData[id].CourtMode;

			if(StageHint[0] == 0 && StageHint[1] == 0)
				WinMode = EWinMode.None;
			else if(StageHint[0] == 0 && StageHint[1] == 1)
				WinMode = EWinMode.NoTimeScore;
			else if(StageHint[0] == 0 && StageHint[1] == 2)
				WinMode = EWinMode.NoTimeLostScore;
			else if(StageHint[0] == 0 && StageHint[1] == 3)
				WinMode = EWinMode.NoTimeScoreCompare;
			else if(StageHint[0] == 1 && StageHint[1] == 0)
				WinMode = EWinMode.TimeNoScore;
			else if(StageHint[0] == 1 && StageHint[1] == 1)
				WinMode = EWinMode.TimeScore;
			else if(StageHint[0] == 1 && StageHint[1] == 2)
				WinMode = EWinMode.TimeLostScore;
			else if(StageHint[0] == 1 && StageHint[1] == 3)
				WinMode = EWinMode.TimeScoreCompare;


			GameWinTimeValue = GameData.DStageData[id].Bit0Num;
			GameWinValue =  GameData.DStageData[id].Bit1Num;
//			GameStart.Get.FriendNumber =  GameData.DStageData[id].FriendNumber;
		}

	}
}
