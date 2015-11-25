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
	private UILabel labelSkillInfoSubhead;
	private UILabel labelSkillInfoKind1;
	private UILabel labelSkillInfoKind2;
	private UILabel labelSkillInfoKind3;
	private UILabel labelSkillInfoKind4;

	//Left/Buff Ability
	private GameObject buffAbility;
	private UILabel labelSubhead;
	private BuffView[] buffViews;


	//Left/Explain
	private UILabel labelSkillExplainSubhead;
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

	//TopRight
	private UILabel labelEquip;
	private GameObject btnEquip;
	private UILabel labelCraft;
	private UILabel labelUpgrade;

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

			if(GameData.DSkillData.ContainsKey(info.ID)) {
				TSkillData skillData = GameData.DSkillData[info.ID];
				int lv = int.Parse(info.Lv);
				
				//MediumCard
				Get.spriteSkillCard.spriteName = "cardlevel_" + Mathf.Clamp(skillData.Quality, 1, 3);
				Get.textureSkillPic.mainTexture = GameData.CardTexture(info.ID);
				Get.labelSkillCardName.text = info.Name;
				Get.spriteSkillCardLevel.spriteName = "Cardicon" + info.Lv;
				Get.spriteSkillStar.spriteName = "Staricon" + Mathf.Clamp(skillData.Star, 1, 5).ToString();
				if(info.ID >= GameConst.ID_LimitActive) {
					Get.spriteSkillKind.spriteName = "ActiveIcon";
					Get.labelSkillInfoKind4.text = TextConst.S(7207);
				} else {
					Get.spriteSkillKind.spriteName = "PassiveIcon";
					Get.labelSkillInfoKind4.text = TextConst.S(7206);
				}
				
				//SkillInfo
				Get.spriteSkillQuality.spriteName = "Levelball" + Mathf.Clamp(skillData.Quality, 1, 3);
				Get.spriteSkillLevel.spriteName = "Cardicon" + info.Lv;
				Get.labelSkillSpace.text = skillData.Space(lv).ToString();
				Get.labelSkillExp.text = "0"; //=======
				Get.sliderSkillExpBar.value = 0; //======
				Get.labelSkillDemandValue.text = skillData.Rate(lv).ToString();
				
				//Buff Ability
				int index = 0;
				if(skillData.Distance(lv) > 0) {
					Get.buffViews[index].ShowDistance(skillData.Distance(lv));
					index ++;
				}
				
				if(skillData.Kind == 210 || skillData.Kind == 220 || skillData.Kind == 230) {
					Get.buffViews[index].ShowTime(skillData.AttrKind, skillData.LifeTime(lv), skillData.Value(lv));
					index ++;
				}

				if(index == 0)
				{
					Get.labelSubhead.gameObject.SetActive(false);
				}
				
				//Explain
				Get.labelSkillExplain.text = GameFunction.GetStringExplain(skillData.Explain, info.ID, int.Parse(info.Lv));
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
		labelSkillInfoSubhead = GameObject.Find (UIName + "/Left/SkillInfo/Labels/LabelSubhead").GetComponent<UILabel>();
		labelSkillInfoKind1 = GameObject.Find (UIName + "/Left/SkillInfo/Labels/LabelKind1").GetComponent<UILabel>();
		labelSkillInfoKind2 = GameObject.Find (UIName + "/Left/SkillInfo/Labels/LabelKind2").GetComponent<UILabel>();
		labelSkillInfoKind3 = GameObject.Find (UIName + "/Left/SkillInfo/Labels/LabelKind3").GetComponent<UILabel>();
		labelSkillInfoKind4 = GameObject.Find (UIName + "/Left/SkillInfo/Labels/LabelKind4").GetComponent<UILabel>();
		
		//Buff Ability
		buffAbility = GameObject.Find (UIName + "/Left/BuffAbility");
		labelSubhead = GameObject.Find (UIName + "/Left/BuffAbility/LabelSubhead").GetComponent<UILabel>();
		buffViews = GetComponentsInChildren<BuffView>();
		
		//Explain
		labelSkillExplainSubhead = GameObject.Find (UIName + "/Left/Explain/LabelSubhead").GetComponent<UILabel>();
		labelSkillExplain = GameObject.Find (UIName + "/Left/Explain/SkillArea/SkillExplain").GetComponent<UILabel>();
		
		//Card
		spriteSkillCard = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillCard").GetComponent<UISprite>();
		textureSkillPic = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillPic").GetComponent<UITexture>();
		spriteSkillCardLevel = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillLevel").GetComponent<UISprite>();
		labelSkillCardName = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillName").GetComponent<UILabel>();
		spriteSkillStar = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillStar").GetComponent<UISprite>();
		spriteSkillKind = GameObject.Find (UIName + "/Left/BtnMediumCard/ItemSkillCard/SkillKind").GetComponent<UISprite>();

		labelCraft = GameObject.Find (UIName + "/TopRight/CraftingBtn/Label").GetComponent<UILabel>();
		labelUpgrade = GameObject.Find (UIName + "/TopRight/UpgradeBtn/Label").GetComponent<UILabel>();

		SetBtnFun(UIName + "/Center/BG", OnClose);
		SetBtnFun(UIName + "/BottomRight/BackBtn", OnClose);
		SetBtnFun(UIName + "/TopRight/EquipBtn", OnEquip);
		SetBtnFun(UIName + "/TopRight/CraftingBtn", OnCrafting);
		SetBtnFun(UIName + "/TopRight/UpgradeBtn", OnUpgrade);
		SetBtnFun(UIName + "/Left/BtnMediumCard/ItemSkillCard", OpenCard);
	}
	
	protected override void InitData() {
		labelSkillInfoSubhead.text = TextConst.S(7202);
		labelSkillInfoKind1.text = TextConst.S(7203);
		labelSkillInfoKind2.text = TextConst.S(7204);
		labelSkillInfoKind3.text = TextConst.S(7205);
		labelSkillExplainSubhead.text = TextConst.S(7208);
		labelSubhead.text = TextConst.S(7209);
		labelCraft.text = TextConst.S(7212);
		labelUpgrade.text = TextConst.S(7213);
	}
	
	protected override void OnShow(bool isShow) {
		
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

