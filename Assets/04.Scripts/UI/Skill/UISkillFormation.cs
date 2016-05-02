﻿using System.Collections.Generic;
using DG.Tweening;
using GameEnum;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;

public struct TEquipSkillCardResult {
	public TSkill[] SkillCards;
	public TSkill[] PlayerCards;
	public TSkillCardPage[] SkillCardPages;
	public int Money;
}

public struct TActiveStruct {
	public GameObject ItemEquipActiveCard;
	public GameObject GridActiveCardBase;
	public UISprite SpriteActiveFieldIcon;
	public GameObject ItemActiveSelect;
	public int CardIndex;
	public int CardID;
	public int CardSN;
	public int CardLV;
	public void Init (string name, int index) {
		GridActiveCardBase = GameObject.Find (name + "/Center/MainView/Right/ActiveCardBase"+index.ToString());
		ItemActiveSelect = GameObject.Find (name + "/Center/MainView/Right/ActiveCardBase" + index.ToString() + "/ActiveField/Selected");
		ItemActiveSelect.SetActive(false);
		SpriteActiveFieldIcon = GameObject.Find (name + "/Center/MainView/Right/ActiveCardBase" + index.ToString() + "/ActiveField/Icon").GetComponent<UISprite>();
		SpriteActiveFieldIcon.transform.parent.name = index.ToString();
	}
	public void ActiveClear () {
		this.ItemEquipActiveCard = null;
		this.CardIndex = -1;
		this.CardID = 0;
		this.CardSN = -1;
		this.CardLV = 0;
	}

	public void SetData (TActiveStruct active, Vector3 pos, Transform parent = null){
		if(ItemEquipActiveCard == null)
			this.ItemEquipActiveCard = active.ItemEquipActiveCard;

		if(parent != null) {
			this.ItemEquipActiveCard.transform.SetParent(parent);
		}
		active.ItemEquipActiveCard.transform.localPosition = pos;
		this.ItemEquipActiveCard.transform.localPosition = pos;
		this.ItemEquipActiveCard = active.ItemEquipActiveCard;
		this.CardIndex = active.CardIndex;
		this.CardID = active.CardID;
		this.CardSN = active.CardSN;
		this.CardLV = active.CardLV;
	}

	public void SetData (TUICard uiCard, GameObject obj) {
		this.ItemEquipActiveCard = obj;
		this.CardID = uiCard.skillCard.Skill.ID;
		this.CardIndex = uiCard.CardIndex;
		this.CardLV = uiCard.skillCard.Skill.Lv;
		this.CardSN = uiCard.skillCard.Skill.SN;
	}

	public bool CheckBeInstall { 
		get{return ItemEquipActiveCard != null;}
	}

	public string GetSelfName{
		get{
			if(ItemEquipActiveCard != null)
				return ItemEquipActiveCard.name;
			else
				return "";
		}
	}
}
/// <summary>
/// TActiveSkillCard指的是左列卡牌 .
/// </summary>
public struct TUICard{
	public GameObject Card;
	public TActiveSkillCard skillCard;
	public int CardIndex;
	public int Cost;

	private bool recordIsEquip;
	public bool RecordIsEquip {
		get {return recordIsEquip;}
	}

	public void SetIsEquip (bool isEquip) {
		recordIsEquip = isEquip;
	}

	public void SetCoin (int money) {
		skillCard.SetCoin(money);
	}

	public void Init (GameObject obj, int index, TSkill skill, bool isEquip, EventDelegate btnFunc = null, UIEventListener.VoidDelegate suitCardBtn = null, UIEventListener.VoidDelegate suitItemBtn = null) {
		Card = obj;
		CardIndex = index;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			Cost = Mathf.Max(GameData.DSkillData[skill.ID].Space(skill.Lv), 1);

			skillCard = new TActiveSkillCard();
			skillCard.Init(obj, btnFunc, true);

			if(suitCardBtn != null)
				skillCard.UpdateSuitCardButton(suitCardBtn);
			if(suitItemBtn != null)
				skillCard.UpdateSuitItemButton(suitItemBtn);

			skillCard.UpdateSuitCardLight(GameData.DSkillData[skill.ID].SuitCard);
			skillCard.UpdateSuitItem(GameData.DSkillData[skill.ID].Suititem);
			UpdateRedPoint(isEquip, skill);
		}
	}

	public void UpdateRedPoint (bool isEquip, TSkill skill) {
		recordIsEquip = isEquip;
		//合成不須判斷紅點(20160324 GameData.Team.IsExtraCard)
		if(!isEquip) {
			skillCard.CheckRedPoint =  ((GameData.Team.IsEnoughMaterial(skill) && skill.Lv == GameData.DSkillData[skill.ID].MaxStar && GameData.IsOpenUIEnableByPlayer(EOpenID.SkillEvolution)) || GameData.Team.CheckCardCost(skill));
		} else {
			skillCard.CheckRedPoint =  (GameData.Team.IsEnoughMaterial(skill) && skill.Lv == GameData.DSkillData[skill.ID].MaxStar && GameData.IsOpenUIEnableByPlayer(EOpenID.SkillEvolution));
		}
	}

	public void RefreshRedPoint (int currentCostSpace, bool isEquip, TSkill skill ) { // For All
		//合成不須判斷紅點(20160324 UISkillFormation.Get.CheckCardnoInstallIgnoreSelf(Card.name))
		if(!isEquip) {
			skillCard.CheckRedPoint =  ((GameData.Team.IsEnoughMaterial(skillCard.Skill) && skill.Lv == GameData.DSkillData[skill.ID].MaxStar && GameData.IsOpenUIEnableByPlayer(EOpenID.SkillEvolution)) || Cost <= currentCostSpace);
		} else {
			skillCard.CheckRedPoint =  (GameData.Team.IsEnoughMaterial(skillCard.Skill) && skill.Lv == GameData.DSkillData[skill.ID].MaxStar && GameData.IsOpenUIEnableByPlayer(EOpenID.SkillEvolution));
		}
	}
}

public struct TSkillCardSell {
	public UILabel LabelSell;
	public GameObject SellCount;
	public UILabel LabelTotalPrice;
	public int SellPrice;

	public void Init() {
		SellCount.SetActive(false);
		SellPrice = 0;
	}

	public void Refresh (bool isEdit) {
		SellCount.SetActive(isEdit);
		SellPrice = 0;
		LabelTotalPrice.text = "0";
		if(isEdit)
			LabelSell.text = "SELLX0";
		else 
			LabelSell.text = "SELL";
	}

	public void AddSell (int money, int count) {
		SellPrice += money;
		LabelTotalPrice.text = SellPrice.ToString();
		LabelSell.text = "SELLX"+count.ToString();
	}
}

public struct TSkillFormationRight {
	public UILabel LabelLock;
	public UIGrid GridPassiveCardBase;
	public UIScrollView ScrollViewItemList;
	public UISprite PassiveCheck;
	public UISprite ActiveCheck;
	public GameObject ActiveLock;
	public GameObject ItemPassiveField;
	public GameObject ItemPassiveSelected;
	public UIToggle[] ToggleCheckBoxSkill;

	public void Init () {
		ToggleCheckBoxSkill = new UIToggle[2];
	}

	public void RefreshPassiveItem () {
		GridPassiveCardBase.Reposition();
		GridPassiveCardBase.repositionNow = true;
		ScrollViewItemList.restrictWithinPanel = false;
		ScrollViewItemList.restrictWithinPanel = true;
	}

	public void UpdateSort(int eFilter) {
		switch(eFilter) {
		case (int)EFilter.All:
			ToggleCheckBoxSkill[0].value = true;
			ToggleCheckBoxSkill[1].value = true;
			ActiveCheckShow(true);
			PassiveCheckShow(true);
			break;
		case (int)EFilter.Active:
			ToggleCheckBoxSkill[0].value = true;
			ToggleCheckBoxSkill[1].value = false;
			ActiveCheckShow(true);
			PassiveCheckShow(false);
			break;
		case (int)EFilter.Passive:
			ToggleCheckBoxSkill[0].value = false;
			ToggleCheckBoxSkill[1].value = true;
			ActiveCheckShow(false);
			PassiveCheckShow(true);
			break;
		case (int)EFilter.Available:
		case (int)EFilter.Select:
			ToggleCheckBoxSkill[0].value = false;
			ToggleCheckBoxSkill[1].value = false;
			ActiveCheckShow(false);
			PassiveCheckShow(false);
			break;
		}
	}

	public void ActiveCheckShow (bool isClick){
		if(isClick)
			ActiveCheck.spriteName = "button_orange1";
		else
			ActiveCheck.spriteName = "button_gray";
	}

	public void PassiveCheckShow (bool isClick){
		if(isClick)
			PassiveCheck.spriteName = "button_orange1";
		else
			PassiveCheck.spriteName = "button_gray";
	}

	public bool ItemPassiveSelectedVisible {
		set {ItemPassiveSelected.SetActive(value);}
	} 

	public bool ItemPassiveFieldVisible {
		set {ItemPassiveField.SetActive(value);}
	} 

	public bool ActiveLockVisible {
		set {ActiveLock.SetActive(value);}
	}
}


public class UISkillFormation : UIBase {
	private static UISkillFormation instance = null;
	private const string UIName = "UISkillFormation";

	//Send Value
	private int[] removeIndexs = new int[0]; //From already setted skillCard's index
	private int[] addIndexs = new int[0];//From skillCards's index in the center area
	private int[] orderSNs = new int[0];//From activeStruct index
	private int[] sellIndexs = new int[0];

	//Sell Value
	private List<int> sellNames = new List<int>();
	
	//Instantiate Object
	private GameObject itemSkillCard;
	private GameObject itemCardEquipped;
	
	//Original for compare DoFinish
	private List<int> activeOriginalSN = new List<int>();
	private List<GameObject> skillOriginalCards = new List<GameObject>();//By Sort
	private List<string> skillsOriginal = new List<string>();//record alread equiped   rule: Index _ ID _ SN _ LV

	private List<GameObject> skillSortCards = new List<GameObject>();//By Sort
	private List<string> skillsRecord = new List<string>();

	//key:Index _ ID _ SN _ LV Value: TSkill   For Get Level
	//only record skillsOriginal(Equiped) 0:Not Equiped 1:Equiped (First Time)
	private Dictionary<string, TSkill> skillActiveCards = new Dictionary<string, TSkill>(); 
	private Dictionary<string, TSkill> skillPassiveCards = new Dictionary<string, TSkill>();
	private Dictionary<string, TUICard> uiCards = new Dictionary<string, TUICard>();

	public TActiveStruct[] activeStruct = new TActiveStruct[3];//Record of Active
	private List<GameObject> itemPassiveCards = new List<GameObject>(); // 目前擁有的被動技卡牌
	private TSkillFormationRight viewRight;
	//Total Cost
	private UILabel labelCostValue;
	private UILabel labelFrameCount;
	//Left CardView
	private GameObject gridCardList;
	private UIScrollView scrollViewCardList;
	private UIBetterGrid betterGrid;
	//Sell
	public bool IsBuyState = false;
	private TSkillCardSell cardSell;
	//Desk(Page)
	private UIToggle[] toggleDecks = new UIToggle[5];

	private int costSpace = 0;
	private int costSpaceMax = GameConst.Max_CostSpace;
	private int activeFieldLimit = 3;
	private int eCondition;
	private int eFilter;

	//page
	private int tempPage = 0;
	private bool isChangePage = false;
	private bool isLeave = false;

	public bool IsDragNow = false;
	public bool IsCardActive = false;//For UISKskillsOriginalillCardDrag

	private float runShineInternal = 5f;
	private float runShine = 0;

	//ForReinforce
	private bool isReinforce = false;
	private bool isEvolution = false;
	private int infoIndex = -1;
	private bool isAlreadyEquip = false;

	void OnDestroy() {
		activeOriginalSN.Clear();
		sellNames.Clear();
		skillOriginalCards.Clear();
		skillSortCards.Clear();
		skillsOriginal.Clear();
		skillsRecord.Clear();
		skillActiveCards.Clear();
		skillPassiveCards.Clear();
		uiCards.Clear();
	}

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

        set
        {
            if (instance) {
                if (!value)
                    RemoveUI(instance.gameObject);
                else
                    instance.Show(value);
            } else
            if (value)
                Get.Show(value);

            if(value)
            {
                Statistic.Ins.LogScreen(11);
                Statistic.Ins.LogEvent(108);
            }
        }
	}
	
	public static UISkillFormation Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillFormation;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			instance.Show(isShow);
		} else
		if (isShow)
			Get.Show(isShow);
    }

	void FixedUpdate () {
		if(runShine > 0) {
			runShine -= Time.deltaTime;
			if(runShine <= 0) {
				runShineCard ();
				runShine = runShineInternal;
			}
		}
	}

	protected override void InitCom() {
		itemSkillCard = Resources.Load(UIPrefabPath.ItemSkillCard) as GameObject;
		itemCardEquipped = Resources.Load(UIPrefabPath.ItemCardEquipped) as GameObject;

		tempPage = GameData.Team.Player.SkillPage;

		for(int i=0; i<toggleDecks.Length; i++) {
			toggleDecks[i] = GameObject.Find(UIName + "/Center/MainView/Right/DecksList/DecksBtn"+ i.ToString()).GetComponent<UIToggle>();
			toggleDecks[i].name = i.ToString();
			UIEventListener.Get (toggleDecks[i].gameObject).onClick = OnChangePage;
			toggleDecks[i].value = (i == tempPage);
			toggleDecks[i].gameObject.SetActive((i<2));// it need judge by player level
		}
		for(int i=0; i<activeStruct.Length; i++) {
			activeStruct[i].Init(UIName, i);
		}

		viewRight.Init();
		viewRight.ActiveLock = GameObject.Find(UIName + "/Center/MainView/Right/ActiveCardBase2/Lock");
		viewRight.LabelLock = GameObject.Find(UIName + "/Center/MainView/Right/ActiveCardBase2/Lock/TabLabel").GetComponent<UILabel>();
		viewRight.GridPassiveCardBase = GameObject.Find (UIName + "/Center/MainView/Right/PassiveCardBase/PassiveList/UIGrid").GetComponent<UIGrid>();
		viewRight.ScrollViewItemList = GameObject.Find (UIName + "/Center/MainView/Right/PassiveCardBase/PassiveList").GetComponent<UIScrollView>();
		viewRight.ToggleCheckBoxSkill[0] = GameObject.Find (UIName + "/Center/MainView/Right/STitle/ActiveCheck").GetComponent<UIToggle>();
		viewRight.ToggleCheckBoxSkill[1] = GameObject.Find (UIName + "/Center/MainView/Right/STitle/PassiveCheck").GetComponent<UIToggle>();
		viewRight.ItemPassiveField = GameObject.Find (UIName + "/Center/MainView/Right/PassiveCardBase/PassiveField/Icon");
		viewRight.ItemPassiveSelected = GameObject.Find (UIName + "/Center/MainView/Right/PassiveCardBase/PassiveField/Selected");
		viewRight.ActiveCheck = GameObject.Find (UIName + "/Center/MainView/Right/STitle/ActiveCheck/Background/Btn").GetComponent<UISprite>();
		viewRight.PassiveCheck = GameObject.Find (UIName + "/Center/MainView/Right/STitle/PassiveCheck/Background/Btn").GetComponent<UISprite>();
		viewRight.ScrollViewItemList.onDragFinished = ItemDragFinish;
		viewRight.ItemPassiveSelectedVisible = false;
		viewRight.ItemPassiveField.transform.parent.name = "4";
		UIEventListener.Get(GameObject.Find (UIName + "/Center/MainView/Right/STitle/ActiveCheck")).onClick = DoOpenActive;
		UIEventListener.Get(GameObject.Find (UIName + "/Center/MainView/Right/STitle/PassiveCheck")).onClick = DoOpenPassive;

		labelCostValue = GameObject.Find (UIName + "/Center/LabelCost/CostValue").GetComponent<UILabel>();
		labelFrameCount = GameObject.Find (UIName + "/Center/FrameCount").GetComponent<UILabel>();

		gridCardList = GameObject.Find (UIName + "/Center/CardsView/Left/CardsGroup/CardList/Grid");
		betterGrid = GameObject.Find (UIName + "/Center/CardsView/Left/CardsGroup/CardList/Grid").GetComponent<UIBetterGrid>();
		scrollViewCardList = GameObject.Find (UIName + "/Center/CardsView/Left/CardsGroup/CardList").GetComponent<UIScrollView>();
		scrollViewCardList.onDragStarted = CardDragStart;
		scrollViewCardList.onDragFinished = CardDragFinish;
		scrollViewCardList.onStoppedMoving = CardDragEnd;

		cardSell = new TSkillCardSell();
		cardSell.LabelSell = GameObject.Find (UIName + "/Center/SellBtn/Icon").GetComponent<UILabel>();
		cardSell.SellCount = GameObject.Find (UIName + "/Center/SellBtn/SellCount");
		cardSell.LabelTotalPrice = GameObject.Find (UIName + "/Center/SellBtn/SellCount/TotalPrice").GetComponent<UILabel>();
		cardSell.Init();

		GameObject.Find(UIName + "/Center/SortBtn").SetActive(false);

		SetBtnFun (UIName + "/Center/SortBtn", DoSort);
		SetBtnFun (UIName + "/BottomLeft/BackBtn", DoBack);
		SetBtnFun (UIName + "/Center/SellBtn", DoSellState);
		SetBtnFun (UIName + "/Center/SellBtn/SellCount/CancelBtn", DoCloseSell);

		initCards ();
		PlayerPrefs.SetInt (ESave.SkillCardFilter.ToString(), EFilter.All.GetHashCode());
		UpdateSort();
		refreshFrameCount ();
		CardDragFinish();
		//企劃說目前不需要所以先隱藏(20160309)
		labelFrameCount.gameObject.SetActive(false);
		GameObject.Find(UIName + "/Center/SellBtn").SetActive(false);
	}
	
	protected override void OnShow(bool isShow) {
		if(PlayerPrefs.HasKey(ESave.NewCardFlag.ToString()))
		{
			PlayerPrefs.DeleteKey(ESave.NewCardFlag.ToString());
			PlayerPrefs.Save();

			if (UISelectRole.Visible)
				UISelectRole.Get.DisableRedPoint();
		}

		refreshRedPoint();
		isChangePage = false;
		isLeave = false;
	}

	public void ShowView (int tab = 0, int suitId = 0) {
		Visible = true;
	}

	private void hide() {
		if(UISort.Visible)
			UISort.UIShow(false);

		if (!UISelectRole.Visible)
			UIMainLobby.Get.Show();
		else
			UISelectRole.Get.InitPartnerPosition();
		
		Visible = false;  
	}

	private void runShineCard () {
		if(skillsRecord.Count > 0) 
			for (int i=0; i<skillsRecord.Count; i++) 
				if(uiCards.ContainsKey(skillsRecord[i])) 
					if(uiCards[skillsRecord[i]].skillCard.IsInstall) 
						uiCards[skillsRecord[i]].skillCard.LightAnimation.Play();
	}

	private void refresh(){
		costSpace = 0;
		betterGrid.mTrans.DestroyChildren();
		removeIndexs = new int[0];
		addIndexs = new int[0];
		orderSNs = new int[0];
		sellIndexs = new int[0];

		activeOriginalSN.Clear();
		sellNames.Clear();
		skillOriginalCards.Clear();
		skillSortCards.Clear();
		skillsOriginal.Clear();
		skillsRecord.Clear();
		skillActiveCards.Clear();
		skillPassiveCards.Clear();
		uiCards.Clear();
		for (int i=0; i<activeStruct.Length; i++) {
			if(activeStruct[i].ItemEquipActiveCard != null)
				Destroy(activeStruct[i].ItemEquipActiveCard);
			activeStruct[i].SpriteActiveFieldIcon.gameObject.SetActive(true);
			activeStruct[i].ActiveClear();
		}
		for (int i=0; i<itemPassiveCards.Count; i++) {
			Destroy(itemPassiveCards[i]);
		}
		itemPassiveCards.Clear();
		viewRight.ItemPassiveFieldVisible = true;
	}

	public void RefreshAddCard () {
		refresh();
		initCards ();
		refreshFrameCount ();
		CardDragFinish();
	}

	private void refreshAfterInstall () {
		refresh();
		initCards ();
		refreshFrameCount ();
		CardDragFinish();
	}

	private void refreshBeforeSell () {
		refresh();
		initCards (true);
		setEditState(true);
		refreshFrameCount ();
		CardDragFinish();
	}
	
	private void refreshAfterSell () {
		refresh();
		DoCloseSell();
		initCards ();
		refreshFrameCount ();
		CardDragFinish();
	}

	private void initCards (bool isCheckBuy = false) {
//		costSpaceMax = GameData.Team.Player.MaxSkillSpace;
		runShine = runShineInternal;
		int index = -1;
		int actvieIndex = -1;
		//Already Equiped
		if(GameData.Team.Player.SkillCards != null && GameData.Team.Player.SkillCards.Length > 0) {
			for (int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
				if (GameData.DSkillData.ContainsKey(GameData.Team.Player.SkillCards[i].ID)) {
					GameObject obj = null;
					index ++;
					obj = addUICards(i,
					                 index, 
					                 GameData.Team.Player.SkillCards[i],
					                 gridCardList, 
					                 true);
					if(obj != null) {
						skillsOriginal.Add (obj.name);
						if(GameFunction.IsActiveSkill(GameData.Team.Player.SkillCards[i].ID)) {
							if(!skillActiveCards.ContainsKey(obj.name)) {
								actvieIndex++;
								addItems(uiCards[obj.name], actvieIndex);
								activeOriginalSN.Add(uiCards[obj.name].skillCard.Skill.SN);
								skillActiveCards.Add(obj.name, GameData.Team.Player.SkillCards[i]);
							}
						} else {
							if(!skillPassiveCards.ContainsKey (obj.name)) {
								addItems(uiCards[obj.name]);
								skillPassiveCards.Add(obj.name, GameData.Team.Player.SkillCards[i]);
							}
						}
						skillOriginalCards.Add(obj);
						skillSortCards.Add(obj);
					} else 
						index -- ;
				}
			}
		}
		// not equiped
		if(GameData.Team.SkillCards != null && GameData.Team.SkillCards.Length > 0) {
			TSkill[] sortSkillCards = new TSkill[GameData.Team.SkillCards.Length];
			sortSkillCardByID(sortSkillCards);
			for(int i=0; i<sortSkillCards.Length; i++) {
				GameObject obj = null;
				
				if(GameData.DSkillData.ContainsKey(sortSkillCards[i].ID) && 
					!isSkillCardInOtherPlayer(sortSkillCards[i].SN)) {
					index ++;
					obj = addUICards(sortSkillCards[i].Index,
					                 index, 
									 sortSkillCards[i],
					                 gridCardList, 
					                 false);
					if(obj != null) {
						if(GameFunction.IsActiveSkill(sortSkillCards[i].ID)) {
							if(!skillActiveCards.ContainsKey (obj.name)) {
								skillActiveCards.Add(obj.name, sortSkillCards[i]);
								skillOriginalCards.Add(obj);
								skillSortCards.Add(obj);
							}
						} else {
							if(!skillPassiveCards.ContainsKey(obj.name)) {
								skillPassiveCards.Add(obj.name, sortSkillCards[i]);
								skillOriginalCards.Add(obj);
								skillSortCards.Add(obj);
							}
						}
					} else 
						index --;
				}
			}
		}
		checkCostIfMask();
		labelCostValue.text = costSpace + "/" + costSpaceMax;

		betterGrid.init();
		betterGrid.mChildren = skillSortCards;
		resetScrollPostion ();
		refreshActiveItems ();
		viewRight.RefreshPassiveItem();
		refreshRedPoint();

		if(IsOpenThirdActive) 
			activeFieldLimit = 3;
		else {
			activeFieldLimit = 2;
			viewRight.LabelLock.text = string.Format(TextConst.S(7013), LimitTable.Ins.GetLv(EOpenID.ThirdActive));
		}

		viewRight.ActiveLockVisible = !IsOpenThirdActive;
		activeStruct[activeStruct.Length - 1].SpriteActiveFieldIcon.gameObject.SetActive(IsOpenThirdActive);
	}

	private void sortSkillCardByID (TSkill[] skills) {
		if(skills.Length == GameData.Team.SkillCards.Length) {
			for(int i=0; i<skills.Length; i++) {
				skills[i] = GameData.Team.SkillCards[i];
				skills[i].Index = i;
			}
			
			for(int i=0; i<skills.Length; i++) {
				for (int j=i+1; j<skills.Length; j++){
					if (skills[i].ID >= skills[j].ID){
						TSkill temp = skills[i];
						skills[i] = skills[j];
						skills[j] = temp;
					}
				}
			}
		}
	}

	private bool IsEquip (string name) {
		if(skillActiveCards.ContainsKey(name) || skillPassiveCards.ContainsKey(name))
			return true;

		return false;
	}

	private void refreshFrameCount () {
		labelFrameCount.text = GetCardFrameCount + "/" + GameData.Team.SkillCardMax;
	}

	private int getActiveFieldNull{
		get {
			for(int i=0; i<activeStruct.Length; i++)
				if(activeStruct[i].ItemEquipActiveCard == null)
					return i;
			return -1;
		}
	}

	private int getContainActiveSN (int sn) {
		for(int i=0; i<activeStruct.Length; i++) 
			if(activeStruct[i].ItemEquipActiveCard != null)
				if(activeStruct[i].CardSN == sn)
					return i;

		return -1;
	}

	private void checkCostIfMask() {
		foreach (KeyValuePair<string, TUICard> uicard in uiCards){
			if(skillsRecord.Contains(uicard.Value.Card.name)) {
				uicard.Value.skillCard.IsInstall = true;
				uicard.Value.skillCard.IsCanUse = false;
			} else {
				uicard.Value.skillCard.IsInstall = false;
				uicard.Value.skillCard.IsCanUse = (uicard.Value.Cost > (costSpaceMax - costSpace));
			}
		}
	}

	public bool CheckCardUsed (string name) {
		string nameSplit = name.Replace("(Clone)", "");
		if (uiCards.ContainsKey(nameSplit))
			return uiCards[nameSplit].skillCard.IsInstall;
		else
			return false;
	}

	private int getActiveInstall {
		get {
			int count = 0;
			for(int i=0; i<activeStruct.Length; i++) 
				if(activeStruct[i].ItemEquipActiveCard != null)
					count++;
			 
			return count;
		}
	}

	private bool checkCost (int space) {
		int temp = costSpace;
		if((temp + space) <= costSpaceMax) {
			temp += space;
			return true;
		} else 
			return false;
	}

	private bool setCost(int space) {
		if((costSpace + space) <= costSpaceMax) {
			costSpace += space;
			labelCostValue.text = costSpace + "/" + costSpaceMax; 
			return true;
		} else 
			return false;
	}

	public void OnSuitCard (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			if(GameData.IsOpenUIEnableByPlayer(EOpenID.SuitCard)) 
				UISuitAvatar.Get.ShowView(1, 0, result);
			else 
				UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.SuitCard)),LimitTable.Ins.GetLv(EOpenID.SuitCard)) , Color.black);
		}

	}

	public void OnSuitItem (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			if(GameData.IsOpenUIEnableByPlayer(EOpenID.SuitItem)) 
				UISuitAvatar.Get.ShowView(result, 1);
			else 
				UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.SuitItem)),LimitTable.Ins.GetLv(EOpenID.SuitItem)) , Color.black);
		}
	}

	private GameObject addUICards (int skillCardIndex, int positionIndex, TSkill skill, GameObject parent, bool isEquip) {
		if (GameData.DSkillData.ContainsKey(skill.ID)) {
			string name = skillCardIndex.ToString() + "_" + skill.ID.ToString()+ "_" + skill.SN.ToString() + "_" + skill.Lv.ToString();
			if(!uiCards.ContainsKey(name)) {
				GameObject obj = Instantiate(itemSkillCard, Vector3.zero, Quaternion.identity) as GameObject;
				obj.transform.name = name;
				obj.transform.parent = parent.transform;
				obj.transform.localPosition = new Vector3(-230 + 200 * (positionIndex / 2), 100 - 265 * (positionIndex % 2), 0);
				obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

				UISkillCardDrag drag = obj.AddComponent<UISkillCardDrag>();
				drag.cloneOnDrag = true;
				drag.restriction = UIDragDropItem.Restriction.PressAndHold;
				drag.pressAndHoldDelay = 0.25f;
				
				TUICard uicard = new TUICard();
				uicard.Init(obj, skillCardIndex, skill, isEquip, new EventDelegate(OnCardDetailInfo), OnSuitCard, OnSuitItem);
				uicard.skillCard.UpdateViewFormation(skill, isEquip);
				uiCards.Add(obj.transform.name, uicard);
				return obj;
			}
		}
		return null;
	}
	//TPassiveSkillCard指的是右列安裝的卡牌 .
	private GameObject addUIItems (TUICard uicard, GameObject parent, int positionIndex = 0) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = uicard.CardIndex.ToString() + "_" + uicard.skillCard.Skill.ID.ToString() + "_" + uicard.skillCard.Skill.SN.ToString() + "_" + uicard.skillCard.Skill.Lv.ToString();
		if(GameFunction.IsActiveSkill(uicard.skillCard.Skill.ID)) {
			obj.transform.localPosition = Vector3.zero;
			UISkillCardDrag drag = obj.AddComponent<UISkillCardDrag>();
			drag.restriction = UIDragDropItem.Restriction.Vertical;
			drag.isDragItem = true;
		} else 
			obj.transform.localPosition = new Vector3(0, 110 - 70 * positionIndex, 0);
		
		TPassiveSkillCard skillCard = new TPassiveSkillCard();
		skillCard.InitFormation(obj);
		UIEventListener.Get(skillCard.BtnRemove).onPress = OnRemoveItem;
		skillCard.UpdateViewFormation(uicard.skillCard.Skill.ID, uicard.skillCard.Skill.Lv);

		UIEventListener.Get(obj).onClick = OnItemDetailInfo;
		return obj;
	}

	/// <summary>
	/// CardIndex : index  ,  CardId: id   ,  lv  ,  equiptype: 
	/// </summary>
	/// <returns><c>true</c>, if items was added, <c>false</c> otherwise.</returns>
	/// <param name="cardIndex">Card index.</param>
	/// <param name="cardId">Card identifier.</param>
	/// <param name="lv">Lv.</param>
	/// <param name="equiptype">Equiptype.</param>
	private bool addItems(TUICard uicard, int activeStructIndex = -1) {
		if(setCost(Mathf.Max(GameData.DSkillData[uicard.skillCard.Skill.ID].Space(uicard.skillCard.Skill.Lv), 1))) {
			GameObject obj = null;
			if(!GameFunction.IsActiveSkill(uicard.skillCard.Skill.ID)) {
				obj = addUIItems(uicard, viewRight.GridPassiveCardBase.gameObject, itemPassiveCards.Count);
				if(obj != null) {
					itemPassiveCards.Add(obj);
					viewRight.ItemPassiveFieldVisible = false;
				}
			} else {
				if(activeStructIndex != -1 && activeStructIndex < 3) {
					obj = addUIItems(uicard, activeStruct[activeStructIndex].GridActiveCardBase);
					if(obj != null) {
						activeStruct[activeStructIndex].SetData(uicard, obj);
						activeStruct[activeStructIndex].SpriteActiveFieldIcon.gameObject.SetActive(false);
					}
				}
			}
			if(obj != null) 
				if(!skillsRecord.Contains (obj.name))
					skillsRecord.Add(obj.name);
			
			checkCostIfMask();
			uicard.UpdateRedPoint(true, uicard.skillCard.Skill);
			uicard.SetIsEquip(true);
			return true;
		} 
		return false;
	}

	public void AddItem(GameObject go, int index) {
		string name = go.name.Replace("(Clone)", "");
		if(uiCards.ContainsKey(name)) {
			if(!uiCards[name].skillCard.IsInstall) {
				if(index < 4){
					//Active
					if(getActiveInstall == activeFieldLimit) {
						//Delete
						bool flag = false;
						index --;
						if(activeStruct[index].CheckBeInstall) {
							if(checkCost(-uiCards[activeStruct[index].ItemEquipActiveCard.name].Cost))
								if(checkCost(uiCards[name].Cost))
									flag = true;

							if(flag) {
								if(setCost(-uiCards[activeStruct[index].ItemEquipActiveCard.name].Cost)) {
									if(skillsRecord.Contains(activeStruct[index].GetSelfName))
										skillsRecord.Remove(activeStruct[index].GetSelfName);
									
									Destroy(activeStruct[index].ItemEquipActiveCard);
									activeStruct[index].ActiveClear();
									if(addItems(uiCards[name], index)){
										AudioMgr.Get.PlaySound(SoundType.SD_Compose);
									}
								}
							}
						} else {
							removeItems(activeStruct[activeFieldLimit - 1].CardID, activeStruct[activeFieldLimit - 1].CardSN, activeStruct[activeFieldLimit - 1].ItemEquipActiveCard);
							if(addItems(uiCards[name], activeFieldLimit - 1)) 
								AudioMgr.Get.PlaySound(SoundType.SD_Compose);
							
						}
					} else {
						if(!activeStruct[index].CheckBeInstall) {
							if(checkCost(uiCards[name].Cost)) 
								if(addItems(uiCards[name], index))
									AudioMgr.Get.PlaySound(SoundType.SD_Compose);
							
						} else {
							if(checkCost(uiCards[name].Cost)) {
								for (int i=0; i<activeStruct.Length; i++) {
									if(i == index) {
										for (int j=activeStruct.Length-1; j>i; j--) {
											activeStruct[j - 1].ItemEquipActiveCard.transform.parent = activeStruct[j].GridActiveCardBase.transform;
											activeStruct[j].SetData(activeStruct[j - 1], Vector3.zero);
											activeStruct[j].SpriteActiveFieldIcon.gameObject.SetActive((!activeStruct[j].CheckBeInstall));
										}
										break;
									}
								}
								if(addItems(uiCards[name], index)) 
									AudioMgr.Get.PlaySound(SoundType.SD_Compose);
								
							}
						}
						refreshActiveItems();
					}
				} else {
					//Passive
					if(addItems(uiCards[name])) {
						AudioMgr.Get.PlaySound(SoundType.SD_Compose);
						viewRight.RefreshPassiveItem();
					}
						
				}
			}
			refreshRedPoint();
			refreshFrameCount ();
		}
	}

	public void SwitchItem(int sourceIndex, int targetIndex){
		if(sourceIndex != targetIndex && sourceIndex != -1 && targetIndex != -1) {
			TActiveStruct temp = activeStruct[sourceIndex];
			if(activeStruct[targetIndex].ItemEquipActiveCard != null) {
				activeStruct[sourceIndex].SetData(activeStruct[targetIndex], Vector3.zero, activeStruct[targetIndex].GridActiveCardBase.transform);
				activeStruct[targetIndex].SetData(temp, Vector3.zero, temp.GridActiveCardBase.transform);
			} else {
				activeStruct[targetIndex].SetData(temp, Vector3.zero, temp.GridActiveCardBase.transform);
				activeStruct[sourceIndex].ActiveClear();
			}

			refreshActiveItems();
		}
	}

	public void ShowInstallLight (GameObject go, bool isShow) {
		string name = go.name.Replace("(Clone)", "");
		if(uiCards.ContainsKey (name)) {
			if(GameFunction.IsActiveSkill(uiCards[name].skillCard.Skill.ID)) {
				IsCardActive = true;
				for(int i=0; i<activeStruct.Length; i++) 
					if((i+1) <= activeFieldLimit)
						activeStruct[i].ItemActiveSelect.SetActive(isShow);
				
			} else {
				IsCardActive = false;
				viewRight.ItemPassiveSelectedVisible = isShow;
			}
		}
	}

	private void removeItems(int id, int sn, GameObject go = null) {
		if(uiCards.ContainsKey(go.name)) {
			if(setCost(-uiCards[go.name].Cost)){
				if(skillsRecord.Contains(go.name))
					skillsRecord.Remove(go.name);
				
				if(!GameFunction.IsActiveSkill(id)) {
					for(int i=0 ;i<itemPassiveCards.Count; i++) {
						if(itemPassiveCards[i].name.Equals(go.name)){
							Destroy(itemPassiveCards[i]);
							itemPassiveCards.RemoveAt(i);
							break;
						}
					}
					
					if(itemPassiveCards.Count == 0)
						viewRight.ItemPassiveFieldVisible  = true;
					
					viewRight.RefreshPassiveItem();
				} else {
					int index = getContainActiveSN(sn);
					if(activeStruct[index].CheckBeInstall) {
						if(skillsRecord.Contains(activeStruct[index].GetSelfName))
							skillsRecord.Remove(activeStruct[index].GetSelfName);
						
						Destroy(activeStruct[index].ItemEquipActiveCard);
					}
					activeStruct[index].ActiveClear();
					refreshActiveItems();
				}
				checkCostIfMask();
			}
			uiCards[go.name].UpdateRedPoint(false, uiCards[go.name].skillCard.Skill);
			uiCards[go.name].SetIsEquip(false);
			refreshFrameCount ();
		}
	}
	
	private void refreshCards() {
		for(int i=0 ;i<skillSortCards.Count; i++) 
			uiCards[skillSortCards[i].name].skillCard.IsInstall = skillsRecord.Contains(uiCards[skillSortCards[i].name].Card.name);
	}

	private void refreshActiveItems() {
		if(activeStruct.Length > 1) {
			for (int i=0; i<activeStruct.Length; i++) {
				if(!activeStruct[i].CheckBeInstall) {
					for (int j=i+1; j<activeStruct.Length; j++) {
						if(activeStruct[j].CheckBeInstall) {
							TActiveStruct temp = activeStruct[j];
							activeStruct[i].SetData(temp, Vector3.zero, activeStruct[i].GridActiveCardBase.transform);
							activeStruct[j].ActiveClear();
							activeStruct[i].SpriteActiveFieldIcon.gameObject.SetActive(false);
							activeStruct[j].SpriteActiveFieldIcon.gameObject.SetActive(true);
							break;
						} else {
							activeStruct[i].SpriteActiveFieldIcon.gameObject.SetActive(true);
							activeStruct[j].SpriteActiveFieldIcon.gameObject.SetActive(true);
						}
					}
				}
			}
		}
			
		viewRight.ActiveLockVisible = !IsOpenThirdActive;
		activeStruct[activeStruct.Length - 1].SpriteActiveFieldIcon.gameObject.SetActive(IsOpenThirdActive);
	}

	//page 0 1 2 3 4
	private void changePage (int page) {
		if(page != tempPage) {
			for(int i=0; i<toggleDecks.Length; i++) 
				toggleDecks[i].value = (i == page);

			isChangePage = true;
			tempPage = page;
			DoFinish();
		}
	}

	//For Sell
	private bool isSkillCardCanSell(int sn) {
		if(GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 0) {
			for (int i=0; i<GameData.Team.PlayerBank.Length; i++) {
				if(GameData.Team.PlayerBank[i].ID != GameData.Team.Player.ID) {
					if(GameData.Team.PlayerBank[i].SkillCardPages != null && GameData.Team.PlayerBank[i].SkillCardPages.Length > 0) {
						for (int j=0; j<GameData.Team.PlayerBank[i].SkillCardPages.Length; j++) {
							int[] SNs = GameData.Team.PlayerBank[i].SkillCardPages[j].SNs;
							if (SNs.Length > 0) {
								for (int k=0; k<SNs.Length; k++)
									if (SNs[k] == sn)
										return false;
							}
						}
					}
				}
			}
		}

		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) 
			if(GameData.Team.Player.SkillCards[i].SN == sn)
				return false;

		return true;
	}

	private bool isSkillCardInOtherPlayer(int sn) {
		if(GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 0) {
			for (int i=0; i<GameData.Team.PlayerBank.Length; i++) {
				if(GameData.Team.PlayerBank[i].ID != GameData.Team.Player.ID) {
					if(GameData.Team.PlayerBank[i].SkillCardPages != null && GameData.Team.PlayerBank[i].SkillCardPages.Length > 0) {
						for (int j=0; j<GameData.Team.PlayerBank[i].SkillCardPages.Length; j++) {
							int[] SNs = GameData.Team.PlayerBank[i].SkillCardPages[j].SNs;
							if (SNs.Length > 0) {
								for (int k=0; k<SNs.Length; k++)
									if (SNs[k] == sn)
										return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	private void resetScrollPostion () {
		scrollViewCardList.Scroll(0);
	}

	private void setEditState (bool isEditState) {
		int index = 0;
		cardSell.Refresh(isEditState);
		IsBuyState = isEditState;
		betterGrid.IsBuy = isEditState;
		if(isEditState) {
			sellNames.Clear();
			index = 0;
			for(int i=0; i<skillSortCards.Count; i++) { 
				if(sortIsCanSell(skillSortCards[i])) {
					uiCards[skillSortCards[i].name].skillCard.ShowSell = true;
					uiCards[skillSortCards[i].name].skillCard.IsCanUse = false;
					if(GameData.DSkillData.ContainsKey(uiCards[skillSortCards[i].name].skillCard.Skill.ID))
						uiCards[skillSortCards[i].name].skillCard.SetCoin(GameData.DSkillData[uiCards[skillSortCards[i].name].skillCard.Skill.ID].Money);
					else 
						uiCards[skillSortCards[i].name].skillCard.SetCoin(100);
					
					skillSortCards[i].transform.localPosition = new Vector3(-230 + 200 * (index / 2), 100 - 265 * (index % 2), 0);
					skillSortCards[i].SetActive(true);
					index++;
				} else
					skillSortCards[i].SetActive(false);
				
			}
			CardDragFinish();
		} else {
			index = 0;
			for(int i=0; i<skillSortCards.Count; i++) {
				if(uiCards.ContainsKey(skillSortCards[i].name)) {
					if(i % 2 == 0)
						index++;
					skillSortCards[i].transform.localPosition = new Vector3(-230 + 200 * (index / 2), 100 - 265 * (index % 2), 0);
					skillSortCards[i].SetActive(true);
					uiCards[skillSortCards[i].name].skillCard.ShowSell = false; 
				}
			}
			refreshAfterInstall();
		}
	}
	
	private void addSellCards (string name) {
		if(uiCards.ContainsKey(name)) {
			if(!sellNames.Contains(uiCards[name].CardIndex))
				sellNames.Add(uiCards[name].CardIndex);
			
			cardSell.AddSell(GameData.DSkillData[uiCards[name].skillCard.Skill.ID].Money, sellNames.Count);
		}
	}
	
	private void removeSellCards (string name) {
		if(uiCards.ContainsKey(name)) {
			if(sellNames.Contains(uiCards[name].CardIndex))
				sellNames.Remove(uiCards[name].CardIndex);

			cardSell.AddSell(-GameData.DSkillData[uiCards[name].skillCard.Skill.ID].Money, sellNames.Count);
		}
	}

	private bool sortIsCanSell (GameObject card) {
		if(uiCards.ContainsKey(card.name))
			return isSkillCardCanSell(uiCards[card.name].skillCard.Skill.SN);
		return false;
	}

	private bool sortIsAvailable(GameObject card) {
		if(uiCards.ContainsKey(card.name))
			return !uiCards[card.name].skillCard.IsInstall;
		return false;
	}

	private bool sortIsSelected(GameObject card) {
		if(uiCards.ContainsKey(card.name))
			return uiCards[card.name].skillCard.IsInstall;
		return false;
	}

	private bool sortIsActive (GameObject card) {
		if(skillActiveCards.ContainsKey(card.name))
			return true;
		return false;
	}

	private bool sortIsPassive (GameObject card) {
		if(skillPassiveCards.ContainsKey(card.name))
			return true;
		return false;
	}
	
	private void sortSkillCondition(int condition) {
		int value1 = 0;
		int value2 = 0;
		if(condition == ECondition.None.GetHashCode()) {
			skillSortCards.Clear();
			for(int i=0; i<skillOriginalCards.Count; i++) 
				skillSortCards.Add(skillOriginalCards[i]);
		} else {
			for(int i=0; i<skillSortCards.Count; i++) {
				for (int j=i+1; j<skillSortCards.Count; j++){
					int cardIdi = uiCards[skillSortCards[i].name].skillCard.Skill.ID;
					int cardIdj = uiCards[skillSortCards[j].name].skillCard.Skill.ID;
					string cardIdistr = uiCards[skillSortCards[i].name].Card.name;
					string cardIdjstr = uiCards[skillSortCards[j].name].Card.name;
					
					if(condition == ECondition.Rare.GetHashCode()) {
						if(uiCards.ContainsKey(cardIdistr))
							value1 = GameData.DSkillData[cardIdi].Quality;
						
						if(uiCards.ContainsKey(cardIdjstr))
							value2 = GameData.DSkillData[cardIdj].Quality;
						
					} else 
					if(condition == ECondition.Kind.GetHashCode()){
						if(GameData.DSkillData.ContainsKey(cardIdi))
							value1 = GameData.DSkillData[cardIdi].Kind;
							
						if(GameData.DSkillData.ContainsKey(cardIdj))
							value2 = GameData.DSkillData[cardIdj].Kind;
							
					} else 
					if(condition == ECondition.Attribute.GetHashCode()){
						if(GameData.DSkillData.ContainsKey(cardIdi))
							value1 = GameData.DSkillData[cardIdi].AttrKind;
								
						if(GameData.DSkillData.ContainsKey(cardIdj))
							value2 = GameData.DSkillData[cardIdj].AttrKind;
								
					}  else 
					if(condition == ECondition.Level.GetHashCode()){
						if(GameData.DSkillData.ContainsKey(cardIdi) && GameData.DSkillData.ContainsKey(cardIdj)){
							if(skillPassiveCards.ContainsKey(cardIdistr)) 
								value1 = skillPassiveCards[cardIdistr].Lv;
							else 
								if(skillActiveCards.ContainsKey(cardIdistr))
									value1 = skillActiveCards[cardIdistr].Lv;
							
							if(skillPassiveCards.ContainsKey(cardIdjstr))
								value2 = skillPassiveCards[cardIdjstr].Lv;
							else
								if(skillActiveCards.ContainsKey(cardIdjstr))
									value2 = skillActiveCards[cardIdjstr].Lv;
						}
					} else 
					if(condition == ECondition.Cost.GetHashCode()){
						if(GameData.DSkillData.ContainsKey(cardIdi) && GameData.DSkillData.ContainsKey(cardIdj)){
							if(skillPassiveCards.ContainsKey(cardIdistr)) 
								value1 = GameData.DSkillData[cardIdi].Space(skillPassiveCards[cardIdistr].Lv);
							else 
								if(skillActiveCards.ContainsKey(cardIdistr))
									value1 = GameData.DSkillData[cardIdi].Space(skillActiveCards[cardIdistr].Lv);
							
							if(skillPassiveCards.ContainsKey(cardIdjstr))
								value2 = GameData.DSkillData[cardIdj].Space(skillPassiveCards[cardIdjstr].Lv);
							else
								if(skillActiveCards.ContainsKey(cardIdjstr))
									value2 = GameData.DSkillData[cardIdj].Space(skillActiveCards[cardIdjstr].Lv);
						}
					}
					
					if (value1 <= value2){
						GameObject temp = skillSortCards[i];
						skillSortCards[i] = skillSortCards[j];
						skillSortCards[j] = temp;
					}
				}
			}
		}
	}
	
	private void sortSkillFilter (int filter){
		int index = 0;
		for(int i=0; i<skillSortCards.Count; i++) { 
			bool result = false;
			switch (filter) {
			case 4://All Choose
				result = true;
				break;

			case 0://Available
				result = sortIsAvailable(skillSortCards[i]);
				break;

			case 1://Select
				result = sortIsSelected(skillSortCards[i]);
				break;
			case 2://Active
				result = sortIsActive(skillSortCards[i]);
				break;

			case 3://Passive
				result = sortIsPassive(skillSortCards[i]);
				break;

			}

			if(result){
				skillSortCards[i].transform.localPosition = new Vector3(-230 + 200 * (index / 2), 100 - 265 * (index % 2), 0);
				index++;
			}

			skillSortCards[i].SetActive(result);
		}
	}

	public void CardDragStart() {IsDragNow = true;}

	public void CardDragFinish() {betterGrid.WrapContent(false);}

	public void CardDragEnd() {IsDragNow = false;}

	public void SetMask (bool isShow, string name) {
		for(int i=0; i<skillSortCards.Count; i++) 
			if(!name.Contains(skillSortCards[i].name)) 
				uiCards[skillSortCards[i].name].skillCard.DragCard = isShow;
	} 

	public void ItemDragFinish(){
		if(itemPassiveCards.Count < 3)
			viewRight.RefreshPassiveItem();
	}

	public int GetCardFrameCount {
		get {
			int count = 0;
			foreach (KeyValuePair<string, TUICard> uicard in uiCards){
				if(!uicard.Value.skillCard.IsInstall)
					count ++;
			}
			return count;
		}
	}

	/// <summary>
	/// 檢查是否有卡片有未安裝, 且不超過costSpace
	/// </summary>
	/// <value><c>true</c> if check cardno install; otherwise, <c>false</c>.</value>
	public bool CheckCardnoInstall{
		get {
			foreach (KeyValuePair<string, TUICard> uicard in uiCards)
				if (!skillsRecord.Contains(uicard.Value.Card.name))
					if(uicard.Value.Cost <= ExtraCostSpace)
						return true;
			
			return false;
		}
	}
	//取得已用空間數
	private int getSkillCost {
		get {
			int cost = 0;
			foreach (KeyValuePair<string, TUICard> uicard in uiCards)
				if (skillsRecord.Contains(uicard.Value.Card.name))
					cost += uicard.Value.Cost;
			
			return cost;
		}
	}
	//檢查空間是否有卡牌可以安裝
	private bool isSurplusCost {
		get {
			int surplus = GameConst.Max_CostSpace - getSkillCost;
			foreach (KeyValuePair<string, TUICard> uicard in uiCards)
				if (!skillsRecord.Contains(uicard.Value.Card.name))
					if(uicard.Value.Cost <= surplus)	
						return true;
				
			return false;
		}
	}
	//檢查是否有未安裝的卡
	private bool isExtraCard {
		get {
			if(GameData.IsOpenUIEnableByPlayer(EOpenID.SkillReinforce))
				foreach (KeyValuePair<string, TUICard> uicard in uiCards)
					if (!skillsRecord.Contains(uicard.Value.Card.name))
						return true;

			return false;
		}	
	}

	public bool CheckCardnoInstallIgnoreSelf (string ignoreName){
		foreach (KeyValuePair<string, TUICard> uicard in uiCards){
			if(!ignoreName.Equals(uicard.Value.Card.name) ){
				if (!skillsRecord.Contains(uicard.Value.Card.name))
						return true;
			}
		}
		return false;
	}

	private void refreshRedPoint () {
		foreach (KeyValuePair<string, TUICard> uicard in uiCards){
			uicard.Value.skillCard.RefreshInListColor(ExtraCostSpace >= uicard.Value.Cost || uicard.Value.skillCard.IsInstall);
			uicard.Value.RefreshRedPoint(ExtraCostSpace, skillsRecord.Contains(uicard.Value.Card.name), uicard.Value.skillCard.Skill);
		}
	}

	public void DoUnEquipCard (TUICard uicard){
		if(!IsBuyState) {
			if(uicard.Card != null && uiCards.ContainsKey(uicard.Card.name)) {
				removeItems(uicard.skillCard.Skill.ID, uicard.skillCard.Skill.SN, uicard.Card);
				uicard.UpdateRedPoint(false, uicard.skillCard.Skill);
				uicard.SetIsEquip(false);
			}
			refreshCards();
			refreshRedPoint (); 
		}
	}

	public void DoEquipCard (TUICard uicard){
		if(!IsBuyState) {
			if(GameFunction.IsActiveSkill(uicard.skillCard.Skill.ID)) 
				AddItem(uicard.Card, getActiveInstall);
			else {
				if(uicard.skillCard.IsInstall) { //Selected to NoSelected
					uicard.skillCard.IsInstall = !uicard.skillCard.IsInstall;
					uicard.UpdateRedPoint(false, uicard.skillCard.Skill);
					uicard.SetIsEquip(false);
					removeItems(uicard.skillCard.Skill.ID, uicard.skillCard.Skill.SN, uicard.Card);
				} else { //NoSelected to Selected
					if(addItems(uicard)){
						AudioMgr.Get.PlaySound(SoundType.SD_Compose);
						uicard.skillCard.IsInstall = !uicard.skillCard.IsInstall;
					}
				}

				viewRight.RefreshPassiveItem();
			}

			refreshCards();
			refreshRedPoint();
		} 
		else Debug.LogWarning ("It's Buy State.");
	}

	public void OnCardDetailInfo (){
		if(uiCards.ContainsKey (UIButton.current.name)) {
			TUICard uicard = uiCards[UIButton.current.name];
			if(!IsBuyState) {
				//Click Card
				UISkillInfo.Get.ShowFromSkill(uicard, uicard.skillCard.IsInstall, uicard.skillCard.IsCanUse);
				if(UISort.Visible)
					UISort.UIShow(false);
			} else {
				if(!uicard.skillCard.IsInstall) {
					if(!uicard.skillCard.IsSold)
						addSellCards(UIButton.current.name);
					else
						removeSellCards(UIButton.current.name);
					
					uicard.skillCard.IsSold = !uicard.skillCard.IsSold;
				}
			}
		}
	}

	public void OnItemDetailInfo (GameObject go){
		UISkillInfo.Get.ShowFromSkill(uiCards[go.name], true, false);
		if(UISort.Visible)
			UISort.UIShow(false);
	}

	public void RefreshFromReinEvo (int sn) {
		foreach (KeyValuePair<string, TUICard> uicard in uiCards){
			if(uicard.Value.skillCard.Skill.SN == sn) {
				UISkillInfo.Get.RefreshUICard(uicard.Value);
				return ;
			}
		}
	}

	//From Item RemoveButton
	public void OnRemoveItem(GameObject go, bool state){
		removeItems(uiCards[go.transform.parent.name].skillCard.Skill.ID, uiCards[go.transform.parent.name].skillCard.Skill.SN, go.transform.parent.gameObject);
		refreshCards();
		refreshRedPoint ();
	}
	
	public void UpdateSort () {
		resetScrollPostion ();
		eCondition = PlayerPrefs.GetInt(ESave.SkillCardCondition.ToString(), ECondition.None.GetHashCode());
		sortSkillCondition(eCondition);

		eFilter = PlayerPrefs.GetInt(ESave.SkillCardFilter.ToString(), EFilter.All.GetHashCode());
		sortSkillFilter(eFilter);

		viewRight.UpdateSort(eFilter);
	}

	public void DoOpenActive (GameObject go){
		//Open Actvie Cards
		if(UISort.Visible)
			UISort.UIShow(false);

		if(!IsBuyState) {
			switch(eFilter) {
			case (int)EFilter.All:
				PlayerPrefs.SetInt (ESave.SkillCardFilter.ToString(), EFilter.Passive.GetHashCode());
				break;
			case (int)EFilter.Available:
			case (int)EFilter.Select:
			case (int)EFilter.Active:
				PlayerPrefs.SetInt (ESave.SkillCardFilter.ToString(), EFilter.Active.GetHashCode());
				break;
			case (int)EFilter.Passive:
				PlayerPrefs.SetInt (ESave.SkillCardFilter.ToString(), EFilter.All.GetHashCode());
				break;
			}
			PlayerPrefs.Save();
			UpdateSort();
		} else 
			viewRight.ToggleCheckBoxSkill[0].value = (eFilter == EFilter.Active.GetHashCode());
		
		CardDragFinish();
	}

	public void DoOpenPassive (GameObject go){
		//Open Passive Cards
		if(UISort.Visible)
			UISort.UIShow(false);

		if(!IsBuyState) {
			switch(eFilter) {
			case (int)EFilter.All:
				PlayerPrefs.SetInt (ESave.SkillCardFilter.ToString(), EFilter.Active.GetHashCode());
				break;
			case (int)EFilter.Available:
			case (int)EFilter.Select:
			case (int)EFilter.Passive:
				PlayerPrefs.SetInt (ESave.SkillCardFilter.ToString(), EFilter.Passive.GetHashCode());
				break;
			case (int)EFilter.Active:
				PlayerPrefs.SetInt (ESave.SkillCardFilter.ToString(), EFilter.All.GetHashCode());
				break;
			}
			PlayerPrefs.Save();
			UpdateSort();
		} else 
			viewRight.ToggleCheckBoxSkill[1].value = (eFilter == EFilter.Passive.GetHashCode());
		
		CardDragFinish();
	}

	public void DoSellState() {
		if(UISort.Visible)
			UISort.UIShow(false);
		
		if(!IsBuyState) {
			IsBuyState = true;
			DoFinish();
		} else {
			//update sell index
			DoSell();
		}
	}

	public void DoCloseSell () {
		IsBuyState = false;
		setEditState(false);
	}
	
	public void DoSort() {
		if(!IsBuyState) 
			UISort.UIShow(!UISort.Visible, 0);
		
	}

	public void DoBack() {
		isLeave = true;
		DoFinish();
	}

	public void OnChangePage (GameObject obj) {
		if(!IsBuyState) {
			int index;
			if(int.TryParse (obj.name, out index))
				changePage(index);
			
		} else {
			for(int i=0; i<toggleDecks.Length; i++) 
				toggleDecks[i].value = (i == tempPage);
			
		}
	}

	public void DoSell () {
		if(sellNames.Count > 0) {
			sellIndexs = new int[sellNames.Count];
			for(int i=0; i<sellIndexs.Length; i++)
				sellIndexs[i] = sellNames[i];

			for(int i=0; i<sellIndexs.Length; i++) {
				for (int j=i+1; j<sellIndexs.Length; j++){
					if (sellIndexs[i] >= sellIndexs[j]){
						int temp = sellIndexs[i];
						sellIndexs[i] = sellIndexs[j];
						sellIndexs[j] = temp;
					}
				}
			}
			UIMessage.Get.ShowMessage(TextConst.S(202), TextConst.S(203), SendSellCard);
		} else 
			DoCloseSell();
	}

	public bool IsReinforce {
		get{return isReinforce;}
		set{ isReinforce = value;}
	} 

	public bool IsEvolution {
		get{return isEvolution;}
		set{isEvolution = value;}
	}

	public void DoFinish() {
		List<string> tempNoUpdate = new List<string>();
		List<string> tempRemoveIndex = new List<string>();
		List<string> tempAddIndex = new List<string>();
		for (int i =0; i<skillsOriginal.Count; i++) {
			if(skillsRecord.Contains(skillsOriginal[i])) 
				//it doesn't need to update
				tempNoUpdate.Add(skillsOriginal[i]);
			else 
				//it need to add removeIndexs
				tempRemoveIndex.Add(skillsOriginal[i]);
			
		}

		removeIndexs = new int[tempRemoveIndex.Count];
		if(tempRemoveIndex.Count > 0) 
			for (int i=0; i<removeIndexs.Length; i++) 
				removeIndexs[i] = uiCards[tempRemoveIndex[i]].CardIndex;

		for (int i=0; i<skillsRecord.Count; i++) 
			if(!tempNoUpdate.Contains(skillsRecord[i])) 
				tempAddIndex.Add(skillsRecord[i]);	

		addIndexs = new int[tempAddIndex.Count];
		if(tempAddIndex.Count > 0) {
			string statisticStr = "";
			for(int i=0; i<addIndexs.Length; i++) {
				addIndexs[i] = uiCards[tempAddIndex[i]].CardIndex;

				if(i > 0)
					statisticStr += "_";
				
				statisticStr += uiCards[tempAddIndex[i]].skillCard.Skill.ID.ToString();
			}

			if(GameData.IsMainStage)
				Statistic.Ins.LogEvent(55, statisticStr);
			else if(GameData.IsInstance)
				Statistic.Ins.LogEvent(104, statisticStr);
			else
				Statistic.Ins.LogEvent(109, statisticStr);
		}
		
		bool flag = false;

		orderSNs = new int[getActiveInstall];
		for (int i=0; i<getActiveInstall; i++) 
			orderSNs[i] = activeStruct[i].CardSN;

		if(orderSNs.Length == activeOriginalSN.Count) {
			for(int i=0; i<orderSNs.Length; i++) 
				if(!orderSNs[i].Equals(activeOriginalSN[i])) {
					flag = true;
					break;
				}
		} else
			flag = true;


		//Bobble Sort
		if(removeIndexs.Length > 1) {
			for(int i=0; i<removeIndexs.Length; i++) {
				for (int j=i+1; j<removeIndexs.Length; j++){
					if (removeIndexs[i] >= removeIndexs[j]){
						int temp = removeIndexs[i];
						removeIndexs[i] = removeIndexs[j];
						removeIndexs[j] = temp;
					}
				}
			}
		}
		//Bobble Sort
		if(addIndexs.Length > 1) {
			for(int i=0; i<addIndexs.Length; i++) {
				for (int j=i+1; j<addIndexs.Length; j++){
					if (addIndexs[i] >= addIndexs[j]){
						int temp = addIndexs[i];
						addIndexs[i] = addIndexs[j];
						addIndexs[j] = temp;
					}
				}
			}
		}
		
		if(addIndexs.Length > 0 || removeIndexs.Length > 0 || flag) {
			if(isChangePage) 
				UIMessage.Get.ShowMessage(TextConst.S(202), TextConst.S(204), SendEquipSkillCard, SendChangeSkillPage);
			else
				SendEquipSkillCard(null);
			
		} else{
			if(!isEvolution) {
				if(!isReinforce) {
					if(!isLeave) {
						if(isChangePage) 
							SendChangeSkillPage();
						else {
							if(IsBuyState)
								setEditState(IsBuyState);
						}
					} else 
						hide();
				} else {
					isReinforce = false;
					if(UISkillInfo.Visible) 
						UISkillReinforce.Get.Show( UISkillInfo.Get.MyUICard.skillCard.Skill,  UISkillInfo.Get.MyUICard.CardIndex,  UISkillInfo.Get.IsEquip, 0);
					
				}
			} else {
				isEvolution = false;
				if(UISkillInfo.Visible) 
					UISkillReinforce.Get.Show( UISkillInfo.Get.MyUICard.skillCard.Skill,  UISkillInfo.Get.MyUICard.CardIndex,  UISkillInfo.Get.IsEquip, 1);
				
			}
		}
	}

	public void SendEquipSkillCard (object obj) {
		WWWForm form = new WWWForm();
		form.AddField("RemoveIndexs", JsonConvert.SerializeObject(removeIndexs));
		form.AddField("AddIndexs", JsonConvert.SerializeObject(addIndexs));
		form.AddField("OrderIndexs", JsonConvert.SerializeObject(orderSNs));
		SendHttp.Get.Command(URLConst.EquipsSkillCard, waitEquipSkillCard, form);
	}

	public void SendChangeSkillPage() {
		WWWForm form = new WWWForm();
		form.AddField("Page", tempPage);
		SendHttp.Get.Command(URLConst.ChangeSkillPage, waitChangeSkillPage, form);
	}

	public void SendSellCard(object obj) {
		WWWForm form = new WWWForm();
		form.AddField("SellIndexs", JsonConvert.SerializeObject(sellIndexs));
		SendHttp.Get.Command(URLConst.SellSkillcard, waitSellSkillPage, form);
	}

	private void waitEquipSkillCard(bool ok, WWW www) {
		if (ok) {
            TEquipSkillCardResult result = JsonConvertWrapper.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerCards;
			GameData.Team.Player.SkillCardPages = result.SkillCardPages;
			GameData.Team.PlayerInit();
			GameData.Team.InitSkillCardCount();

			if(!isEvolution) {
				if(!isReinforce) {
					if(!isLeave) {
						if(!IsBuyState) {
							if(isChangePage) 
								SendChangeSkillPage();
							
							refreshAfterInstall ();
						} else 
							refreshBeforeSell();
						
					} else {
						refreshAfterInstall ();
						hide();
						UIHint.Get.ShowHint(TextConst.S(533), Color.black);
						AudioMgr.Get.PlaySound(SoundType.SD_Check_Btn);
					}
				} else {
					refreshAfterInstall ();
					isReinforce = false;
					if(UISkillInfo.Visible) {
						TSkill skill = findSkill(UISkillInfo.Get.MyUICard.skillCard.Skill);
						UISkillReinforce.Get.Show( skill, infoIndex, isAlreadyEquip, 0);
					}
				}
			} else {
				refreshAfterInstall ();
				isEvolution = false;
				if(UISkillInfo.Visible) {
					TSkill skill = findSkill(UISkillInfo.Get.MyUICard.skillCard.Skill);
					UISkillReinforce.Get.Show( skill, infoIndex, isAlreadyEquip, 1);
				}
			}

		} else {
			Debug.LogError("text:"+www.text);
		}
	}

	private void waitChangeSkillPage(bool ok, WWW www) {
		if (ok) {
            TEquipSkillCardResult result = JsonConvertWrapper.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerCards;
			GameData.Team.InitSkillCardCount();
			isChangePage = false; 
			GameData.Team.Player.SkillPage = tempPage;
			refreshAfterInstall ();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

	private void waitSellSkillPage(bool ok, WWW www) {
		if (ok) {
            TEquipSkillCardResult result = JsonConvertWrapper.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Money = result.Money;
			GameData.Team.Player.SkillPage = tempPage;
			GameData.Team.InitSkillCardCount();
			setEditState(false);
			UIMainLobby.Get.UpdateUI();
			refreshAfterInstall ();
			AudioMgr.Get.PlaySound (SoundType.SD_Sell);
		} else {
			Debug.LogError("text:"+www.text);
		}
	}

	private TSkill findSkill(TSkill skill) {
		for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
			if(GameData.Team.SkillCards[i].SN == skill.SN){
				isAlreadyEquip = false;
				infoIndex = i;
				return GameData.Team.SkillCards[i];
			}
		}

		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
			if(GameData.Team.Player.SkillCards[i].SN == skill.SN){
				isAlreadyEquip = true;
				infoIndex = i;
				return GameData.Team.Player.SkillCards[i];
			}
		}

		return skill;
	}

	public bool IsOpenThirdActive {
		get {
			return GameData.IsOpenUIEnableByPlayer(EOpenID.ThirdActive);
		}
	}

	public int CostSpace {
		get {return costSpace;}
	}

	public int ExtraCostSpace {
		get {return GameConst.Max_CostSpace - costSpace;}
	}
}
