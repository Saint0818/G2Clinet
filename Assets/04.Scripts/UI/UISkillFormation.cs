using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameStruct;
using DG.Tweening;

public struct TSkillInfo {
	public int ID;
	public string Name;
	public string Lv;
	public string Info;
	public TSkillInfo (int i){
		this.ID = 0;
		this.Name = "";
		this.Lv = "";
		this.Info = "";
	}
}

public struct TEquipSkillCardResult {
	public TSkill[] SkillCards;
	public TSkill[] PlayerCards;
	public int[, ] SkillCardPages;
	//public TSkillCardPage[] SkillCardPages;
}

public struct TActiveStruct {
	public GameObject itemEquipActiveCard;
	public GameObject itemActiveField;
	public GameObject gridActiveCardBase;
	public int CardIndex;
	public int CardID;
	public int CardSN;
	public int CardLV;
	public TActiveStruct (int i){
		this.itemEquipActiveCard = null;
		this.itemActiveField = null;
		this.gridActiveCardBase = null;
		this.CardIndex = -1;
		this.CardID = 0;
		this.CardSN = -1;
		this.CardLV = 0;
	}
	public void ActiveClear () {
		this.itemEquipActiveCard = null;
		this.CardIndex = -1;
		this.CardID = 0;
		this.CardSN = -1;
		this.CardLV = 0;
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
	public int CardIndex;
	public int CardID;
	public int CardLV;
	public int Cost;
	public int CardSN;
	public TUICard (int i){
		Self = null;
		SkillCard = null;
		SkillPic = null;
		SkillLevel = null;
		SkillName = null;
		SkillStar = null;
		UnavailableMask = null;
		Selected = null;
		InListCard = null;
		SellSelect = null;
		SellSelectCover = null;
		CardIndex = -1;
		CardID = 0;
		CardLV = 0;
		Cost = 0;
		CardSN = -1;
	}
}

public class UISkillFormation : UIBase {
	private static UISkillFormation instance = null;
	private const string UIName = "UISkillFormation";

	//Send Value
	private int[] removeIndexs = new int[0]; //From already setted skillCard's index
	private int[] addIndexs = new int[0];//From skillCards's index in the center area

	//Sell Value
	private List<string> sellNames = new List<string>();
	
	//Instantiate Object
	private GameObject itemSkillCard;
	private GameObject itemCardEquipped;

	//CenterCard
	private List<GameObject> skillSortCards = new List<GameObject>();//By Sort
	private List<string> skillsOriginal = new List<string>();//record alread equiped   rule: index_id_skillsOriginal(Equiped)_cardLV
	private List<string> skillsRecord = new List<string>();

	//key:GameData.SkillCards.index_cardID_skillsOriginal(Equiped)_cardLV Value: TSkill   For Get Level
	//only record skillsOriginal(Equiped) 0:Not Equiped 1:Equiped (First Time)
	private Dictionary<string, TSkill> skillActiveCards = new Dictionary<string, TSkill>(); 
	private Dictionary<string, TSkill> skillPassiveCards = new Dictionary<string, TSkill>();
	private Dictionary<string, TUICard> uiCards = new Dictionary<string, TUICard>();

	//RightItem  name:Skill
	private TActiveStruct[] activeStruct = new TActiveStruct[3];
	private List<GameObject> itemPassiveCards = new List<GameObject>();
	private GameObject itemPassiveField;

	//Right(itemCardEquipped Parent)
	private GameObject gridPassiveCardBase;
	private UIScrollView scrollViewItemList;

	//Total Cost
	private UILabel labelCostValue;

	//Left CardView
	private GameObject gridCardList;
	private UIScrollView scrollViewCardList;

	//Sell
	private UILabel labelSell;
	private GameObject goSellCount;
	private UILabel labelTotalPrice;
	private int sellPrice;

	//Desk
	private UIToggle[] toggleDecks = new UIToggle[5];

	//Info
	private TSkillInfo skillInfo = new TSkillInfo();
	//for InfoEquip temp
	private TUICard tempUICard;
	private GameObject tempObj;


	private Vector3 point;
	private int costSpace = 0;
	private int costSpaceMax = 15;
	private bool isEdit = false;
	private ECondition eCondition = ECondition.Rare;
	private int eFilter = 0;

	//page
	private int tempPage = 0;
	private bool isChangePage = false;
	private Dictionary<int,List<int>> skillPagesOriginal = new Dictionary<int, List<int>>(); // page,  SN
	private Dictionary<int,List<int>> skillPages = new Dictionary<int, List<int>>(); // page,  SN

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

	protected override void InitCom() {
		itemSkillCard = Resources.Load("Prefab/UI/Items/ItemSkillCard") as GameObject;
		itemCardEquipped = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;
		tempPage = GameData.Team.Player.SkillPage;
		for(int i=0; i<toggleDecks.Length; i++) {
			toggleDecks[i] = GameObject.Find(UIName + "/MainView/Right/DecksList/DecksBtn"+ i.ToString()).GetComponent<UIToggle>();
			toggleDecks[i].name = i.ToString();
			UIEventListener.Get (toggleDecks[i].gameObject).onClick = OnChangePage;
			toggleDecks[i].value = (i == tempPage);
			toggleDecks[i].gameObject.SetActive((i<2));// it need judge by player level
		}
		for(int i=0; i<activeStruct.Length; i++) {
			activeStruct[i].gridActiveCardBase = GameObject.Find (UIName + "/MainView/Right/ActiveCardBase"+i.ToString());
			activeStruct[i].itemActiveField = GameObject.Find (UIName + "/MainView/Right/ActiveCardBase" + i.ToString() + "/ActiveField/Icon");
		}
		gridPassiveCardBase = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveList");
		labelCostValue = GameObject.Find (UIName + "/BottomRight/LabelCost/CostValue").GetComponent<UILabel>();
		scrollViewItemList = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveList").GetComponent<UIScrollView>();
		scrollViewItemList.transform.localPosition = new Vector3(0, 15, 0);
		scrollViewItemList.panel.clipOffset = new Vector2(12, 0);
		scrollViewItemList.onStoppedMoving =ItemDragFinish;

		gridCardList = GameObject.Find (UIName + "/CardsView/Left/CardsGroup/CardList");
		scrollViewCardList = GameObject.Find (UIName + "/CardsView/Left/CardsGroup/CardList").GetComponent<UIScrollView>();
		itemPassiveField = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveField/Icon");

		labelSell = GameObject.Find (UIName + "/BottomLeft/SellBtn/Icon").GetComponent<UILabel>();
		goSellCount = GameObject.Find (UIName + "/BottomLeft/SellBtn/SellCount");
		goSellCount.SetActive(false);
		labelTotalPrice = GameObject.Find (UIName + "/BottomLeft/SellBtn/SellCount/TotalPrice").GetComponent<UILabel>();

		SetBtnFun (UIName + "/BottomLeft/SortBtn", DoSort);
//		SetBtnFun (UIName + "/BottomLeft/SellBtn", DoSell);
//		SetBtnFun (UIName + "/BottomLeft/SellBtn/SellCount/CancelBtn", DoCloseSell);
		SetBtnFun (UIName + "/BottomLeft/BackBtn", DoBack);
		SetBtnFun (UIName + "/BottomRight/CheckBtn", DoFinish);
		initCards ();
		labelCostValue.text = costSpace + "/" + costSpaceMax;
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	private void refresh(){
		costSpace = 0;
		for(int i=0; i<skillSortCards.Count; i++) {
			Destroy(skillSortCards[i]);
		}
		skillPages.Clear();
		skillPagesOriginal.Clear();
		skillSortCards.Clear();
		skillsOriginal.Clear();
		skillsRecord.Clear();
		skillActiveCards.Clear();
		skillPassiveCards.Clear();
		uiCards.Clear();
		for (int i=0; i<activeStruct.Length; i++) {
			if(activeStruct[i].itemEquipActiveCard != null)
				Destroy(activeStruct[i].itemEquipActiveCard);
			activeStruct[i].itemActiveField.SetActive(true);
			activeStruct[i].ActiveClear();
		}
		for (int i=0; i<itemPassiveCards.Count; i++) {
			Destroy(itemPassiveCards[i]);
		}
		itemPassiveCards.Clear();
		itemPassiveField.SetActive(true);
	}

	private void refreshAfterInstall () {
		refresh();
		initCards ();
		SetSort(eCondition, eFilter);
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
		SetSort(eCondition, eFilter);
	}

	private void initCards () {
//		costSpaceMax = GameData.Team.Player.MaxSkillSpace;
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
					
						skillsOriginal.Add (obj.name);
					if(GameData.Team.Player.SkillCards[i].ID >= GameConst.ID_LimitActive) {
						actvieIndex++;
						addItems(uiCards[obj.name], actvieIndex);
						skillActiveCards.Add(obj.name, GameData.Team.Player.SkillCards[i]);
					} else {
						addItems(uiCards[obj.name]);
						skillPassiveCards.Add(obj.name, GameData.Team.Player.SkillCards[i]);
					}
					skillSortCards.Add(obj);
				}
			}
		}
		// not equiped
		if(GameData.Team.SkillCards != null && GameData.Team.SkillCards.Length > 0) {
			for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
				GameObject obj = null;
				if(GameData.Team.SkillCards[i].ID > 100 && GameData.DSkillData.ContainsKey(GameData.Team.SkillCards[i].ID)){
					index ++;
					obj = addUICards(i,
					                 index, 
					                 GameData.Team.SkillCards[i],
					                 gridCardList, 
					                 false);
					if(GameData.Team.SkillCards[i].ID >= GameConst.ID_LimitActive) 
						skillActiveCards.Add(obj.name, GameData.Team.SkillCards[i]);
					else 
						skillPassiveCards.Add(obj.name, GameData.Team.SkillCards[i]);

					skillSortCards.Add(obj);
				}
			}
		}

		refreshPassiveItems();
		checkCostIfMask();
	}

	private int getActiveFieldNull () {
		for(int i=0; i<activeStruct.Length; i++)
			if(activeStruct[i].itemEquipActiveCard == null)
				return i;
		return -1;
	}

	private int getContainActiveID (int id) {
		for(int i=0; i<activeStruct.Length; i++) 
			if(activeStruct[i].itemEquipActiveCard != null)
				if(activeStruct[i].CardID == id)
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

	private void setEditState (bool isedit) {
		goSellCount.SetActive(isedit);
		labelTotalPrice.text = "0";
		if(isedit) {
			sellNames.Clear();
			sellPrice = 0;
			foreach (KeyValuePair<string, TUICard> uicard in uiCards){
				if(!uicard.Value.InListCard.activeInHierarchy) 
					uicard.Value.SellSelect.SetActive (true);
				uicard.Value.SellSelectCover.SetActive(false);
			}
		} else {
			foreach (KeyValuePair<string, TUICard> uicard in uiCards)
				uicard.Value.SellSelect.SetActive (false);
			labelSell.text = "SELL";
		}
	}

	private void addSellCards (string name) {
		if(!sellNames.Contains(name))
			sellNames.Add(name);
		sellPrice ++;
		labelTotalPrice.text = sellPrice.ToString();
		labelSell.text = "SELL"+sellNames.Count.ToString();
	}

	private void removeSellCards (string name) {
		if(sellNames.Contains(name))
			sellNames.Remove(name);
		sellPrice --;
		labelTotalPrice.text = sellPrice.ToString();
		labelSell.text = "SELL"+sellNames.Count.ToString();
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
			obj.transform.parent = parent.transform;
			obj.transform.name = skillCardIndex.ToString() + "_" + skill.ID.ToString()+ "_" + skill.SN.ToString() + "_" + skill.Lv.ToString();
			obj.transform.localPosition = new Vector3(-230 + 200 * (positionIndex / 2), 100 - 265 * (positionIndex % 2), 0);
			obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			UIEventListener.Get(obj).onClick = OnCardDetailInfo;

			TUICard uicard = new TUICard(1);
			uicard.Self = obj;
			uicard.CardID = skill.ID;
			uicard.CardIndex = skillCardIndex;
			uicard.CardLV = skill.Lv;
			uicard.CardSN = skill.SN;
			if(GameData.DSkillData.ContainsKey(skill.ID))
				uicard.Cost = Mathf.Max(GameData.DSkillData[skill.ID].Space(skill.Lv), 1);

			Transform t = obj.transform.FindChild("SkillCard");
			if(t != null) {
				uicard.SkillCard = t.gameObject.GetComponent<UISprite>();
				uicard.SkillCard .spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[skill.ID].Quality, 1, 5).ToString();
			}

			t = obj.transform.FindChild("SkillPic");
			if(t != null){
				uicard.SkillPic = t.gameObject.GetComponent<UITexture>();
				uicard.SkillPic.mainTexture = GameData.CardTexture(skill.ID);
			}
			
			t = obj.transform.FindChild("SkillLevel");
			if(t != null) {
				uicard.SkillLevel = t.gameObject.GetComponent<UISprite>();
				uicard.SkillLevel.spriteName = "Cardicon" + Mathf.Clamp(skill.Lv, 1, 5).ToString();
			}

			t = obj.transform.FindChild("SkillName");
			if(t != null) {
				uicard.SkillName = t.gameObject.GetComponent<UILabel>();
				if(GameData.DSkillData.ContainsKey(skill.ID))
					uicard.SkillName.text = GameData.DSkillData[skill.ID].Name;
			}

			t = obj.transform.FindChild("SkillStar");
			if(t != null) {
				uicard.SkillStar = t.gameObject.GetComponent<UISprite>();
				if(GameData.DSkillData.ContainsKey(skill.ID))
					uicard.SkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[skill.ID].Star, 1, GameData.DSkillData[skill.ID].MaxStar).ToString();
			}

		t = obj.transform.FindChild("UnavailableMask");
		if(t != null) {
			uicard.UnavailableMask = t.gameObject;
			uicard.UnavailableMask.SetActive(false);
		}

		t = obj.transform.FindChild("Selected");
		if(t != null) {
			uicard.Selected = t.gameObject;
			uicard.Selected.SetActive(false);
		}

		t = obj.transform.FindChild("InListCard");
		if(t != null) {
			uicard.InListCard = t.gameObject;
			uicard.InListCard.SetActive(isEquip);
		}

		t = obj.transform.FindChild("SellSelect");
		if(t != null) {
			uicard.SellSelect = t.gameObject;
			uicard.SellSelect.SetActive(false);
		}

		t = obj.transform.FindChild("SellSelect/SellCover");
		if(t != null)  {
			uicard.SellSelectCover = t.gameObject;
			uicard.SellSelectCover.SetActive(false);
		}


		uiCards.Add(obj.transform.name, uicard);

			return obj;
		} else
			return null;
	}

	private GameObject addUIItems (TUICard uicard, int positionIndex, GameObject parent) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = uicard.CardIndex.ToString() + "_" + uicard.CardID.ToString() + "_" + uicard.CardSN.ToString() + "_" + uicard.CardLV.ToString();
		if(uicard.CardID >= GameConst.ID_LimitActive)
			obj.transform.localPosition = Vector3.zero;
		else 
			obj.transform.localPosition = new Vector3(12, 110 - 70 * positionIndex, 0);
		obj.transform.localScale = Vector3.one;

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

		//no texture
//		t = obj.transform.FindChild("SkillTexture");
//		if(t != null)
//			if(GameData.DSkillData.ContainsKey(iduicard.CardID))
//				t.gameObject.GetComponent<UITexture>().mainTexture = GameData.CardTexture(uicard.CardID);


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
			if(uicard.CardID < GameConst.ID_LimitActive) {
				obj = addUIItems(uicard, itemPassiveCards.Count, gridPassiveCardBase);
				itemPassiveCards.Add(obj);
				itemPassiveField.SetActive(false);
			} else {
				if(activeStructIndex != -1 && activeStructIndex < 3) {
					obj = addUIItems(uicard, 0, activeStruct[activeStructIndex].gridActiveCardBase);
					activeStruct[activeStructIndex].itemEquipActiveCard = obj;
					activeStruct[activeStructIndex].CardID = uicard.CardID;
					activeStruct[activeStructIndex].CardIndex = uicard.CardIndex;
					activeStruct[activeStructIndex].CardLV = uicard.CardLV;
					activeStruct[activeStructIndex].CardSN = uicard.CardSN;
					activeStruct[activeStructIndex].itemActiveField.SetActive(false);
				}
			}
			if(obj != null) {
				if(!skillsRecord.Contains (obj.name))
					skillsRecord.Add(obj.name);
			} else 
				Debug.LogError("addItems obj: null");

			checkCostIfMask();
			return true;
		} else 
			UIHint.Get.ShowHint("More than SpaceMax", Color.red);

		return false;
	}

	private void removeItems(int id, GameObject go = null) {
		if(setCost(-uiCards[go.name].Cost)){
			if(skillsRecord.Contains(go.name))
				skillsRecord.Remove(go.name);
			if(id < GameConst.ID_LimitActive) {
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
				int index = getContainActiveID(id);
				if(activeStruct[index].itemEquipActiveCard != null) {
					if(skillsRecord.Contains(activeStruct[index].itemEquipActiveCard.name))
						skillsRecord.Remove(activeStruct[index].itemEquipActiveCard.name);
					Destroy(activeStruct[index].itemEquipActiveCard);
				}
				activeStruct[index].ActiveClear();
				
				refreshActiveItems();
			}
			checkCostIfMask();
		}
	}
	
	private void refreshCards(string name) {
		for(int i=0 ;i<skillSortCards.Count; i++) {
			if(skillSortCards[i].name.Equals(name)) {
				uiCards[skillSortCards[i].name].InListCard.SetActive(skillsRecord.Contains(uiCards[skillSortCards[i].name].Self.name));
			}
		}
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
	}
	
	private void refreshPassiveItems() {
		for(int i=0 ;i<itemPassiveCards.Count; i++) {
			itemPassiveCards[i].transform.localPosition = new Vector3(12, 110 - 70 * i, 0);
		}

		gridPassiveCardBase.SetActive(false);
		gridPassiveCardBase.SetActive(true);
	}

	private void refreshActiveItems() {
		for (int i=0; i<activeStruct.Length; i++) {
			if(activeStruct[i].itemEquipActiveCard == null) {
				for (int j=i+1; j<activeStruct.Length; j++) {
					if(activeStruct[j].itemEquipActiveCard != null) {
						TActiveStruct temp = activeStruct[j];
						activeStruct[i].itemEquipActiveCard = temp.itemEquipActiveCard;
						activeStruct[i].CardID = temp.CardID;
						activeStruct[i].CardIndex = temp.CardIndex;
						activeStruct[i].CardLV = temp.CardLV;
						activeStruct[i].CardSN = temp.CardSN;
						temp.itemEquipActiveCard.transform.parent = activeStruct[i].gridActiveCardBase.transform;
						temp.itemEquipActiveCard.transform.localPosition = Vector3.zero;
						activeStruct[j].ActiveClear();
						activeStruct[i].itemActiveField.SetActive(false);
						activeStruct[j].itemActiveField.SetActive(true);
						activeStruct[i].gridActiveCardBase.SetActive(false);
						activeStruct[i].gridActiveCardBase.SetActive(true);
						activeStruct[j].gridActiveCardBase.SetActive(false);
						activeStruct[j].gridActiveCardBase.SetActive(true);
						break;
					} else {
						activeStruct[i].itemActiveField.SetActive(true);
						activeStruct[j].itemActiveField.SetActive(true);
					}
				}
			}
		}
	}

	//page 0 1 2 3 4
	private void changePage (int page) {
		if(page != tempPage) {
			if(!skillPages.ContainsKey(page)){
				skillPages.Add(page, new List<int>());
				skillPagesOriginal.Add(page, new List<int>());
			}
			for(int i=0; i<toggleDecks.Length; i++) {
				toggleDecks[i].value = (i == page);
			}
			isChangePage = true;
			tempPage = page;
			DoFinish();
		}
	}

	//For Sell
//	private bool isSkillCardInPages(int sn) {
//		for (int i=0; i<5; i++) {
//			int[] SNs = GameData.Team.Player.SkillCardPages[i].SNs;
//			if (SNs.Length > 0) {
//				for (int j=0; j<SNs.Length; j++)
//					if (SNs[j] == sn)
//						return true;
//			}
//		}
//		return false;
//	}

	private bool sortIsAvailable(GameObject card) {
		if(uiCards[card.name].InListCard != null)
			return !uiCards[card.name].InListCard.activeSelf;
		return false;
	}

	private bool sortIsSelected(GameObject card) {
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
	
	private void sortSkillCondition(ECondition condition) {
		int value1 = 0;
		int value2 = 0;
		for(int i=0; i<skillSortCards.Count; i++) {
			for (int j=i+1; j<skillSortCards.Count; j++){
				int cardIdi = uiCards[skillSortCards[i].name].CardID;
				int cardIdj = uiCards[skillSortCards[j].name].CardID;
				string cardIdistr = uiCards[skillSortCards[i].name].Self.name;
				string cardIdjstr = uiCards[skillSortCards[j].name].Self.name;

				if(condition == ECondition.Rare) {
					if(GameData.DSkillData.ContainsKey(cardIdi))
						value1 = Mathf.Clamp(GameData.DSkillData[cardIdi].Star, 1, 5);
					if(GameData.DSkillData.ContainsKey(cardIdj))
						value2 =Mathf.Clamp(GameData.DSkillData[cardIdj].Star, 1, 5);
				} else 
				if(condition == ECondition.Kind){
					if(GameData.DSkillData.ContainsKey(cardIdi))
						value1 = GameData.DSkillData[cardIdi].Kind;
					if(GameData.DSkillData.ContainsKey(cardIdj))
						value2 = GameData.DSkillData[cardIdj].Kind;
				} else 
				if(condition == ECondition.Attribute){
					if(GameData.DSkillData.ContainsKey(cardIdi))
						value1 = GameData.DSkillData[cardIdi].AttrKind;
					if(GameData.DSkillData.ContainsKey(cardIdj))
						value2 = GameData.DSkillData[cardIdj].AttrKind;
				}  else 
				if(condition == ECondition.Level){
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
				if(condition == ECondition.Cost){
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
				
				if (value1 > value2){
					GameObject temp = skillSortCards[i];
					skillSortCards[i] = skillSortCards[j];
					skillSortCards[j] = temp;
				}
			}
		}
	}
	
	private void sortSkillFilter (int filter){
		int index = 0;
		for(int i=0; i<skillSortCards.Count; i++) { 
			bool result = false;
			switch (filter) {
			case 0:
			case 15://All Choose
				result = true;
				break;
			case 1://Available
				result = sortIsAvailable(skillSortCards[i]);
				break;
			case 2://Select
				result = sortIsSelected(skillSortCards[i]);
				break;
			case 3://Available + Select
				result = (sortIsAvailable(skillSortCards[i]) || sortIsSelected(skillSortCards[i]));
				break;
			case 4://Active
				result = sortIsActive(skillSortCards[i]);
				break;
			case 5://Available + Active
				result = (sortIsAvailable(skillSortCards[i]) || sortIsActive(skillSortCards[i]));
				break;
			case 6://Select + Active
				result = (sortIsSelected(skillSortCards[i]) || sortIsActive(skillSortCards[i]));
				break;
			case 7://Available + Select + Active
				result = (sortIsAvailable(skillSortCards[i]) || sortIsSelected(skillSortCards[i]) || sortIsActive(skillSortCards[i]));
				break;
			case 8://Passive
				result = sortIsPassive(skillSortCards[i]);
				break;
			case 9://Available + Passive
				result = (sortIsAvailable(skillSortCards[i]) || sortIsPassive(skillSortCards[i]));
				break;
			case 10://Select + Passive
				result = (sortIsSelected(skillSortCards[i]) || sortIsPassive(skillSortCards[i]));
				break;
			case 11://Available + Select + Passive
				result = (sortIsAvailable(skillSortCards[i]) || sortIsSelected(skillSortCards[i]) || sortIsPassive(skillSortCards[i]));
				break;
			case 12://Passive + Active
				result = (sortIsActive(skillSortCards[i]) || sortIsPassive(skillSortCards[i]));
				break;
			case 13://Available + Passive + Active
				result = (sortIsAvailable(skillSortCards[i]) || sortIsActive(skillSortCards[i]) || sortIsPassive(skillSortCards[i]));
				break;
			case 14://Available + Passive + Select
				result = (sortIsAvailable(skillSortCards[i]) || sortIsSelected(skillSortCards[i]) || sortIsPassive(skillSortCards[i]));
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
			skillInfo.Name = GameData.DSkillData[uiCards[go.name].CardID].Name;
			skillInfo.Lv = uiCards[go.name].CardLV.ToString();
			skillInfo.Info = GameData.DSkillData[uiCards[go.name].CardID].Explain;
		} else 
			Debug.LogError("cardId:"+uiCards[go.name].CardID);
	}

	public void ItemDragFinish(){
		if(itemPassiveCards.Count < 5){
			scrollViewItemList.transform.DOLocalMoveY(15, 0.2f).OnUpdate(UpdateClipOffset);
		}
	}

	public void UpdateClipOffset(){
		scrollViewItemList.panel.clipOffset = new Vector2(12, 0);
	}

	public void ItemMoveOne(){ 
		if(itemPassiveCards.Count > 4){
			scrollViewItemList.transform.DOLocalMoveY(scrollViewItemList.transform.localPosition.y - 75 , 0.2f);
			scrollViewItemList.panel.clipOffset = new Vector2(12, scrollViewItemList.panel.clipOffset.y + 75);
		}
	}

	public void DoUnEquipCard (){
		if(!isEdit) {
			removeItems(uiCards[tempObj.name].CardID, tempObj);
			refreshCards(tempObj.name);
		} else 
			UIHint.Get.ShowHint("It's Buy State.", Color.red);
	}

	public void DoEquipCard (){
		if(!isEdit) {
			if(tempUICard.CardID >= GameConst.ID_LimitActive) {
				if(getContainActiveID(tempUICard.CardID) == -1){
					int temp = getActiveFieldNull();
					if(temp != -1) {
						if(addItems(tempUICard, temp)) 
							tempUICard.InListCard.SetActive(true);
					} else 
						UIHint.Get.ShowHint("Active is Full.", Color.red); 
				} else {
					UIHint.Get.ShowHint("ActiveID is Same.", Color.red); 
				}
				refreshActiveItems();
			} else {
				if(tempUICard.InListCard.activeInHierarchy) {
					//Selected to NoSelected
					tempUICard.InListCard.SetActive(!tempUICard.InListCard.activeInHierarchy);
					removeItems(tempUICard.CardID, tempObj);
				} else {
					//NoSelected to Selected
					if(addItems(tempUICard))
						tempUICard.InListCard.SetActive(!tempUICard.InListCard.activeInHierarchy);
					
				}
			}
			refreshCards(tempObj.name);
		} else 
			UIHint.Get.ShowHint("It's Buy State.", Color.red);
	}

	public void OnCardDetailInfo (GameObject go){
		TUICard uicard = uiCards[go.name];
		if(!isEdit) {
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
				}
			}
		} else {
			if(uicard.InListCard != null && uicard.InListCard.activeSelf)
				UIHint.Get.ShowHint("It's Buy State.", Color.red);
			else {
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
	}

	//From Item RemoveButton
	public void OnRemoveItem(GameObject go, bool state){
		removeItems(uiCards[go.transform.parent.name].CardID, go.transform.parent.gameObject);
		refreshCards(go.transform.parent.name);
	}
	
	public void SetSort (ECondition condition, int filter) {
		eCondition = condition;
		eFilter = filter;
		sortSkillCondition(condition);
		sortSkillFilter(filter);
		
		scrollViewCardList.ResetPosition();
		gridCardList.transform.localPosition = Vector3.zero;
		scrollViewCardList.panel.clipOffset = new Vector2(0, scrollViewCardList.panel.clipOffset.y);
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
	}

	public void DoOpenActive (){
		//Open Actvie Cards
		sortSkillFilter((int)EFilter.Active);
		scrollViewCardList.ResetPosition();
		scrollViewCardList.panel.clipOffset = new Vector2(0, scrollViewCardList.panel.clipOffset.y);
		gridCardList.transform.localPosition = Vector3.zero;
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
	}

	public void DoOpenPassive (){
		//Open Passive Cards
		sortSkillFilter((int)EFilter.Passive);
		scrollViewCardList.ResetPosition();
		scrollViewCardList.panel.clipOffset = new Vector2(0, scrollViewCardList.panel.clipOffset.y);
		gridCardList.transform.localPosition = Vector3.zero;
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
	}

	public void DoSell() {
		if(!isEdit) {
			isEdit = true;
			DoFinish();
		} else {
			//update sell index
		}
	}

	public void DoCloseSell () {
		isEdit = false;
		setEditState(false);
	}
	
	public void DoSort() {
		if(!isEdit) {
			UISort.UIShow(!UISort.Visible, 0);
		} else 
			UIHint.Get.ShowHint("It's Buy State.", Color.red);
	}

	public void DoBack() {
		UIShow(false);
		UIMainLobby.Get.Show();
	}

	public void OnChangePage (GameObject obj) {
		int index;
		if(int.TryParse (obj.name, out index))
			changePage(index);
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
		if(tempRemoveIndex.Count > 0) {
			for (int i=0; i<removeIndexs.Length; i++) {
				removeIndexs[i] = uiCards[tempRemoveIndex[i]].CardIndex;
			}
		}

		for (int i=0; i<skillsRecord.Count; i++) {
			if(!tempNoUpdate.Contains(skillsRecord[i])) {
				tempAddIndex.Add(skillsRecord[i]);	
			}
		}

		addIndexs = new int[tempAddIndex.Count];
		if(tempAddIndex.Count > 0) {
			for(int i=0; i<addIndexs.Length; i++) 
				addIndexs[i] = uiCards[tempAddIndex[i]].CardIndex;
		}

		for (int i = 0; i > addIndexs.Length; i++){
			for (int j = 0; j < addIndexs.Length; j++){
				if (addIndexs[i] > addIndexs[j]){

				}
			}
		}

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
		
		if(addIndexs.Length > 0 || removeIndexs.Length > 0) {
			WWWForm form = new WWWForm();
			form.AddField("RemoveIndexs", JsonConvert.SerializeObject(removeIndexs));
			form.AddField("AddIndexs", JsonConvert.SerializeObject(addIndexs));
			SendHttp.Get.Command(URLConst.EquipsSkillCard, waitEquipSkillCard, form);
		} else
			if(isChangePage) {
				WWWForm form = new WWWForm();
				form.AddField("Page", tempPage);
				SendHttp.Get.Command(URLConst.ChangeSkillPage, waitChangeSkillPage, form);
			} else 
				setEditState(isEdit);
	}

	private void waitEquipSkillCard(bool ok, WWW www) {
		if (ok) {
			TEquipSkillCardResult result = JsonConvert.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerCards;
			GameData.Team.Player.SkillCardPages = result.SkillCardPages;
			GameData.Team.Player.Init();

			if(!isEdit) {
				if(!isChangePage)
					UIHint.Get.ShowHint("Install Success!!", Color.red);	
				else {
					isChangePage = false; 
					GameData.Team.Player.SkillPage = tempPage;
				}
				refreshAfterInstall ();
			} else 
				refreshBeforeSell();

		} else {
			Debug.LogError("text:"+www.text);
		}
	}

	private void waitChangeSkillPage(bool ok, WWW www) {
		if (ok) {
			TEquipSkillCardResult result = JsonConvert.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerCards;
			GameData.Team.Player.SkillCardPages = result.SkillCardPages;
			GameData.Team.Player.Init();
			isChangePage = false; 
			GameData.Team.Player.SkillPage = tempPage;
			refreshAfterInstall ();
			
		} else {
			Debug.LogError("text:"+www.text);
		}
	}
}
