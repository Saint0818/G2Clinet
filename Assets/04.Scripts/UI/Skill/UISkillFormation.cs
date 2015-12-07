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
	public GameObject itemEquipActiveCard;
	public GameObject gridActiveCardBase;
	public UISprite spriteActiveFieldIcon;
	public GameObject itemActiveSelect;
	public int CardIndex;
	public int CardID;
	public int CardSN;
	public int CardLV;
	public void Init (string name, int index) {
		gridActiveCardBase = GameObject.Find (name + "/MainView/Right/ActiveCardBase"+index.ToString());
		itemActiveSelect = GameObject.Find (name + "/MainView/Right/ActiveCardBase" + index.ToString() + "/ActiveField/Selected");
		itemActiveSelect.SetActive(false);
		spriteActiveFieldIcon = GameObject.Find (name + "/MainView/Right/ActiveCardBase" + index.ToString() + "/ActiveField/Icon").GetComponent<UISprite>();
		spriteActiveFieldIcon.transform.parent.name = index.ToString();
	}
	public void ActiveClear () {
		this.itemEquipActiveCard = null;
		this.CardIndex = -1;
		this.CardID = 0;
		this.CardSN = -1;
		this.CardLV = 0;
	}

	public void SetData (TActiveStruct active){
		this.itemEquipActiveCard = active.itemEquipActiveCard;
		this.CardIndex = active.CardIndex;
		this.CardID = active.CardID;
		this.CardSN = active.CardSN;
		this.CardLV = active.CardLV;
	}

	public bool CheckBeInstall { 
		get{return itemEquipActiveCard != null;}
	}

	public string GetSelfName{
		get{
			if(itemEquipActiveCard != null)
				return itemEquipActiveCard.name;
			else
				return "";
		}
	}
}

public struct TUICard{
	public GameObject Self;
	public UISprite SkillCard;
	public UITexture SkillPic;
	public UISprite SkillLevel;
	public UILabel SkillName;
	public UISprite SkillStar;
	public GameObject UnavailableMask;
	public GameObject Selected;
	public GameObject InListCard;
	public GameObject SellSelect;
	public GameObject SellSelectCover;
	public UISpriteAnimation LightAnimation;
	public UISprite SkillKind;
	public int CardIndex;
	public int CardID;
	public int CardLV;
	public int Cost;
	public int CardSN;

	public bool IsInstall {
		get {return (InListCard != null && InListCard.activeSelf);}
	}

	public bool IsInstallIfDisapper {
		get {return (InListCard != null && InListCard.activeInHierarchy);}
	}

	public void Init (GameObject obj, int index, TSkill skill, bool isEquip) {
		Self = obj;
		CardIndex = index;
		CardID = skill.ID;
		CardLV = skill.Lv;
		CardSN = skill.SN;
		if(GameData.DSkillData.ContainsKey(skill.ID))
			Cost = Mathf.Max(GameData.DSkillData[skill.ID].Space(skill.Lv), 1);
		
		Transform t = obj.transform.FindChild("SkillCard");
		if(t != null) {
			SkillCard = t.gameObject.GetComponent<UISprite>();
			SkillCard .spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[skill.ID].Quality, 1, 3).ToString();
		}
		
		t = obj.transform.FindChild("SkillPic");
		if(t != null){
			SkillPic = t.gameObject.GetComponent<UITexture>();
			SkillPic.mainTexture = GameData.CardTexture(skill.ID);
		}
		
		t = obj.transform.FindChild("SkillLevel");
		if(t != null) {
			SkillLevel = t.gameObject.GetComponent<UISprite>();
			SkillLevel.spriteName = "Cardicon" + Mathf.Clamp(skill.Lv, 1, 5).ToString();
		}
		
		t = obj.transform.FindChild("SkillName");
		if(t != null) {
			SkillName = t.gameObject.GetComponent<UILabel>();
			if(GameData.DSkillData.ContainsKey(skill.ID))
				SkillName.text = GameData.DSkillData[skill.ID].Name;
		}
		
		t = obj.transform.FindChild("SkillStar");
		if(t != null) {
			SkillStar = t.gameObject.GetComponent<UISprite>();
			if(GameData.DSkillData.ContainsKey(skill.ID))
				SkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[skill.ID].Star, 1, GameData.DSkillData[skill.ID].MaxStar).ToString();
		}
		
		t = obj.transform.FindChild("UnavailableMask");
		if(t != null) {
			UnavailableMask = t.gameObject;
			UnavailableMask.SetActive(false);
		}
		
		t = obj.transform.FindChild("Selected");
		if(t != null) {
			Selected = t.gameObject;
			Selected.SetActive(false);
		}
		
		t = obj.transform.FindChild("InListCard/SpriteAnim/Shine");
		if(t != null) {
			LightAnimation = t.GetComponent<UISpriteAnimation>();
		}
		
		t = obj.transform.FindChild("InListCard");
		if(t != null) {
			InListCard = t.gameObject;
			InListCard.SetActive(isEquip);
		}
		
		t = obj.transform.FindChild("SellSelect");
		if(t != null) {
			SellSelect = t.gameObject;
			SellSelect.SetActive(false);
		}
		
		t = obj.transform.FindChild("SellSelect/SellCover");
		if(t != null)  {
			SellSelectCover = t.gameObject;
			SellSelectCover.SetActive(false);
		}
		
		t = obj.transform.FindChild("SkillKind");
		if(t != null)  {
			SkillKind = t.gameObject.GetComponent<UISprite>();
			if(GameFunction.IsActiveSkill(skill.ID))
				SkillKind.spriteName = "ActiveIcon";
			else 
				SkillKind.spriteName = "PasstiveIcon";
		}
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
	private GameObject equipEffect;
	
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
	private UILabel labelSell;
	private GameObject goSellCount;
	private UILabel labelTotalPrice;
	private int sellPrice;

	//Desk
	private UIToggle[] toggleDecks = new UIToggle[5];

	//Info
	private TSkill skillInfo = new TSkill();
	//for InfoEquip temp
	private TUICard tempUICard;
	private GameObject tempObj;

	private int costSpace = 0;
	private int costSpaceMax = 15;
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
			if (!isShow)
				RemoveUI(UIName);
			else
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
		equipEffect = Resources.Load("Effect/UIEquipSkill") as GameObject;
		tempPage = GameData.Team.Player.SkillPage;

		for(int i=0; i<toggleDecks.Length; i++) {
			toggleDecks[i] = GameObject.Find(UIName + "/MainView/Right/DecksList/DecksBtn"+ i.ToString()).GetComponent<UIToggle>();
			toggleDecks[i].name = i.ToString();
			UIEventListener.Get (toggleDecks[i].gameObject).onClick = OnChangePage;
			toggleDecks[i].value = (i == tempPage);
			toggleDecks[i].gameObject.SetActive((i<2));// it need judge by player level
		}
		for(int i=0; i<activeStruct.Length; i++) {
			activeStruct[i].Init(UIName, i);
		}
		gridPassiveCardBase = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveList");
		labelCostValue = GameObject.Find (UIName + "/BottomRight/LabelCost/CostValue").GetComponent<UILabel>();
		scrollViewItemList = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveList").GetComponent<UIScrollView>();
		scrollViewItemList.transform.localPosition = new Vector3(0, -13, 0);
		scrollViewItemList.panel.clipOffset = new Vector2(12, 26);
		scrollViewItemList.onDragFinished = ItemDragFinish;

		toggleCheckBoxSkill[0] = GameObject.Find (UIName + "/MainView/Right/STitle/ActiveCheck").GetComponent<UIToggle>();
		toggleCheckBoxSkill[1] = GameObject.Find (UIName + "/MainView/Right/STitle/PassiveCheck").GetComponent<UIToggle>();
		ActiveCheck = GameObject.Find (UIName + "/MainView/Right/STitle/ActiveCheck/Background").GetComponent<UISprite>();
		PassiveCheck = GameObject.Find (UIName + "/MainView/Right/STitle/PassiveCheck/Background").GetComponent<UISprite>();
		SetBtnFun (UIName + "/MainView/Right/STitle/ActiveCheck", DoOpenActive);
		SetBtnFun (UIName + "/MainView/Right/STitle/PassiveCheck", DoOpenPassive);

		gridCardList = GameObject.Find (UIName + "/CardsView/Left/CardsGroup/CardList");
		scrollViewCardList = GameObject.Find (UIName + "/CardsView/Left/CardsGroup/CardList").GetComponent<UIScrollView>();
		scrollViewCardList.onDragStarted = CardDragStart;
		scrollViewCardList.onStoppedMoving = CardDragEnd;

		itemPassiveField = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveField/Icon");
		itemPassiveSelected = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveField/Selected");
		itemPassiveSelected.SetActive(false);
		itemPassiveField.transform.parent.name = "4";

		labelSell = GameObject.Find (UIName + "/BottomLeft/SellBtn/Icon").GetComponent<UILabel>();
		goSellCount = GameObject.Find (UIName + "/BottomLeft/SellBtn/SellCount");
		goSellCount.SetActive(false);
		labelTotalPrice = GameObject.Find (UIName + "/BottomLeft/SellBtn/SellCount/TotalPrice").GetComponent<UILabel>();

		SetBtnFun (UIName + "/BottomLeft/SortBtn", DoSort);
		SetBtnFun (UIName + "/BottomLeft/BackBtn", DoBack);
//		SetBtnFun (UIName + "/BottomRight/CheckBtn", DoFinish);
		SetBtnFun (UIName + "/BottomLeft/SellBtn", DoSellState);
		SetBtnFun (UIName + "/BottomLeft/SellBtn/SellCount/CancelBtn", DoCloseSell);
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
		}
	}

	private void runShineCard () {
		if(skillsRecord.Count > 0) 
			for (int i=0; i<skillsRecord.Count; i++) 
				if(uiCards.ContainsKey(skillsRecord[i])) 
					if(uiCards[skillsRecord[i]].IsInstallIfDisapper) 
						uiCards[skillsRecord[i]].LightAnimation.Play();
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
			if(activeStruct[i].itemEquipActiveCard != null)
				Destroy(activeStruct[i].itemEquipActiveCard);
			activeStruct[i].spriteActiveFieldIcon.gameObject.SetActive(true);
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
								activeOriginalSN.Add(uiCards[obj.name].CardSN);
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
				   !isSkillCardInPages(GameData.Team.SkillCards[i].SN)) {
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

		if(skillSortCards.Count <= 6)
			scrollViewCardList.enabled = false;
		else 
			scrollViewCardList.enabled = true;
	}

	private int getActiveFieldNull{
		get {
			for(int i=0; i<activeStruct.Length; i++)
				if(activeStruct[i].itemEquipActiveCard == null)
					return i;
			return -1;
		}
	}

	private int getContainActiveSN (int sn) {
		for(int i=0; i<activeStruct.Length; i++) 
			if(activeStruct[i].itemEquipActiveCard != null)
				if(activeStruct[i].CardSN == sn)
					return i;

		return -1;
	}

	private void checkCostIfMask() {
		foreach (KeyValuePair<string, TUICard> uicard in uiCards){
			if(skillsRecord.Contains(uicard.Value.Self.name)) {
				uicard.Value.InListCard.SetActive(true);
				uicard.Value.UnavailableMask.SetActive(false);
			} else {
				uicard.Value.InListCard.SetActive(false);
				uicard.Value.UnavailableMask.SetActive((uicard.Value.Cost > (costSpaceMax - costSpace)));
			}
		}
	}

	public bool CheckCardUsed (string name) {
		string nameSplit = name.Replace("(Clone)", "");
		if (uiCards.ContainsKey(nameSplit))
			return uiCards[nameSplit].InListCard.activeSelf;
		else
			return false;
	}

	private int getActiveInstall {
		get {
			int count = 0;
			for(int i=0; i<activeStruct.Length; i++) 
				if(activeStruct[i].itemEquipActiveCard != null)
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
			GameObject obj = Instantiate(itemSkillCard, Vector3.zero, Quaternion.identity) as GameObject;
			obj.transform.name = skillCardIndex.ToString() + "_" + skill.ID.ToString()+ "_" + skill.SN.ToString() + "_" + skill.Lv.ToString();
			if(!uiCards.ContainsKey(obj.transform.name)) {
				obj.transform.parent = parent.transform;
				obj.transform.localPosition = new Vector3(-230 + 200 * (positionIndex / 2), 100 - 265 * (positionIndex % 2), 0);
				obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				UIEventListener.Get(obj).onClick = OnCardDetailInfo;
				UISkillCardDrag drag = obj.AddComponent<UISkillCardDrag>();
				drag.cloneOnDrag = true;
				drag.restriction = UIDragDropItem.Restriction.PressAndHold;
				drag.pressAndHoldDelay = 0.5f;
				
				TUICard uicard = new TUICard();
				uicard.Init(obj, skillCardIndex, skill, isEquip);
				
				uiCards.Add(obj.transform.name, uicard);
				
				return obj;
			} else {
				Destroy(obj);
				return null;
			}
		} else
			return null;
	}

	private GameObject addUIItems (TUICard uicard, GameObject parent, int positionIndex = 0) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = uicard.CardIndex.ToString() + "_" + uicard.CardID.ToString() + "_" + uicard.CardSN.ToString() + "_" + uicard.CardLV.ToString();
		if(GameFunction.IsActiveSkill(uicard.CardID)) {
			obj.transform.localPosition = Vector3.zero;
			UISkillCardDrag drag = obj.AddComponent<UISkillCardDrag>();
			drag.restriction = UIDragDropItem.Restriction.Vertical;
			drag.isDragItem = true;
		} else 
			obj.transform.localPosition = new Vector3(12, 110 - 70 * positionIndex, 0);
		obj.transform.localScale = Vector3.one;

		GameObject uiEquipEffect = Instantiate(equipEffect) as GameObject;
		uiEquipEffect.transform.parent = obj.transform;
		uiEquipEffect.transform.localPosition = Vector3.zero;
		uiEquipEffect.transform.localScale = Vector3.one;

		UIEventListener.Get(obj).onClick = OnItemDetailInfo;

		if(obj.transform.FindChild("BtnRemove") != null) 
			UIEventListener.Get(obj.transform.FindChild("BtnRemove").gameObject).onPress = OnRemoveItem;
		
		Transform t = obj.transform.FindChild("SkillLevel");
		if(t != null)
			t.gameObject.GetComponent<UISprite>().spriteName = "Cardicon" + Mathf.Clamp(uicard.CardLV, 1, 5).ToString();
		
		t = obj.transform.FindChild("SkillName");
		if(t != null)
			if(GameData.DSkillData.ContainsKey(uicard.CardID))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[uicard.CardID].Name;
		
		t = obj.transform.FindChild("SkillCost");
		if(t != null)
			if(GameData.DSkillData.ContainsKey(uicard.CardID))
				t.gameObject.GetComponent<UILabel>().text = Mathf.Max(GameData.DSkillData[uicard.CardID].Space(uicard.CardLV), 1).ToString();

		t = obj.transform.FindChild("SkillCard");
		if(t != null)
			if(GameData.DSkillData.ContainsKey(uicard.CardID))
				t.gameObject.GetComponent<UISprite>().spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[uicard.CardID].Quality, 1, 3).ToString() + "s";

		t = obj.transform.FindChild("SkillLevel/Levelball");
		if(t != null) 
			t.gameObject.GetComponent<UISprite>().spriteName = "Levelball" + Mathf.Clamp(GameData.DSkillData[uicard.CardID].Quality, 1, 3).ToString();

		t = obj.transform.FindChild("SkillTexture");
		if(t != null)
			if(GameData.DSkillData.ContainsKey(uicard.CardID))
				t.gameObject.GetComponent<UITexture>().mainTexture = GameData.CardItemTexture(uicard.CardID);


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
		if(setCost(Mathf.Max(GameData.DSkillData[uicard.CardID].Space(uicard.CardLV), 1))) {
			GameObject obj = null;
			if(!GameFunction.IsActiveSkill(uicard.CardID)) {
				obj = addUIItems(uicard, gridPassiveCardBase, itemPassiveCards.Count);
				if(obj != null) {
					itemPassiveCards.Add(obj);
					itemPassiveField.SetActive(false);
				}
			} else {
				if(activeStructIndex != -1 && activeStructIndex < 3) {
					obj = addUIItems(uicard, activeStruct[activeStructIndex].gridActiveCardBase);
					if(obj != null) {
						activeStruct[activeStructIndex].itemEquipActiveCard = obj;
						activeStruct[activeStructIndex].CardID = uicard.CardID;
						activeStruct[activeStructIndex].CardIndex = uicard.CardIndex;
						activeStruct[activeStructIndex].CardLV = uicard.CardLV;
						activeStruct[activeStructIndex].CardSN = uicard.CardSN;
						activeStruct[activeStructIndex].spriteActiveFieldIcon.gameObject.SetActive(false);
					}
				}
			}
			if(obj != null) {
				if(!skillsRecord.Contains (obj.name))
					skillsRecord.Add(obj.name);
			} 

			checkCostIfMask();
			return true;
		} 
//		else 
//			UIHint.Get.ShowHint("More than SpaceMax", Color.red);

		return false;
	}

	public void AddItem(GameObject go, int index) {
		string name = go.name.Replace("(Clone)", "");
		if(uiCards.ContainsKey(name)) {
			if(!uiCards[name].InListCard.activeSelf) {
				if(index < 4){
					//Active
					if(getActiveInstall == activeFieldLimit) {
						//Delete
						bool flag = false;
//						if(activeStruct[index].itemEquipActiveCard != null) {
						if(activeStruct[index].CheckBeInstall) {
							if(checkCost(-uiCards[activeStruct[index].itemEquipActiveCard.name].Cost))
								if(checkCost(uiCards[name].Cost))
									flag = true;

							if(flag) {
								if(setCost(-uiCards[activeStruct[index].itemEquipActiveCard.name].Cost)) {
									if(skillsRecord.Contains(activeStruct[index].GetSelfName))
										skillsRecord.Remove(activeStruct[index].GetSelfName);
									Destroy(activeStruct[index].itemEquipActiveCard);
									activeStruct[index].ActiveClear();
									addItems(uiCards[name], index);
								}
//								else 
//									UIHint.Get.ShowHint("More than SpaceMax", Color.red);
							}
						} else {
							removeItems(activeStruct[activeFieldLimit - 1].CardID, activeStruct[activeFieldLimit - 1].CardSN, activeStruct[activeFieldLimit - 1].itemEquipActiveCard);
							addItems(uiCards[name], activeFieldLimit - 1);
						}

					} else {
//						if(activeStruct[index].itemEquipActiveCard == null) {
						if(!activeStruct[index].CheckBeInstall) {
							if(checkCost(uiCards[name].Cost)) {
								addItems(uiCards[name], index);
							} 
//							else 
//								UIHint.Get.ShowHint("More than SpaceMax", Color.red);
						} else {
							if(checkCost(uiCards[name].Cost)) {
								for (int i=0; i<activeStruct.Length; i++) {
									if(i == index) {
										for (int j=activeStruct.Length-1; j>i; j--) {
											activeStruct[j - 1].itemEquipActiveCard.transform.parent = activeStruct[j].gridActiveCardBase.transform;
											activeStruct[j].SetData(activeStruct[j - 1]);
											activeStruct[j].itemEquipActiveCard.transform.localPosition = Vector3.zero;
											activeStruct[j].spriteActiveFieldIcon.gameObject.SetActive((!activeStruct[j].CheckBeInstall));
										}
										break;
									}
								}
								addItems(uiCards[name], index);
							} 
//							else 
//								UIHint.Get.ShowHint("More than SpaceMax", Color.red);

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
			if(activeStruct[targetIndex].itemEquipActiveCard != null) {
				activeStruct[sourceIndex].itemEquipActiveCard.transform.SetParent(activeStruct[targetIndex].gridActiveCardBase.transform);
				activeStruct[sourceIndex].itemEquipActiveCard.transform.localPosition = Vector3.zero;
				activeStruct[sourceIndex].SetData(activeStruct[targetIndex]);
				activeStruct[targetIndex].itemEquipActiveCard.transform.SetParent(temp.gridActiveCardBase.transform);
				activeStruct[targetIndex].itemEquipActiveCard.transform.localPosition = Vector3.zero;
				activeStruct[targetIndex].SetData(temp);
			} else {
				activeStruct[targetIndex].SetData(temp);
				activeStruct[targetIndex].itemEquipActiveCard.transform.SetParent(temp.gridActiveCardBase.transform);
				activeStruct[targetIndex].itemEquipActiveCard.transform.localPosition = Vector3.zero;
				activeStruct[sourceIndex].ActiveClear();
			}

			refreshActiveItems();
		}
	}

	public void ShowInstallLight (GameObject go, bool isShow) {
		string name = go.name.Replace("(Clone)", "");
		if(uiCards.ContainsKey (name)) {
			if(GameFunction.IsActiveSkill(uiCards[name].CardID)) {
				IsCardActive = true;
				for(int i=0; i<activeStruct.Length; i++) {
					if((i+1) <= activeFieldLimit)
						activeStruct[i].itemActiveSelect.SetActive(isShow);
				}
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
						Destroy(activeStruct[index].itemEquipActiveCard);
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
			uiCards[skillSortCards[i].name].InListCard.SetActive(skillsRecord.Contains(uiCards[skillSortCards[i].name].Self.name));
	}
	
	private void refreshPassiveItems() {
		for(int i=0 ;i<itemPassiveCards.Count; i++) {
			itemPassiveCards[i].transform.localPosition = new Vector3(12, 110 - 70 * i, 0);
		}
	}

	private void refreshActiveItems() {
		if(activeStruct.Length > 1) {
			for (int i=0; i<activeStruct.Length; i++) {
				if(!activeStruct[i].CheckBeInstall) {
					for (int j=i+1; j<activeStruct.Length; j++) {
						if(activeStruct[j].CheckBeInstall) {
							TActiveStruct temp = activeStruct[j];
							activeStruct[i].SetData(temp);
							temp.itemEquipActiveCard.transform.parent = activeStruct[i].gridActiveCardBase.transform;
							temp.itemEquipActiveCard.transform.localPosition = Vector3.zero;
							activeStruct[j].ActiveClear();
							activeStruct[i].spriteActiveFieldIcon.gameObject.SetActive(false);
							activeStruct[j].spriteActiveFieldIcon.gameObject.SetActive(true);
							break;
						} else {
							activeStruct[i].spriteActiveFieldIcon.gameObject.SetActive(true);
							activeStruct[j].spriteActiveFieldIcon.gameObject.SetActive(true);
						}
					}
				}
			}
		}
		
		for (int i=0; i<activeStruct.Length; i++) {
			if((i+1) > activeFieldLimit)
				activeStruct[i].spriteActiveFieldIcon.spriteName = "Icon_lock";
		}
	}

	//page 0 1 2 3 4
	private void changePage (int page) {
		if(page != tempPage) {
			for(int i=0; i<toggleDecks.Length; i++) {
				toggleDecks[i].value = (i == page);
			}
			isChangePage = true;
			tempPage = page;
			DoFinish();
		}
	}

	//For Sell
	private bool isSkillCardInPages(int sn) {
		if(GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 0) {
			for (int i=0; i<GameData.Team.PlayerBank.Length; i++) {
				if(GameData.Team.PlayerBank[i].RoleIndex != GameData.Team.Player.RoleIndex) {
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

		if(GameData.Team.Player.SkillCardPages != null && GameData.Team.Player.SkillCardPages.Length > 0) {
			for (int i=0; i<GameData.Team.Player.SkillCardPages.Length; i++) {
				int[] SNs = GameData.Team.Player.SkillCardPages[i].SNs;
				if (SNs.Length > 0) {
					for (int j=0; j<SNs.Length; j++)
						if (SNs[j] == sn)
							return true;
				}
			}
		}
		return false;
	}

	private void setEditState (bool isEditState) {
		goSellCount.SetActive(isEditState);
		labelTotalPrice.text = "0";
		IsBuyState = isEditState;
		if(isEditState) {
			sellNames.Clear();
			sellPrice = 0;
			int index = 0;
			for(int i=0; i<skillSortCards.Count; i++) { 
				if(sortIsCanSell(skillSortCards[i])) {
					uiCards[skillSortCards[i].name].SellSelect.SetActive(true);
					uiCards[skillSortCards[i].name].UnavailableMask.SetActive(false);
					skillSortCards[i].transform.localPosition = new Vector3(-230 + 200 * (index / 2), 100 - 265 * (index % 2), 0);
					skillSortCards[i].SetActive(true);
					index++;
				} else
					skillSortCards[i].SetActive(false);
			}
			labelSell.text = "SELLX0";
		} else {
			labelSell.text = "SELL";
			for(int i=0; i<skillSortCards.Count; i++) {
				if(uiCards.ContainsKey(skillSortCards[i].name)) {
					skillSortCards[i].SetActive(true);
					uiCards[skillSortCards[i].name].SellSelect.SetActive(false); 
				}
			}
			refreshAfterInstall();
		}

	}
	
	private void addSellCards (string name) {
		if(uiCards.ContainsKey(name)) {
			if(!sellNames.Contains(uiCards[name].CardIndex))
				sellNames.Add(uiCards[name].CardIndex);
			sellPrice += GameData.DSkillData[uiCards[name].CardID].Money;
			labelTotalPrice.text = sellPrice.ToString();
			labelSell.text = "SELLX"+sellNames.Count.ToString();
		}
	}
	
	private void removeSellCards (string name) {
		if(uiCards.ContainsKey(name)) {
			if(sellNames.Contains(uiCards[name].CardIndex))
				sellNames.Remove(uiCards[name].CardIndex);
			sellPrice -= GameData.DSkillData[uiCards[name].CardID].Money;
			labelTotalPrice.text = sellPrice.ToString();
			labelSell.text = "SELLX"+sellNames.Count.ToString();
		}
	}

	private bool sortIsCanSell (GameObject card) {
		if(uiCards.ContainsKey(card.name))
			return (!isSkillCardInPages(uiCards[card.name].CardSN));
		return false;
	}

	private bool sortIsAvailable(GameObject card) {
		if(uiCards.ContainsKey(card.name))
			if(uiCards[card.name].InListCard != null)
				return !uiCards[card.name].InListCard.activeSelf;
		return false;
	}

	private bool sortIsSelected(GameObject card) {
		if(uiCards.ContainsKey(card.name))
			if(uiCards[card.name].InListCard != null)
				return uiCards[card.name].InListCard.activeSelf;
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
					int cardIdi = uiCards[skillSortCards[i].name].CardID;
					int cardIdj = uiCards[skillSortCards[j].name].CardID;
					string cardIdistr = uiCards[skillSortCards[i].name].Self.name;
					string cardIdjstr = uiCards[skillSortCards[j].name].Self.name;
					
					if(condition == ECondition.Rare.GetHashCode()) {
						if(GameData.DSkillData.ContainsKey(cardIdi))
							value1 = Mathf.Clamp(GameData.DSkillData[cardIdi].Star, 1, 5);
						if(GameData.DSkillData.ContainsKey(cardIdj))
							value2 =Mathf.Clamp(GameData.DSkillData[cardIdj].Star, 1, 5);
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

	private void setInfo (GameObject go) {
		if(GameData.DSkillData.ContainsKey(uiCards[go.name].CardID)) {
			skillInfo.ID = uiCards[go.name].CardID;
			skillInfo.Lv = uiCards[go.name].CardLV;
			skillInfo.SN = uiCards[go.name].CardSN;
//			skillInfo.Exp = 
		} else 
			Debug.LogWarning("cardId:"+uiCards[go.name].CardID);
	}

	public void CardDragStart() {
		IsDragNow = true;
	}

	public void CardDragEnd() {
		IsDragNow = false;
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

	public void DoUnEquipCard (){
		if(!IsBuyState) {
			if(tempObj != null && uiCards.ContainsKey(tempObj.name))
				removeItems(uiCards[tempObj.name].CardID, uiCards[tempObj.name].CardSN, tempObj);
			refreshCards();
		}
//		else 
//			UIHint.Get.ShowHint("It's Buy State.", Color.red);
	}

	public void DoEquipCard (){
		if(!IsBuyState) {
			if(GameFunction.IsActiveSkill(tempUICard.CardID)) {
				if(getContainActiveSN(tempUICard.CardSN) == -1){
					if(getActiveFieldNull != -1) {
						if(addItems(tempUICard, getActiveFieldNull)) 
							tempUICard.InListCard.SetActive(true);
					}
//					else 
//						UIHint.Get.ShowHint("Active is Full.", Color.red); 
				} 
//				else 
//					UIHint.Get.ShowHint("ActiveID is Same.", Color.red); 
				refreshActiveItems();
			} else {
				if(tempUICard.IsInstallIfDisapper) { //Selected to NoSelected
					tempUICard.InListCard.SetActive(!tempUICard.InListCard.activeInHierarchy);
					removeItems(tempUICard.CardID, tempUICard.CardSN, tempObj);
				} else { //NoSelected to Selected
					if(addItems(tempUICard))
						tempUICard.InListCard.SetActive(!tempUICard.InListCard.activeInHierarchy);
					
				}
			}
			refreshCards();
		} 
//		else 
//			UIHint.Get.ShowHint("It's Buy State.", Color.red);
	}

	public void OnCardDetailInfo (GameObject go){
		TUICard uicard = uiCards[go.name];
		if(!IsBuyState) {
			if(uicard.UnavailableMask != null) {
				if(tempObj != null) {
					if(tempObj != go) {
						if(tempUICard.Selected != null)
							tempUICard.Selected.SetActive(false);
					}
				}
				tempObj = go;
				tempUICard = uicard;
				setInfo(go);
				
				//Click Card
				if(uicard.Selected != null && uicard.InListCard != null) {
					uicard.Selected.SetActive(true);
					UISkillInfo.UIShow(true, skillInfo, uicard.InListCard.gameObject.activeSelf, uicard.UnavailableMask.activeSelf);
					if(UISort.Visible)
						UISort.UIShow(false);
				}
			}
		} else {
			if(!uicard.IsInstall) {
				if(!uicard.SellSelectCover.activeSelf)
					addSellCards(go.name);
				else
					removeSellCards(go.name);
				uicard.SellSelectCover.SetActive(!uicard.SellSelectCover.activeSelf);
			}
		}
	}

	public void OnItemDetailInfo (GameObject go){
		setInfo(go);
		UISkillInfo.UIShow(true, skillInfo, true, false);
		if(UISort.Visible)
			UISort.UIShow(false);
	}

	//From Item RemoveButton
	public void OnRemoveItem(GameObject go, bool state){
		removeItems(uiCards[go.transform.parent.name].CardID, uiCards[go.transform.parent.name].CardSN, go.transform.parent.gameObject);
		refreshCards();
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
				ActiveCheck.color = new Color32(255, 255, 255, 255);
				PassiveCheck.color = new Color32(255, 255, 255, 255);
				break;
			case (int)EFilter.Active:
				toggleCheckBoxSkill[0].value = true;
				toggleCheckBoxSkill[1].value = false;
				ActiveCheck.color = new Color32(255, 255, 255, 255);
				PassiveCheck.color = new Color32(50, 50, 50, 255);
				break;
			case (int)EFilter.Passive:
				toggleCheckBoxSkill[0].value = false;
				toggleCheckBoxSkill[1].value = true;
				ActiveCheck.color = new Color32(50, 50, 50, 255);
				PassiveCheck.color = new Color32(255, 255, 255, 255);
				break;
			case (int)EFilter.Available:
			case (int)EFilter.Select:
				toggleCheckBoxSkill[0].value = false;
				toggleCheckBoxSkill[1].value = false;
				ActiveCheck.color = new Color32(50, 50, 50, 255);
				PassiveCheck.color = new Color32(50, 50, 50, 255);
				break;
		}
	}

	public void DoOpenActive (){
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

	public void DoOpenPassive (){
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
			if(!isLeave) {
				if(isChangePage) {
					SendChangeSkillPage();
				} else 
					if(IsBuyState)
						setEditState(IsBuyState);
			} else {
				UIShow(false);
				if(UISort.Visible)
					UISort.UIShow(false);
				UIMainLobby.Get.Show();
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

			if(!isLeave) {
				if(!IsBuyState) {
					if(isChangePage) {
						SendChangeSkillPage();
					}
					refreshAfterInstall ();
				} else 
					refreshBeforeSell();
			} else {
				UIShow(false);
				if(UISort.Visible)
					UISort.UIShow(false);
				UIMainLobby.Get.Show();
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
			setEditState(false);
			UIMainLobby.Get.UpdateUI();
		} else {
			Debug.LogError("text:"+www.text);
		}
	}
}
