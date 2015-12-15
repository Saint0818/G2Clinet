using DG.Tweening;
using GameStruct;
using UnityEngine;

public class UISkillInfo : UIBase {
	private static UISkillInfo instance = null;
	private const string UIName = "UISkillInfo";

	//Left/SkillInfo
	private UISprite spriteSkillQuality;
	private UISprite spriteSkillLevel;
	private UILabel labelSkillSpace;
	private UILabel labelSkillExp;
	private UILabel labelSkillDemandValue;
	private UISlider sliderSkillExpBar;

	//Left/Buff Ability
	private UILabel labelSubhead;
	private BuffView[] buffViews;

	//Left/Explain
	private UILabel labelSkillExplain;

	//Left/Card
	private GameObject btnMedium;
	private GameObject btnMediumTop;
	private UISprite spriteSkillCard;
	private UITexture textureSkillPic;
	private UISprite spriteSkillCardLevel;
	private UILabel labelSkillCardName;
	private UISprite spriteSkillStar;
	private UISprite spriteSkillKind;
	private UILabel labelSkillInfoKind4;

	//TopRight
	private UILabel labelEquip;
	private GameObject btnEquip;

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
	
	public static void UIShow(bool isShow, TSkill info, bool isEquip, bool isMaskOpen, bool isGetNewCard = false){
		if(isShow) {
			if(!isGetNewCard)
				Get.btnEquip.SetActive(!isMaskOpen);
			else
				Get.btnEquip.SetActive(false);

			Get.isAlreadyEquip = isEquip;
			if(isEquip)
				Get.labelEquip.text = "UNEQUIP";
			else
				Get.labelEquip.text = "EQUIP";

			if(GameData.DSkillData.ContainsKey(info.ID)) {
				TSkillData skillData = GameData.DSkillData[info.ID];
				
				//MediumCard
				Get.spriteSkillCard.spriteName = "cardlevel_" + Mathf.Clamp(skillData.Quality, 1, 3);
				Get.textureSkillPic.mainTexture = GameData.CardTexture(info.ID);
				Get.labelSkillCardName.text = GameData.DSkillData[info.ID].Name;
				Get.spriteSkillCardLevel.spriteName = "Cardicon" + info.Lv.ToString();
				Get.spriteSkillStar.spriteName = "Staricon" + Mathf.Clamp(skillData.Star, 1, 5).ToString();
				if(info.ID >= GameConst.ID_LimitActive) {
					Get.spriteSkillKind.spriteName = "ActiveIcon";
					Get.labelSkillInfoKind4.text = TextConst.S(7207);
				} else {
					Get.spriteSkillKind.spriteName = "PasstiveIcon";
					Get.labelSkillInfoKind4.text = TextConst.S(7206);
				}
				
				//SkillInfo
				Get.spriteSkillQuality.spriteName = "Levelball" + Mathf.Clamp(skillData.Quality, 1, 3);
				Get.spriteSkillLevel.spriteName = "Cardicon" + info.Lv.ToString();
				Get.labelSkillSpace.text = skillData.Space(info.Lv).ToString();
				Get.labelSkillExp.text = "0"; //=======
				Get.sliderSkillExpBar.value = 0; //======
				if(info.ID >= GameConst.ID_LimitActive)
					Get.labelSkillDemandValue.text = skillData.MaxAnger.ToString();
				else 
					Get.labelSkillDemandValue.text = skillData.Rate(info.Lv).ToString() + "%";
				
				//Buff Ability
				int index = 0;
				if(skillData.Distance(info.Lv) > 0) {
					Get.buffViews[index].ShowDistance(skillData.Distance(info.Lv));
					index ++;
				}
				
				if(skillData.Kind == 210 || skillData.Kind == 220 || skillData.Kind == 230) {
					Get.buffViews[index].ShowTime(skillData.AttrKind, skillData.LifeTime(info.Lv), skillData.Value(info.Lv));
					index ++;
				}

				if(index == 0)
				{
					Get.labelSubhead.gameObject.SetActive(false);
				}
				
				//Explain
				Get.labelSkillExplain.text = GameFunction.GetStringExplain(skillData.Explain, info.ID, info.Lv);
			}
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
		labelSkillDemandValue = GameObject.Find (UIName + "/Left/SkillInfo/SkillDemandValue").GetComponent<UILabel>();
		sliderSkillExpBar = GameObject.Find (UIName + "/Left/SkillInfo/SkillExpBar").GetComponent<UISlider>();
		labelSkillInfoKind4  = GameObject.Find (UIName + "/Left/SkillInfo/Labels/LabelKind4").GetComponent<UILabel>();
		
		//Buff Ability
		labelSubhead = GameObject.Find (UIName + "/Left/BuffAbility/LabelSubhead").GetComponent<UILabel>();
		buffViews = GetComponentsInChildren<BuffView>();
		
		//Explain
		labelSkillExplain = GameObject.Find (UIName + "/Left/Explain/SkillArea/SkillExplain").GetComponent<UILabel>();
		
		//Card
		spriteSkillCard = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillCard").GetComponent<UISprite>();
		textureSkillPic = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillPic").GetComponent<UITexture>();
		spriteSkillCardLevel = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillLevel").GetComponent<UISprite>();
		labelSkillCardName = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillName").GetComponent<UILabel>();
		spriteSkillStar = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillStar").GetComponent<UISprite>();
		spriteSkillKind = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillKind").GetComponent<UISprite>();

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

	private void openCardTurn(bool isRight) {
		isOpen = true;
		btnMedium.transform.DOKill();
		btnMediumTop.SetActive(true);
		btnMedium.transform.DOLocalMoveX(640, openCardSpeed);
		btnMedium.transform.DOLocalMoveY(0, openCardSpeed);
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
		btnMedium.transform.DOLocalMoveY(-10, openCardSpeed);
		btnMedium.transform.DOScale(new Vector3(1.35f, 1.35f, 1), openCardSpeed);
		btnMedium.transform.DOLocalRotate(new Vector3(0, 0, 0), openCardSpeed);
	}

	public void OnClose() {
		TSkill info = new TSkill();
		UIShow(false, info, false, false);
		if(UIGameResult.Visible && UIGameResult.Get.IsShowFirstCard) {
			UIGameResult.Get.ShowBonusItem();
		}
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

