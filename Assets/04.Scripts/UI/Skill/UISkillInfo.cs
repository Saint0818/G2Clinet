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

	private TUICard mUICard;

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

	public void ShowFromSkill (TUICard uicard, bool isEquip, bool isMaskOpen) {
		UIShow (true);
		mUICard = uicard;
		isAlreadyEquip = isEquip;

		if(isEquip)
			labelEquip.text = "UNEQUIP";
		else
			labelEquip.text = "EQUIP";

		if(GameData.DSkillData.ContainsKey(uicard.CardID)) {
			TSkillData skillData = GameData.DSkillData[uicard.CardID];

			//MediumCard
			spriteSkillCard.spriteName = "cardlevel_" + Mathf.Clamp(skillData.Quality, 1, 3);
			textureSkillPic.mainTexture = GameData.CardTexture(uicard.CardID);
			labelSkillCardName.text = GameData.DSkillData[uicard.CardID].Name;
			spriteSkillCardLevel.spriteName = "Cardicon" + uicard.CardLV.ToString();
			spriteSkillStar.spriteName = "Staricon" + Mathf.Clamp(skillData.Star, 1, 5).ToString();
			if(GameFunction.IsActiveSkill(uicard.CardID)) {
				spriteSkillKind.spriteName = "ActiveIcon";
				labelSkillInfoKind4.text = TextConst.S(7207);
			} else {
				spriteSkillKind.spriteName = "PasstiveIcon";
				labelSkillInfoKind4.text = TextConst.S(7206);
			}

			//SkillInfo
			spriteSkillQuality.spriteName = "Levelball" + Mathf.Clamp(skillData.Quality, 1, 3);
			spriteSkillLevel.spriteName = "Cardicon" + uicard.CardLV.ToString();
			labelSkillSpace.text = skillData.Space(uicard.CardLV).ToString();
			labelSkillExp.text = "0"; //=======
			sliderSkillExpBar.value = 0; //======
			if(GameFunction.IsActiveSkill(uicard.CardID))
				Get.labelSkillDemandValue.text = skillData.MaxAnger.ToString();
			else 
				Get.labelSkillDemandValue.text = skillData.Rate(uicard.CardLV).ToString() + "%";

			//Buff Ability
			int index = 0;
			if(skillData.Distance(uicard.CardLV) > 0) {
				buffViews[index].ShowDistance(skillData.Distance(uicard.CardLV));
				index ++;
			}

			if(skillData.Kind == 210 || skillData.Kind == 220 || skillData.Kind == 230) {
				buffViews[index].ShowTime(skillData.AttrKind, skillData.LifeTime(uicard.CardLV), skillData.Value(uicard.CardLV));
				index ++;
			}

			if(index == 0) {
				labelSubhead.gameObject.SetActive(false);
			}

			//Explain
			labelSkillExplain.text = GameFunction.GetStringExplain(skillData.Explain, uicard.CardID, uicard.CardLV);
		}
	}

	public void ShowFromNewCard (TSkill skill) {
		UIShow (true);
		btnEquip.SetActive(false);

		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			TSkillData skillData = GameData.DSkillData[skill.ID];

			//MediumCard
			spriteSkillCard.spriteName = "cardlevel_" + Mathf.Clamp(skillData.Quality, 1, 3);
			textureSkillPic.mainTexture = GameData.CardTexture(skill.ID);
			labelSkillCardName.text = GameData.DSkillData[skill.ID].Name;
			spriteSkillCardLevel.spriteName = "Cardicon" + skill.Lv.ToString();
			spriteSkillStar.spriteName = "Staricon" + Mathf.Clamp(skillData.Star, 1, 5).ToString();
			if(GameFunction.IsActiveSkill(skill.ID)) {
				spriteSkillKind.spriteName = "ActiveIcon";
				labelSkillInfoKind4.text = TextConst.S(7207);
			} else {
				spriteSkillKind.spriteName = "PasstiveIcon";
				labelSkillInfoKind4.text = TextConst.S(7206);
			}

			//SkillInfo
			spriteSkillQuality.spriteName = "Levelball" + Mathf.Clamp(skillData.Quality, 1, 3);
			spriteSkillLevel.spriteName = "Cardicon" + skill.Lv.ToString();
			labelSkillSpace.text = skillData.Space(skill.Lv).ToString();
			labelSkillExp.text = "0"; //=======
			sliderSkillExpBar.value = 0; //======
			if(GameFunction.IsActiveSkill(skill.ID))
				labelSkillDemandValue.text = skillData.MaxAnger.ToString();
			else 
				labelSkillDemandValue.text = skillData.Rate(skill.Lv).ToString() + "%";

			//Buff Ability
			int index = 0;
			if(skillData.Distance(skill.Lv) > 0) {
				buffViews[index].ShowDistance(skillData.Distance(skill.Lv));
				index ++;
			}

			if(skillData.Kind == 210 || skillData.Kind == 220 || skillData.Kind == 230) {
				buffViews[index].ShowTime(skillData.AttrKind, skillData.LifeTime(skill.Lv), skillData.Value(skill.Lv));
				index ++;
			}

			if(index == 0)
			{
				labelSubhead.gameObject.SetActive(false);
			}

			//Explain
			labelSkillExplain.text = GameFunction.GetStringExplain(skillData.Explain, skill.ID, skill.Lv);
		}
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
//		TSkill info = new TSkill();
		UIShow(false);
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
			UISkillFormation.Get.DoUnEquipCard(mUICard);
		else
			UISkillFormation.Get.DoEquipCard(mUICard);
		OnClose();
	}

	public void OnCrafting () {
		UIHint.Get.ShowHint("Coming Soon.", Color.red);
	}

	public void OnUpgrade() {
		UIHint.Get.ShowHint("Coming Soon.", Color.red);
	}

}

