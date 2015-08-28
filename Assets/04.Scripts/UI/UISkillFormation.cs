using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public enum ESkillFormationPage {
	Active,
	Passive
}

public class UISkillFormation : UIBase {
	private static UISkillFormation instance = null;
	private const string UIName = "UISkillFormation";
	private const int IDLimit = 10000;

	//CenterCard
	private List<GameObject> skillCards = new List<GameObject>();
	private List<GameObject> skillSortCards = new List<GameObject>();
	private Dictionary<int, TSkill> skillActiveCards = new Dictionary<int, TSkill>();//key:ID Value: TSkill
	private Dictionary<int, TSkill> skillPassiveCards = new Dictionary<int, TSkill>();//key:ID Value: TSkill

	//RightItem
	private GameObject itemActiveCard;
	private List<GameObject> itemPassiveCards = new List<GameObject>();

	//Instantiate Object
	private GameObject itemSkillCard;
	private GameObject itemCardEquipped;

	//Right
	private GameObject gridActiveCardBase;//Put itemCardEquipped
	private GameObject gridPassiveCardBase;//Put itemCardEquipped
	
	private UILabel labelCostValue;

	//Center
	private GameObject uiCardGroup;
	private GameObject objCardList;
	private UIScrollView scrollViewCardList;

	private ECondition cardCondition = ECondition.Date;
	private EFilter cardFilter = EFilter.All;

	//LongPress 
	private bool isPressDown = false;
	private float longPressMaxTime = 0.5f;
	private float longPressTime = 0;

	//Info
	private string infoName;
	private string infoLv;
	private string infoInfo;

	private float yPos = 0;
	private ESkillFormationPage page = ESkillFormationPage.Active;

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

	void FixedUpdate(){
		if(isPressDown)
			if (Time.realtimeSinceStartup - longPressTime > longPressMaxTime) 
				if(Mathf.Abs(yPos - scrollViewCardList.panel.clipOffset.y) < 1)
					doLongPress(); 
		
	}

	protected override void InitCom() {
		gridActiveCardBase = GameObject.Find (UIName + "/MainView/Right/ActiveCardBase");
		gridPassiveCardBase = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveList/UIGrid");
		labelCostValue = GameObject.Find (UIName + "/MainView/Top/LabelCost/CostValue").GetComponent<UILabel>();

		uiCardGroup = GameObject.Find (UIName + "/CardsView/Center/CardsGroup");
		objCardList = GameObject.Find (UIName + "/CardsView/Center/CardsGroup/CardList/UIGrid");
		scrollViewCardList = GameObject.Find (UIName + "/CardsView/Center/CardsGroup/CardList").GetComponent<UIScrollView>();

		itemSkillCard = Resources.Load("Prefab/UI/Items/ItemSkillCard") as GameObject;
		itemCardEquipped = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;

		SetBtnFun (UIName + "/CardsView/Center/SortBtn", DoSort);
		SetBtnFun (UIName + "/MainView/Right/ActiveField", DoOpenActive);
		SetBtnFun (UIName + "/MainView/Right/PassiveField", DoOpenPassive);
		initCards ();
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	private void initCards () {
		int index = -1;
		for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
			if(GameData.Team.SkillCards[i].ID >= 100 && GameData.Team.SkillCards[i].ID < IDLimit) {
				GameObject obj = addCards(0, 
				                          GameData.Team.SkillCards[i].ID, 
				                          1, //Lv Temp
				                          objCardList, 
				                          GameData.DSkillData[GameData.Team.SkillCards[i].ID].Space(1));
				skillCards.Add(obj);
				skillSortCards.Add(obj);
				obj.SetActive(false);
				skillPassiveCards.Add(GameData.Team.SkillCards[i].ID, GameData.Team.SkillCards[i]);
			} else
			if(GameData.Team.SkillCards[i].ID >= IDLimit){
				index ++;
				GameObject obj = addCards(index, 
				                          GameData.Team.SkillCards[i].ID, 
				                          1, //Lv Temp
				                          objCardList, 
				                          GameData.DSkillData[GameData.Team.SkillCards[i].ID].Space(1));
				skillCards.Add(obj);
				skillSortCards.Add(obj);
				skillActiveCards.Add(GameData.Team.SkillCards[i].ID, GameData.Team.SkillCards[i]);
			}
			 
		}
	}

	private GameObject addCards (int index, int id, int lv, GameObject parent, int cost) {
		GameObject obj = Instantiate(itemSkillCard, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = id.ToString();
		obj.transform.localPosition = new Vector3(-120 + 200 * (index % 4), 120 - 265 * (index / 4), 0);
		obj.transform.localScale = Vector3.one;
		UIEventListener.Get(obj).onPress = ShowCardDetailInfo;

		Transform t = obj.transform.FindChild("SkillCard");
		if(t != null)
			t.gameObject.GetComponent<UISprite>().spriteName = "SkillCard" + lv.ToString();
		
		t = obj.transform.FindChild("SkillPic");
		if(t != null){
			if(GameData.DCardTextures.ContainsKey(id))
				t.gameObject.GetComponent<UITexture>().mainTexture = GameData.DCardTextures[id];
		}
		
		t = obj.transform.FindChild("SkillLevel");
		if(t != null)
			t.gameObject.GetComponent<UILabel>().text = lv.ToString();

		t = obj.transform.FindChild("SkillName");
		if(t != null) {
			if(GameData.DSkillData.ContainsKey(id))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Name;
		}

		t = obj.transform.FindChild("SkillCost");
		if(t != null){
			if(GameData.DSkillData.ContainsKey(id))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Space(lv).ToString();
		}
		return obj;
	}

	private GameObject addItems (int index, int id, int lv, GameObject parent, int cost) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = id.ToString();
		if(id >= IDLimit)
			obj.transform.localPosition = Vector3.zero;
		else 
			obj.transform.localPosition = new Vector3(0, 200 - 80 * index, 0);
		obj.transform.localScale = Vector3.one;

		UIEventListener.Get(obj).onPress = ShowItemDetailInfo;

		if(obj.transform.FindChild("BtnRemove") != null) {
			UIEventListener.Get(obj.transform.FindChild("BtnRemove").gameObject).onPress = SetRomveItem;
		}
		
		Transform t = obj.transform.FindChild("SkillLevel");
		if(t != null)
			t.gameObject.GetComponent<UILabel>().text = lv.ToString();
		
		t = obj.transform.FindChild("SkillName");
		if(t != null) {
			if(GameData.DSkillData.ContainsKey(id))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Name;
		}
		
		t = obj.transform.FindChild("SkillCost");
		if(t != null){
			if(GameData.DSkillData.ContainsKey(id))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Space(lv).ToString();
		}
		return obj;
	}

	private void doLongPress() {
		isPressDown = false;
		UISkillInfo.UIShow(true, infoName, infoLv, infoInfo);
	}

	public void ShowCardDetailInfo (GameObject go, bool state){
		isPressDown = state;

		if (state) {
			yPos = scrollViewCardList.panel.clipOffset.y;
			longPressTime = Time.realtimeSinceStartup; 
		} else {
			if(!UISkillInfo.Visible){
				//Click Card
				if(Mathf.Abs(yPos - scrollViewCardList.panel.clipOffset.y) < 1) {
					Transform t = go.transform.FindChild("Selected");
					if(t != null) {
						if(page == ESkillFormationPage.Active) {
							if(itemActiveCard == null){
								t.gameObject.SetActive(true);
								//It need compare Cost============= skillActiveCards[int.Parse(go.name)].Lv
								itemActiveCard = addItems(0,
								                          int.Parse(go.name),
								                          1,
								                          gridActiveCardBase,
								                          GameData.DSkillData[int.Parse(go.name)].Space(skillActiveCards[int.Parse(go.name)].Lv));
							} else {
								if(itemActiveCard.name.Equals(go.name)) {
									Destroy(itemActiveCard);
									t.gameObject.SetActive(false);
								}else {
									//It need compare Cost============= skillActiveCards[int.Parse(go.name)].Lv
									t.gameObject.SetActive(true);
									refreshCards(int.Parse(itemActiveCard.name));
									Destroy(itemActiveCard);
									itemActiveCard = addItems(0,
									                          int.Parse(go.name),
									                          1,
									                          gridActiveCardBase,
									                          GameData.DSkillData[int.Parse(go.name)].Space(skillActiveCards[int.Parse(go.name)].Lv));
								}
							}
						} else {
							//It need compare Cost=============
							t.gameObject.SetActive(!t.gameObject.activeInHierarchy);
							if(t.gameObject.activeInHierarchy) {
								//NoSelected to Selected
								addPassiveItems(go);
							} else {
								//Selected to NoSelected
								removePassiveItems(go);
							}
						}
					}
				}
			}
		}

		infoName = GameData.DSkillData[int.Parse(go.name)].Name;
		infoLv = go.transform.FindChild("SkillLevel").GetComponent<UILabel>().text;
		infoInfo = GameData.DSkillData[int.Parse(go.name)].Explain;
	}

	public void ShowItemDetailInfo (GameObject go, bool state){
		isPressDown = state;
		
		if (state) {
			yPos = scrollViewCardList.panel.clipOffset.y;
			longPressTime = Time.realtimeSinceStartup; 
		} else {
			if(!UISkillInfo.Visible) {
				//Click Item
				if(Mathf.Abs(yPos - scrollViewCardList.panel.clipOffset.y) < 1) {
					UISkillInfo.UIShow(true, infoName, infoLv, infoInfo);
				}
			}
		}
		
		infoName = GameData.DSkillData[int.Parse(go.name)].Name;
		infoLv = go.transform.FindChild("SkillLevel").GetComponent<UILabel>().text;
		infoInfo = GameData.DSkillData[int.Parse(go.name)].Explain;
	}

	//From Item RemoveButton
	public void SetRomveItem(GameObject go, bool state){
		int id = int.Parse(go.transform.parent.name);
		if(id >= IDLimit) {
			//Active
			Destroy(itemActiveCard);
		} else {
			//Passive
			removePassiveItems(go.transform.parent.gameObject);
			refreshPassiveItems();
		}
		refreshCards(id);

	}

	private void addPassiveItems(GameObject go) {
		int index = itemPassiveCards.Count;
		itemPassiveCards.Add(addItems(index,
		                              int.Parse(go.name),
		                              1,
		                              gridPassiveCardBase,
		                              GameData.DSkillData[int.Parse(go.name)].Space(skillPassiveCards[int.Parse(go.name)].Lv)));
	}

	private void removePassiveItems(GameObject go) {
		for(int i=0 ;i<itemPassiveCards.Count; i++) {
			if(itemPassiveCards[i].name.Equals(go.name)){
				Destroy(itemPassiveCards[i]);
				itemPassiveCards.RemoveAt(i);
				break;
			}
		}

		refreshPassiveItems();
	}

	private void refreshCards(int id) {
		for(int i=0 ;i<skillCards.Count; i++) {
			if(int.Parse(skillCards[i].name) == id) {
				Transform t = skillCards[i].transform.FindChild("Selected");
				if(t != null)
					t.gameObject.SetActive(false);
			}
		}

		for(int i=0 ;i<skillSortCards.Count; i++) {
			if(int.Parse(skillSortCards[i].name) == id) {
				Transform t = skillSortCards[i].transform.FindChild("Selected");
				if(t != null)
					t.gameObject.SetActive(false);
			}
		}

		objCardList.SetActive(false);
		objCardList.SetActive(true);
	}

	private void refreshPassiveItems() {
		for(int i=0 ;i<itemPassiveCards.Count; i++) {
			itemPassiveCards[i].transform.localPosition = new Vector3(0, 200 - 80 * i, 0);
		}
		
		gridPassiveCardBase.SetActive(false);
		gridPassiveCardBase.SetActive(true);
	}

	public void DoOpenActive (){
		//Open Actvie Cards
		page = ESkillFormationPage.Active;
		int index = 0;
		for (int i=0; i<skillCards.Count; i++) {
			if(cardCondition == ECondition.Date) {
				if(cardsVisible(skillActiveCards, i, index, skillCards[i]))
					index++;
			} else {
				if(cardsVisible(skillActiveCards, i, index, skillSortCards[i]))
					index++;
			}
		}
		scrollViewCardList.ResetPosition();
		objCardList.SetActive(false);
		objCardList.SetActive(true);
	}

	public void DoOpenPassive (){
		//Open Passive Cards
		page = ESkillFormationPage.Passive;
		int index = 0;
		for (int i=0; i<skillCards.Count; i++) {
			if(cardCondition == ECondition.Date) {
				if(cardsVisible(skillPassiveCards, i, index, skillCards[i]))
					index ++;
			} else {
				if(cardsVisible(skillPassiveCards, i, index, skillSortCards[i]))
					index ++;
			}
		}
		scrollViewCardList.ResetPosition();
		objCardList.SetActive(false);
		objCardList.SetActive(true);
	}

	private bool cardsVisible (Dictionary<int, TSkill> dcard, int i, int index, GameObject card){
		if(dcard.ContainsKey(int.Parse(card.name))) {
			if(cardFilter == EFilter.All) {
				card.transform.localPosition = new Vector3(-120 + 200 * (index % 4), 120 - 265 * (index / 4), 0);
				card.SetActive(true);
				return true;
			} else
			if(cardFilter == EFilter.Available){
				Transform t = card.transform.FindChild("Selected");
				if(t != null){
					if(!t.gameObject.activeSelf){
						card.transform.localPosition = new Vector3(-120 + 200 * (index % 4), 120 - 265 * (index / 4), 0);
						card.SetActive(true);
						return true;
					}
				}
			} else {
				Transform t = card.transform.FindChild("Selected");
				if(t != null){
					if(t.gameObject.activeSelf){
						card.transform.localPosition = new Vector3(-120 + 200 * (index % 4), 120 - 265 * (index / 4), 0);
						card.SetActive(true);
						return true;
					}
				}
			}
		}
		card.SetActive(false);
		return false;
	}

	public void DoSort() {
		UISort.UIShow(!UISort.Visible);
	}

	public void SetSort (ECondition condition, EFilter filter) {
		cardCondition = condition;
		cardFilter = filter;

		sortSkill(condition);

		if(page == ESkillFormationPage.Active)
			DoOpenActive();
		else
			DoOpenPassive();

	}

	private void sortSkill(ECondition condition) {
		int value1 = 0;
		int value2 = 0;
		for(int i=0; i<skillSortCards.Count; i++) {
			for (int j=i+1; j<skillSortCards.Count; j++){
				if(condition == ECondition.Rare) {
					value1 = GameData.DSkillData[int.Parse(skillSortCards[i].name)].Star;
					value2 = GameData.DSkillData[int.Parse(skillSortCards[j].name)].Star;
				} else 
				if(condition == ECondition.Kind){
					value1 = GameData.DSkillData[int.Parse(skillSortCards[i].name)].Kind;
					value2 = GameData.DSkillData[int.Parse(skillSortCards[j].name)].Kind;
				} else 
				if(condition == ECondition.Attribute){
					value1 = GameData.DSkillData[int.Parse(skillSortCards[i].name)].AttrKind;
					value2 = GameData.DSkillData[int.Parse(skillSortCards[j].name)].AttrKind;
				}  else 
				if(condition == ECondition.Level){
					if(GameData.DSkillData.ContainsKey(int.Parse(skillSortCards[i].name)) && GameData.DSkillData.ContainsKey(int.Parse(skillSortCards[j].name))){
						if(skillPassiveCards.ContainsKey(int.Parse(skillSortCards[i].name))) 
							value1 = skillPassiveCards[int.Parse(skillSortCards[i].name)].Lv;
						else 
						if(skillPassiveCards.ContainsKey(int.Parse(skillSortCards[j].name)))
							value2 = skillPassiveCards[int.Parse(skillSortCards[j].name)].Lv;
						else 
						if(skillActiveCards.ContainsKey(int.Parse(skillSortCards[i].name)))
							value1 = skillActiveCards[int.Parse(skillSortCards[i].name)].Lv;
						else
						if(skillActiveCards.ContainsKey(int.Parse(skillSortCards[j].name)))
							value2 = skillActiveCards[int.Parse(skillSortCards[j].name)].Lv;
					}
				} else 
				if(condition == ECondition.Cost){
					if(GameData.DSkillData.ContainsKey(int.Parse(skillSortCards[i].name)) && GameData.DSkillData.ContainsKey(int.Parse(skillSortCards[j].name))){
						if(skillPassiveCards.ContainsKey(int.Parse(skillSortCards[i].name))) 
							value1 = GameData.DSkillData[int.Parse(skillSortCards[i].name)].Space(skillPassiveCards[int.Parse(skillSortCards[i].name)].Lv);
						else 
						if(skillPassiveCards.ContainsKey(int.Parse(skillSortCards[j].name)))
							value2 = GameData.DSkillData[int.Parse(skillSortCards[j].name)].Space(skillPassiveCards[int.Parse(skillSortCards[j].name)].Lv);
						else 
						if(skillActiveCards.ContainsKey(int.Parse(skillSortCards[i].name)))
							value1 = GameData.DSkillData[int.Parse(skillSortCards[i].name)].Space(skillActiveCards[int.Parse(skillSortCards[i].name)].Lv);
						else
						if(skillActiveCards.ContainsKey(int.Parse(skillSortCards[j].name)))
							value2 = GameData.DSkillData[int.Parse(skillSortCards[j].name)].Space(skillActiveCards[int.Parse(skillSortCards[j].name)].Lv);
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


}
