using UnityEngine;
using System.Collections;

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
					if(hintIndex > 0 && hintIndex < mTargets.Length)
						mTargets[hintCount - hintIndex].UpdateFin(mTargets[hintCount - hintIndex].IsComplete);

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
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {

	}

	public void Init () {
		updateResult(GameData.StageID);
		Invoke("showFinish", 3);
	}

	private void showFinish () {
		isShowFinish = true;
		finishTime = finishInterval;
	}

	public void OnReturn (GameObject go) {
		Time.timeScale = 1;
		UIShow(false);
		if (GameController.Visible && GameController.Get.StageData.IsTutorial) {
			if (StageTable.Ins.HasByID(GameController.Get.StageData.ID + 1)) {
				GameData.StageID = GameController.Get.StageData.ID + 1;
				int courtNo = StageTable.Ins.GetByID(GameData.StageID).CourtNo;
				SceneMgr.Get.CurrentScene = "";
				SceneMgr.Get.ChangeLevel (courtNo);
			} else {
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			}
		} else {
			UILoading.OpenUI = UILoading.OpenStageUI;
			if (isStage)
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			else
				SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
		}
	}

	private void updateResult(int stageID)
	{
		if(StageTable.Ins.HasByID(stageID))
		{
			hideAllTargets();
			
			TStageData stageData = StageTable.Ins.GetByID(stageID);
			int[] hintBits = stageData.HintBit;
			hintIndex = 0;
			bool isFin = false;
			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);
			
			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				mTargets[hintIndex].UpdateUI(getText(1, value, 9),
				                             getText(1, value, 7),
				                             (minute * 60 + second).ToString(), "/" + stageData.Bit0Num.ToString(),
				                             true,
				                             false);
				hintIndex ++;
			}
			
			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[hintIndex].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];

				isFin = (UIGame.Get.Scores[(int) ETeamKind.Self] > UIGame.Get.Scores[(int) ETeamKind.Npc]);
				if(hintBits[1] == 2){
					isFin = (score >= stageData.Bit1Num);
				}else if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
					isFin = (score <= stageData.Bit1Num);
				} else if(hintBits[1] == 4) {
					score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
					isFin = (score >= stageData.Bit1Num);
				}
				mTargets[hintIndex].UpdateUI(getText(2, hintBits[1], 9),
				                             getText(2, hintBits[1], 7),
				                             score.ToString(), "/" + stageData.Bit1Num.ToString(),
				                             isFin,
				                             false);

				hintIndex++;
			}
			
			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				mTargets[hintIndex].Show();
				isFin = (getConditionCount(hintBits[2]) >=  stageData.Bit2Num);
				mTargets[hintIndex].UpdateUI(getText(3, hintBits[2], 9),
				                             getText(3, hintBits[2], 7),
				                             getConditionCount(hintBits[2]).ToString(), "/" + stageData.Bit2Num.ToString(),
				                             isFin,
				                             false);
				hintIndex++;
			}
			
			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				mTargets[hintIndex].Show();
				isFin = (getConditionCount(hintBits[3]) >=  stageData.Bit3Num);
				mTargets[hintIndex].UpdateUI(getText(3, hintBits[3], 9),
				                             getText(3, hintBits[3], 7),
				                             getConditionCount(hintBits[3]).ToString(), "/" + stageData.Bit3Num.ToString(),
				                             isFin,
				                             false);
				hintIndex++;
			}
		} else 
		{
			int[] hintBits = GameController.Get.StageData.HintBit;
			hintIndex = 0;
			bool isFin = false;
			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				
				mTargets[hintIndex].UpdateUI(getText(1, value, 9),
				                             getText(1, value, 7),
				                             (Mathf.RoundToInt(GameController.Get.GameTime)).ToString(), "/" + GameController.Get.StageData.BitNum[0].ToString(),
				                             true,
				                             false);
				hintIndex++;
			}
			
			if(hintBits.Length > 1 && hintBits[1] > 1)
			{
				mTargets[hintIndex].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];
				isFin = (score >= GameController.Get.StageData.Bit1Num);
				if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
					isFin = (score <= GameController.Get.StageData.Bit1Num);
				} else {
					if(hintBits[1] == 4)
						score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
					isFin = (score >= GameController.Get.StageData.Bit1Num);
				}
				mTargets[hintIndex].UpdateUI(getText(2, hintBits[1], 9),
				                             getText(2, hintBits[1], 7),
				                             UIGame.Get.Scores[ETeamKind.Self.GetHashCode()].ToString(), "/" + GameController.Get.StageData.BitNum[1].ToString(),
				                             isFin,
				                             false);
				hintIndex ++;
			}
		}
		
		hintCount = hintIndex;
	}

	private void hideAllTargets()
	{
		for(int i = 0; i < mTargets.Length; i++)
		{
			mTargets[i].Hide();
		}
	}

	private int getConditionCount(int type) {
		return GameController.Get.GetSelfTeamCondition(type);
	}
	
	private string getText(int index, int value, int id)
	{
		int baseValue = 2000000 + (int)(Mathf.Pow(10,index) * value) + id;
		return TextConst.S(baseValue);
	}

	public bool isStage
	{
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
