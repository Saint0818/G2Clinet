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
}

public class UISkillFormation : UIBase {
	private static UISkillFormation instance = null;
	private const string UIName = "UISkillFormation";
	private const int IDActiveLimit = 10000;

	//Send Value
	private int[] removeIndexs = new int[0]; //From already setted skillCard's index
	private int[] addIndexs = new int[0];//From skillCards's index in the center area

	//CenterCard
	private List<GameObject> skillSortCards = new List<GameObject>();//By Sort
	private List<string> skillsOriginal = new List<string>();//record alread setted   rule: index_id_skillsOriginal(Equiped)
	private List<string> skillsRecord = new List<string>();

	//key:GameData.SkillCards.index_cardID_skillsOriginal(Equiped) Value: TSkill   For Get Level
	//only record skillsOriginal(Equiped) 0:Not Equiped 1:Equiped (First Time)
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
	private UIScrollView scrollViewItemList;
	
	private UILabel labelCostValue;

	//Center
	private GameObject gridCardList;
	private UIScrollView scrollViewCardList;

	//LongPress 
	private bool isPressDown = false;
	private float longPressMaxTime = 0.5f;
	private float longPressTime = 0;

	//Info
	private TSkillInfo skillInfo = new TSkillInfo();

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
		scrollViewItemList = GameObject.Find (UIName + "/MainView/Right/PassiveCardBase/PassiveList").GetComponent<UIScrollView>();
		scrollViewItemList.onStoppedMoving =ItemDragFinish;

		gridCardList = GameObject.Find (UIName + "/CardsView/Center/CardsGroup/CardList/UIGrid");
		scrollViewCardList = GameObject.Find (UIName + "/CardsView/Center/CardsGroup/CardList").GetComponent<UIScrollView>();


		itemSkillCard = Resources.Load("Prefab/UI/Items/ItemSkillCard") as GameObject;
		itemCardEquipped = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;

		SetBtnFun (UIName + "/CardsView/Center/SortBtn", DoSort);
		SetBtnFun (UIName + "/MainView/Right/ActiveField", DoOpenActive);
		SetBtnFun (UIName + "/MainView/Right/PassiveField", DoOpenPassive);
		SetBtnFun (UIName + "/MainView/Right/CheckBtn", DoFinish);
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
		if(GameData.Team.Player.SkillCards.Length > 0) {
			for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
				if(GameData.Team.Player.SkillCards[i].ID > 100) {
					GameObject obj = null;
					index ++;
					obj = addUICards(i,
					                 index, 
					                 GameData.Team.Player.SkillCards[i].ID, 
					                 GameData.Team.Player.SkillCards[i].Lv,
					                 gridCardList, 
					                 "1");
					addItems(i, GameData.Team.Player.SkillCards[i].ID, GameData.Team.Player.SkillCards[i].Lv, "1");
					if(GameData.Team.Player.SkillCards[i].ID < IDActiveLimit) {
						skillPassiveCards.Add(obj.name, GameData.Team.Player.SkillCards[i]);
					} else {
						skillActiveCards.Add(obj.name, GameData.Team.Player.SkillCards[i]);
					}

					Transform t = obj.transform.FindChild("Selected");
					if(t != null)
						t.gameObject.SetActive(true);

					skillSortCards.Add(obj);
					skillsRecord.Add(obj.name);
					skillsOriginal.Add(obj.name);
				}
			}
		}

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
					if(GameData.Team.SkillCards[i].ID < IDActiveLimit) 
						skillPassiveCards.Add(obj.name, GameData.Team.SkillCards[i]);
					else 
						skillActiveCards.Add(obj.name, GameData.Team.SkillCards[i]);

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

	private string combineToName(int cardIndex, int cardId, string equiptype){
		return string.Format("{0}_{1}_{2}",cardIndex.ToString(),cardId.ToString(),equiptype);
	}

	private void doLongPress() {
		isPressDown = false;
		UISkillInfo.UIShow(true, skillInfo);
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
		obj.transform.name = skillCardIndex.ToString() + "_" + id.ToString() + "_" + equiptype;
		obj.transform.localPosition = new Vector3(-120 + 200 * (positionIndex % 4), 120 - 265 * (positionIndex / 4), 0);
		obj.transform.localScale = Vector3.one;
		UIEventListener.Get(obj).onPress = OnCardDetailInfo;

		Transform t = obj.transform.FindChild("SkillCard");
		if(t != null)
			t.gameObject.GetComponent<UISprite>().spriteName = "SkillCard" + Mathf.Clamp(lv, 1, 3).ToString();
		
		t = obj.transform.FindChild("SkillPic");
		if(t != null)
			t.gameObject.GetComponent<UITexture>().mainTexture = GameData.CardTexture(id);
		
		t = obj.transform.FindChild("SkillLevel");
		if(t != null)
			t.gameObject.GetComponent<UILabel>().text = lv.ToString();

		t = obj.transform.FindChild("SkillName");
		if(t != null) 
			if(GameData.DSkillData.ContainsKey(id))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Name;

		t = obj.transform.FindChild("LabelValue");
		if(t != null)
			if(GameData.DSkillData.ContainsKey(id))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Space(lv).ToString();

		return obj;
	}

	private GameObject addUIItems (int skillCardIndex, int positionIndex, int id, int lv, GameObject parent, string equiptype) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent.transform;
		obj.transform.name = skillCardIndex.ToString() + "_" + id.ToString() + "_" + equiptype;
		if(id >= IDActiveLimit)
			obj.transform.localPosition = Vector3.zero;
		else 
			obj.transform.localPosition = new Vector3(0, 200 - 80 * positionIndex, 0);
		obj.transform.localScale = Vector3.one;

		UIEventListener.Get(obj).onPress = OnItemDetailInfo;

		if(obj.transform.FindChild("BtnRemove") != null) 
			UIEventListener.Get(obj.transform.FindChild("BtnRemove").gameObject).onPress = OnRomveItem;
		
		Transform t = obj.transform.FindChild("SkillLevel");
		if(t != null)
			t.gameObject.GetComponent<UILabel>().text = Mathf.Clamp(lv, 1, 3).ToString();
		
		t = obj.transform.FindChild("SkillName");
		if(t != null)
			if(GameData.DSkillData.ContainsKey(id))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Name;
		
		t = obj.transform.FindChild("SkillCost");
		if(t != null)
			if(GameData.DSkillData.ContainsKey(id))
				t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Space(lv).ToString();

		return obj;
	}

	private bool addItems(int cardIndex, int cardId, int lv, string equiptype) {
		if(setCost(GameData.DSkillData[cardId].Space(lv))) {
			if(cardId < IDActiveLimit) {
				itemPassiveCards.Add(addUIItems(cardIndex,
				                                itemPassiveCards.Count,
				                                cardId,
				                                lv,
				                                gridPassiveCardBase,
				                                equiptype));
				
				scrollViewItemList.UpdateScrollbars();
			} else {
				if(itemActiveCard != null)
					refreshCards(itemActiveCard.name);
				itemActiveCard = addUIItems(cardIndex,
				                            0,
				                            cardId,
				                            lv,
				                            gridActiveCardBase,
				                            equiptype);
			}
			if(!skillsRecord.Contains (combineToName(cardIndex, cardId, equiptype)))
				skillsRecord.Add(combineToName(cardIndex, cardId, equiptype));

			return true;
		} else 
			UIHint.Get.ShowHint("More than SpaceMax", Color.red);
		return false;
	}

	private void removeItems(int id, GameObject go = null) {
		if(id < IDActiveLimit) {
			Transform t = go.transform.FindChild("SkillCost");
			if(t)
				setCost(-int.Parse(t.GetComponent<UILabel>().text));
			
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
		} else {
			Transform t = itemActiveCard.transform.FindChild("SkillCost");
			if(t)
				setCost(-int.Parse(t.GetComponent<UILabel>().text));
			
			if(skillsRecord.Contains(itemActiveCard.name))
				skillsRecord.Remove(itemActiveCard.name);
			Destroy(itemActiveCard);
		}
	}
	
	private void refreshCards(string id) {
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
		
		scrollViewItemList.RestrictWithinBounds(true);
		gridPassiveCardBase.SetActive(false);
		gridPassiveCardBase.SetActive(true);
	}

	private bool sortIsAvailable(GameObject card) {
		Transform t = card.transform.FindChild("Selected");
		if(t != null)
			if(!t.gameObject.activeSelf)
				return true;
		return false;
	}

	private bool sortIsSelected(GameObject card) {
		Transform t = card.transform.FindChild("Selected");
		if(t != null)
			if(t.gameObject.activeSelf)
				return true;
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
				int cardIdi = getIndex(skillSortCards[i].name, false);
				int cardIdj = getIndex(skillSortCards[j].name, false);
				string cardIdistr = getIndex(skillSortCards[i].name, true).ToString() + "_" + getIndex(skillSortCards[i].name, false).ToString() + "_" + getEquipedType(skillSortCards[i].name);
				string cardIdjstr = getIndex(skillSortCards[j].name, true).ToString() + "_" + getIndex(skillSortCards[j].name, false).ToString() + "_" + getEquipedType(skillSortCards[j].name);

				if(condition == ECondition.Rare) {
					if(GameData.DSkillData.ContainsKey(cardIdi))
						value1 = GameData.DSkillData[cardIdi].Star;
					if(GameData.DSkillData.ContainsKey(cardIdj))
						value2 = GameData.DSkillData[cardIdj].Star;
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
				skillSortCards[i].transform.localPosition = new Vector3(-120 + 200 * (index % 4), 120 - 265 * (index / 4), 0);
				index++;
			}
			skillSortCards[i].SetActive(result);
		}
	}

	private void setInfo (GameObject go, int cardId) {
		skillInfo.ID = cardId;
		skillInfo.Name = GameData.DSkillData[cardId].Name;
		skillInfo.Lv = go.transform.FindChild("SkillLevel").GetComponent<UILabel>().text;
		skillInfo.Info = GameData.DSkillData[cardId].Explain;
	}

	public void ItemDragFinish(){
		if(itemPassiveCards.Count < 5){
			scrollViewItemList.transform.DOLocalMoveY(-4, 0.2f).OnUpdate(updateClipOffset);
		}
	}

	public void updateClipOffset(){
		scrollViewItemList.panel.clipOffset = new Vector2(0, -scrollViewItemList.transform.localPosition.y);
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
						if(cardId >= IDActiveLimit) {
							if(itemActiveCard == null){
								if(addItems(cardIndex, cardId, skillActiveCards[go.name].Lv, equipType))
									t.gameObject.SetActive(true);
							} else {
								removeItems(cardId);
								if(itemActiveCard.name.Equals(go.name)) {
									t.gameObject.SetActive(false);
								}else {
									if(addItems(cardIndex, cardId, skillActiveCards[go.name].Lv, equipType))
									t.gameObject.SetActive(true);
								}
							}
						} else {
							if(t.gameObject.activeInHierarchy) {
								//Selected to NoSelected
								t.gameObject.SetActive(!t.gameObject.activeInHierarchy);
								removeItems(cardId, go);
							} else {
								//NoSelected to Selected
								if(addItems(cardIndex, cardId, skillPassiveCards[go.name].Lv, equipType ))
									t.gameObject.SetActive(!t.gameObject.activeInHierarchy);

							}
						}
					}
				}
			}
		}
		
		isPressDown = state;
		setInfo(go, cardId);
	}

	public void OnItemDetailInfo (GameObject go, bool state){
		int cardId = getIndex(go.name, false);
		
		if (state) {
			longPressTime = Time.realtimeSinceStartup; 
		} else {
			if(!UISkillInfo.Visible) {
				//Click Item
				if(isPressDown) {
					UISkillInfo.UIShow(true, skillInfo);
				}
			}
		}

		isPressDown = state;
		setInfo(go, cardId);
	}

	//From Item RemoveButton
	public void OnRomveItem(GameObject go, bool state){
		int id = getIndex(go.transform.parent.name, false);
		removeItems(id, go.transform.parent.gameObject);
		refreshCards(go.transform.parent.name);
	}
	
	public void SetSort (ECondition condition, int filter) {
		sortSkillCondition(condition);
		sortSkillFilter(filter);
		
		scrollViewCardList.ResetPosition();
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
	}

	public void DoOpenActive (){
		//Open Actvie Cards
		sortSkillFilter((int)EFilter.Active);
		scrollViewCardList.ResetPosition();
		gridCardList.SetActive(false);
		gridCardList.SetActive(true);
	}

	public void DoOpenPassive (){
		//Open Passive Cards
		sortSkillFilter((int)EFilter.Passive);
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

			if(addIndexs.Length > 0 || removeIndexs.Length > 0) {
				WWWForm form = new WWWForm();
				form.AddField("RemoveIndexs", JsonConvert.SerializeObject(removeIndexs));
				form.AddField("AddIndexs", JsonConvert.SerializeObject(addIndexs));
				SendHttp.Get.Command(URLConst.EquipsSkillCard, waitEquipSkillCard, form);
			}
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
			GameData.Team.Player.Init();
			UIShow(false);
		} else {
			Debug.LogError("text:"+www.text);
		}
	}
}
