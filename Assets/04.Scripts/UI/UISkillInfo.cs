using UnityEngine;
using System.Collections;

public class UISkillInfo : UIBase {
	private static UISkillInfo instance = null;
	private const string UIName = "UISkillInfo";

	private GameObject buttonEquip;
	
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
	
	public static void UIShow(bool isShow, TSkillInfo info, bool isEquip){
		if(isShow) {
			Get.buttonEquip.SetActive(!isEquip);

			Get.labelSkillName.text = info.Name;
			Get.labelSkillLevel.text = info.Lv;
			Get.labelSkillInfo.text = info.Info;

			Get.spriteSkillCard.spriteName = "SkillCard" + info.Lv;
			Get.textureSkillPic.mainTexture = GameData.CardTexture(info.ID);
			Get.labelSkillCardName.text = info.Name;
			Get.labelSkillCardLevel.text = info.Lv;
			Get.labelSkillCardCost.text = GameData.DSkillData[info.ID].Space(int.Parse(info.Lv)).ToString(); 
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
		buttonEquip = GameObject.Find (UIName + "/EquipBtn");

		labelSkillName = GameObject.Find (UIName + "/Window/LabelNameTW").GetComponent<UILabel>();
		labelSkillLevel = GameObject.Find (UIName + "/Window/LabelLevel").GetComponent<UILabel>();
		labelSkillInfo = GameObject.Find (UIName + "/Window/LabelSkillinfo").GetComponent<UILabel>();

		spriteSkillCard = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillCard").GetComponent<UISprite>();
		textureSkillPic = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillPic").GetComponent<UITexture>();
		labelSkillCardLevel = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillLevel").GetComponent<UILabel>();
		labelSkillCardName = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillName").GetComponent<UILabel>();
		labelSkillCardCost = GameObject.Find (UIName + "/Window/BtnMediumCard/SkillCost/LabelValue").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find (UIName + "/BoxCollider")).onClick = Close;

//		SetBtnFun(UIName + "/BoxCollider", Close);
		SetBtnFun(UIName + "/EquipBtn", OnEquip);
		SetBtnFun(UIName + "/Window/BtnMediumCard", OpenCard);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void Close(GameObject go) {
		TSkillInfo info = new TSkillInfo();
		UIShow(false, info, false);
	}

	public void OpenCard() {

	}

	public void OnEquip() {
		UISkillFormation.Get.DoEquipCard();
		Close(null);
	}

}

