using GamePlayEnum;
using GameStruct;
using UnityEngine;

public class UIGameResult : UIBase {
	private static UIGameResult instance = null;
	private const string UIName = "UIGameResult";

	private GameObject uiStatsNext;
	private GameObject uiAwardSkip;

	//Center/TopView
	private GameObject uiVictory;
	private GameObject uiDefeat;

	//Center/BottomView
	private GameObject viewStageTarget;
	private UIStageHint uiStageHint;
	
	private GameObject viewStat;
	private GameObject uiLeftBtn;
	private GameObject uiRightBtn;
	private GameObject teamStats;
	private PlayerStats playerStats;
	private TeamValue teamValue;
	private PlayerValue[] playerValue = new PlayerValue[6];
	private int statsPage = 1;

	private GameObject viewAward;

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static UIGameResult Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGameResult;
			
			return instance;
		}
	} 

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else
		if (isShow)
			Get.Show(isShow);
	}
	
	protected override void InitCom() {
		uiStatsNext = GameObject.Find(UIName + "/BottomRight/StatsNextLabel");
		uiAwardSkip = GameObject.Find(UIName + "/BottomRight/AwardSkipLabel");
		
		//Center/TopView
		uiVictory = GameObject.Find(UIName + "/Center/TopView/Victory");
//		uiDefeat;
		
		//Center/BottomView
		viewStageTarget = GameObject.Find(UIName + "/Center/BottomView/StageTargetView");
		uiStageHint = Instantiate(Resources.Load<GameObject>(UIPrefabPath.UIStageHint)).GetComponent<UIStageHint>();
		uiStageHint.transform.parent = GameObject.Find(UIName + "/Center/BottomView/StageTargetView/View").transform;
		uiStageHint.transform.localPosition = Vector3.zero;
		uiStageHint.transform.localRotation = Quaternion.identity;
		uiStageHint.transform.localScale = Vector3.one;
		uiStageHint.SetInterval(150, 150);
		uiStageHint.ResultInit();
		
		viewStat = GameObject.Find(UIName + "/Center/BottomView/StatsView");
		uiLeftBtn = GameObject.Find (UIName + "/Center/BottomView/StatsView/LeftBtn");
		uiRightBtn = GameObject.Find (UIName + "/Center/BottomView/StatsView/RightBtn");
		teamStats = GameObject.Find (UIName + "/Center/BottomView/StatsView/TeamStats");
		playerStats = GetComponentInChildren<PlayerStats>();
		playerValue = GetComponentsInChildren<PlayerValue>();
		teamValue = GetComponentInChildren<TeamValue>();
		viewAward = GameObject.Find (UIName + "/Center/BottomView/AwardsView");

		UIEventListener.Get (uiStatsNext).onClick = OnNext;
		UIEventListener.Get (uiAwardSkip).onClick = OnReturn;
//		SetBtnFun(UIName + "/BottomRight/StatsNextLabel", OnNext);
//		SetBtnFun(UIName + "/BottomRight/AwardSkipLabel", OnReturn);
		SetBtnFun(UIName + "/Center/BottomView/StatsView/LeftBtn", OnShowAwayStats);
		SetBtnFun(UIName + "/Center/BottomView/StatsView/RightBtn", OnShowHomeStats);
	}
	
	protected override void InitData() {

	}
	
	protected override void OnShow(bool isShow) {

	}

	private void showStats () {
		uiStageHint.Hide();
		viewStageTarget.SetActive(false);
		viewStat.SetActive(true);
		uiStatsNext.SetActive(true);
		showTeamStats ();
	}

	private void showTeamStats () {
		uiRightBtn.SetActive(true);
		uiLeftBtn.SetActive(true);
		teamStats.SetActive(true);
		playerStats.Hide();
	}

	public void OnShowHomeStats () {
		if(statsPage == 0) {
			statsPage = 1;
			showTeamStats();
		} else if(statsPage == 1) {
			statsPage = 2;
			uiRightBtn.SetActive(false);
			teamStats.SetActive(false);
			playerStats.ShowSelf();
		}
	}

	public void OnShowAwayStats () {
		if(statsPage == 2) {
			statsPage = 1;
			showTeamStats();
		} else if(statsPage == 1) {
			statsPage = 0;
			uiLeftBtn.SetActive(false);
			teamStats.SetActive(false);
			playerStats.ShowNPC();
		}
	}

	public void OnNext (GameObject go) {
		viewStat.SetActive(false);
		viewAward.SetActive(true);
		uiStatsNext.SetActive(false);
		uiAwardSkip.SetActive(true);
	}

	public void OnReturn(GameObject go) {
		Time.timeScale = 1;
		UIShow(false);
		if (isStage)
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
		else
			SceneMgr.Get.ChangeLevel (ESceneName.SelectRole, false);
	}

	public void SetGameRecord(ref TGameRecord record, bool isVictory) {
		uiVictory.SetActive(false);
		//		uiDefeat
		teamValue.SetValue(record);
		if(record.Done) {
			for (int i=0; i<GameController.Get.GamePlayers.Count; i++) {
				playerStats.SetPlayerName(i, GameController.Get.GamePlayers[i].Attribute.Name);
				playerValue[i].SetValue(GameController.Get.GamePlayers[i].GameRecord);
			}
		}
		viewStageTarget.SetActive(true);
		uiStatsNext.SetActive(false);
		uiAwardSkip.SetActive(false);
		viewStat.SetActive(false);
		viewAward.SetActive(false);

		if (isVictory)
			uiVictory.SetActive(true);

		if(isStage)
			uiStageHint.UpdateResult(GameData.StageID);
		uiStageHint.ResultInit();

		Invoke("showStats", 5);
	}


	public bool isStage
    {
//		get {return GameData.DStageData.ContainsKey(GameData.StageID); }
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
