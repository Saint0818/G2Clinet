using UnityEngine;

public class UIGameLoseResult : UIBase {
	private static UIGameLoseResult instance = null;
	private const string UIName = "UIGameLoseResult";

	private GameStageTargetLose[] mTargets;
	private const float finishInterval = 0.2f;
	private int hintIndex;
	private int hintCount;
	private bool isShowFinish = false;
	private float finishTime = 0;
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
					RemoveUI(UIName);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else
			if (isShow)
				Get.Show(isShow);
	}
	
	public static UIGameLoseResult Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGameLoseResult;
			
			return instance;
		}
	}

	void FixedUpdate () {
		//Show StageHint
		if(isShowFinish && hintIndex >= -1) {
			finishTime -= Time.deltaTime;
			if(finishTime <= 0) {
				if(hintIndex == -1) {
					isShowFinish = false;
				} else {
					if(hintIndex > 0 && hintIndex < mTargets.Length) {
						if(mTargets[hintCount - hintIndex].IsComplete)
							AudioMgr.Get.PlaySound(SoundType.SD_ResultCount);
						mTargets[hintCount - hintIndex].UpdateFin(mTargets[hintCount - hintIndex].IsComplete);
					}

					finishTime = finishInterval;
					hintIndex --;
				}
			}
		}
	}
	
	protected override void InitCom() {
		mTargets = GetComponentsInChildren<GameStageTargetLose>();

		UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/StatsNextLabel")).onClick = OnReturn;
	}

	public void Init () {
		hintCount = UIStageHintManager.UpdateHintLose(GameData.StageID, ref mTargets);
		hintIndex = hintCount;
		Invoke("showFinish", 3);
	}

	private void showFinish () {
		isShowFinish = true;
		finishTime = finishInterval;
	}

	public void OnReturn (GameObject go) {
		UIGame.Visible = false;
		UIGameResult.Visible = false;
		UIGameLoseResult.Visible = false;
		UIGamePause.Visible = false;
		UIDoubleClick.Visible = false;
		UIPassiveEffect.Visible = false;
		UITransition.Visible = false;
		UICourtInstant.Visible = false;
		UIInGameMission.Visible = false;


		Time.timeScale = 1;
		UIShow(false);
		UILoading.StageID = GameData.StageID;
		if(GameData.IsMainStage)
		{
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenStageUI;
		}
		else if(GameData.IsInstance)
		{
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenInstanceUI;
		}
		else if (GameData.IsPVP)
		{
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenStageUI;
		}
		else
		{
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenStageUI;
		}
	}
}
