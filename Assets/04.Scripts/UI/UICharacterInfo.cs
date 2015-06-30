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

	private UILabel labelActiveName;
	private UILabel labelActiveLevel;
	private UISprite spriteActivePic;
	private UISprite spriteActiveCard;
	
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
		labelActiveName = GameObject.Find (UIName + "/CharacterInfo/SkillCards/ActiveSkills/SkillName").GetComponent<UILabel>();
		labelActiveLevel = GameObject.Find (UIName + "/CharacterInfo/SkillCards/ActiveSkills/SkillLeval").GetComponent<UILabel>();
		spriteActiveCard = GameObject.Find (UIName + "/CharacterInfo/SkillCards/ActiveSkills/SkillCard").GetComponent<UISprite>();
		spriteActivePic = GameObject.Find (UIName + "/CharacterInfo/SkillCards/ActiveSkills/SkillPic").GetComponent<UISprite>();
		
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
	}

	public void CloseInfo (GameObject go){
		UIShow(false);
	}

	private void setSubAttr(int Index, float Value) {
		arraySelectAttrData [Index].Slider.value = 0;//Value / 100;
		arraySelectAttrData [Index].Value.text = Value.ToString ();
	}

	public void SetAttribute(TGreatPlayer data) {
		labelActiveLevel.text = data.ActiveLV.ToString();
		spriteActiveCard.spriteName = "SkillCard" + data.ActiveLV;

		labelActiveName.text = GameData.SkillData[data.Active].Name;
		spriteActivePic.spriteName = data.Name;

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
	}
}
