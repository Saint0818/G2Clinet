using GamePlayEnum;
using GameStruct;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;

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
	private Dictionary<int, ItemAwardGroup> bonusAwardItems;
	private GameObject awardScrollView;
	private GameObject uiItem;
	private int awardIndex;
	private int awardMax;
	private bool isShowAward = false;
	private float awardGetTime = 0;
	private float awardGetTimeInterval = 0.5f;

	private ItemAwardGroup[] itemAwardGroup = new ItemAwardGroup[3];

	private bool isChooseLucky = false;
	private int chooseIndex = 0;
	private int chooseCount = 0;

	private int tempMoney;
	private int tempExp;
	private int tempDia;

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
		//Show StageHint
		if(isShowFinish && hintIndex >= -1) {
			finishTime -= Time.deltaTime;
			if(finishTime <= 0) {
				if(hintIndex == -1) {
					isShowFinish = false;
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
					showBonusItem ();
				} else {
					if((awardMax - awardIndex) < awardMax ){
						if(awardItemTempIDs[(awardMax - awardIndex)] > 0)
							alreadyGetItems[(awardMax - awardIndex)].Show(GameData.DItemData[awardItemTempIDs[(awardMax - awardIndex)]]);
						else if(awardItemTempIDs[(awardMax - awardIndex)] == -1) 
//							alreadyGetItems[(awardMax - awardIndex)].Show(new TItemData() , 0 , tempMoney, true);
							alreadyGetItems[awardMax - awardIndex].ShowMoney(tempMoney);
						else if(awardItemTempIDs[(awardMax - awardIndex)] == -2) 
//							alreadyGetItems[(awardMax - awardIndex)].Show(new TItemData(), 1 , tempExp, true);
							alreadyGetItems[awardMax - awardIndex].ShowExp(tempExp);
						else if(awardItemTempIDs[(awardMax - awardIndex)] == -3) 
//							alreadyGetItems[(awardMax - awardIndex)].Show(new TItemData(), 2 , tempDia, true);
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
		itemAwardGroup = GetComponentsInChildren<ItemAwardGroup>();

		animatorAward = gameObject.GetComponent<Animator>();
		animatorBottomView = GameObject.Find (UIName + "/Center/BottomView").GetComponent<Animator>();
		playerStats = GetComponentInChildren<PlayerStats>();
		playerValue = GetComponentsInChildren<PlayerValue>();
		teamValue = GetComponentInChildren<TeamValue>();

		for (int i=0; i<playerStats.PlayerInGameBtn.Length; i++) {
			playerStats.PlayerInGameBtn[i].name = i.ToString();
			UIEventListener.Get (playerStats.PlayerInGameBtn[i]).onClick = OnShowPlayerInfo;
		}

		awardScrollView = GameObject.Find(UIName + "/AwardsView/AwardsList/ScrollView/ScaleView");

		UIEventListener.Get (uiStatsNext).onClick = OnNext;
		UIEventListener.Get (uiAwardSkip).onClick = OnReturn;
		SetBtnFun(UIName + "/Center/BottomView/StatsView/LeftBtn", OnShowAwayStats);
		SetBtnFun(UIName + "/Center/BottomView/StatsView/RightBtn", OnShowHomeStats);

	}
	
	protected override void InitData() {

	}
	
	protected override void OnShow(bool isShow) {

	}

	public void OnShowPlayerInfo (GameObject go) {

	}

	public void OnShowAwardInfo (GameObject go) {
		
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
			UILoading.OpenUI = UILoading.OpenStageUI;
			if (isStage)
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			else
				SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
		}
	}
	
	public void ChooseLucky(int index) {
		chooseIndex = index;
		if(chooseCount == 0) {
			Invoke ("showReturnButton", 2);
			isChooseLucky = true;
			chooseItem (index);
		} else {
			stageRewardAgain(GameData.StageID);
		}
	}

	private void chooseItem (int index) {
		UI3DGameResult.Get.ChooseStart(index);
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

	private void showItem (int index) {
		chooseIndex = index;
		itemAwardGroup[index].gameObject.SetActive(true);
		if(GameData.DItemData.ContainsKey(alreadGetBonusID))  {
			itemAwardGroup[index].Show(GameData.DItemData[alreadGetBonusID]);
		}
		Invoke("MoveItem",1);
		chooseCount ++ ;
	}

	private void MoveItem () {
		itemAwardGroup[chooseIndex].transform.DOLocalMove(new Vector3(Mathf.Min(-300 + (alreadyGetItems.Count * 100), 400), -170, 0), 1f).OnComplete(MoveItemFin);
	}

	public void MoveItemFin () {
		itemAwardGroup[chooseIndex].Hide();
		addItemToBack(alreadGetBonusID);
	}

	private void init () {
		for (int i=0; i<itemAwardGroup.Length; i++) {
			itemAwardGroup[i] = GameObject.Find(UIName + "/ThreeAward/" + i.ToString()).GetComponent<ItemAwardGroup>();
			if(i >= 0 && i < 3) {
				if(GameData.DItemData.ContainsKey(bonusItemIDs[i]))
					itemAwardGroup[i].Show(GameData.DItemData[bonusItemIDs[i]]);
			}
		}
		hideThree();

		alreadyGetItems = new List<ItemAwardGroup>();
		bonusAwardItems = new Dictionary<int, ItemAwardGroup>();
		awardIndex = awardItemTempIDs.Count;
		awardMax = awardItemTempIDs.Count;
		for(int i=0; i<awardItemTempIDs.Count; i++) {
			if(GameData.DItemData.ContainsKey(awardItemTempIDs[i])){
				alreadyGetItems.Add(addItemToAward(i, GameData.DItemData[awardItemTempIDs[i]]));
			} else {
				alreadyGetItems.Add(addItemToAward(i, new TItemData(), true, true));	
			}
		}

		for(int i=0; i<bonusItemIDs.Length; i++) {
			 if(GameData.DItemData.ContainsKey(bonusItemIDs[i]))
				bonusAwardItems.Add(bonusItemIDs[i], addItemToAward(i, GameData.DItemData[bonusItemIDs[i]], false));
		}
	}

	//Show Stage Hint Check
	// it's need to get three items, and first items
	private void showFinish () {
		isShowFinish = true;
		finishTime = finishInterval;
	}

	private void finishStageHint (){
		animatorBottomView.SetTrigger("Down");
		uiStatsNext.SetActive(true);}

	//Show Team Stats
	private void showTeamStats () {
		animatorBottomView.SetTrigger("TeamStats");
	}

	//Show Award and LuckyThree
	private void showAward () {
		if(awardIndex == 0) {
			showBonusItem ();
		} else {
			isShowAward = true;
			awardGetTime = awardGetTimeInterval;
		}
	}

	private void showThree () {
		for (int i=0; i<itemAwardGroup.Length; i++) {
			itemAwardGroup[i].gameObject.SetActive(true);
		}
	}

	private void hideThree () {
		for (int i=0; i<itemAwardGroup.Length; i++) {
			itemAwardGroup[i].gameObject.SetActive(false);
		}
	}

	private ItemAwardGroup addItemToAward (int index, TItemData itemData, bool isNeedAdd = true, bool isOther = false) {
		GameObject obj = Instantiate(uiItem) as GameObject;

		if(!isOther) {
			obj.name = itemData.ID.ToString();
		} else {
			obj.name = index.ToString();
		}

		if(isNeedAdd) {
			obj.transform.parent = awardScrollView.transform;
			obj.transform.localPosition = new Vector3(-450 + (150 * index), 0, 0);
			obj.transform.localScale = Vector3.one;
		}
		obj.SetActive(isNeedAdd);

		UIEventListener.Get(obj).onClick = OnShowAwardInfo;
		return obj.GetComponent<ItemAwardGroup>();
	}

	private void addItemToBack (int id) {
		bonusAwardItems[id].gameObject.name = id.ToString();
		bonusAwardItems[id].transform.parent = awardScrollView.transform;
		bonusAwardItems[id].transform.localPosition = new Vector3(-450 + (150 * alreadyGetItems.Count), 0, 0);
		bonusAwardItems[id].transform.localScale = Vector3.one;
		if(GameData.DItemData.ContainsKey(id))
			bonusAwardItems[id].Show(GameData.DItemData[id]);

		bonusAwardItems[id].gameObject.SetActive(true);
		UIEventListener.Get(bonusAwardItems[id].gameObject).onClick = OnShowAwardInfo;
		alreadyGetItems.Add(bonusAwardItems[id]);
	}

	private void showBonusItem () {
		animatorAward.SetTrigger ("AwardViewDown");
		Invoke("showLuckyThree", 1);
	}
	
	private void showLuckyThree () {
		showThree ();
		Invoke("show3DBasket", 1);
	}

	private void show3DBasket () {
		hideThree ();
		UI3DGameResult.UIShow(true);
	}

	private void showReturnButton () {
		uiAwardSkip.SetActive(true);
	}

	public void SetGameRecord(ref TGameRecord record) {
		teamValue.SetValue(record);
		if(record.Done) {
			for (int i=0; i<GameController.Get.GamePlayers.Count; i++) {
				playerStats.SetPlayerName(i, GameController.Get.GamePlayers[i].Attribute.Name);
				playerStats.SetPlayerIcon(i, GameController.Get.GamePlayers[i].Attribute.FacePicture);
//				if(i == 1 || i == 2)//need get friend list
//					playerStats.ShowAddFriendBtn(i);
				playerValue[i].SetValue(GameController.Get.GamePlayers[i].GameRecord);
			}
		}
		uiStatsNext.SetActive(false);
		uiAwardSkip.SetActive(false);
		stageRewardStart(GameData.StageID);
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

	/// <summary>
	/// Stages the reward start.
	/// </summary>
	/// <param name="stageID">Stage I.</param>
	private void stageRewardStart(int stageID)
	{
		WWWForm form = new WWWForm();
		form.AddField("StageID", stageID);
		SendHttp.Get.Command(URLConst.StageRewardStart, waitStageRewardStart, form);
	}

	private void waitStageRewardStart(bool ok, WWW www)
	{
		Debug.LogFormat("waitStageRewardStart, ok:{0}", ok);

		updateResult(GameData.StageID);
		Invoke("showFinish", 4);

		if(ok)
		{
			var reward = JsonConvert.DeserializeObject<TStageRewardStart>(www.text);
			
			Debug.Log(reward);
			
			GameData.Team.Money = reward.Money;
			GameData.Team.Diamond = reward.Diamond;
			GameData.Team.Player = reward.Player;
			GameData.Team.Player.Init();
			GameData.Team.Items = reward.Items;

			
			awardItemTempIDs = new List<int>();
			if(reward.SurelyItemIDs == null) {
				awardItemIDs = new int[0];
			}else
				awardItemIDs = reward.SurelyItemIDs;
			bonusItemIDs = reward.CandidateItemIDs;
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
		}
		else
			UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
	}

	/// <summary>
	/// Stages the reward again.
	/// </summary>
	/// <param name="stageID">Stage I.</param>
	private void stageRewardAgain(int stageID)
	{
		WWWForm form = new WWWForm();
		form.AddField("StageID", stageID);
		SendHttp.Get.Command(URLConst.StageRewardAgain, waitStageRewardAgain, form);
	}
	
	private void waitStageRewardAgain(bool ok, WWW www)
	{
		Debug.LogFormat("waitStageRewardAgain, ok:{0}", ok);
		
		if (ok)
		{
			var reward = JsonConvert.DeserializeObject<TStageRewardAgain>(www.text);
			GameData.Team.Diamond = reward.Diamond;
			GameData.Team.Items = reward.Items;
			
			alreadGetBonusID = reward.RandomItemID;
			chooseItem(chooseIndex);
		}
		else
			UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
	}

	public bool isStage
    {
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
