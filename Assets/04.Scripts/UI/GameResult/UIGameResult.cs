using GameStruct;
using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using GameEnum;

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
	private int alreadGetBonusID = 3;
	private List<int> awardItemTempIDs = new List<int>();
	private int[] awardItemIDs;
	private int[] bonusItemIDs;

	private List<ItemAwardGroup> alreadyGetItems;
	private GameObject awardScaleView;
	private UIScrollView awardScrollView;
	private GameObject uiItem;
	private int awardIndex;
	private int awardMax;
	private bool isShowAward = false;
	private float awardGetTime = 0;
	private float awardGetTimeInterval = 0.25f;

	private ItemAwardGroup[] itemAwardGroup = new ItemAwardGroup[3];
	private UILabel[] diamondPay = new UILabel[3];

	private bool isChooseLucky = false;
	private int chooseIndex = 0;
	private int chooseCount = 0;

	private int tempMoney;
	private int tempExp;
	private int tempDia;

	private bool isHaveBonus = false;
	private bool isGetAward = false;
	private bool isCanChooseLucky = false;
	private bool isLevelUp = false;
	private TPlayer beforePlayer;
	private TPlayer afterPlayer;
	public List<int> GetCardLists = new List<int>();
	public bool IsShowFirstCard = true;

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
		if (isShow) {
			Get.Show(isShow);
			UITutorial.UIShow(false);
		}
	}

	void FixedUpdate () {
		//Show StageHint
		if(isShowFinish && hintIndex >= -1) {
			finishTime -= Time.deltaTime;
			if(finishTime <= 0) {
				if(hintIndex == -1) {
					isShowFinish = false;
					uiStatsNext.SetActive(true);
					Invoke("finishStageHint", 1);
				} else {
					if(hintIndex > 0 && hintIndex < mTargets.Length)
						mTargets[hintCount - hintIndex].UpdateFin(true);

					finishTime = finishInterval;
					hintIndex --;
				}
			}
		}
		//Show Award
		if(isShowAward && awardIndex >= -1) {
			awardGetTime -= Time.deltaTime;
			if(awardGetTime <= 0) {
				if(awardIndex == -1) {
					isShowAward = false;
					ShowBonusItem ();
				} else {
					if((awardMax - awardIndex) < awardMax ){
						if(awardItemTempIDs[(awardMax - awardIndex)] > 0){
							if(GameData.DItemData.ContainsKey (awardItemTempIDs[(awardMax - awardIndex)]))
								alreadyGetItems[(awardMax - awardIndex)].Show(GameData.DItemData[awardItemTempIDs[(awardMax - awardIndex)]]);
						}else if(awardItemTempIDs[(awardMax - awardIndex)] == -1) 
							alreadyGetItems[awardMax - awardIndex].ShowMoney(tempMoney);
						else if(awardItemTempIDs[(awardMax - awardIndex)] == -2) 
							alreadyGetItems[awardMax - awardIndex].ShowExp(tempExp);
						else if(awardItemTempIDs[(awardMax - awardIndex)] == -3)
							alreadyGetItems[awardMax - awardIndex].ShowGem(tempDia);
					}

					awardGetTime = awardGetTimeInterval;
					awardIndex --;
				}
			}
		}
	}
	
	protected override void InitCom() {
		uiItem = Resources.Load(UIPrefabPath.ItemAwardGroup) as GameObject;
		uiStatsNext = GameObject.Find(UIName + "/BottomRight/StatsNextLabel");
		uiAwardSkip = GameObject.Find(UIName + "/BottomRight/AwardSkipLabel");
		
		//Center/BottomView
		mTargets = GetComponentsInChildren<UIStageHintTarget>();
		itemAwardGroup =  GameObject.Find(UIName + "/ThreeAward").GetComponentsInChildren<ItemAwardGroup>();
		for(int i=0; i<diamondPay.Length; i++) {
			diamondPay[i] = GameObject.Find(UIName + "/ShowWords/"+i.ToString()+"/GemLabel").GetComponent<UILabel>();
			diamondPay[i].gameObject.SetActive(false);
		}

		animatorAward = gameObject.GetComponent<Animator>();
		animatorBottomView = GameObject.Find (UIName + "/Center/BottomView").GetComponent<Animator>();
		playerStats = GetComponentInChildren<PlayerStats>();
		playerValue = GetComponentsInChildren<PlayerValue>();
		teamValue = GetComponentInChildren<TeamValue>();

		for (int i=0; i<playerStats.PlayerInGameBtn.Length; i++) {
			playerStats.PlayerInGameBtn[i].name = i.ToString();
			UIEventListener.Get (playerStats.PlayerInGameBtn[i]).onClick = OnShowPlayerInfo;
		}

		awardScaleView = GameObject.Find(UIName + "/AwardsView/AwardsList/ScrollView/ScaleView");
		awardScrollView = GameObject.Find(UIName + "/AwardsView/AwardsList/ScrollView").GetComponent<UIScrollView>();

		UIEventListener.Get (uiStatsNext).onClick = OnNext;
		UIEventListener.Get (uiAwardSkip).onClick = OnReturn;
		SetBtnFun(UIName + "/Center/BottomView/StatsView/LeftBtn", OnShowAwayStats);
		SetBtnFun(UIName + "/Center/BottomView/StatsView/RightBtn", OnShowHomeStats);

	}

	//Click Event
	public void OnShowPlayerInfo (GameObject go) {

	}

	public void OnShowAwardInfo (GameObject go) {
		int index = -1;
		if(int.TryParse(go.name, out index)) {
			if(GameData.DItemData.ContainsKey(index))
				UIItemHint.Get.OnShow(GameData.DItemData[index].ID);
		}
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
		if (GameController.Visible && GameController.Get.IsPVP) {
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            GameData.PVPEnemyMembers[0].Identifier = string.Empty;
		}
		else if (GameController.Visible && GameController.Get.StageData.IsTutorial) {
			if (StageTable.Ins.HasByID(GameController.Get.StageData.ID + 1)) {
				UIShow(false);
				GameData.StageID = GameController.Get.StageData.ID + 1;
				int courtNo = StageTable.Ins.GetByID(GameData.StageID).CourtNo;
				SceneMgr.Get.CurrentScene = "";
				SceneMgr.Get.ChangeLevel (courtNo);
			} else {
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			}
		} else {
			if(SendHttp.Get.CheckNetwork(false) && isGetAward) {
				uiStatsNext.SetActive(false);
				animatorAward.SetTrigger("AwardViewStart");
				Invoke("showAward", 1);
			} else  {
				if(isLevelUp) {
					UIShow(false);
					UI3DGameResult.UIShow(false);
					UILevelUp.Get.Show(beforePlayer, afterPlayer);
				} else {
					backToLobby ();
				}
				
			}
		}
	}

	public void OnReturn(GameObject go) {
		if(isChooseLucky) {
			if(isLevelUp) {
				UIShow(false);
				UI3DGameResult.UIShow(false);
				UILevelUp.Get.Show(beforePlayer, afterPlayer);
			} else {
//				if(IsExpUnlock) {
//					UIShow(false);
//					//Unlock UI
//				} else
					backToLobby ();
			}
		}
	}

	private void backToLobby () {
		Time.timeScale = 1;
		UIShow(false);
		UILoading.OpenUI = UILoading.OpenStageUI;
		if (isStage)
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
		else
			SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
	}

	private void showMissionBoard () {
		hintCount = UIStageHintManager.UpdateHintResult(GameData.StageID, ref mTargets);
		hintIndex = hintCount;
		Invoke("showFinish", 4);
	}

	/*
	 * 1. SetGameRecord is called form GameController
	 * 2. StartReward(Server) -> (Server Success)Run init
	 * 3. Run showFinish (Show Stage Hint Check) -> finishStageHint
	 * 4. Run showTeamStats (Show Team Stats)
	 * 5. (Server Success) Run showAward ( Show Award and LuckyThree) -> showBonusItem
	 *    (Server Fail) Run backToLobby
	 * 6. Run showLuckyThree 
	 * 7. (Level Up) Show UILevelUp
	 * 	  (No Level Up) Run backToLobby
	 */

	public void SetGameRecord(ref TGameRecord record) {
		teamValue.SetValue(record);
		if(record.Done) {
			for (int i=0; i<GameController.Get.GamePlayers.Count; i++) {
				playerStats.SetPlayerName(i, GameController.Get.GamePlayers[i].Attribute.Name);
				playerStats.SetPlayerIcon(i, GameController.Get.GamePlayers[i].Attribute.FacePicture);
				playerStats.SetPositionIcon(i, GameController.Get.GamePlayers[i].Attribute.BodyType);
				if(!string.IsNullOrEmpty(record.Identifier) && GameData.Team.Friends != null && GameData.Team.Friends.ContainsKey(record.Identifier)) {//need get friend list 
					playerStats.ShowAddFriendBtn(i, record.Identifier);

                    if (i < playerStats.AddFriendBtn.Length)
					    SetBtnFun(ref playerStats.AddFriendBtn[i], OnMakeFriend);
				}
				playerValue[i].SetValue(GameController.Get.GamePlayers[i].GameRecord);
			}
		}
		uiStatsNext.SetActive(false);
		uiAwardSkip.SetActive(false);

		if (GameController.Get.IsPVP) {
			SendPVPEnd (record.Score1, record.Score2);			
		} else {
			stageRewardStart(GameData.StageID);
		}
	}

	private void SendPVPEnd(int score1, int score2)
	{
		WWWForm form = new WWWForm();
		form.AddField("Score1", score1);
		form.AddField("Score2", score2);
		SendHttp.Get.Command(URLConst.PVPEnd, WaitPVPEnd, form, false);
	}

	public void WaitPVPEnd(bool ok, WWW www)
	{
		if (ok) {
			TPVPResult reslut = JsonConvert.DeserializeObject <TPVPResult>(www.text, SendHttp.Get.JsonSetting);	
            GameData.Team.PVPLv = reslut.PVPLv;
            GameData.Team.PVPIntegral = reslut.PVPIntegral;
            GameData.Team.PVPCoin = reslut.PVPCoin;
			GameData.Team.LifetimeRecord = reslut.LifetimeRecord;
			showMissionBoard ();
		} else {
		}

		uiStatsNext.SetActive (true);
	}
	

	public void OnMakeFriend () {
		int result = -1;
		if(int.TryParse(UIButton.current.name, out result)) {
			playerStats.HideAddFriendBtn(result);
			SendHttp.Get.MakeFriend(null, playerStats.tempID[result]);
		}
	}
	
	private void init () {
		if(IsHaveBonus) {
			for (int i=0; i<itemAwardGroup.Length; i++) {
				itemAwardGroup[i] = GameObject.Find(UIName + "/ThreeAward/" + i.ToString()).GetComponent<ItemAwardGroup>();
				if(i >= 0 && i < 3) {
					if(GameData.DItemData.ContainsKey(bonusItemIDs[i]))
						itemAwardGroup[i].Show(GameData.DItemData[bonusItemIDs[i]]);
				}
			}
			hideThree();
		}
		
		alreadyGetItems = new List<ItemAwardGroup>();
		awardIndex = awardItemTempIDs.Count;
		awardMax = awardItemTempIDs.Count;
		for(int i=0; i<awardItemTempIDs.Count; i++) {
			if(GameData.DItemData.ContainsKey(awardItemTempIDs[i])){
				alreadyGetItems.Add(addItemToAward(i, GameData.DItemData[awardItemTempIDs[i]]));
			} else {
				alreadyGetItems.Add(addItemToAward(i, new TItemData(), true, true));	
			}
		}
	}

	private void showFinish () {
		isShowFinish = true;
		finishTime = finishInterval;
	}

	private void finishStageHint (){
		animatorBottomView.SetTrigger("Down");
	}

	private void showTeamStats () {
		animatorBottomView.SetTrigger("TeamStats");
	}

	private void showAward () {
		if(awardIndex == 0) {
			ShowBonusItem ();
		} else {
			isShowAward = true;
			awardGetTime = awardGetTimeInterval;
		}
	}

	private void showThree () {
		if(IsHaveBonus) {
			for (int i=0; i<itemAwardGroup.Length; i++) {
				itemAwardGroup[i].gameObject.SetActive(true);
			}
		}
	}

	private void hideThree () {
		if(IsHaveBonus) {
			for (int i=0; i<itemAwardGroup.Length; i++) {
				itemAwardGroup[i].gameObject.SetActive(false);
			}
		}
	}

	public void ChooseLucky(int index) {
		if(isCanChooseLucky) {
			chooseIndex = index;
			if(chooseCount == 0) {
				Invoke ("showReturnButton", 2);
				chooseItem (index);
			} else {
				PayChooseReward ();
			}
		}
	}

	public void PayChooseReward () {
		if (GameData.Team.Diamond >= chooseCount * 50)
			stageRewardAgain(GameData.StageID);
		else
			UIHint.Get.ShowHint(TextConst.S (233), Color.red);
	}
	
	private void chooseItem (int index) {
		UI3DGameResult.Get.ChooseStart(index);
		isCanChooseLucky = false;
		if(index == 0) {
			Invoke("showOneItem", 0.75f);
		} else if(index == 1) {
			Invoke("showTwoItem", 0.75f);
		} else if(index == 2) {
			Invoke("showThreeItem", 0.75f);
		}
	}
	
	private void showOneItem () {showItem (0);}
	private void showTwoItem () {showItem (1);}
	private void showThreeItem () {showItem (2);}

	private void showPayDiamond (int index) {
		for(int i=0; i<diamondPay.Length; i++) {
			if(i != index)
				diamondPay[i].gameObject.SetActive(true);
		}
	}

	private void setPayDiamond (int value) {
		for(int i=0; i<diamondPay.Length; i++) 
			diamondPay[i].text = value.ToString();
	}
	
	private void showItem (int index) {
		chooseIndex = index;
		itemAwardGroup[index].gameObject.SetActive(true);
		if(GameData.DItemData.ContainsKey(alreadGetBonusID))  {
			itemAwardGroup[index].Show(GameData.DItemData[alreadGetBonusID]);
		}
		Invoke("MoveItem",1);
		if(chooseCount == 0) {
			showPayDiamond(index);
			setPayDiamond(50);
		} else if (chooseCount == 1) {
			diamondPay[index].gameObject.SetActive(false);
			setPayDiamond(100);
		}else {
			diamondPay[index].gameObject.SetActive(false);
		}
		chooseCount ++ ;
	}
	
	private void MoveItem () {
		if(chooseIndex >= 0 && chooseIndex < itemAwardGroup.Length)
			itemAwardGroup[chooseIndex].transform.DOLocalMove(new Vector3(Mathf.Min(-300 + (alreadyGetItems.Count * 100), 400), -170, 0), 0.5f).OnComplete(MoveItemFin);
	}
	
	public void MoveItemFin () {
		if(chooseIndex >= 0 && chooseIndex < itemAwardGroup.Length)
			itemAwardGroup[chooseIndex].Hide();
		addItemToBack(alreadGetBonusID);
		isCanChooseLucky = true;
	}

	//isOther:  Because Money,Diamond,Exp are not Item, it will be deleted in the future.
	private ItemAwardGroup addItemToAward (int index, TItemData itemData, bool isNeedAdd = true, bool isOther = false) {
		GameObject obj = Instantiate(uiItem) as GameObject;

		if(!isOther) {
			obj.name = itemData.ID.ToString();
		} else {
			obj.name = "-1";
		}

		if(isNeedAdd) {
			obj.transform.parent = awardScaleView.transform;
			obj.transform.localPosition = new Vector3(-450 + (150 * index), 0, 0);
			obj.transform.localScale = Vector3.one;
		}
		obj.SetActive(isNeedAdd);

		UIEventListener.Get(obj).onClick = OnShowAwardInfo;
		return obj.GetComponent<ItemAwardGroup>();
	}

	private void addItemToBack (int id) {
		ItemAwardGroup itemAwardGroup =  addItemToAward(alreadyGetItems.Count, GameData.DItemData[id], true);
		itemAwardGroup.Show(GameData.DItemData[id]);
		alreadyGetItems.Add(itemAwardGroup);

		if(alreadyGetItems.Count > 7)
			awardScrollView.MoveRelative(new Vector3(-90 * (alreadyGetItems.Count - 7), 0, 0));

		if(GameData.DItemData[id].Kind == 21) 
			if(GameData.Team.CheckSkillCardisNew(GameData.DItemData[id].Avatar))
				showSkillInfo(id);
	}

	public void ShowBonusItem () {
		if(GetCardLists.Count > 0) {
				showSkillInfo(GetCardLists[0]);
				GetCardLists.RemoveAt(0);
		} else {
			if(isHaveBonus)
				moveBonusItem ();
			else 
				showReturnButton();
		}
	}

	private void showSkillInfo (int itemID) {
		PlayerPrefs.SetInt(ESave.NewCardFlag.ToString(), 0);
		UIGetSkillCard.Get.Show(itemID);
	}

	private void moveBonusItem () {
		IsShowFirstCard = false;
		animatorAward.SetTrigger ("AwardViewDown");
		if(isHaveBonus)
			Invoke("showLuckyThree", 0.5f);
		else 
			showReturnButton ();
	}

	private void showLuckyThree () {
		showThree ();
		Invoke("show3DBasket", 0.5f);
	}

	private void show3DBasket () {
		hideThree ();
		UI3DGameResult.UIShow(true);
		Invoke("canChooseLucky",4);

	}

	private void canChooseLucky () {
		isCanChooseLucky = true;
	}

	private void showReturnButton () {
		isChooseLucky = true;
		uiAwardSkip.SetActive(true);
	}

	/// <summary>
	/// Stages the reward start.
	/// </summary>
	/// <param name="stageID">Stage I.</param>
	private void stageRewardStart(int stageID)
	{
		showMissionBoard ();
		beforePlayer = GameData.Team.Player;
		if(!string.IsNullOrEmpty(GameData.Team.Identifier)) {
			if (GameController.Visible && GameController.Get.StageData.Chapter == 0)  {

			}
            else
            {
                MainStageWinProtocol winProtocol = new MainStageWinProtocol();
                winProtocol.Send(stageID, waitMainStageWin);
			}
		}
	}

	private void waitMainStageWin(bool ok, TStageReward reward)
	{
		if(ok)
		{
			try {
//				var reward = JsonConvert.DeserializeObject<TStageRewardStart>(www.text);

				if(reward.SurelyItemIDs != null && reward.SurelyItemIDs.Length > 0) {
					for(int i=0; i<reward.SurelyItemIDs.Length; i++) {
						if(GameData.DItemData.ContainsKey(reward.SurelyItemIDs[i]) && GameData.DItemData[reward.SurelyItemIDs[i]].Kind == 21) {
							if(GameData.Team.CheckSkillCardisNew(GameData.DItemData[reward.SurelyItemIDs[i]].Avatar)) {
								GetCardLists.Add(reward.SurelyItemIDs[i]);
								IsShowFirstCard = true;
							}
						}
					}
				}
				
				if(beforePlayer.Lv != reward.Player.Lv) {
					isLevelUp = true;
					afterPlayer = reward.Player;
					if(GameData.DExpData.ContainsKey(reward.Player.Lv) && GameData.DExpData[reward.Player.Lv].OpenIndex > 0) {
						PlayerPrefs.SetInt (ESave.LevelUpFlag.ToString(), GameData.DExpData[afterPlayer.Lv].UI);
					}
				}

				if(reward.SurelyItemIDs != null && reward.SurelyItemIDs.Length > 0)
				{
					for(int i = 0; i < reward.SurelyItemIDs.Length; i++)
						if(GameData.DItemData.ContainsKey(reward.SurelyItemIDs[i]) && GameData.DItemData[reward.SurelyItemIDs[i]].Kind > 0 && GameData.DItemData[reward.SurelyItemIDs[i]].Kind < 8)
						{
							if(GameData.Setting.NewAvatar.ContainsKey(GameData.DItemData[reward.SurelyItemIDs[i]].Kind))
							{
								GameData.Setting.NewAvatar[GameData.DItemData[reward.SurelyItemIDs[i]].Kind] = reward.SurelyItemIDs[i];
							}
						}
				}

				if(GameData.DItemData.ContainsKey(reward.RandomItemID) && GameData.DItemData[reward.RandomItemID].Kind > 0 && GameData.DItemData[reward.RandomItemID].Kind < 8)
				{
					if(GameData.Setting.NewAvatar.ContainsKey(GameData.DItemData[reward.RandomItemID].Kind))
						GameData.Setting.NewAvatar[GameData.DItemData[reward.RandomItemID].Kind] = reward.RandomItemID;
				}

				GameData.Team.SkillCards = reward.SkillCards;

				isGetAward = true;
				awardItemTempIDs = new List<int>();
				if(reward.SurelyItemIDs == null) {
					awardItemIDs = new int[0];
				}else
					awardItemIDs = reward.SurelyItemIDs;

				// 玩家可能得到的亂數獎勵.
				bonusItemIDs = reward.CandidateItemIDs;
				isHaveBonus = IsHaveBonus;

				alreadGetBonusID = reward.RandomItemID;
				
				for (int i=0; i<awardItemIDs.Length; i++) {
					awardItemTempIDs.Add(awardItemIDs[i]);
				}
				
				if(reward.AddMoney > 0) {
					awardItemTempIDs.Add(-1);
					tempMoney = reward.AddMoney;
				}
				if(reward.AddExp > 0) {
					awardItemTempIDs.Add(-2);
					tempExp = reward.AddExp;
				}
				if(reward.AddDiamond > 0) {
					awardItemTempIDs.Add(-3);
					tempDia = reward.AddDiamond;
				}
				
				init ();
                if (GameController.Visible)
                    GameController.Get.SendGameRecord();
			} catch (Exception e) {
				Debug.Log(e.ToString());
				isGetAward = false;
			}
		}
		else {
			isGetAward = false;
			UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
		}
	}

	/// <summary>
	/// Stages the reward again.
	/// </summary>
	/// <param name="stageID">Stage I.</param>
	private void stageRewardAgain(int stageID)
	{
//		WWWForm form = new WWWForm();
//		form.AddField("StageID", stageID);
//		SendHttp.Get.Command(URLConst.MainStageRewardAgain, WaitMainStageRewardAgain, form);

	    var again = new MainStageRewardAgainProtocol();
        again.Send(stageID, waitMainStageRewardAgain);
	}
	
	private void waitMainStageRewardAgain(bool ok, TStageRewardAgain reward)
	{
		Debug.LogFormat("WaitMainStageRewardAgain, ok:{0}", ok);
		
		if (ok)
		{
//			var reward = JsonConvert.DeserializeObject<TMainStageRewardAgain>(www.text);
//			GameData.Team.Money = reward.Money;
//			GameData.Team.Diamond = reward.Diamond;
//			GameData.Team.Player.Lv = reward.PlayerLv;
//			GameData.Team.Player.Exp = reward.PlayerExp;
//			GameData.Team.Items = reward.Items;
//			GameData.Team.SkillCards = reward.SkillCards;
			
			alreadGetBonusID = reward.RandomItemID;
			chooseItem(chooseIndex);
		}
		else
			UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
	}

	public bool IsHaveBonus {
		get {return (bonusItemIDs != null && bonusItemIDs.Length > 0);}
	}

	public bool isStage
    {
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}