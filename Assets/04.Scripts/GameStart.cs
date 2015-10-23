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

//		StageJoin(101);
		GameData.StageID = -1;
	}

//	public void StageJoin(int id)
//    {
//		if(StageTable.Ins.HasByID(id))
//        {
//			int[] stageHint = StageTable.Ins.GetByID(id).HintBit;
//			GameData.StageID = id;
//			
////			GameStart.Get.CourtMode = (ECourtMode)GameData.DStageData[id].CourtMode;
//
//			if(stageHint[0] == 0 && stageHint[1] == 0)
//				WinMode = EWinMode.None;
//			else if(stageHint[0] == 0 && stageHint[1] == 1)
//				WinMode = EWinMode.NoTimeScore;
//			else if(stageHint[0] == 0 && stageHint[1] == 2)
//				WinMode = EWinMode.NoTimeLostScore;
//			else if(stageHint[0] == 0 && stageHint[1] == 3)
//				WinMode = EWinMode.NoTimeScoreCompare;
//			else if(stageHint[0] == 1 && stageHint[1] == 0)
//				WinMode = EWinMode.TimeNoScore;
//			else if(stageHint[0] == 1 && stageHint[1] == 1)
//				WinMode = EWinMode.TimeScore;
//			else if(stageHint[0] == 1 && stageHint[1] == 2)
//				WinMode = EWinMode.TimeLostScore;
//			else if(stageHint[0] == 1 && stageHint[1] == 3)
//				WinMode = EWinMode.TimeScoreCompare;
//
//			GameWinTimeValue = StageTable.Ins.GetByID(id).Bit0Num;
//			GameWinValue = StageTable.Ins.GetByID(id).Bit1Num;
//		}
//	}
}
