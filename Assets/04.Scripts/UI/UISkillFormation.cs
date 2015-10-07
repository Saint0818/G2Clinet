﻿using UnityEngine;
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
}

public struct TActiveStruct {
	public GameObject itemEquipActiveCard;
	public GameObject itemActiveField;
	public GameObject gridActiveCardBase;
	public int CardIndex;
	public int CardID;
	public string EquipedType;
	public int CardLV;
	public TActiveStruct (int i){
		this.itemEquipActiveCard = null;
		this.itemActiveField = null;
		this.gridActiveCardBase = null;
		this.CardIndex = -1;
		this.CardID = 0;
		this.EquipedType = "0";
		this.CardLV = 0;
	}
	public void ActiveClear () {
		this.itemEquipActiveCard = null;
		this.CardIndex = -1;
		this.CardID = 0;
		this.EquipedType = "0";
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
	public int CardIndex;
	public int CardID;
	public string EquipedType;
	public int CardLV;
	public int Cost;
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
		CardIndex = -1;
		CardID = 0;
		EquipedType = "0";
		CardLV = 0;
		Cost = 0;
	}
}

public class UISkillFormation : UIBase {
	private static UISkillFormation instance = null;
	private const string UIName = "UISkillFormation";

	//Send Value
	private int[] removeIndexs = new int[0]; //From already setted skillCard's index
	private int[] addIndexs = new int[0];//From skillCards's index in the center area

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

	//Instantiate Object
	private GameObject itemSkillCard;
	private GameObject itemCardEquipped;

	//Right(Put itemCardEquipped Object)
	private GameObject gridPassiveCardBase;
	private UIScrollView scrollViewItemList;
	
	private UILabel labelCostValue;

	//Center
	private GameObject gridCardList;
	private UIScrollView scrollViewCardList;

	//Info
	private TSkillInfo skillInfo = new TSkillInfo();
	//for InfoEquip temp
	private TUICard tempUICard;
	private GameObject tempObj;


	private Vector3 point;
	private int costSpace = 0;
	private int costSpaceMax = 15;

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

		itemSkillCard = Resources.Load("Prefab/UI/Items/ItemSkillCard") as GameObject;
		itemCardEquipped = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;
		itemPassiveField = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveField/Icon");

		SetBtnFun (UIName + "/BottomLeft/SortBtn", DoSort);
		SetBtnFun (UIName + "/BottomLeft/BackBtn", DoBack);
		SetBtnFun (UIName + "/BottomRight/CheckBtn", DoFinish);
		initCards ();
		labelCostValue.text = costSpace + "/" + costSpaceMax;
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	private void initCards () {
//		costSpaceMax = GameData.Team.Player.MaxSkillSpace;
		int index = -1;
		//Already Equiped
		if(GameData.Team.Player.SkillCards.Length > 0) {
			for (int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
				GameObject obj = null;
				index ++;
				obj = addUICards(i,
				                 index, 
				                 GameData.Team.Player.SkillCards[i].ID, 
				                 GameData.Team.Player.SkillCards[i].Lv,
				                 gridCardList, 
				                 "1");
				skillsOriginal.Add (obj.name);
				addItems(uiCards[obj.name], i);

				if(GameData.Team.Player.SkillCards[i].ID >= GameConst.ID_LimitActive) {
					skillActiveCards.Add(obj.name, GameData.Team.Player.SkillCards[i]);
				} else {
					skillPassiveCards.Add(obj.name, GameData.Team.Player.SkillCards[i]);
				}
				skillSortCards.Add(obj);
			}
		}
		// not equiped
		if(GameData.Team.SkillCards.Length > 0) {
			for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
				GameObject obj = null;
				if(GameData.Team.SkillCards[i].ID > 100) {
					index ++;
					obj = addUICards(i,
					                 index, 
					                 GameData.Team.SkillCards[i].ID, 
					                 GameData.Team.SkillCards[i].Lv,
					                 gridCardList, 
					                 "0");
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

	private bool setCost(int space) {
		if((costSpace + space) <= costSpaceMax) {
			costSpace += space;
			labelCostValue.text = costSpace + "/" + costSpaceMax; 
			return true;
		} else 
			return false;
	}

	private GameObject addUICards (int skillCardIndex, int positionIndex, int id, int lv, GameObject parent, string equiptype) {
		GameObject obj = Instantiate(itemSkillCard, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = skillCardIndex.ToString() + "_" + id.ToString()+ "_" + equiptype + "_" + lv.ToString();
		obj.transform.localPosition = new Vector3(-230 + 200 * (positionIndex / 2), 100 - 265 * (positionIndex % 2), 0);
		obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		UIEventListener.Get(obj).onClick = OnCardDetailInfo;

		TUICard uicard = new TUICard(1);
		uicard.Self = obj;
		uicard.CardID = id;
		uicard.CardIndex = skillCardIndex;
		uicard.CardLV = lv;
		uicard.EquipedType = equiptype;
		if(GameData.DSkillData.ContainsKey(id))
			uicard.Cost = GameData.DSkillData[id].Space(lv);

		Transform t = obj.transform.FindChild("SkillCard");
		if(t != null) {
			uicard.SkillCard = t.gameObject.GetComponent<UISprite>();
			uicard.SkillCard .spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[id].Quality, 1, 5).ToString();
		}

		t = obj.transform.FindChild("SkillPic");
		if(t != null){
			uicard.SkillPic = t.gameObject.GetComponent<UITexture>();
			uicard.SkillPic.mainTexture = GameData.CardTexture(id);
		}
		
		t = obj.transform.FindChild("SkillLevel");
		if(t != null) {
			uicard.SkillLevel = t.gameObject.GetComponent<UISprite>();
			uicard.SkillLevel.spriteName = "Levelball" + Mathf.Clamp(lv, 1, 5).ToString();
		}

		t = obj.transform.FindChild("SkillName");
		if(t != null) {
			uicard.SkillName = t.gameObject.GetComponent<UILabel>();
			if(GameData.DSkillData.ContainsKey(id))
				uicard.SkillName.text = GameData.DSkillData[id].Name;
		}

		t = obj.transform.FindChild("SkillStar");
		if(t != null) {
			uicard.SkillStar = t.gameObject.GetComponent<UISprite>();
			if(GameData.DSkillData.ContainsKey(id))
				uicard.SkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[id].Star, 1, GameData.DSkillData[id].MaxStar).ToString();
		}

		t = obj.transform.FindChild("UnavailableMask");
		if(t != null)
			uicard.UnavailableMask = t.gameObject;

		t = obj.transform.FindChild("Selected");
		if(t != null)
			uicard.Selected = t.gameObject;

		t = obj.transform.FindChild("InListCard");
		if(t != null)
			uicard.InListCard = t.gameObject;

		uicard.InListCard.SetActive((equiptype.Equals("1")));

		t = obj.transform.FindChild("SellSelect");
		if(t != null)
			uicard.SellSelect = t.gameObject;

		uiCards.Add(obj.transform.name, uicard);

		return obj;
	}

	private GameObject addUIItems (TUICard uicard, int positionIndex, GameObject parent) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = uicard.CardIndex.ToString() + "_" + uicard.CardID.ToString() + "_" + uicard.EquipedType + "_" + uicard.CardLV.ToString();
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
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[uicard.CardID].Space(uicard.CardLV).ToString();

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
		if(setCost(GameData.DSkillData[uicard.CardID].Space(uicard.CardLV))) {
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
					activeStruct[activeStructIndex].EquipedType = uicard.EquipedType;
					activeStruct[activeStructIndex].itemActiveField.SetActive(false);
				}
			}
			if(!skillsRecord.Contains (obj.name))
				skillsRecord.Add(obj.name);
			checkCostIfMask();
			return true;
		} else 
			UIHint.Get.ShowHint("More than SpaceMax", Color.red);
		return false;
	}

	private void removeItems(int id, GameObject go = null) {
//		Transform t = go.transform.FindChild("SkillCost");
//		if(t) 
//		if(setCost(-int.Parse(t.GetComponent<UILabel>().text)))
		
		if(setCost(-uiCards[go.name].Cost)){
			if(skillsRecord.Contains(go.name))
				skillsRecord.Remove(go.name);
			if(id < GameConst.ID_LimitActive) {
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
//				if(skillsRecord.Contains(activeStruct[index].itemEquipActiveCard.name))
//					skillsRecord.Remove(activeStruct[index].itemEquipActiveCard.name);
				if(activeStruct[index].itemEquipActiveCard != null)
					Destroy(activeStruct[index].itemEquipActiveCard);

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
						activeStruct[i].EquipedType = temp.EquipedType;
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
				string cardIdistr = uiCards[skillSortCards[i].name].CardIndex.ToString() + "_" + uiCards[skillSortCards[i].name].CardID.ToString() + "_" + uiCards[skillSortCards[i].name].EquipedType;
				string cardIdjstr = uiCards[skillSortCards[j].name].CardIndex.ToString() + "_" + uiCards[skillSortCards[j].name].CardID.ToString() + "_" + uiCards[skillSortCards[j].name].EquipedType;

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

	public void DoUnEquipCard (){
		removeItems(uiCards[tempObj.name].CardID, tempObj);
		refreshCards(tempObj.name);
	}

	public void DoEquipCard (){
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
	}

	public void OnCardDetailInfo (GameObject go){
		TUICard uicard = uiCards[go.name];
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
	
	public void DoSort() {
		UISort.UIShow(!UISort.Visible);
	}

	public void DoBack() {
		UIShow(false);
	}

	public void DoFinish() {
		List<string> tempNoUpdate = new List<string>();
		List<string> tempRemoveIndex = new List<string>();
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

		if(tempNoUpdate.Count > 0) {
			for (int i=0; i<tempNoUpdate.Count; i++) {
				if(skillsRecord.Contains(tempNoUpdate[i])) {
					skillsRecord.Remove(tempNoUpdate[i]);	
				}
			}
		}

		addIndexs = new int[skillsRecord.Count];
		if(skillsRecord.Count > 0) {
			for(int i=0; i<addIndexs.Length; i++) 
				addIndexs[i] = uiCards[skillsRecord[i]].CardIndex;
		}

		if(addIndexs.Length > 1) {
			for(int i=0; i<addIndexs.Length; i++) {
				for (int j=i+1; j<addIndexs.Length; j++){
					if (addIndexs[i] <= addIndexs[j]){
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
			UIShow(false);
	}

	private void waitEquipSkillCard(bool ok, WWW www) {
		if (ok) {
			TEquipSkillCardResult result = JsonConvert.DeserializeObject <TEquipSkillCardResult>(www.text); 
			
			for (int i=0; i<result.PlayerCards.Length; i++) 
				Debug.Log("result.PlayerCards["+i+"]:"+ result.PlayerCards[i].ID);

			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerCards;
			GameData.Team.Player.Init();
			UIShow(false);
		} else {
			Debug.LogError("text:"+www.text);
		}
	}
}
