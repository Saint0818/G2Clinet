using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameStruct;

public enum ESkillFormationPage {
	Active,
	Passive
}

public struct TEquipSkillCardResult {
	public TSkill[] SkillCards;
	public TSkill[] PlayerCards;
}

public class UISkillFormation : UIBase {
	private static UISkillFormation instance = null;
	private const string UIName = "UISkillFormation";
	private const int IDActiveLimit = 10000;

	private int[] removeIndexs = new int[0]; //From already setted skillCard's index
	private int[] addIndexs = new int[0];//From skillCards's index in the center area

	//CenterCard
	private List<GameObject> skillCards = new List<GameObject>();//By Date of Sort
	private List<GameObject> skillSortCards = new List<GameObject>();//By Sort
	private List<string> skillsOriginal = new List<string>();//record alread setted   rule: index_id
	private List<string> skillsRecord = new List<string>();

	//key:GameData.SkillCards.index_cardID_skillsOriginal(Equiped) Value: TSkill   For Get Level
	//skillsOriginal(Equiped) 0:Not Equiped 1:Equiped (First Time)
	private Dictionary<string, TSkill> skillActiveCards = new Dictionary<string, TSkill>(); 
	private Dictionary<string, TSkill> skillPassiveCards = new Dictionary<string, TSkill>();

	//RightItem  name:Skill
	private GameObject itemActiveCard;
	private List<GameObject> itemPassiveCards = new List<GameObject>();

	//Instantiate Object
	private GameObject itemSkillCard;
	private GameObject itemCardEquipped;

	//Right(Put itemCardEquipped Object)
	private GameObject gridActiveCardBase; 
	private GameObject gridPassiveCardBase;
	
	private UILabel labelCostValue;

	//Center
	private GameObject uiCardGroup;
	private GameObject gridCardList;
	private UIScrollView scrollViewCardList;

	private ECondition cardCondition = ECondition.Date;
	private EFilter cardFilter = EFilter.All;

	//LongPress 
	private bool isPressDown = false;
	private float longPressMaxTime = 0.5f;
	private float longPressTime = 0;

	//Info
	private int infoID;
	private string infoName;
	private string infoLv;
	private string infoInfo;

	private Vector3 point;
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
		if(Input.GetMouseButtonDown (0)){
			point = Input.mousePosition;
		} else if(Input.GetMouseButton (0)) {
			if (Vector3.Distance(point ,Input.mousePosition) > 1)
				isPressDown = false;
		}

		if(isPressDown)
			if (Time.realtimeSinceStartup - longPressTime > longPressMaxTime) 
				doLongPress(); 

		
	}

	protected override void InitCom() {
		gridActiveCardBase = GameObject.Find (UIName + "/MainView/Right/ActiveCardBase");
		gridPassiveCardBase = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveList/UIGrid");
		labelCostValue = GameObject.Find (UIName + "/MainView/Top/LabelCost/CostValue").GetComponent<UILabel>();

		uiCardGroup = GameObject.Find (UIName + "/CardsView/Center/CardsGroup");
		gridCardList = GameObject.Find (UIName + "/CardsView/Center/CardsGroup/CardList/UIGrid");
		scrollViewCardList = GameObject.Find (UIName + "/CardsView/Center/CardsGroup/CardList").GetComponent<UIScrollView>();

		itemSkillCard = Resources.Load("Prefab/UI/Items/ItemSkillCard") as GameObject;
		itemCardEquipped = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;

		SetBtnFun (UIName + "/CardsView/Center/SortBtn", DoSort);
		SetBtnFun (UIName + "/MainView/Right/ActiveField", DoOpenActive);
		SetBtnFun (UIName + "/MainView/Right/PassiveField", DoOpenPassive);
		SetBtnFun (UIName + "/MainView/Right/CheckBtn", DoFinish);
		initCards ();
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	private void initCards () {
		int index = -1;
		if(GameData.Team.Player.SkillCards.Length > 0) {
			for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
				if(GameData.Team.Player.SkillCards[i].ID > 100) {
					GameObject obj = null;
					Transform t;
					if(GameData.Team.Player.SkillCards[i].ID < IDActiveLimit) {
						addPassiveItems(i, GameData.Team.Player.SkillCards[i].ID, GameData.Team.Player.SkillCards[i].Lv, "1");
						obj = addCards(i,
			                           0, // First time it Show Active Card, so value is 0; 
						               GameData.Team.Player.SkillCards[i].ID, 
						               GameData.Team.Player.SkillCards[i].Lv,
			                           gridCardList, 
						               GameData.DSkillData[GameData.Team.Player.SkillCards[i].ID].Space(GameData.Team.Player.SkillCards[i].Lv),
						               "1");
						obj.SetActive(false);
						t = obj.transform.FindChild("Selected");
						if(t != null)
							t.gameObject.SetActive(true);
						skillPassiveCards.Add(i.ToString() + "_" + GameData.Team.Player.SkillCards[i].ID.ToString() + "_1", GameData.Team.Player.SkillCards[i]);
					} else {
						addActiveItems(i, GameData.Team.Player.SkillCards[i].ID, GameData.Team.Player.SkillCards[i].Lv, "1");
						index ++;
						obj = addCards(i,
			                           index, 
						               GameData.Team.Player.SkillCards[i].ID, 
						               GameData.Team.Player.SkillCards[i].Lv,
			                           gridCardList, 
						               GameData.DSkillData[GameData.Team.Player.SkillCards[i].ID].Space(GameData.Team.Player.SkillCards[i].Lv),
						               "1");
						t = obj.transform.FindChild("Selected");
						if(t != null)
							t.gameObject.SetActive(true);
						skillActiveCards.Add(i.ToString() + "_" + GameData.Team.Player.SkillCards[i].ID.ToString() + "_1", GameData.Team.Player.SkillCards[i]);
					}
					skillCards.Add(obj);
					skillSortCards.Add(obj);
					skillsOriginal.Add(i + "_" + GameData.Team.Player.SkillCards[i].ID + "_1");
				}
			}
		}

		if(GameData.Team.SkillCards.Length > 0) {
			for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
				GameObject obj = null;
				if(GameData.Team.SkillCards[i].ID > 100) {
					if(GameData.Team.SkillCards[i].ID < IDActiveLimit) {
						obj = addCards(i,
						               0, // First time it Show Active Card, so value is 0; 
						               GameData.Team.SkillCards[i].ID, 
						               GameData.Team.SkillCards[i].Lv,
						               gridCardList, 
						               GameData.DSkillData[GameData.Team.SkillCards[i].ID].Space(GameData.Team.SkillCards[i].Lv),
						               "0");
						obj.SetActive(false);
						skillPassiveCards.Add(i.ToString() + "_" + GameData.Team.SkillCards[i].ID.ToString() + "_0", GameData.Team.SkillCards[i]);
					} else
					if(GameData.Team.SkillCards[i].ID >= IDActiveLimit){
						index ++;
						obj = addCards(i,
						               index, 
						               GameData.Team.SkillCards[i].ID, 
						               GameData.Team.SkillCards[i].Lv,
						               gridCardList, 
						               GameData.DSkillData[GameData.Team.SkillCards[i].ID].Space(GameData.Team.SkillCards[i].Lv),
						               "0");
						skillActiveCards.Add(i.ToString() + "_" + GameData.Team.SkillCards[i].ID.ToString() + "_0", GameData.Team.SkillCards[i]);
					}
					skillCards.Add(obj);
					skillSortCards.Add(obj);
				}
			}
		}
	}

	private int getIndex (string name, bool isGetCardIndex) {
		string[] splitName = name.Split("_"[0]);
		if(isGetCardIndex) {
			return int.Parse(splitName[0]);
		} else {
			return int.Parse(splitName[1]);
		}
	}

	private string getEquipedType (string name){
		if(skillsOriginal.Contains(name))
			return "1";
		return "0";
	}

	private bool checkCardBeEquiped (int id) {
		for (int i=0; i<skillsRecord.Count; i++) {
			int recordID = getIndex(skillsRecord[i], false);
			if(recordID == id)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Adds the cards.
	/// </summary>
	/// <returns>The cards.</returns>
	/// <param name="skillCardIndex">GameData.Team.SkillCards's index.</param>
	/// <param name="positionIndex">Put ScrollView Position index.</param>
	/// <param name="id">Identifier.</param>
	/// <param name="lv">Lv.</param>
	/// <param name="parent">Parent.</param>
	/// <param name="cost">Cost.</param>
	private GameObject addCards (int skillCardIndex, int positionIndex, int id, int lv, GameObject parent, int cost, string equiptype) {
		GameObject obj = Instantiate(itemSkillCard, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = skillCardIndex.ToString() + "_" + id.ToString() + "_" + equiptype;
		obj.transform.localPosition = new Vector3(-120 + 200 * (positionIndex % 4), 120 - 265 * (positionIndex / 4), 0);
		obj.transform.localScale = Vector3.one;
		UIEventListener.Get(obj).onPress = OnCardDetailInfo;

		Transform t = obj.transform.FindChild("SkillCard");
		if(t != null)
			t.gameObject.GetComponent<UISprite>().spriteName = "SkillCard" + Mathf.Clamp(lv, 1, 3).ToString();
		
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

	private GameObject addItems (int skillCardIndex, int positionIndex, int id, int lv, GameObject parent, int cost, string equiptype) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = skillCardIndex.ToString() + "_" + id.ToString() + "_" + equiptype;
		if(id >= IDActiveLimit)
			obj.transform.localPosition = Vector3.zero;
		else 
			obj.transform.localPosition = new Vector3(0, 200 - 80 * positionIndex, 0);
		obj.transform.localScale = Vector3.one;

		UIEventListener.Get(obj).onPress = OnItemDetailInfo;

		if(obj.transform.FindChild("BtnRemove") != null) {
			UIEventListener.Get(obj.transform.FindChild("BtnRemove").gameObject).onPress = OnRomveItem;
		}
		
		Transform t = obj.transform.FindChild("SkillLevel");
		if(t != null)
			t.gameObject.GetComponent<UILabel>().text = Mathf.Clamp(lv, 1, 3).ToString();
		
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
		UISkillInfo.UIShow(true, infoID, infoName, infoLv, infoInfo);
	}
	
	private void addActiveItems(int cardIndex, int cardId, int lv, string equiptype) {
		itemActiveCard = addItems(cardIndex,
		                          0,
		                          cardId,
		                          lv,
		                          gridActiveCardBase,
		                          GameData.DSkillData[cardId].Space(lv),
		                          equiptype);
		if(!skillsRecord.Contains (cardIndex.ToString() + "_" + cardId.ToString() + "_" + equiptype))
			skillsRecord.Add(cardIndex.ToString() + "_" + cardId.ToString() + "_" + equiptype);
	}
	
	private void removeActiveItems() {
		if(skillsRecord.Contains(itemActiveCard.name))
			skillsRecord.Remove(itemActiveCard.name);
		Destroy(itemActiveCard);
	}
	
	private void addPassiveItems(int cardIndex, int cardId, int lv, string equiptype) {
		int index = itemPassiveCards.Count;
		itemPassiveCards.Add(addItems(cardIndex,
									  index,
		                              cardId,
		                              lv,
		                              gridPassiveCardBase,
		                              GameData.DSkillData[cardId].Space(lv),
		                              equiptype));
		if(!skillsRecord.Contains (cardIndex.ToString() + "_" + cardId.ToString() + "_" + equiptype))
			skillsRecord.Add(cardIndex.ToString() + "_" + cardId.ToString() + "_" + equiptype);
	}
	
	private void removePassiveItems(GameObject go) {
		if(skillsRecord.Contains(go.name))
			skillsRecord.Remove(go.name);
		for(int i=0 ;i<itemPassiveCards.Count; i++) {
			if(itemPassiveCards[i].name.Equals(go.name)){
				Destroy(itemPassiveCards[i]);
				itemPassiveCards.RemoveAt(i);
				break;
			}
		}
		refreshPassiveItems();
	}
	
	private void refreshCards(string id) {
		for(int i=0 ;i<skillCards.Count; i++) {
			if(skillCards[i].name.Equals(id)) {
				Transform t = skillCards[i].transform.FindChild("Selected");
				if(t != null)
					t.gameObject.SetActive(false);
			}
		}
		
		for(int i=0 ;i<skillSortCards.Count; i++) {
			if(skillSortCards[i].name.Equals(id)) {
				Transform t = skillSortCards[i].transform.FindChild("Selected");
				if(t != null)
					t.gameObject.SetActive(false);
			}
		}
		
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
	}
	
	private void refreshPassiveItems() {
		for(int i=0 ;i<itemPassiveCards.Count; i++) {
			itemPassiveCards[i].transform.localPosition = new Vector3(0, 200 - 80 * i, 0);
		}
		
		gridPassiveCardBase.SetActive(false);
		gridPassiveCardBase.SetActive(true);
	}

	private bool cardsVisible (Dictionary<string, TSkill> dcard, int i, int index, GameObject card){
		if(dcard.ContainsKey(card.name)) {
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

	private void sortSkill(ECondition condition) {
		int value1 = 0;
		int value2 = 0;
		for(int i=0; i<skillSortCards.Count; i++) {
			for (int j=i+1; j<skillSortCards.Count; j++){
				int cardIdi = getIndex(skillSortCards[i].name, false);
				int cardIdj = getIndex(skillSortCards[j].name, false);
				string cardIdistr = getIndex(skillSortCards[i].name, true).ToString() + "_" + getIndex(skillSortCards[i].name, false).ToString() + "_" + getEquipedType(skillSortCards[i].name);
				string cardIdjstr = getIndex(skillSortCards[j].name, true).ToString() + "_" + getIndex(skillSortCards[j].name, false).ToString() + "_" + getEquipedType(skillSortCards[j].name);

				if(condition == ECondition.Rare) {
					value1 = GameData.DSkillData[cardIdi].Star;
					value2 = GameData.DSkillData[cardIdj].Star;
				} else 
				if(condition == ECondition.Kind){
					value1 = GameData.DSkillData[cardIdi].Kind;
					value2 = GameData.DSkillData[cardIdj].Kind;
				} else 
				if(condition == ECondition.Attribute){
					value1 = GameData.DSkillData[cardIdi].AttrKind;
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

	public void OnCardDetailInfo (GameObject go, bool state){
		int cardIndex = getIndex(go.name, true);
		int cardId = getIndex(go.name, false);
		string equipType = getEquipedType(go.name);

		if (state) {
			longPressTime = Time.realtimeSinceStartup; 
		} else {
			if(!UISkillInfo.Visible){
				//Click Card
				if(isPressDown) {
					Transform t = go.transform.FindChild("Selected");
					if(t != null) {
						if(page == ESkillFormationPage.Active) {
							if(itemActiveCard == null){
								t.gameObject.SetActive(true);
								addActiveItems(cardIndex, cardId, skillActiveCards[cardIndex.ToString() + "_" + cardId.ToString() + "_" + equipType].Lv, equipType);

							} else {
								if(itemActiveCard.name.Equals(go.name)) {
									removeActiveItems();
									t.gameObject.SetActive(false);
								}else {
									t.gameObject.SetActive(true);
									refreshCards(itemActiveCard.name);
									removeActiveItems();
									addActiveItems(cardIndex, cardId, skillActiveCards[cardIndex.ToString() + "_" + cardId.ToString()+ "_" + equipType].Lv, equipType);
								}
							}
						} else {
							if(t.gameObject.activeInHierarchy) {
								//Selected to NoSelected
								t.gameObject.SetActive(!t.gameObject.activeInHierarchy);
								removePassiveItems(go);
							} else {
								//NoSelected to Selected
								if(!checkCardBeEquiped(cardId)){
									t.gameObject.SetActive(!t.gameObject.activeInHierarchy);
									addPassiveItems(cardIndex, cardId, skillPassiveCards[cardIndex.ToString() + "_" + cardId.ToString()+ "_" + equipType].Lv, equipType );
								} else {
									//be equiped (Same CardID)
								}
							}
						}
					}
				}
			}
		}
		
		isPressDown = state;
		infoID = cardId;
		infoName = GameData.DSkillData[cardId].Name;
		infoLv = go.transform.FindChild("SkillLevel").GetComponent<UILabel>().text;
		infoInfo = GameData.DSkillData[cardId].Explain;
	}

	public void OnItemDetailInfo (GameObject go, bool state){
		int cardIndex = getIndex(go.name, true);
		int cardId = getIndex(go.name, false);
		
		if (state) {
			longPressTime = Time.realtimeSinceStartup; 
		} else {
			if(!UISkillInfo.Visible) {
				//Click Item
				if(isPressDown) {
					UISkillInfo.UIShow(true, infoID, infoName, infoLv, infoInfo);
				}
			}
		}

		isPressDown = state;
		infoID = cardId;
		infoName = GameData.DSkillData[cardId].Name;
		infoLv = go.transform.FindChild("SkillLevel").GetComponent<UILabel>().text;
		infoInfo = GameData.DSkillData[cardId].Explain;
	}

	//From Item RemoveButton
	public void OnRomveItem(GameObject go, bool state){
		int id = getIndex(go.transform.parent.name, false);
		if(id >= IDActiveLimit) {
			//Active
			removeActiveItems();
		} else {
			//Passive
			removePassiveItems(go.transform.parent.gameObject);
			refreshPassiveItems();
		}
		refreshCards(go.transform.parent.name);

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
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
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
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
	}



	public void DoSort() {
		UISort.UIShow(!UISort.Visible);
	}

	public void DoFinish() {
		List<string> tempNoUpdate = new List<string>();
		List<string> tempRemoveIndex = new List<string>();
		if(skillsRecord.Count > 0 && !skillsRecord.Equals (skillsOriginal)) {
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
					removeIndexs[i] = getIndex(tempRemoveIndex[i], true);
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
					addIndexs[i] = getIndex(skillsRecord[i], true);
			}

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
			for (int i=0; i<result.SkillCards.Length; i++) 
				Debug.Log("result.SkillCards["+i+"]:"+ result.SkillCards[i]);
			
			for (int i=0; i<result.PlayerCards.Length; i++) 
				Debug.Log("result.PlayerCards["+i+"]:"+ result.PlayerCards[i]);

			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerCards;
			UIShow(false);
		} else {
			Debug.LogError("text:"+www.text);
		}
	}
}
