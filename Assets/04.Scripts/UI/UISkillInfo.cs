using DG.Tweening;
using GameStruct;
using UnityEngine;

public class UISkillInfo : UIBase {
	private static UISkillInfo instance = null;
	private const string UIName = "UISkillInfo";

	private UILabel labelEquip;
	private GameObject btnEquip;

	//SkillInfo
	private UISprite spriteSkillQuality;
	private UISprite spriteSkillLevel;
	private UILabel labelSkillSpace;
	private UILabel labelSkillExp;
	private UISlider sliderSkillExpBar;

	//Buff Ability
	private UILabel labelSkillAttrKind;
	private UILabel labelSkillAttrKindValue;
	private UILabel labelSkillLifeTime;

	//Trigger
	private UILabel labelSkillRate;
	private UILabel labelSkillDistance;
	private UISlider sliderProbability;
	private UISlider sliderDistance;

	//Explain
	private UILabel labelSkillExplain;

	//Card
	private GameObject btnMedium;
	private GameObject btnMediumTop;
	private UISprite spriteSkillCard;
	private UITexture textureSkillPic;
	private UISprite spriteSkillCardLevel;
	private UILabel labelSkillCardName;
	private UISprite spriteSkillStar;

	private bool isAlreadyEquip;

	private bool isOpen = false;
	private float openCardSpeed = 0.1f;
	
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
			Get.btnEquip.SetActive(!isMaskOpen);

			Get.isAlreadyEquip = isEquip;
			if(isEquip)
				Get.labelEquip.text = "UNEQUIP";
			else
				Get.labelEquip.text = "EQUIP";

			TSkillData skillData = GameData.DSkillData[info.ID];
			int lv = int.Parse(info.Lv);

			//MediumCard
			Get.spriteSkillCard.spriteName = "cardlevel_" + Mathf.Clamp(skillData.Quality, 1, 3);
			Get.textureSkillPic.mainTexture = GameData.CardTexture(info.ID);
			Get.labelSkillCardName.text = info.Name;
			Get.spriteSkillCardLevel.spriteName = "Cardicon" + info.Lv;
			Get.spriteSkillStar.spriteName = "Staricon" + Mathf.Clamp(skillData.Star, 1, 5).ToString();

			//SkillInfo
			Get.spriteSkillQuality.spriteName = "Levelball" + Mathf.Clamp(skillData.Quality, 1, 3);
			Get.spriteSkillLevel.spriteName = "Cardicon" + info.Lv;
			Get.labelSkillSpace.text = skillData.Space(lv).ToString();
			Get.labelSkillExp.text = "0"; //=======
			Get.sliderSkillExpBar.value = 0; //======

			//Buff Ability
			Get.labelSkillAttrKind.text = "[00caff]" + Get.getKindName(skillData.AttrKind);
			Get.labelSkillAttrKindValue.text  = skillData.Value(lv).ToString();
			Get.labelSkillLifeTime.text = skillData.LifeTime(lv).ToString();

			//Trigger
			Get.labelSkillRate.text = skillData.Rate(lv).ToString();
			Get.labelSkillDistance.text = skillData.Distance(lv).ToString();
			Get.sliderProbability.value = skillData.Rate(lv) / 100f;
			Get.sliderDistance.value = skillData.Distance(lv) / 30f;
			
			//Explain
			Get.labelSkillExplain.text = skillData.Explain;
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
		btnEquip = GameObject.Find (UIName + "/TopRight/EquipBtn");
		btnMedium = GameObject.Find (UIName + "/Left/BtnMediumCard");
		btnMediumTop = GameObject.Find (UIName + "/Left/BtnMediumCard/Top");
		btnMediumTop.SetActive(false);
		//SkillInfo
		spriteSkillQuality = GameObject.Find (UIName + "/Left/SkillInfo/SkillQuality").GetComponent<UISprite>();
		spriteSkillLevel = GameObject.Find (UIName + "/Left/SkillInfo/SkillQuality/SkillLevel").GetComponent<UISprite>();
		labelSkillSpace = GameObject.Find (UIName + "/Left/SkillInfo/SkillSpace ").GetComponent<UILabel>();
		labelSkillExp = GameObject.Find (UIName + "/Left/SkillInfo/SkillExp").GetComponent<UILabel>();
		sliderSkillExpBar = GameObject.Find (UIName + "/Left/SkillInfo/SkillExpBar").GetComponent<UISlider>();
		
		//Buff Ability
		labelSkillAttrKind = GameObject.Find (UIName + "/Left/BuffAbility/SkillAttrKind").GetComponent<UILabel>();
		labelSkillAttrKindValue  = GameObject.Find (UIName + "/Left/BuffAbility/SkillAttrKindValue").GetComponent<UILabel>();
		labelSkillLifeTime = GameObject.Find (UIName + "/Left/BuffAbility/SkillLifeTime").GetComponent<UILabel>();
		
		//Trigger
		labelSkillRate = GameObject.Find (UIName + "/Left/Trigger/SkillRate").GetComponent<UILabel>();
		labelSkillDistance = GameObject.Find (UIName + "/Left/Trigger/SkillDistance").GetComponent<UILabel>();
		sliderProbability = GameObject.Find (UIName + "/Left/Trigger/ProbabilityBar").GetComponent<UISlider>();
		sliderDistance = GameObject.Find (UIName + "/Left/Trigger/DistanceBar").GetComponent<UISlider>();
		
		//Explain
		labelSkillExplain = GameObject.Find (UIName + "/Left/Explain/SkillArea/SkillExplain").GetComponent<UILabel>();
		
		//Card
		spriteSkillCard = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillCard").GetComponent<UISprite>();
		textureSkillPic = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillPic").GetComponent<UITexture>();
		spriteSkillCardLevel = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillLevel").GetComponent<UISprite>();
		labelSkillCardName = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillName").GetComponent<UILabel>();
		spriteSkillStar = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillStar").GetComponent<UISprite>();

//		UIEventListener.Get(GameObject.Find (UIName + "/FullScreen")).onClick = Close;
		
		SetBtnFun(UIName + "/Center/BG", OnClose);
		SetBtnFun(UIName + "/BottomRight/BackBtn", OnClose);
		SetBtnFun(UIName + "/TopRight/EquipBtn", OnEquip);
		SetBtnFun(UIName + "/TopRight/CraftingBtn", OnCrafting);
		SetBtnFun(UIName + "/TopRight/UpgradeBtn", OnUpgrade);
		SetBtnFun(UIName + "/Left/BtnMediumCard/ItemSkillCard", OpenCard);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}
	private string getKindName (int index) {
		switch (index) {
		case 1:
			return "2PT";
		case 2:
			return "3PT";
		case 3:
			return "SPD";
		case 4:
			return "STA";
		case 5:
			return "STR";
		case 6:
			return "DNK";
		case 7:
			return "REB";
		case 8:
			return "BLK";
		case 9:
			return "DEF";
		case 10:
			return "STL";
		case 11:
			return "DRB";
		case 12:
			return "PAS";
		case 13:
			return "";
		case 14:
			return "";
		case 15:
			return "";
		case 16:
			return "";
		case 17:
			return "";
		default:
			return "";
		}
	}

	private void openCardTurn(bool isRight) {
		isOpen = true;
		btnMedium.transform.DOKill();
		btnMediumTop.SetActive(true);
		btnMedium.transform.DOLocalMoveX(640, openCardSpeed);
		btnMedium.transform.DOScale(new Vector3(2f, 2f, 1), openCardSpeed);
		if(isRight)
			btnMedium.transform.DOLocalRotate(new Vector3(0, 0, 90), openCardSpeed);
		else 
			btnMedium.transform.DOLocalRotate(new Vector3(0, 0, -90), openCardSpeed);
	}

	private void closeCardTurn (){
		isOpen = false;
		btnMedium.transform.DOKill();
		btnMediumTop.SetActive(false);
		btnMedium.transform.DOLocalMoveX(255, openCardSpeed);
		btnMedium.transform.DOScale(new Vector3(1.35f, 1.35f, 1), openCardSpeed);
		btnMedium.transform.DOLocalRotate(new Vector3(0, 0, 0), openCardSpeed);
	}

	public void OnClose() {
		TSkillInfo info = new TSkillInfo();
		UIShow(false, info, false, false);
	}

	public void OpenCard() {
		if(!isOpen) {
			if(Screen.orientation == ScreenOrientation.LandscapeLeft) {
				openCardTurn(true);
			} else if(Screen.orientation == ScreenOrientation.LandscapeRight) {
				openCardTurn(false);
			} else {
				openCardTurn(true);
			}
		} else
			closeCardTurn();
	}

	public void OnEquip() {
		if(isAlreadyEquip)
			UISkillFormation.Get.DoUnEquipCard();
		else
			UISkillFormation.Get.DoEquipCard();
		OnClose();
	}

	public void OnCrafting () {
		UIHint.Get.ShowHint("Coming Soon.", Color.red);
	}

	public void OnUpgrade() {
		UIHint.Get.ShowHint("Coming Soon.", Color.red);
	}

}

