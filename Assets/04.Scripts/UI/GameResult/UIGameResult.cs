using GameStruct;
using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using GameEnum;

public struct TGameResultCenter {
	public Animator AnimatorBottomView;// Down, HomePlayer, AwayPlayer, TeamStats
	public PlayerStats PlayerStats;
	public TeamValue TeamValue;
	public PlayerValue[] PlayerValue;
	private int StatsPage;
	public UILabel Away;
	public UILabel Home;

	public void Init () {
		PlayerValue = new PlayerValue[6];
		StatsPage = 1;
	}

	public void PlayAnimation(string name) {
		AnimatorBottomView.SetTrigger(name);
	}

	public void SetTeamValue (TGameRecord record) {
		TeamValue.SetValue(record);
	}

	public void SetPlayerStat(int index, PlayerBehaviour player) {
		PlayerStats.SetID(index, player.Attribute.Identifier);
		PlayerStats.SetPlayerName(index, player.Attribute.Name);
		PlayerStats.SetPlayerIcon(index, player.Attribute.FacePicture);
		PlayerStats.SetPositionIcon(index, player.Attribute.BodyType);
		PlayerStats.ShowAddFriendBtn(index);
		if(index >= 0 && index < PlayerValue.Length)
			PlayerValue[index].SetValue(player.GameRecord);
	}

	public void ShowHomeStats (EventDelegate.Callback callback) {
		if(StatsPage == 0) {
			StatsPage = 1;
			callback ();
		} else if(StatsPage == 1) {
			StatsPage = 2;
			PlayAnimation("HomePlayer");
		}
	}

	public void ShowAwayStats (EventDelegate.Callback callback) {
		if(StatsPage == 2) {
			StatsPage = 1;
			callback ();
		} else if(StatsPage == 1) {
			StatsPage = 0;
			PlayAnimation("AwayPlayer");
		}
	}

	public void SetScore (int home, int away) {
		Away.text = away.ToString();
		Home.text = home.ToString();
	}
}

public struct TGameResultAwardParam {
	public int AwardIndex;
	public int AwardMax;
	public bool IsShowAward;
	public float AwardGetTime;
	public float AwardGetTimeInterval;
	public int AlreadGetBonusID;
	public List<int> AwardItemTempIDs; // -1:Money -2:Exp -3:Diamond -4:PVPCoin
	public List<ItemAwardGroup> AlreadyGetItems;
	public int[] AwardItemIDs;
	public int[] BonusItemIDs;

	public int TempMoney;
	public int TempExp;
	public int TempDia;
	public int TempPVP;

	public void Init () {
		AwardItemTempIDs = new List<int>();
		AlreadyGetItems =  new List<ItemAwardGroup>();
		IsShowAward = false;
		AwardGetTimeInterval = 0.25f;
	}

	public void DestroyObj () {
		AwardItemTempIDs.Clear();
		AlreadyGetItems.Clear();
	}

	public void SetAwardValue (TStageReward reward) {
		if(reward.SurelyItemIDs == null) {
			AwardItemIDs = new int[0];
		}else
			AwardItemIDs = reward.SurelyItemIDs;

		for (int i=0; i<AwardItemIDs.Length; i++) {
			AwardItemTempIDs.Add(AwardItemIDs[i]);
		}

		if(reward.AddMoney > 0) {
			AwardItemTempIDs.Add(-1);
			TempMoney = reward.AddMoney;
		}
		if(reward.AddExp > 0) {
			AwardItemTempIDs.Add(-2);
			TempExp = reward.AddExp;
		}
		if(reward.AddDiamond > 0) {
			AwardItemTempIDs.Add(-3);
			TempDia = reward.AddDiamond;
		}

		// 玩家可能得到的亂數獎勵.
		BonusItemIDs = reward.CandidateItemIDs;
		AlreadGetBonusID = reward.RandomItemID;
	}

	public void SetPVPAward (ItemAwardGroup item, int pvpCoin) {
		AwardIndex = 1;
		AwardMax = 1;
		AwardItemTempIDs.Add(-4);
		AlreadyGetItems.Add(item);
		TempPVP = pvpCoin;
	}

	public bool CanShowAward {
		get {return (IsShowAward && AwardIndex >= -1);}
	}

	public int ExtraAward {
		get {return AwardMax - AwardIndex;}
	}

	public void ShowAward () {
		IsShowAward = true;
		AwardGetTime = AwardGetTimeInterval;
	}

	public void CostAward () {
		AwardGetTime = AwardGetTimeInterval;
		AwardIndex --;
	}

	public int AwardCount {get{return AlreadyGetItems.Count;}}

	public bool IsAwardExtra {get{return (AwardIndex > 0);} }
	public bool IsAwardEmpty {get{return (AwardIndex == 0);} }
	public bool IsHaveBonus{get {return (BonusItemIDs != null && BonusItemIDs.Length > 0);}}
}

public struct TGameResultExpView {
	public UISlider ProgressBar;
	public UILabel ExpLabel;
	public GameObject LvUpFx;

	private float lvHideTime;

	private bool isShowExp;
	private int totalExp;
	private int nowExp;
	private int maxExp;
	private int nextMaxExp;
	private bool isRunFin;

	private EventDelegate.Callback runFinish;

	public void Init (EventDelegate.Callback expFinish) {
		LvUpFx.SetActive(false);
		isShowExp = false;
		runFinish = expFinish;
		isRunFin = false;
	}

	public void SetValue (int now, int max, int nextMax, int total) {
		nowExp = now;
		maxExp = max;
		nextMaxExp = nextMax;
		totalExp = total;
		ProgressBar.value = (float)nowExp / (float)maxExp;
		ExpLabel.text = string.Format("{0} / {1}", nowExp, maxExp);
	}

	public void UpdateUI () {
		if(isShowExp) {
			if(totalExp > 0) {
				if(nowExp < maxExp) {
					totalExp -- ;
					nowExp ++;
					ProgressBar.value = (float)nowExp / (float)maxExp;
					ExpLabel.text = string.Format("{0} / {1}", nowExp, maxExp);
				} else {
					AddLevelUpValue();
				}
			} else {
				if(runFinish != null)
					runFinish();
				
				isShowExp = false;
				isRunFin = true;
			}
		}

		if(lvHideTime > 0) {
			lvHideTime -= Time.deltaTime;
			if(lvHideTime <= 0)
				LvUpFx.SetActive(false);
		}
	}

	public void AddLevelUpValue () {
		nowExp = 0;
		maxExp = nextMaxExp;
		LvUpFx.SetActive(true);
		lvHideTime = 0.5f;
	}

	public bool IsRunFin {
		get {return isRunFin;}
	}

	public bool IsShowExp {
		set {
			isShowExp = value;
		}
	}
}

public struct TGameResultThreeAward {
	public ItemAwardGroup[] ItemAwardGroup;
	public UILabel[] DiamondPay;
	public GameObject ClickTip;
	private bool isChooseLucky;
	public int ChooseIndex;
	public int ChooseCount;

	public void Init () {
		ItemAwardGroup = new ItemAwardGroup[3];
		DiamondPay = new UILabel[3];
	}

	public void SetThreeVisible (bool isShow) {
		for (int i=0; i<ItemAwardGroup.Length; i++) 
			ItemAwardGroup[i].gameObject.SetActive(isShow);
	}

	public void ShowItem (int index) {
		ChooseIndex = index;
		ItemAwardGroup[index].gameObject.SetActive(true);
		if(ChooseCount == 0) {
			showPayDiamond(index);
			setPayDiamond(GameConst.Stage3Pick1Diamond2);
		} else if (ChooseCount == 1) {
			DiamondPay[index].gameObject.SetActive(false);
			setPayDiamond(GameConst.Stage3Pick1Diamond3);
		}else {
			DiamondPay[index].gameObject.SetActive(false);
		}
		ChooseCount ++ ;
	}

	public void MoveItem (int count, EventDelegate.Callback moveFin) {
		if(ChooseIndex >= 0 && ChooseIndex < ItemAwardGroup.Length)
			ItemAwardGroup[ChooseIndex].transform.DOLocalMove(new Vector3(Mathf.Min(-300 + (count * 100), 400), -170, 0), 0.2f).OnComplete(MoveFin);
	}

	public void MoveFin () {
		if(ChooseIndex >= 0 && ChooseIndex < ItemAwardGroup.Length)
			ItemAwardGroup[ChooseIndex].Hide();
	}

	private void showPayDiamond (int index) {
		for(int i=0; i<DiamondPay.Length; i++) {
			if(i != index)
				DiamondPay[i].gameObject.SetActive(true);
		}
	}
		
	private void setPayDiamond (int value) {
		for(int i=0; i<DiamondPay.Length; i++) 
			DiamondPay[i].text = value.ToString();
	}

	public bool IsChooseLucky {
		get {return isChooseLucky;}
		set {isChooseLucky = value;}
	}

	public bool ClickTipVisible {
		get {return ClickTip.activeSelf;}
		set {ClickTip.SetActive(value);}
	}

	public int Max { get {return 3;}}
}

public class UIGameResult : UIBase {
	private static UIGameResult instance = null;
	private const string UIName = "UIGameResult";
	//Resourced
	private GameObject uiItem;
	private GameObject uiItemEffect;

	private Animator animatorAward;//AwardViewStart, AwardViewDown, EXPViewStart, PVPView
	//Center StageHint
	private TGameResultCenter resultCenter = new TGameResultCenter();
	private THintValue hintValue = new THintValue();
	//AwardView
	private GameObject awardScaleView;
	private UIScrollView awardScrollView;
	//AwardItems
	private TGameResultAwardParam resultAwardParam = new TGameResultAwardParam();
	//ExpView
	private TGameResultExpView resultExpView = new TGameResultExpView();
	//Three
	private TGameResultThreeAward resultThree = new TGameResultThreeAward();

//	private TSkill[] oldSkillCards;
	private Dictionary<int, int> newGotItems = new Dictionary<int, int>();
	private bool isGetAward = false;
	private bool isCanChooseLucky = false;
	private bool isLevelUp = false;
	private bool isShow3Dbasket = false;
	public List<int> GetCardLists = new List<int>();
	public bool IsShowFirstCard = true;

	//BottomRight
	private GameObject uiStatsNext;
	private GameObject uiAwardSkip;

	//RankView
	private TPVPObj pvpObj = new TPVPObj();
	private TPVPRank pvpRank = new TPVPRank();
	private TPVPValue pvpValue = new TPVPValue();

	private TPlayer beforePlayer;
	private TPlayer afterPlayer;
	private TPVPResult beforeTeam = new TPVPResult();
	private TPVPResult afterTeam = new TPVPResult();

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
                    RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
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
		if (isShow) {							
			Get.Show(isShow);
			UITutorial.UIShow(false);
		}
	}

	void OnDestroy () {
		resultAwardParam.DestroyObj();
	}

	void FixedUpdate () {
		//Show StageHint
		hintValue.UpdateUI(Time.deltaTime, HintAchieveOne, HintComplete);

		//Show Award
		if(resultAwardParam.CanShowAward) {
			resultAwardParam.AwardGetTime -= Time.deltaTime;
			if(resultAwardParam.AwardGetTime <= 0) {
				if(resultAwardParam.AwardIndex == -1) {
					resultAwardParam.IsShowAward = false;
					if(!GameData.IsPVP)
						ShowBonusItem ();
				} else {
					if(resultAwardParam.ExtraAward < resultAwardParam.AwardMax){
						if(resultAwardParam.AwardItemTempIDs[(resultAwardParam.ExtraAward)] > 0){
							if(GameData.DItemData.ContainsKey (resultAwardParam.AwardItemTempIDs[(resultAwardParam.ExtraAward)]))
								resultAwardParam.AlreadyGetItems[(resultAwardParam.ExtraAward)].Show(GameData.DItemData[resultAwardParam.AwardItemTempIDs[(resultAwardParam.ExtraAward)]]);
						}else if(resultAwardParam.AwardItemTempIDs[(resultAwardParam.ExtraAward)] == -1) 
							resultAwardParam.AlreadyGetItems[resultAwardParam.ExtraAward].ShowOther(1, resultAwardParam.TempMoney);
						else if(resultAwardParam.AwardItemTempIDs[(resultAwardParam.ExtraAward)] == -2) 
							resultAwardParam.AlreadyGetItems[resultAwardParam.ExtraAward].ShowOther(3, resultAwardParam.TempExp);
						else if(resultAwardParam.AwardItemTempIDs[(resultAwardParam.ExtraAward)] == -3)
							resultAwardParam.AlreadyGetItems[resultAwardParam.ExtraAward].ShowOther(2, resultAwardParam.TempDia);
						else if(resultAwardParam.AwardItemTempIDs[(resultAwardParam.ExtraAward)] == -4)
							resultAwardParam.AlreadyGetItems[resultAwardParam.ExtraAward].ShowOther(4, resultAwardParam.TempPVP);
					}
					resultAwardParam.CostAward () ;
				}
			}
		}

		if(pvpValue.IsRunRankExp) {
			if(pvpValue.NowValue <= 0 || pvpValue.NowValue >= pvpRank.BeforeLowScore && pvpValue.NowValue < pvpRank.BeforeHighScore) {
				pvpValue.MinusValue --;
				pvpValue.NowValue ++ ;
				pvpObj.SetValue(pvpValue, false);
			} else {
				pvpValue.IsShowRank = false;
				if(!UILevelUp.Visible)
					UILevelUp.Get.ShowRank(pvpRank.BeforeLv, pvpRank.AfterLv);
			}
		}

		resultExpView.UpdateUI();
	}

	public void HintAchieveOne () {
		if(hintValue.HintIndex >= 0 && hintValue.HintIndex < hintValue.WinTargets.Length) {
			hintValue.WinTargets[hintValue.ExtraHint].UpdateFin(true);
			AudioMgr.Get.PlaySound(SoundType.SD_ResultCount);
		}
	}

	public void HintComplete () {
		Invoke("finishStageHint", 1);
	}
	
	protected override void InitCom() {
		uiItem = Resources.Load(UIPrefabPath.ItemAwardGroup) as GameObject;
		uiItemEffect = Resources.Load(UIPrefabPath.UIFXAwardGetItem) as GameObject;
		uiStatsNext = GameObject.Find(UIName + "/BottomRight/StatsNextLabel");
		uiAwardSkip = GameObject.Find(UIName + "/BottomRight/AwardSkipLabel");
		animatorAward = gameObject.GetComponent<Animator>();

		hintValue.Init();
		hintValue.WinTargets = gameObject.GetComponentsInChildren<UIStageHintTarget>();

		resultCenter.Init();
		resultCenter.AnimatorBottomView = GameObject.Find (UIName + "/Center/BottomView").GetComponent<Animator>();
		resultCenter.PlayerStats = GetComponentInChildren<PlayerStats>();
		resultCenter.PlayerValue = GetComponentsInChildren<PlayerValue>();
		resultCenter.TeamValue = GetComponentInChildren<TeamValue>();
		resultCenter.Away = GameObject.Find (UIName + "/Center/TopView/ScoreBoard/Away").GetComponent<UILabel>();
		resultCenter.Home = GameObject.Find (UIName + "/Center/TopView/ScoreBoard/Home").GetComponent<UILabel>();

//		for (int i=0; i<resultCenter.PlayerStats.PlayerInGameBtn.Length; i++) {
//			resultCenter.PlayerStats.PlayerInGameBtn[i].name = i.ToString();
//			UIEventListener.Get (resultCenter.PlayerStats.PlayerInGameBtn[i]).onClick = OnShowPlayerInfo;
//		}
		awardScaleView = GameObject.Find(UIName + "/AwardsView/AwardsList/ScrollView/ScaleView");
		awardScrollView = GameObject.Find(UIName + "/AwardsView/AwardsList/ScrollView").GetComponent<UIScrollView>();
		resultAwardParam.Init();

		resultExpView.ProgressBar = GameObject.Find(UIName + "/EXPView/ProgressBar").GetComponent<UISlider>();
		resultExpView.ExpLabel = GameObject.Find(UIName + "/EXPView/ExpLabel").GetComponent<UILabel>();
		resultExpView.LvUpFx = GameObject.Find(UIName + "/EXPView/LvUpFX");
		resultExpView.Init(ExpRunFinish);

		resultThree.Init();
		for(int i=0; i<resultThree.ItemAwardGroup.Length; i++)
			resultThree.ItemAwardGroup[i] = GameObject.Find(UIName + "/ThreeAward/" + i.ToString()).GetComponent<ItemAwardGroup>();
		
		for(int i=0; i<resultThree.DiamondPay.Length; i++) {
			resultThree.DiamondPay[i] = GameObject.Find(UIName + "/ShowWords/"+i.ToString()+"/GemLabel").GetComponent<UILabel>();
			resultThree.DiamondPay[i].gameObject.SetActive(false);
		}
		resultThree.ClickTip = GameObject.Find(UIName + "/ThreeAward/ClickLabel");
		resultThree.ClickTip.SetActive(false);

		pvpObj.PVPRankIcon = GameObject.Find(UIName + "/RankView/PvPRankIcon").GetComponent<UISprite>();
		pvpObj.LabelRankName = GameObject.Find(UIName + "/RankView/PvPRankIcon/RankNameLabel").GetComponent<UILabel>();
		pvpObj.LabelMinusPoint = GameObject.Find(UIName + "/RankView/RankPoint/GetPointLabel").GetComponent<UILabel>();
		pvpObj.LabelNowPoint = GameObject.Find(UIName + "/RankView/NowPoint").GetComponent<UILabel>();
		pvpObj.SliderBar = GameObject.Find(UIName + "/RankView/ProgressBar").GetComponent<UISlider>();

		UIEventListener.Get (uiStatsNext).onClick = OnNext;
		UIEventListener.Get (uiAwardSkip).onClick = OnReturn;
		SetBtnFun(UIName + "/Center/BottomView/StatsView/LeftBtn", OnShowAwayStats);
		SetBtnFun(UIName + "/Center/BottomView/StatsView/RightBtn", OnShowHomeStats);
	}
	//此介面開啟的入口
	public void SetGameRecord(ref TGameRecord record) {
		UIShow(true);
		resultCenter.SetScore(record.Score1, record.Score2);
		resultCenter.SetTeamValue(record);
		if(record.Done) {
			for (int i=0; i<GameController.Get.GamePlayers.Count; i++) 
				resultCenter.SetPlayerStat(i, GameController.Get.GamePlayers[i]);

			resultCenter.PlayerStats.CheckFriend();
		}
		uiStatsNext.SetActive(false);
		uiAwardSkip.SetActive(false);

		if(GameData.IsPVP) {
			ShowMissionBoard ();
		}else
			stageRewardStart(GameData.StageID, record.Score1, record.Score2);
	}
	//Click EventSho
	public void OnShowAwardInfo (GameObject go) {
		int index = -1;
		if(int.TryParse(go.name, out index)) {
			if(GameData.DItemData.ContainsKey(index)) {
				if(GameData.DItemData[index].Kind == 21) {
					TSkill skill = new TSkill();
					skill.ID = GameData.DItemData[index].Avatar;
					UISkillInfo.Get.ShowFromNewCard(skill);
				} else
					UIItemHint.Get.OnShow(GameData.DItemData[index].ID);
			}
		}
	}

	public void OnShowHomeStats () {resultCenter.ShowHomeStats(showTeamStats);}
	public void OnShowAwayStats () {resultCenter.ShowAwayStats(showTeamStats);}

	public void OnNext (GameObject go) {
		uiStatsNext.gameObject.SetActive(false);
		if (GameController.Visible && GameData.IsPVP) {
			backToLobby();
		}
		else if (GameController.Visible && GameController.Get.StageData.IsTutorial) {
			if (StageTable.Ins.HasByID(GameController.Get.StageData.ID + 1)) {
				UIShow(false);
				GameData.StageID = GameController.Get.StageData.ID + 1;
				int courtNo = StageTable.Ins.GetByID(GameData.StageID).CourtNo;
				SceneMgr.Get.CurrentScene = "";
				SceneMgr.Get.ChangeLevel (courtNo);
			} else 
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
		} else {
			if(SendHttp.Get.CheckNetwork(false) && isGetAward) {
				if(resultAwardParam.IsAwardExtra) {
					animatorAward.SetTrigger("AwardViewStart");
					Invoke("showAward", 1);
				} else {
					if(resultExpView.IsRunFin && !isShow3Dbasket) {
						ExpRunFinishDelay();
					} else 
					if(isShow3Dbasket) {
						show3DBasket();
						isShow3Dbasket = false;
					}
				}
			} else  {
				if(isLevelUp) {
					UIShow(false);
					UI3DGameResult.UIShow(false);
					UILevelUp.Get.Show(beforePlayer, afterPlayer);
				} else 
					backToLobby ();
			}
		}
	}

	public void OnReturn(GameObject go) {
		if(GameData.IsPVP) 
			backToLobby();
		else {
			if(resultThree.IsChooseLucky) {
				if(isLevelUp) {
					UIShow(false);
					UI3DGameResult.UIShow(false);
					UILevelUp.Get.Show(beforePlayer, afterPlayer);
				} else 
					backToLobby ();
				
			}
		}
	}

	private void backToLobby()
    {
		Time.timeScale = 1;
        UILoading.StageID = GameData.StageID;
	    if(GameData.IsMainStage)
		{
			UIShow(false);
	        SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            UILoading.OpenUI = UILoading.OpenStageUI;
        }
        else if(GameData.IsInstance)
		{
			UIShow(false);
            SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            UILoading.OpenUI = UILoading.OpenInstanceUI;
        }
		else if (GameData.IsPVP)
		{
			if(!pvpValue.IsShowRank) {
				pvpNext();
			} else {
				UIShow(false);
				if(afterTeam.PVPLv != beforeTeam.PVPLv) {
					UILevelUp.Get.ShowRank(afterTeam.PVPLv, beforeTeam.PVPLv);
				} else {
					SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
					UILoading.OpenUI = UILoading.OpenPVPUI;
				}
			}
		}
        else
		{
			UIShow(false);
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
        }
	}

	public void ShowMissionBoard () {
		hintValue.SetHintCount(UIStageHintManager.UpdateHintResult(GameData.StageID, ref hintValue.WinTargets));
		Invoke("showFinish", GameConst.GameEndWait);
	}

	public void SetPVPData (TPVPResult before, TPVPResult after) {
		resultAwardParam.SetPVPAward(addItemToAward(0, new TItemData(), true, true), Mathf.Abs(after.PVPCoin - before.PVPCoin));

		before.PVPLv = GameFunction.GetPVPLv(before.PVPIntegral);
		after.PVPLv = GameFunction.GetPVPLv(after.PVPIntegral);
		beforeTeam = before;
		afterTeam = after;
		if(GameData.DPVPData.ContainsKey(before.PVPLv) && GameData.DPVPData.ContainsKey(after.PVPLv)) {
			pvpRank.BeforeLv = before.PVPLv;
			pvpRank.BeforeName = GameData.DPVPData[before.PVPLv].Name;
			pvpRank.BeforeScore = before.PVPIntegral;
			pvpRank.BeforeLowScore = GameData.DPVPData[before.PVPLv].LowScore;
			pvpRank.BeforeHighScore = GameData.DPVPData[before.PVPLv].HighScore;
			pvpRank.AfterLv = after.PVPLv;
			pvpRank.AfterName = GameData.DPVPData[after.PVPLv].Name;
			pvpRank.AfterScore = after.PVPIntegral;
			pvpRank.AfterLowScore = GameData.DPVPData[after.PVPLv].LowScore;
			pvpRank.AfterHighScore = GameData.DPVPData[after.PVPLv].HighScore;

			pvpValue.SetValue(pvpRank);
			pvpObj.SetValue(pvpRank, false);
		}
	}
	/*
	 * 1.PVE + 三選一
	 * 顯示競賽目標 ＋下一步
	 * 出現獲得獎賞（隔0.25秒出現一個）
	 * 跑經驗值
	 * 出現三選一
	 * 點選獎賞之後出現結束
	 * 
	 * 2.PVE 
	 * 顯示競賽目標 ＋下一步
	 * 出現獲得獎賞（隔0.25秒出現一個）
	 * 跑經驗值之後出現結束
	 * 
	 * 3.PVP
	 * 顯示競賽目標 ＋下一步
	 * 出現獲得獎賞（隔0.25秒出現一個）
	 * 跑PVP經驗值之後出現結束
	*/

	private void showFinish () {
		uiStatsNext.SetActive (true); 
		hintValue.InitFinish();
	}

	private void showAward () {
		if(resultAwardParam.IsAwardEmpty)
			ShowBonusItem ();
		else 
			resultAwardParam.ShowAward();
	}

	public void ShowBonusItem () {
		if(GetCardLists.Count > 0) {
			showSkillInfo(GetCardLists[0]);
			GetCardLists.RemoveAt(0);
		} else {
			animatorAward.SetTrigger("EXPViewStart");
			if(GameData.DExpData.ContainsKey(beforePlayer.Lv) && GameData.DExpData.ContainsKey(afterPlayer.Lv)) 
				resultExpView.SetValue(beforePlayer.Exp, GameData.DExpData[beforePlayer.Lv].LvUpExp, GameData.DExpData[afterPlayer.Lv].LvUpExp, resultAwardParam.TempExp);
			
			Invoke("ShowExpRun", 0.5f);
		}
	}

	public void ShowExpRun () {
		resultExpView.IsShowExp = true;
	}

	public void ExpRunFinish () {
		if(resultAwardParam.IsHaveBonus && resultThree.ChooseCount == 0)
			uiStatsNext.gameObject.SetActive(true);
		else
			ShowReturnButton();
	}

	public void ExpRunFinishDelay () {
		if(resultAwardParam.IsHaveBonus && resultThree.ChooseCount == 0)
			moveBonusItem ();
		else 
			ShowReturnButton();
	}

	private void pvpNext () {
		animatorAward.SetTrigger ("AwardViewStart");
		Invoke("showAward", 1);
		Invoke("pvpDown", 1.5f);
	}

	private void pvpDown () {
		animatorAward.SetTrigger ("AwardViewDown");
		Invoke("pvpShowRank", 0.5f);
	}

	private void pvpShowRank() {
		animatorAward.SetTrigger("PVPView");
		Invoke("showRank", 1f);
	}

	private void showRank () {
		pvpValue.IsShowRank = true;
		uiAwardSkip.SetActive(true);
	}

	private void init () {
		if(resultAwardParam.IsHaveBonus) {
			for (int i=0; i<resultThree.ItemAwardGroup.Length; i++) 
				if(i >= 0 && i < resultThree.Max) 
					if(GameData.DItemData.ContainsKey(resultAwardParam.BonusItemIDs[i]))
						resultThree.ItemAwardGroup[i].Show(GameData.DItemData[resultAwardParam.BonusItemIDs[i]]);
				
			setThreeVisible (false);
		}

		resultAwardParam.AwardIndex = resultAwardParam.AwardItemTempIDs.Count;
		resultAwardParam.AwardMax = resultAwardParam.AwardItemTempIDs.Count;
		for(int i=0; i<resultAwardParam.AwardItemTempIDs.Count; i++) {
			if(GameData.DItemData.ContainsKey(resultAwardParam.AwardItemTempIDs[i])){
				resultAwardParam.AlreadyGetItems.Add(addItemToAward(i, GameData.DItemData[resultAwardParam.AwardItemTempIDs[i]]));
			} else {
				resultAwardParam.AlreadyGetItems.Add(addItemToAward(i, new TItemData(), true, true));	
			}
		}
	}

	private void finishStageHint (){resultCenter.PlayAnimation("Down");}
	private void showTeamStats () {resultCenter.PlayAnimation("TeamStats");}

	private void setThreeVisible (bool isShow) {
		if(resultAwardParam.IsHaveBonus) 
			resultThree.SetThreeVisible(isShow);
	}

	public bool ChooseLucky(int index) {
		if(isCanChooseLucky) {
			resultThree.ChooseIndex = index;
			if(resultThree.ChooseCount == 0) {
				Invoke ("ShowReturnButton", 2);
				chooseItem (index);
				return true;
			} else 
				return PayChooseReward ();
		}
		return false;
	}

	public bool PayChooseReward () {
		if(resultThree.ChooseCount == 0) {
			stageRewardAgain(GameData.StageID, 0);
			return true;
		} else if (resultThree.ChooseCount == 1) {
			if(GameData.Team.Diamond >= GameConst.Stage3Pick1Diamond2) {
				stageRewardAgain(GameData.StageID, GameConst.Stage3Pick1Diamond2);
				return true;
			} else
				UIRecharge.Get.ShowView(ERechargeType.Diamond.GetHashCode(), false);
		} else if(resultThree.ChooseCount == 2) {
			if(GameData.Team.Diamond >= GameConst.Stage3Pick1Diamond3) {
				stageRewardAgain(GameData.StageID, GameConst.Stage3Pick1Diamond3);
				return true;
			} else
				UIRecharge.Get.ShowView(ERechargeType.Diamond.GetHashCode(), false);
		}
		return false;
	}
	
	private void chooseItem (int index) {
		UI3DGameResult.Get.ChooseStart(index);
		isCanChooseLucky = false;
		if(index == 0)
			Invoke("showOneItem", 0.75f);
		else if(index == 1)
			Invoke("showTwoItem", 0.75f);
		else if(index == 2) 
			Invoke("showThreeItem", 0.75f);
	}
	
	private void showOneItem () {showItem (0);}
	private void showTwoItem () {showItem (1);}
	private void showThreeItem () {showItem (2);}

	
	private void showItem (int index) {
		if(GameData.DItemData.ContainsKey(resultAwardParam.AlreadGetBonusID)) 
			resultThree.ItemAwardGroup[index].Show(GameData.DItemData[resultAwardParam.AlreadGetBonusID]);
		
		Invoke("MoveItem",0.5f);
		resultThree.ShowItem(index);
	}
	
	private void MoveItem () {
		if(resultThree.ChooseIndex >= 0 && resultThree.ChooseIndex < resultThree.ItemAwardGroup.Length)
			resultThree.ItemAwardGroup[resultThree.ChooseIndex].transform.DOLocalMove(new Vector3(Mathf.Min(-300 + (resultAwardParam.AwardCount * 100), 400), -170, 0), 0.2f).OnComplete(MoveItemFin);
	}
	
	public void MoveItemFin () {
		resultThree.MoveFin();
		addItemToBack(resultAwardParam.AlreadGetBonusID);
		isCanChooseLucky = true;
	}
		
	private ItemAwardGroup addItemToAward (int index, TItemData itemData, bool isNeedAdd = true, bool isOther = false) {
		GameObject obj = Instantiate(uiItem) as GameObject;
		GameObject fx = Instantiate(uiItemEffect) as GameObject;
		if(!isOther) {
			obj.name = itemData.ID.ToString();
		} else {
			obj.name = "-1";
		}
		ItemAwardGroup itemAward = obj.GetComponent<ItemAwardGroup>();
		if(itemAward != null) {
			fx.transform.parent = itemAward.Window.transform;
			fx.transform.localScale = Vector3.one;
			fx.transform.localPosition = Vector3.zero;

			itemAward.Hide();

			if(isNeedAdd) {
				obj.transform.parent = awardScaleView.transform;
				obj.transform.localPosition = new Vector3(-450 + (150 * index), 0, 0);
				obj.transform.localScale = Vector3.one;
			}
			obj.SetActive(isNeedAdd);
			
			UIEventListener.Get(obj).onClick = OnShowAwardInfo;
		}
		return itemAward;
	}

	private void addItemToBack (int id) {
		if(GameData.DItemData.ContainsKey(id)) {
			ItemAwardGroup itemAwardGroup =  addItemToAward(resultAwardParam.AwardCount, GameData.DItemData[id], true);
			itemAwardGroup.Show(GameData.DItemData[id]);
			resultAwardParam.AlreadyGetItems.Add(itemAwardGroup);
			
			if(resultAwardParam.AwardCount > 7)
				awardScrollView.MoveRelative(new Vector3(-90 * (resultAwardParam.AwardCount - 7), 0, 0));
			
			if(GameData.DItemData[id].Kind == 21) 
//			if(GameData.Team.CheckSkillCardisNew(GameData.DItemData[id].Avatar, oldSkillCards))
			if(!GameData.Team.IsGetItem(id))
				showSkillInfo(id);

			GameData.Team.GotItemCount = newGotItems;
		}
	}

	private void showSkillInfo (int itemID) {
		PlayerPrefs.SetInt(ESave.NewCardFlag.ToString(), 0);
		UIGetSkillCard.Get.ShowView(itemID);
	}

	private void moveBonusItem () {
		IsShowFirstCard = false;
		animatorAward.SetTrigger ("AwardViewDown");
		if(resultAwardParam.IsHaveBonus)
			Invoke("showLuckyThree", 0.5f);
		else 
			ShowReturnButton ();
	}

	private void showLuckyThree () {
		setThreeVisible (true);
		isShow3Dbasket = true;
		uiStatsNext.SetActive(true);
	}

	private void show3DBasket () {
		setThreeVisible (false);
		UI3DGameResult.UIShow(true);
		Invoke("showChooseLucky",3);
	}

	private void showChooseLucky () {
		isCanChooseLucky = true;
		resultThree.ClickTipVisible = true;
	}

	public void ShowReturnButton () {
		resultThree.IsChooseLucky = true;
		uiAwardSkip.SetActive(true);
	}

    /// <summary>
    /// Stages the reward start.
    /// </summary>
    /// <param name="stageID">Stage I.</param>
    /// <param name="points"></param>
    /// <param name="lostPoints"></param>
    private void stageRewardStart(int stageID, int points, int lostPoints)
	{
	    ShowMissionBoard();
		beforePlayer = GameData.Team.Player;
	    if(!string.IsNullOrEmpty(GameData.Team.Identifier))
	    {
	        if(GameController.Visible && GameController.Get.StageData.Chapter == 0)
	            return;
	        else
	        {
	            var winProtocol = new StageWinProtocol();
	            winProtocol.Send(stageID, points, lostPoints, waitMainStageWin);
	        }
	    }
	}

	private void waitMainStageWin(bool ok, TStageReward reward) {
		if(ok) {
			try {
				if(reward.SurelyItemIDs != null && reward.SurelyItemIDs.Length > 0) {
					for(int i=0; i<reward.SurelyItemIDs.Length; i++) {
						if(GameData.DItemData.ContainsKey(reward.SurelyItemIDs[i]) && GameData.DItemData[reward.SurelyItemIDs[i]].Kind == 21) {
							if(!GameData.Team.IsGetItem(reward.SurelyItemIDs[i])) {
								GetCardLists.Add(reward.SurelyItemIDs[i]);
								IsShowFirstCard = true;
							}
						}
					}
				}
				afterPlayer = reward.Player;
				if(beforePlayer.Lv != reward.Player.Lv) {
					isLevelUp = true;
					PlayerPrefs.SetInt(ESave.LevelUpFlag.ToString(), GameData.DExpData[afterPlayer.Lv].UI);
				}

				if(reward.SurelyItemIDs != null && reward.SurelyItemIDs.Length > 0) 
					for(int i = 0; i < reward.SurelyItemIDs.Length; i++) 
						if(GameData.DItemData.ContainsKey(reward.SurelyItemIDs[i]) && GameData.DItemData[reward.SurelyItemIDs[i]].Kind > 0 && GameData.DItemData[reward.SurelyItemIDs[i]].Kind < 8) 
							if(GameData.Setting.NewAvatar.ContainsKey(GameData.DItemData[reward.SurelyItemIDs[i]].Kind)) 
								GameData.Setting.NewAvatar[GameData.DItemData[reward.SurelyItemIDs[i]].Kind] = reward.SurelyItemIDs[i];

				if(GameData.DItemData.ContainsKey(reward.RandomItemID) && GameData.DItemData[reward.RandomItemID].Kind > 0 && GameData.DItemData[reward.RandomItemID].Kind < 8)
					if(GameData.Setting.NewAvatar.ContainsKey(GameData.DItemData[reward.RandomItemID].Kind))
						GameData.Setting.NewAvatar[GameData.DItemData[reward.RandomItemID].Kind] = reward.RandomItemID;

				newGotItems = reward.GotItemCount;
				GameData.Team.GotItemCount = reward.GotItemCount;
				GameData.Team.SkillCards = reward.SkillCards;

				isGetAward = true;

				resultAwardParam.SetAwardValue(reward);
				
				init ();
			} catch (Exception e) {
				Debug.Log(e.ToString());
				isGetAward = false;
			}
		}
		else {
			isGetAward = false;
			UIHint.Get.ShowHint("Stage Reward fail!", Color.black);
		}
	}

    /// <summary>
    /// Stages the reward again.
    /// </summary>
    /// <param name="stageID">Stage I.</param>
    /// <param name="payDiamond"></param>
    private void stageRewardAgain(int stageID, int payDiamond)
	{
	    var again = new StageRewardAgainProtocol();
        again.Send(stageID, waitMainStageRewardAgain);

        TStageData stageData = StageTable.Ins.GetByID(stageID);
        if (stageData.IsValid() && stageData.HasRandomRewards())
            Statistic.Ins.LogEvent(59, stageID.ToString(), payDiamond);
    }
	
	private void waitMainStageRewardAgain(bool ok, TStageRewardAgain reward)
	{
//		Debug.LogFormat("WaitMainStageRewardAgain, ok:{0}", ok);
		
		if (ok)
		{
			resultAwardParam.AlreadGetBonusID = reward.RandomItemID;
			chooseItem(resultThree.ChooseIndex);
			newGotItems = reward.GotItemCount;
		}
		else
			UIHint.Get.ShowHint("Stage Reward fail!", Color.black);
	}
}