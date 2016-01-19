using System.Collections.Generic;
using DG.Tweening;
using GameEnum;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

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
		this.ItemEquipActiveCard = active.ItemEquipActiveCard;
		this.CardIndex = active.CardIndex;
		this.CardID = active.CardID;
		this.CardSN = active.CardSN;
		this.CardLV = active.CardLV;
		if(parent != null)
			this.ItemEquipActiveCard.transform.SetParent(parent);
		this.ItemEquipActiveCard.transform.localPosition = pos;
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

	public void SetCoin (int money) {
		skillCard.SetCoin(money);
	}

	public void Init (GameObject obj, int index, TSkill skill, bool isEquip, EventDelegate btnFunc = null) {
		Card = obj;
		CardIndex = index;
		if(GameData.DSkillData.ContainsKey(skill.ID))
			Cost = Mathf.Max(GameData.DSkillData[skill.ID].Space(skill.Lv), 1);

		skillCard = new TActiveSkillCard();
		skillCard.Init(obj, btnFunc, true);
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
//	private GameObject equipEffect;
	
	//Original for compare DoFinish
	private List<int> activeOriginalSN = new List<int>();
	private List<GameObject> skillOriginalCards = new List<GameObject>();//By Sort
	private List<string> skillsOriginal = new List<string>();//record alread equiped   rule: index_id_skillsOriginal(Equiped)_cardLV

	private List<GameObject> skillSortCards = new List<GameObject>();//By Sort
	private List<string> skillsRecord = new List<string>();

	//key:GameData.SkillCards.index_cardID_skillsOriginal(Equiped)_cardLV Value: TSkill   For Get Level
	//only record skillsOriginal(Equiped) 0:Not Equiped 1:Equiped (First Time)
	private Dictionary<string, TSkill> skillActiveCards = new Dictionary<string, TSkill>(); 
	private Dictionary<string, TSkill> skillPassiveCards = new Dictionary<string, TSkill>();
	private Dictionary<string, TUICard> uiCards = new Dictionary<string, TUICard>();

	//RightItem  name:Skill
	public TActiveStruct[] activeStruct = new TActiveStruct[3];//Record of Active
	private List<GameObject> itemPassiveCards = new List<GameObject>();// 
	private GameObject itemPassiveField;
	private GameObject itemPassiveSelected;

	//Right(itemCardEquipped Parent)
	private GameObject gridPassiveCardBase;
	private UIScrollView scrollViewItemList;
	private UIToggle[] toggleCheckBoxSkill = new UIToggle[2];
	private UISprite PassiveCheck;
	private UISprite ActiveCheck;

	//Total Cost
	private UILabel labelCostValue;

	//Left CardView
	private GameObject gridCardList;
	private UIScrollView scrollViewCardList;

	//Sell
	public bool IsBuyState = false;
	private TSkillCardSell cardSell;

	//Desk(Page)
	private UIToggle[] toggleDecks = new UIToggle[5];

	private int costSpace = 0;
	private int costSpaceMax = 20;
	private int activeFieldLimit = 3;
	private int eCondition;
	private int eFilter;

	//page
	private int tempPage = 0;
	private bool isChangePage = false;
	private bool isLeave = false;

	public bool IsDragNow = false;
	public bool IsCardActive = false;//For UISKillCardDrag

	private float runShineInternal = 5f;
	private float runShine = 0;

	//ForReinforce
	private bool isReinforce = false;
	private int infoIndex = -1;
	private bool isAlreadyEquip = false;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
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
//		equipEffect = Resources.Load("Effect/UIEquipSkill") as GameObject;

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
		gridPassiveCardBase = GameObject.Find (UIName + "/Center/MainView/Right/PassiveCardBase/PassiveList");
		labelCostValue = GameObject.Find (UIName + "/Center/LabelCost/CostValue").GetComponent<UILabel>();
		scrollViewItemList = GameObject.Find (UIName + "/Center/MainView/Right/PassiveCardBase/PassiveList").GetComponent<UIScrollView>();
		scrollViewItemList.transform.localPosition = new Vector3(0, -13, 0);
		scrollViewItemList.panel.clipOffset = new Vector2(12, 26);
		scrollViewItemList.onDragFinished = ItemDragFinish;

		toggleCheckBoxSkill[0] = GameObject.Find (UIName + "/Center/MainView/Right/STitle/ActiveCheck").GetComponent<UIToggle>();
		toggleCheckBoxSkill[1] = GameObject.Find (UIName + "/Center/MainView/Right/STitle/PassiveCheck").GetComponent<UIToggle>();
		ActiveCheck = GameObject.Find (UIName + "/Center/MainView/Right/STitle/ActiveCheck/Background").GetComponent<UISprite>();
		PassiveCheck = GameObject.Find (UIName + "/Center/MainView/Right/STitle/PassiveCheck/Background").GetComponent<UISprite>();
		UIEventListener.Get( GameObject.Find (UIName + "/Center/MainView/Right/STitle/ActiveCheck")).onClick = DoOpenActive;
		UIEventListener.Get( GameObject.Find (UIName + "/Center/MainView/Right/STitle/PassiveCheck")).onClick = DoOpenPassive;

		gridCardList = GameObject.Find (UIName + "/Center/CardsView/Left/CardsGroup/CardList");
		scrollViewCardList = GameObject.Find (UIName + "/Center/CardsView/Left/CardsGroup/CardList").GetComponent<UIScrollView>();
		scrollViewCardList.onDragStarted = CardDragStart;
		scrollViewCardList.onStoppedMoving = CardDragEnd;

		itemPassiveField = GameObject.Find (UIName + "/Center/MainView/Right/PassiveCardBase/PassiveField/Icon");
		itemPassiveSelected = GameObject.Find (UIName + "/Center/MainView/Right/PassiveCardBase/PassiveField/Selected");
		itemPassiveSelected.SetActive(false);
		itemPassiveField.transform.parent.name = "4";

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
		UpdateSort();
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		if(PlayerPrefs.HasKey(ESave.NewCardFlag.ToString()))
		{
			PlayerPrefs.DeleteKey(ESave.NewCardFlag.ToString());
			PlayerPrefs.Save();

			if (UISelectRole.Visible)
				UISelectRole.Get.DisableRedPoint();
		}

		isChangePage = false;
		isLeave = false;
	}
	
	private void hide() {
		RemoveUI(UIName);
		if(UISort.Visible)
			UISort.UIShow(false);

		if (!UISelectRole.Visible)
			UIMainLobby.Get.Show();
        else
            UISelectRole.Get.InitPlayer();
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
		for(int i=0; i<skillSortCards.Count; i++) {
			Destroy(skillSortCards[i]);
		}
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
		itemPassiveField.SetActive(true);
		scrollViewItemList.transform.localPosition = new Vector3(0, -13, 0);
		scrollViewItemList.panel.clipOffset = new Vector2(12, 26);
	}

	public void RefreshAddCard () {
		refresh();
		initCards ();
		UpdateSort();
	}

	private void refreshAfterInstall () {
		refresh();
		initCards ();
		UpdateSort();
	}

	private void refreshBeforeSell () {
		refresh();
		initCards ();
		setEditState(true);
	}
	
	private void refreshAfterSell () {
		refresh();
		DoCloseSell();
		initCards ();
		UpdateSort();
	}

	private void initCards () {
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
			for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
				GameObject obj = null;
				if(GameData.Team.SkillCards[i].ID > 100 && 
				   GameData.DSkillData.ContainsKey(GameData.Team.SkillCards[i].ID) && 
					!isSkillCardInOtherPlayer(GameData.Team.SkillCards[i].SN)) {
					index ++;
					obj = addUICards(i,
					                 index, 
					                 GameData.Team.SkillCards[i],
					                 gridCardList, 
					                 false);
					if(obj != null) {
						if(GameFunction.IsActiveSkill(GameData.Team.SkillCards[i].ID)) {
							if(!skillActiveCards.ContainsKey (obj.name)) {
								skillActiveCards.Add(obj.name, GameData.Team.SkillCards[i]);
								skillOriginalCards.Add(obj);
								skillSortCards.Add(obj);
							}
						} else {
							if(!skillPassiveCards.ContainsKey(obj.name)) {
								skillPassiveCards.Add(obj.name, GameData.Team.SkillCards[i]);
								skillOriginalCards.Add(obj);
								skillSortCards.Add(obj);
							}
						}
					} else 
						index --;
				}
			}
		}
		refreshActiveItems ();
		refreshPassiveItems();
		checkCostIfMask();
		labelCostValue.text = costSpace + "/" + costSpaceMax;

		scrollViewCardList.enabled = !(skillSortCards.Count <= 6);
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
				uicard.Init(obj, skillCardIndex, skill, isEquip, new EventDelegate(OnCardDetailInfo));
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
			obj.transform.localPosition = new Vector3(12, 110 - 70 * positionIndex, 0);
		
		TPassiveSkillCard skillCard = new TPassiveSkillCard();
		skillCard.InitFormation(obj);
		UIEventListener.Get(skillCard.BtnRemove).onPress = OnRemoveItem;
		skillCard.UpdateViewFormation(uicard.skillCard.Skill.ID, uicard.skillCard.Skill.Lv);

		UIEventListener.Get(obj).onClick = OnItemDetailInfo;

//		GameObject uiEquipEffect = Instantiate(equipEffect) as GameObject;
//		uiEquipEffect.transform.parent = obj.transform;
//		uiEquipEffect.transform.localPosition = Vector3.zero;
//		uiEquipEffect.transform.localScale = Vector3.one;
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
				obj = addUIItems(uicard, gridPassiveCardBase, itemPassiveCards.Count);
				if(obj != null) {
					itemPassiveCards.Add(obj);
					itemPassiveField.SetActive(false);
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
			return true;
		} 
//		else UIHint.Get.ShowHint("More than SpaceMax", Color.red);
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
									addItems(uiCards[name], index);
								}
//								else UIHint.Get.ShowHint("More than SpaceMax", Color.red);
							}
						} else {
							removeItems(activeStruct[activeFieldLimit - 1].CardID, activeStruct[activeFieldLimit - 1].CardSN, activeStruct[activeFieldLimit - 1].ItemEquipActiveCard);
							addItems(uiCards[name], activeFieldLimit - 1);
						}

					} else {
						if(!activeStruct[index].CheckBeInstall) {
							if(checkCost(uiCards[name].Cost)) 
								addItems(uiCards[name], index);
//							else UIHint.Get.ShowHint("More than SpaceMax", Color.red);
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
								addItems(uiCards[name], index);
							}
//							else UIHint.Get.ShowHint("More than SpaceMax", Color.red);

						}
						refreshActiveItems();
					}
					refreshCards();
				} else {
					//Passive
					addItems(uiCards[name]);
				}
			}
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
				itemPassiveSelected.SetActive(isShow);
			}
		}
	}

	private void removeItems(int id, int sn, GameObject go = null) {
		if(uiCards.ContainsKey(go.name)) {
			if(setCost(-uiCards[go.name].Cost)){
				if(skillsRecord.Contains(go.name))
					skillsRecord.Remove(go.name);
				if(!GameFunction.IsActiveSkill(id)) {
					ItemMoveOne();
					for(int i=0 ;i<itemPassiveCards.Count; i++) {
						if(itemPassiveCards[i].name.Equals(go.name)){
							Destroy(itemPassiveCards[i]);
							itemPassiveCards.RemoveAt(i);
							break;
						}
					}
					
					if(itemPassiveCards.Count == 0)
						itemPassiveField.SetActive(true);
					
					refreshPassiveItems();
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
		}
	}
	
	private void refreshCards() {
		for(int i=0 ;i<skillSortCards.Count; i++) 
			uiCards[skillSortCards[i].name].skillCard.IsInstall = skillsRecord.Contains(uiCards[skillSortCards[i].name].Card.name);
	}
	
	private void refreshPassiveItems() {
		for(int i=0 ;i<itemPassiveCards.Count; i++) 
			itemPassiveCards[i].transform.localPosition = new Vector3(12, 110 - 70 * i, 0); 
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
			
		for (int i=0; i<activeStruct.Length; i++) 
			if((i+1) > activeFieldLimit)
				activeStruct[i].SpriteActiveFieldIcon.spriteName = "Icon_lock";
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

	private void setEditState (bool isEditState) {
		cardSell.Refresh(isEditState);
		IsBuyState = isEditState;
		if(isEditState) {
			sellNames.Clear();
			int index = 0;
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
		} else {
			for(int i=0; i<skillSortCards.Count; i++) {
				if(uiCards.ContainsKey(skillSortCards[i].name)) {
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
		scrollViewCardList.ResetPosition();
		scrollViewCardList.panel.clipOffset = new Vector2(0, scrollViewCardList.panel.clipOffset.y);
		gridCardList.transform.localPosition = Vector3.zero;
	}

	public void CardDragStart() {IsDragNow = true;}

	public void CardDragEnd() {IsDragNow = false;}

	public void SetMask (bool isShow, string name) {
		for(int i=0; i<skillSortCards.Count; i++) 
			if(!name.Contains(skillSortCards[i].name)) 
				uiCards[skillSortCards[i].name].skillCard.DragCard = isShow;
	} 

	public void ItemDragFinish(){
		if(itemPassiveCards.Count < 3){
			scrollViewItemList.transform.DOLocalMoveY(0, 0.2f).OnUpdate(UpdateClipOffset);
		}
	}

	public void UpdateClipOffset(){
		scrollViewItemList.DisableSpring();
		scrollViewItemList.panel.clipOffset = new Vector2(12, - scrollViewItemList.transform.localPosition.y + 10);
	}

	public void ItemMoveOne(){ 
		if(itemPassiveCards.Count > 2){
			scrollViewItemList.transform.DOLocalMoveY(scrollViewItemList.transform.localPosition.y - 5 , 0.2f);
			scrollViewItemList.panel.clipOffset = new Vector2(12, scrollViewItemList.panel.clipOffset.y);
		}
	}

	public void DoUnEquipCard (TUICard uicard){
		if(!IsBuyState) {
			if(uicard.Card != null && uiCards.ContainsKey(uicard.Card.name))
				removeItems(uicard.skillCard.Skill.ID, uicard.skillCard.Skill.SN, uicard.Card);
			refreshCards();
		}
//		else UIHint.Get.ShowHint("It's Buy State.", Color.red);
	}

	public void DoEquipCard (TUICard uicard){
		if(!IsBuyState) {
			if(GameFunction.IsActiveSkill(uicard.skillCard.Skill.ID)) {
				if(getContainActiveSN(uicard.skillCard.Skill.SN) == -1){
					if(getActiveFieldNull != -1) {
						if(addItems(uicard, getActiveFieldNull)) 
							uicard.skillCard.IsInstall = true;
					}
					else Debug.LogWarning ("Active is Full.");
				} 
				else Debug.LogWarning ("Active SN is Same."+ uicard.skillCard.Skill.SN);
				refreshActiveItems();
			} else {
				if(uicard.skillCard.IsInstall) { //Selected to NoSelected
					uicard.skillCard.IsInstall = !uicard.skillCard.IsInstall;
					removeItems(uicard.skillCard.Skill.ID, uicard.skillCard.Skill.SN, uicard.Card);
				} else { //NoSelected to Selected
					if(addItems(uicard))
						uicard.skillCard.IsInstall = !uicard.skillCard.IsInstall;
					
				}
			}
			refreshCards();
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

	//From Item RemoveButton
	public void OnRemoveItem(GameObject go, bool state){
		removeItems(uiCards[go.transform.parent.name].skillCard.Skill.ID, uiCards[go.transform.parent.name].skillCard.Skill.SN, go.transform.parent.gameObject);
		refreshCards();
	}

	private void activeCheckShow (bool isClick){
		if(isClick)
			ActiveCheck.spriteName = "button_orange2";
		else
			ActiveCheck.spriteName = "button_orange1";
	}

	private void passiveCheckShow (bool isClick){
		if(isClick)
			PassiveCheck.spriteName = "button_orange2";
		else
			PassiveCheck.spriteName = "button_orange1";
	}
	
	public void UpdateSort () {
		eCondition = PlayerPrefs.GetInt(ESave.SkillCardCondition.ToString(), ECondition.None.GetHashCode());
		sortSkillCondition(eCondition);

		eFilter = PlayerPrefs.GetInt(ESave.SkillCardFilter.ToString(), EFilter.All.GetHashCode());
		sortSkillFilter(eFilter);
		switch(eFilter) {
			case (int)EFilter.All:
				toggleCheckBoxSkill[0].value = true;
				toggleCheckBoxSkill[1].value = true;
				activeCheckShow(true);
				passiveCheckShow(true);
				break;
			case (int)EFilter.Active:
				toggleCheckBoxSkill[0].value = true;
				toggleCheckBoxSkill[1].value = false;
				activeCheckShow(true);
				passiveCheckShow(false);
				break;
			case (int)EFilter.Passive:
				toggleCheckBoxSkill[0].value = false;
				toggleCheckBoxSkill[1].value = true;
				activeCheckShow(false);
				passiveCheckShow(true);
				break;
			case (int)EFilter.Available:
			case (int)EFilter.Select:
				toggleCheckBoxSkill[0].value = false;
				toggleCheckBoxSkill[1].value = false;
				activeCheckShow(false);
				passiveCheckShow(false);
				break;
		}
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
		} else {
			toggleCheckBoxSkill[0].value = (eFilter == EFilter.Active.GetHashCode());
		}

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
		} else {
			toggleCheckBoxSkill[1].value = (eFilter == EFilter.Passive.GetHashCode());
		}
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
		if(!IsBuyState) {
			UISort.UIShow(!UISort.Visible, 0);
		}
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
			for(int i=0; i<toggleDecks.Length; i++) {
				toggleDecks[i].value = (i == tempPage);
			}
		}
	}

	public void DoSell () {
		if(sellNames.Count > 0) {
			sellIndexs = new int[sellNames.Count];
			for(int i=0; i<sellIndexs.Length; i++){
				sellIndexs[i] = sellNames[i];
			}

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

	public void DoFinish() {
		List<string> tempNoUpdate = new List<string>();
		List<string> tempRemoveIndex = new List<string>();
		List<string> tempAddIndex = new List<string>();
		for (int i =0; i<skillsOriginal.Count; i++) {
			if(skillsRecord.Contains(skillsOriginal[i])) {
				//it doesn't need to update
				tempNoUpdate.Add(skillsOriginal[i]);
			} else {
				//it need to add removeIndexs
				tempRemoveIndex.Add(skillsOriginal[i]);
			}
		}

		removeIndexs = new int[tempRemoveIndex.Count];
		if(tempRemoveIndex.Count > 0) 
			for (int i=0; i<removeIndexs.Length; i++) 
				removeIndexs[i] = uiCards[tempRemoveIndex[i]].CardIndex;

		for (int i=0; i<skillsRecord.Count; i++) 
			if(!tempNoUpdate.Contains(skillsRecord[i])) 
				tempAddIndex.Add(skillsRecord[i]);	

		addIndexs = new int[tempAddIndex.Count];
		if(tempAddIndex.Count > 0) 
			for(int i=0; i<addIndexs.Length; i++) 
				addIndexs[i] = uiCards[tempAddIndex[i]].CardIndex;
		

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
			if(isChangePage) {
				UIMessage.Get.ShowMessage(TextConst.S(202), TextConst.S(204), SendEquipSkillCard, SendChangeSkillPage);
			} else
				SendEquipSkillCard(null);
		} else{
			if(!isReinforce) {
				if(!isLeave) {
					if(isChangePage) {
						SendChangeSkillPage();
					} else {
						if(IsBuyState)
							setEditState(IsBuyState);
					}
				} else 
					hide();
			} else {
				isReinforce = false;
				if(UISkillInfo.Visible) {
					UISkillReinforce.Get.Show( findSkill(UISkillInfo.Get.MyUICard.skillCard.Skill), infoIndex, isAlreadyEquip);
					UISkillInfo.UIShow(false);
				}
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
			TEquipSkillCardResult result = JsonConvert.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerCards;
			GameData.Team.Player.SkillCardPages = result.SkillCardPages;
			GameData.Team.Player.Init();
			GameData.Team.InitSkillCardCount();
			
			if(!isReinforce) {
				if(!isLeave) {
					if(!IsBuyState) {
						if(isChangePage) {
							SendChangeSkillPage();
						}
						refreshAfterInstall ();
					} else 
						refreshBeforeSell();
				} else {
					refreshAfterInstall ();
					hide();
					UIHint.Get.ShowHint(TextConst.S(533), Color.black);
				}
			} else {
				isReinforce = false;
				if(UISkillInfo.Visible) {
					UISkillReinforce.Get.Show( findSkill(UISkillInfo.Get.MyUICard.skillCard.Skill), infoIndex, isAlreadyEquip);
					UISkillInfo.UIShow(false);
				}
				
			}

		} else {
			Debug.LogError("text:"+www.text);
		}
	}

	private void waitChangeSkillPage(bool ok, WWW www) {
		if (ok) {
			TEquipSkillCardResult result = JsonConvert.DeserializeObject <TEquipSkillCardResult>(www.text); 
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
			TEquipSkillCardResult result = JsonConvert.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Money = result.Money;
			GameData.Team.Player.SkillPage = tempPage;
			GameData.Team.InitSkillCardCount();
			setEditState(false);
			UIMainLobby.Get.UpdateUI();
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
}
