using UnityEngine;
using System.Collections;

public class UISkillInfo : UIBase {
	private static UISkillInfo instance = null;
	private const string UIName = "UISkillInfo";
	
	private UILabel labelSkillName;
	private UILabel labelSkillLevel;
	private UILabel labelSkillInfo;

	private UISprite spriteSkillCard;
	private UITexture textureSkillPic;
	private UILabel labelSkillCardLevel;
	private UILabel labelSkillCardName;
	private UILabel labelSkillCardCost;
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UISkillInfo Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillInfo;
			
			return instance;
		}
	}
	
	public static void UIShow(bool isShow, int id = 0, string name = "", string level = "", string info = ""){
		if(isShow) {
			Get.labelSkillName.text = name;
			Get.labelSkillLevel.text = level;
			Get.labelSkillInfo.text = info;

			Get.spriteSkillCard.spriteName = "SkillCard" + level;
			if( GameData.DCardTextures.ContainsKey(id))
				Get.textureSkillPic.mainTexture = GameData.DCardTextures[id];
			Get.labelSkillCardName.text = name;
			Get.labelSkillCardLevel.text = level;
			Get.labelSkillCardCost.text = GameData.DSkillData[id].Space(int.Parse(level)).ToString(); 
		}
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
		labelSkillName = GameObject.Find (UIName + "/Window/LabelNameTW").GetComponent<UILabel>();
		labelSkillLevel = GameObject.Find (UIName + "/Window/LabelLevel").GetComponent<UILabel>();
		labelSkillInfo = GameObject.Find (UIName + "/Window/LabelSkillinfo").GetComponent<UILabel>();

		spriteSkillCard = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillCard").GetComponent<UISprite>();
		textureSkillPic = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillPic").GetComponent<UITexture>();
		labelSkillCardLevel = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillLevel").GetComponent<UILabel>();
		labelSkillCardName = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillName").GetComponent<UILabel>();
		labelSkillCardCost = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillCost").GetComponent<UILabel>();

		SetBtnFun(UIName + "/BoxCollider", Close);
		SetBtnFun(UIName + "/Window/BtnMediumCard", OpenCard);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void Close (){
		UIShow(false);
	}
	public void OpenCard (){

	}
}

