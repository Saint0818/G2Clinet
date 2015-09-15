using UnityEngine;
using System.Collections;
using GameStruct;

public struct TSelectAttrData{
	public UISlider Slider;
	public UILabel Value;
}

public class UICharacterInfo : UIBase {
	private static UICharacterInfo instance = null;
	private const string UIName = "UICharacterInfo";

	
	private TSelectAttrData [] arraySelectAttrData = new TSelectAttrData[12];
	private float [] arrayOldValue = new float[12];
	private float [] arrayNewValue = new float[12];
	private float [] arraySortValue = new float[12];

	private int activeID;
	private GameObject viewSkillInfo;
	private UILabel labelSkillLevel;
	private UILabel labelSkillInfo;
	private UILabel labelSkillName;

	private UIGrid gridPassive;
	private GameObject itemPassiveCard;
	private UIPanel panelPassive;

	private UILabel labelActiveName;
	private UILabel labelActiveLevel;
	private UITexture spriteActivePic;
	private UISprite spriteActiveCard;

	private bool isPressDown = false;
	private float longPressMaxTime = 0.3f;
	private float longPressTime = 0;

	public static bool Visible{
		get{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
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
	
	public static UICharacterInfo Get{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UICharacterInfo;
			
			return instance;
		}
	}

	protected override void InitCom() {
		labelActiveName = GameObject.Find (UIName + "/CharacterInfo/SkillCards/ActiveSkills/ActiveCard/SkillName").GetComponent<UILabel>();
		labelActiveLevel = GameObject.Find (UIName + "/CharacterInfo/SkillCards/ActiveSkills/ActiveCard/SkillLevel").GetComponent<UILabel>();
		spriteActiveCard = GameObject.Find (UIName + "/CharacterInfo/SkillCards/ActiveSkills/ActiveCard/SkillCard").GetComponent<UISprite>();
		spriteActivePic = GameObject.Find (UIName + "/CharacterInfo/SkillCards/ActiveSkills/ActiveCard/SkillPic").GetComponent<UITexture>();

		labelSkillLevel = GameObject.Find (UIName + "/SkillInfo/LabelLevel").GetComponent<UILabel>();
		labelSkillInfo = GameObject.Find (UIName + "/SkillInfo/LabelSkillinfo").GetComponent<UILabel>();
		labelSkillName = GameObject.Find (UIName + "/SkillInfo/LabelNameTW").GetComponent<UILabel>();
		viewSkillInfo = GameObject.Find (UIName + "/SkillInfo");
		viewSkillInfo.SetActive(false);

		gridPassive = GameObject.Find (UIName + "/CharacterInfo/SkillCards/PassiveSkills/CardList/UIGrid").GetComponent<UIGrid>();
		panelPassive = GameObject.Find (UIName + "/CharacterInfo/SkillCards/PassiveSkills/CardList").GetComponent<UIPanel>();
		itemPassiveCard = Resources.Load("Prefab/UI/Items/ItemPassiveSkillCard") as GameObject;
		
		arraySelectAttrData [0].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/2Point").GetComponent<UISlider>();
		arraySelectAttrData [0].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/2Point/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [1].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/3Point").GetComponent<UISlider>();
		arraySelectAttrData [1].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/3Point/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [2].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Speed").GetComponent<UISlider>();
		arraySelectAttrData [2].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Speed/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [3].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Stamina").GetComponent<UISlider>();
		arraySelectAttrData [3].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Stamina/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [4].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Rebound").GetComponent<UISlider>();
		arraySelectAttrData [4].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Rebound/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [5].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Dunk").GetComponent<UISlider>();
		arraySelectAttrData [5].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Dunk/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [6].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Block").GetComponent<UISlider>();
		arraySelectAttrData [6].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Block/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [7].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Strength").GetComponent<UISlider>();
		arraySelectAttrData [7].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Strength/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [8].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Defence").GetComponent<UISlider>();
		arraySelectAttrData [8].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Defence/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [9].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Steal").GetComponent<UISlider>();
		arraySelectAttrData [9].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Steal/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [10].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Dribble").GetComponent<UISlider>();
		arraySelectAttrData [10].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Dribble/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [11].Slider = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Pass").GetComponent<UISlider>();
		arraySelectAttrData [11].Value = GameObject.Find (UIName + "/CharacterInfo/AttributeBar/Pass/LabelValue").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find(UIName + "/CharacterInfo")).onClick = CloseInfo;
		UIEventListener.Get(GameObject.Find(UIName + "/CharacterInfo/InfoBoard/Close")).onClick = CloseInfo;
		UIEventListener.Get(GameObject.Find(UIName + "/CharacterInfo/AttributeBar/LineShoot0")).onPress = ShowDetailInfo;
		UIEventListener.Get(GameObject.Find(UIName + "/CharacterInfo/AttributeBar/LineShoot1")).onPress = ShowDetailInfo;
		UIEventListener.Get(GameObject.Find(UIName + "/CharacterInfo/AttributeBar/LineShoot2")).onPress = ShowDetailInfo;
		UIEventListener.Get(GameObject.Find(UIName + "/CharacterInfo/SkillCards/ActiveSkills/ActiveCard")).onPress = ShowDetailInfo;
	}

	protected override void InitData() {
		
	}
	
	protected override void InitText(){
		
	}

	void FixedUpdate(){
		for(int i = 0; i < arrayOldValue.Length; i++) {
			if(arrayOldValue[i] != arrayNewValue[i]) {
				if(arrayOldValue[i] > arrayNewValue[i]) {
					setSubAttr(i, arrayOldValue[i] - 1);
					arrayOldValue[i] -= 1;
				} else {
					setSubAttr(i, arrayOldValue[i] + 1);
					arrayOldValue[i] += 1;
				}
			}
		}

		if(isPressDown)
			if (Time.realtimeSinceStartup - longPressTime > longPressMaxTime) 
				doLongPress(); 

	}

	public void ShowDetailInfo (GameObject go, bool state){
		isPressDown = state;
		if (state) 
			longPressTime = Time.realtimeSinceStartup; 
		else
			viewSkillInfo.SetActive(false);
		if(!go.name.Equals("CharacterInfo")) {
			if(go.name.Equals("ActiveCard")){
				labelSkillName.text = GameData.DSkillData[activeID].Name;
				labelSkillLevel.text = labelActiveLevel.text;
				labelSkillInfo.text = GameData.DSkillData[activeID].ExplainTW;
			} else if(go.name.Equals("LineShoot0")){
				labelSkillName.text = "SCORER";
				labelSkillLevel.text = "0";
				labelSkillInfo.text = TextConst.S(12);
			} else if(go.name.Equals("LineShoot1")){
				labelSkillName.text = "CENTER";
				labelSkillLevel.text = "0";
				labelSkillInfo.text = TextConst.S(13);
			} else if(go.name.Equals("LineShoot2")){
				labelSkillName.text = "POINT GUARD";
				labelSkillLevel.text = "0";
				labelSkillInfo.text = TextConst.S(14);
			} else {
				labelSkillName.text = GameData.DSkillData[int.Parse(go.name)].Name;
				labelSkillLevel.text = go.transform.FindChild("SkillLevel").GetComponent<UILabel>().text;
				labelSkillInfo.text = GameData.DSkillData[int.Parse(go.name)].ExplainTW;
			}
		}
	}

	private void doLongPress() {
		isPressDown = false;
		float x = 0;
		float y = 0;
		if(Input.mousePosition.x < (Screen.width * 0.6f))
			x = Input.mousePosition.x - ((float)Screen.width * 0.25f);
		else 
			x = Input.mousePosition.x - ((float)Screen.width * 0.7f);

		if(Input.mousePosition.y < (Screen.height * 0.5f)) 
			y = 150;
		else 
			y = -50; 

		viewSkillInfo.transform.localPosition = new Vector3( x, y, 0);
		viewSkillInfo.SetActive(true);
	}

	public void CloseInfo (GameObject go){
		UIShow(false);
	}

	private void setSubAttr(int Index, float Value) {
		arraySelectAttrData [Index].Slider.value = 0;//Value / 100;
		arraySelectAttrData [Index].Value.text = Value.ToString ();
	}

	private void clearPassive(){
		for (int i=0; i<gridPassive.GetChildList().Count; i++){
			Destroy(gridPassive.GetChild(i).gameObject);
		}
	}

	private void setActiveCard (TGreatPlayer data) {
		labelActiveLevel.text = data.ActiveLV.ToString();
		spriteActiveCard.spriteName = "SkillCard" + data.ActiveLV;
		
		labelActiveName.text = GameData.DSkillData[data.Active].Name;
		spriteActivePic.mainTexture = GameData.CardTexture(data.Active);
		
		activeID = data.Active;
	}

	private void setPassiveCard(TPlayer player){
		for (int i=0; i<player.SkillCards.Length; i++) {
			if(player.SkillCards[i].ID > 0) {
				addPassiveCard(i, player.SkillCards[i].ID, player.SkillCards[i].Lv);
			}
		}
		panelPassive.transform.localPosition = new Vector3(27, -25, 0);
		panelPassive.clipOffset = new Vector2(8, 0);
	}

	private void addPassiveCard(int index, int id, int lv){
		GameObject obj = Instantiate(itemPassiveCard, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = gridPassive.transform;
		obj.transform.name = id.ToString();
		obj.transform.localPosition = new Vector3(index * 210, 0, 0);
		obj.transform.localScale = Vector3.one;
		UIEventListener.Get(obj).onPress = ShowDetailInfo;
		Transform t = obj.transform.FindChild("SkillCard");
		if(t != null)
			t.gameObject.GetComponent<UISprite>().spriteName = "SkillCard" + lv.ToString();

		t = obj.transform.FindChild("SkillPic");
		if(t != null)
			t.gameObject.GetComponent<UITexture>().mainTexture = GameData.CardTexture(id);

		t = obj.transform.FindChild("SkillLevel");
		if(t != null)
			t.gameObject.GetComponent<UILabel>().text = lv.ToString();

		
		t = obj.transform.FindChild("SkillName");
		if(t != null)
			t.gameObject.GetComponent<UILabel>().text = GameData.DSkillData[id].Name;
	}

	public void SetAttribute(TGreatPlayer data, TPlayer player) {
		clearPassive();
		setPassiveCard(player);
		setActiveCard(data);
		if(arrayOldValue[0] == 0) {
			arrayOldValue[0] = data.Point2;
			arrayNewValue[0] = data.Point2;
			setSubAttr(0, data.Point2);
			arrayOldValue[1] = data.Point3;
			arrayNewValue[1] = data.Point3;
			setSubAttr(1, data.Point3);
			arrayOldValue[2] = data.Speed;
			arrayNewValue[2] = data.Speed;
			setSubAttr(2, data.Speed);
			arrayOldValue[3] = data.Stamina;
			arrayNewValue[3] = data.Stamina;
			setSubAttr(3, data.Stamina);
			arrayOldValue[4] = data.Rebound;
			arrayNewValue[4] = data.Rebound;
			setSubAttr(4, data.Rebound);
			arrayOldValue[5] = data.Dunk;
			arrayNewValue[5] = data.Dunk;
			setSubAttr(5, data.Dunk);
			arrayOldValue[6] = data.Block;
			arrayNewValue[6] = data.Block;
			setSubAttr(6, data.Block);
			arrayOldValue[7] = data.Strength;
			arrayNewValue[7] = data.Strength;
			setSubAttr(7, data.Strength);
			arrayOldValue[8] = data.Defence;
			arrayNewValue[8] = data.Defence;
			setSubAttr(8, data.Defence);
			arrayOldValue[9] = data.Steal;
			arrayNewValue[9] = data.Steal;
			setSubAttr(9, data.Steal);
			arrayOldValue[10] = data.Dribble;
			arrayNewValue[10] = data.Dribble;
			setSubAttr(10, data.Dribble);
			arrayOldValue[11] = data.Pass;
			arrayNewValue[11] = data.Pass;
			setSubAttr(11, data.Pass);
		} else {
			arrayNewValue[0] = data.Point2;
			arrayNewValue[1] = data.Point3;
			arrayNewValue[2] = data.Speed;
			arrayNewValue[3] = data.Stamina;
			arrayNewValue[4] = data.Rebound;
			arrayNewValue[5] = data.Dunk;
			arrayNewValue[6] = data.Block;
			arrayNewValue[7] = data.Strength;
			arrayNewValue[8] = data.Defence;
			arrayNewValue[9] = data.Steal;
			arrayNewValue[10] = data.Dribble;
			arrayNewValue[11] = data.Pass;
		}
		checkThreeHeight();
	}

	private void checkThreeHeight() {
		for(int i=0; i<arrayNewValue.Length; i++) 
			arraySortValue[i] = arrayNewValue[i];

		System.Array.Sort(arraySortValue);
		for(int i=0; i<arrayNewValue.Length; i++) 
			if(arrayNewValue[i] >= arraySortValue[9]) 
				arraySelectAttrData[i].Value.color = new Color(1, 0.69f, 0.075f);
			else 
				arraySelectAttrData[i].Value.color = Color.white;
	}
}
