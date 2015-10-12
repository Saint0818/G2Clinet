using UnityEngine;
using System.Collections;

public class UISkillInfo : UIBase {
	private static UISkillInfo instance = null;
	private const string UIName = "UISkillInfo";

	private UILabel labelEquip;
	
//	private UILabel labelSkillName;
	//space attribute lifetime 
	private UILabel labelSkillLevel;
	private UILabel labelSkillInfo;

	private UISprite spriteSkillCard;
	private UITexture textureSkillPic;
	private UISprite spriteSkillCardLevel;
	private UILabel labelSkillCardName;
	private UISprite spriteSkillStar;

	private bool isAlreadyEquip;
	
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
	
	public static void UIShow(bool isShow, TSkillInfo info, bool isEquip, bool isMaskOpen){
		if(isShow) {
			Get.labelEquip.gameObject.SetActive(!isMaskOpen);

			Get.isAlreadyEquip = isEquip;
			if(isEquip)
				Get.labelEquip.text = "UNEQUIP";
			else
				Get.labelEquip.text = "EQUIP";

//			Get.labelSkillName.text = info.Name;
			Get.labelSkillLevel.text = info.Lv;
			Get.labelSkillInfo.text = info.Info;

			Get.spriteSkillCard.spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[info.ID].Quality, 1, 5);
			Get.textureSkillPic.mainTexture = GameData.CardTexture(info.ID);
			Get.labelSkillCardName.text = info.Name;
			Get.spriteSkillCardLevel.spriteName = "Cardicon" + info.Lv;
			Get.spriteSkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[info.ID].Star, 1, 5).ToString();
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
		labelEquip = GameObject.Find (UIName + "/TopRight/EquipBtn/Label").GetComponent<UILabel>();

//		labelSkillName = GameObject.Find (UIName + "/Left/Info/LabelNameTW").GetComponent<UILabel>();
		labelSkillLevel = GameObject.Find (UIName + "/Left/Info/LabelKindTitle/LabelLv").GetComponent<UILabel>();
		labelSkillInfo = GameObject.Find (UIName + "/Left/Info/LabelExplain").GetComponent<UILabel>();

		spriteSkillCard = GameObject.Find (UIName + "/Left/BtnMediumCard/SkillCard").GetComponent<UISprite>();
		textureSkillPic = GameObject.Find (UIName + "/Left/BtnMediumCard/SkillPic").GetComponent<UITexture>();
		spriteSkillCardLevel = GameObject.Find (UIName + "/Left/BtnMediumCard/SkillLevel").GetComponent<UISprite>();
		labelSkillCardName = GameObject.Find (UIName + "/Left/BtnMediumCard/SkillName").GetComponent<UILabel>();
		spriteSkillStar = GameObject.Find (UIName + "/Left/BtnMediumCard/SkillStar").GetComponent<UISprite>();

		UIEventListener.Get(GameObject.Find (UIName + "/BoxCollider")).onClick = Close;

		SetBtnFun(UIName + "/TopRight/EquipBtn", OnEquip);
		SetBtnFun(UIName + "/Left/BtnMediumCard", OpenCard);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void Close(GameObject go) {
		TSkillInfo info = new TSkillInfo();
		UIShow(false, info, false, false);
	}

	public void OpenCard() {

	}

	public void OnEquip() {
		if(isAlreadyEquip)
			UISkillFormation.Get.DoUnEquipCard();
		else
			UISkillFormation.Get.DoEquipCard();
		Close(null);
	}

}

