using GamePlayEnum;
using GameStruct;
using UnityEngine;

public class UIGameResult : UIBase {
	private static UIGameResult instance = null;
	private const string UIName = "UIGameResult";

	private GameObject uiStatsNext;
	private GameObject uiAwardSkip;

	private Animator animatorAward;//AwardViewStart, AwardViewDown
	private Animator animatorBottomView; // Down, HomePlayer, AwayPlayer, TeamStats
	private PlayerStats playerStats;
	private TeamValue teamValue;
	private PlayerValue[] playerValue = new PlayerValue[6];
	private int statsPage = 1;

	private UIStageHintTarget[] mTargets;
	private const float finishInterval = 0.2f;
	//StageHint
	private int hintIndex;
	private int hintCount;
	private bool isShowFinish = false;
	private float finishTime = 0;

	//AwardItems
	private int awardIndex;
	private int awardCount;
	private bool isShowAward = false;
	private float awardGetTime = 0;

	private bool isChooseLucky = false;

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

	void FixedUpdate () {
		if(isShowFinish && hintIndex >= -1) {
			finishTime -= Time.deltaTime;
			if(finishTime <= 0) {
				if(hintIndex == -1) {
					isShowFinish = false;
					animatorBottomView.SetTrigger("Down");
					uiStatsNext.SetActive(true);
				} else {
					if(hintIndex > 0 && hintIndex < mTargets.Length)
						mTargets[hintCount - hintIndex].UpdateFin(true);

					finishTime = finishInterval;
					hintIndex --;
				}
			}
		}

		if(isShowAward && awardIndex >= -1) {
			awardGetTime -= Time.deltaTime;
			if(awardGetTime <= 0) {
				if(awardIndex == -1) {
					isShowAward = false;
					showLuckyThree ();
				} else {
					//				if(awardIndex > 0)
					
					awardIndex --;
				}
			}
		}
	}
	
	protected override void InitCom() {
		uiStatsNext = GameObject.Find(UIName + "/BottomRight/StatsNextLabel");
		uiAwardSkip = GameObject.Find(UIName + "/BottomRight/AwardSkipLabel");
		
		//Center/BottomView
		mTargets = GetComponentsInChildren<UIStageHintTarget>();

		animatorAward = gameObject.GetComponent<Animator>();
		animatorBottomView = GameObject.Find (UIName + "/Center/BottomView").GetComponent<Animator>();
		playerStats = GetComponentInChildren<PlayerStats>();
		playerValue = GetComponentsInChildren<PlayerValue>();
		teamValue = GetComponentInChildren<TeamValue>();

		UIEventListener.Get (uiStatsNext).onClick = OnNext;
		UIEventListener.Get (uiAwardSkip).onClick = OnReturn;
		SetBtnFun(UIName + "/Center/BottomView/StatsView/LeftBtn", OnShowAwayStats);
		SetBtnFun(UIName + "/Center/BottomView/StatsView/RightBtn", OnShowHomeStats);
	}
	
	protected override void InitData() {

	}
	
	protected override void OnShow(bool isShow) {

	}

	private void showTeamStats () {
		animatorBottomView.SetTrigger("TeamStats");
	}

	public void OnShowHomeStats () {
		if(statsPage == 0) {
			statsPage = 1;
			showTeamStats();
		} else if(statsPage == 1) {
			statsPage = 2;
			animatorBottomView.SetTrigger("HomePlayer");
		}
	}

	public void OnShowAwayStats () {
		if(statsPage == 2) {
			statsPage = 1;
			showTeamStats();
		} else if(statsPage == 1) {
			statsPage = 0;
			animatorBottomView.SetTrigger("AwayPlayer");
		}
	}

	public void OnNext (GameObject go) {
		uiStatsNext.SetActive(false);
		animatorAward.SetTrigger("AwardViewStart");
		Invoke("showAward", 1);
	}

	public void OnReturn(GameObject go) {
		if(isChooseLucky) {
			Time.timeScale = 1;
			UIShow(false);
			if (isStage)
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby, true , true);
			else
				SceneMgr.Get.ChangeLevel (ESceneName.SelectRole, false);
		}
	}
	
	private void showAward () {
		if(awardIndex == 0) {
			showLuckyThree ();
		} else {
			isShowAward = true;
			awardGetTime = finishInterval;
		}
	}
	
	public void showLuckyThree () {
		animatorAward.SetTrigger ("AwardViewDown");
		Invoke("Show3DBasket", 1);
	}

	public void Show3DBasket () {
		UI3DGameResult.UIShow(true);
	}
	
	public void ChooseLucky(int index) {
		Invoke ("ShowReturnButton", 2);
		isChooseLucky = true;
		if(index == 0) {

		} else if(index == 1) {

		} else if(index == 2) {

		}
	}

	public void ShowReturnButton () {
		uiAwardSkip.SetActive(true);
	}

	public void SetGameRecord(ref TGameRecord record) {
		teamValue.SetValue(record);
		if(record.Done) {
			for (int i=0; i<GameController.Get.GamePlayers.Count; i++) {
				playerStats.SetPlayerName(i, GameController.Get.GamePlayers[i].Attribute.Name);
				playerValue[i].SetValue(GameController.Get.GamePlayers[i].GameRecord);
			}
		}
		uiStatsNext.SetActive(false);
		uiAwardSkip.SetActive(false);
		updateResult(GameData.StageID);

		Invoke("showFinish", 5);
	}

	private void updateResult(int stageID)
	{
		if(StageTable.Ins.HasByID(stageID))
		{
			hideAllTargets();
			
			TStageData stageData = StageTable.Ins.GetByID(stageID);
			int[] hintBits = stageData.HintBit;
			hintIndex = 0;

			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);
			
			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				mTargets[hintIndex].UpdateUI(getText(1, value, 9),
				                         getText(1, value, 8),
				                         (minute * 60 + second).ToString(), "/" + stageData.Bit0Num.ToString(),
				                         false);
				hintIndex ++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 1)
			{
				mTargets[hintIndex].Show();
				mTargets[hintIndex].UpdateUI(getText(2, hintBits[1] - 1, 9),
				                         getText(2, hintBits[1] - 1, 8),
				                         UIGame.Get.Scores[ETeamKind.Self.GetHashCode()].ToString(), "/" + stageData.Bit1Num.ToString(),
				                         false);
				hintIndex++;
			}
			
			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				mTargets[hintIndex].Show();
				mTargets[hintIndex].UpdateUI(getText(3, hintBits[2], 9),
				                         getText(3, hintBits[2], 8),
				                         getConditionCount(hintBits[2]).ToString(), "/" + stageData.Bit2Num.ToString(),
				                         false);
				hintIndex++;
			}
			
			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				mTargets[hintIndex].Show();
				mTargets[hintIndex].UpdateUI(getText(3, hintBits[3], 9),
				                         getText(3, hintBits[3], 8),
				                         getConditionCount(hintBits[3]).ToString(), "/" + stageData.Bit3Num.ToString(),
				                         false);
			}
		} else 
		{
			int[] hintBits = GameController.Get.StageData.HintBit;
			hintIndex = 0;
			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				
				mTargets[hintIndex].UpdateUI(getText(1, value, 9),
				                         getText(1, value, 8),
				                         (Mathf.RoundToInt(GameController.Get.GameTime)).ToString(), "/" + GameController.Get.StageData.BitNum[0].ToString(),
				                         false);
				hintIndex++;
			}
			
			if(hintBits.Length > 1 && hintBits[1] > 1)
			{
				mTargets[hintIndex].Show();
				mTargets[hintIndex].UpdateUI(getText(2, hintBits[1] - 1, 9),
				                         getText(2, hintBits[1] - 1, 8),
				                         UIGame.Get.Scores[ETeamKind.Self.GetHashCode()].ToString(), "/" + GameController.Get.StageData.BitNum[1].ToString(),
				                         false);
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

	private void showFinish () {
		isShowFinish = true;
		finishTime = finishInterval;
	}

	public bool isStage
    {
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
