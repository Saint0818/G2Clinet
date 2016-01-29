using DG.Tweening;
using GameStruct;
using UnityEngine;

public class UISkillInfo : UIBase {
	private static UISkillInfo instance = null;
	private const string UIName = "UISkillInfo";

	//Left/SkillInfo
	private UILabel labelSkillQuality;
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
	private UISprite spriteSkillSuit;
	private UILabel labelSkillCardName;
	private SkillCardStar[] skillStars;
	private UISprite spriteSkillKind;
	private UISprite spriteSkillKindBg;
	private UILabel labelSkillInfoKind4;

	//TopRight
	private UILabel labelEquip;
	private GameObject btnEquip;
	private GameObject btnCrafting;
	private GameObject btnUpgrade;

	private GameObject goEquipUnuse;
	private GameObject goCraftUnuse;
	private GameObject goUpgradeUnuse;

	private GameObject goCraftRedPoint;
	private GameObject goUpgradeRedPoint;

	private bool isOpen = false;
	private float openCardSpeed = 0.1f;

	private TUICard mUICard;
	private bool isAlreadyEquip;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
					RemoveUI(UIName);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}
	
	public static UISkillInfo Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillInfo;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		labelEquip = GameObject.Find (UIName + "/Center/TopRight/EquipBtn/Label").GetComponent<UILabel>();
		btnEquip = GameObject.Find (UIName + "/Center/TopRight/EquipBtn");
		btnCrafting = GameObject.Find (UIName + "/Center/TopRight/CraftingBtn");
		btnUpgrade = GameObject.Find (UIName + "/Center/TopRight/UpgradeBtn");
		goEquipUnuse = GameObject.Find (UIName + "/Center/TopRight/EquipBtn/UnUseLabel");
		goCraftUnuse = GameObject.Find (UIName + "/Center/TopRight/CraftingBtn/UnUseLabel");
		goUpgradeUnuse = GameObject.Find (UIName + "/Center/TopRight/UpgradeBtn/UnUseLabel");

		goCraftRedPoint = GameObject.Find (UIName + "/Center/TopRight/CraftingBtn/RedPoint");
		goUpgradeRedPoint = GameObject.Find (UIName + "/Center/TopRight/UpgradeBtn/RedPoint");

		btnMedium = GameObject.Find (UIName + "/Center/Left/BtnMediumCard");
		btnMediumTop = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/Top");
		btnMediumTop.SetActive(false);

		//SkillInfo
		labelSkillQuality = GameObject.Find (UIName + "/Center/Left/SkillInfo/SkillQuality").GetComponent<UILabel>();
		labelSkillSpace = GameObject.Find (UIName + "/Center/Left/SkillInfo/SkillSpace ").GetComponent<UILabel>();
		labelSkillExp = GameObject.Find (UIName + "/Center/Left/SkillInfo/SkillExp").GetComponent<UILabel>();
		labelSkillDemandValue = GameObject.Find (UIName + "/Center/Left/SkillInfo/SkillDemandValue").GetComponent<UILabel>();
		sliderSkillExpBar = GameObject.Find (UIName + "/Center/Left/SkillInfo/SkillExpBar").GetComponent<UISlider>();
		labelSkillInfoKind4  = GameObject.Find (UIName + "/Center/Left/SkillInfo/Labels/LabelKind4").GetComponent<UILabel>();
		
		//Buff Ability
		labelSubhead = GameObject.Find (UIName + "/Center/Left/BuffAbility/LabelSubhead").GetComponent<UILabel>();
		buffViews = GetComponentsInChildren<BuffView>();
		
		//Explain
		labelSkillExplain = GameObject.Find (UIName + "/Center/Left/Explain/SkillArea/SkillExplain").GetComponent<UILabel>();
		
		//Card
		spriteSkillCard = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillCard").GetComponent<UISprite>();
		textureSkillPic = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillPic").GetComponent<UITexture>();
		spriteSkillSuit = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillSuit").GetComponent<UISprite>();
		labelSkillCardName = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillName").GetComponent<UILabel>();
		skillStars = new  SkillCardStar[5];
		for(int i=0; i<skillStars.Length; i++) 
			skillStars[i] = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillStar/StarBG" + i.ToString()).GetComponent<SkillCardStar>();
		spriteSkillKind = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillKind").GetComponent<UISprite>();
		spriteSkillKindBg  = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillKind/KindBg").GetComponent<UISprite>();

		SetBtnFun(UIName + "/Center/BG", OnClose);
		SetBtnFun(UIName + "/Center/BottomRight/BackBtn", OnClose);
		SetBtnFun(UIName + "/Center/TopRight/EquipBtn", OnEquip);
		SetBtnFun(UIName + "/Center/TopRight/CraftingBtn", OnCrafting);
		SetBtnFun(UIName + "/Center/TopRight/UpgradeBtn", OnUpgrade);
		SetBtnFun(UIName + "/Center/Left/BtnMediumCard/ItemSkillCard", OpenCard);
	}
	
	public void ShowFromSkill (TUICard uicard, bool isEquip, bool isMaskOpen) {
		Visible = true;
		isAlreadyEquip = isEquip;
		btnEquip.SetActive(true);
		btnUpgrade.SetActive(true);
		btnCrafting.SetActive(true);

		if(isEquip)
			labelEquip.text = TextConst.S(7215);
		else
			labelEquip.text = TextConst.S(7214);

		if(GameData.DSkillData.ContainsKey(uicard.skillCard.Skill.ID)) {
			TSkillData skillData = GameData.DSkillData[uicard.skillCard.Skill.ID];
			RefreshUICard(uicard);
		}
	}

	public void ShowFromNewCard (TSkill skill) {
		Visible = true;
		btnEquip.SetActive(false);
		btnUpgrade.SetActive(false);
		btnCrafting.SetActive(false);
		Refresh(skill, -1);

	}

	public void RefreshUICard (TUICard uicard) {
		mUICard = uicard;
		Refresh(uicard.skillCard.Skill, uicard.CardIndex);
	}

	public void Refresh (TSkill skill, int cardIndex) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(mUICard.skillCard != null) {
				mUICard.skillCard.Skill = skill;
				mUICard.CardIndex = cardIndex;
				mUICard.Cost = GameData.DSkillData[skill.ID].Space(skill.Lv);
			}
			TSkillData skillData = GameData.DSkillData[skill.ID];
			if(cardIndex != -1) {
				goEquipUnuse.SetActive((mUICard.Cost > UISkillFormation.Get.ExtraCostSpace) && !isAlreadyEquip);
				goUpgradeUnuse.SetActive((skill.Lv == skillData.MaxStar));
				goUpgradeRedPoint.SetActive((skill.Lv < skillData.MaxStar));

				goCraftUnuse.SetActive((skillData.EvolutionSkill == 0));
				goCraftRedPoint.SetActive((skillData.EvolutionSkill != 0) && (skill.Lv == skillData.MaxStar));
			}

			//MediumCard
			spriteSkillCard.spriteName = "cardlevel_" + GameData.DSkillData[skill.ID].Quality.ToString();
			textureSkillPic.mainTexture = GameData.CardTexture(skill.ID);
			labelSkillCardName.text = GameData.DSkillData[skill.ID].Name;
			GameFunction.ShowStar(ref skillStars, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);
			if(GameFunction.IsActiveSkill(skill.ID)) {
				spriteSkillKind.spriteName = "ActiveIcon";
				labelSkillInfoKind4.text = TextConst.S(7207);
			} else {
				spriteSkillKind.spriteName = "PasstiveIcon";
				labelSkillInfoKind4.text = TextConst.S(7206);
			}
			spriteSkillKindBg.spriteName = "APIcon" + GameData.DSkillData[skill.ID].Quality.ToString();

			//SkillInfo
			labelSkillQuality.text =GameFunction.QualityName(GameData.DSkillData[skill.ID].Quality);
			labelSkillSpace.text = skillData.Space(skill.Lv).ToString();
			if(skill.Lv >= GameData.DSkillData[skill.ID].MaxStar) {
				labelSkillExp.text = TextConst.S(7250); 
				sliderSkillExpBar.value = 1;
			}  else {
				labelSkillExp.text = skill.Exp.ToString() + "/" + GameData.DSkillData[skill.ID].GetUpgradeExp(skill.Lv).ToString(); 
				sliderSkillExpBar.value = (float)skill.Exp / (float)GameData.DSkillData[skill.ID].GetUpgradeExp(skill.Lv);
			}

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
		btnMedium.transform.DOLocalMoveX(690, openCardSpeed);
		btnMedium.transform.DOLocalMoveY(-50, openCardSpeed);
		btnMedium.transform.DOScale(new Vector3(2.2f, 2.2f, 1), openCardSpeed);
		if(isRight)
			btnMedium.transform.DOLocalRotate(new Vector3(0, 0, 90), openCardSpeed);
		else 
			btnMedium.transform.DOLocalRotate(new Vector3(0, 0, -90), openCardSpeed);
	}

	private void closeCardTurn (){
		isOpen = false;
		btnMedium.transform.DOKill();
		btnMediumTop.SetActive(false);
		btnMedium.transform.DOLocalMoveX(325, openCardSpeed);
		btnMedium.transform.DOLocalMoveY(0, openCardSpeed);
		btnMedium.transform.DOScale(new Vector3(1.35f, 1.35f, 1), openCardSpeed);
		btnMedium.transform.DOLocalRotate(Vector3.zero, openCardSpeed);
	}

	public void OnClose() {
		Visible = false;
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
		if(isAlreadyEquip) {
			UISkillFormation.Get.DoUnEquipCard(mUICard);
			OnClose();
		} else {
			if(mUICard.Cost <= UISkillFormation.Get.ExtraCostSpace) {
				UISkillFormation.Get.DoEquipCard(mUICard);
				OnClose();
			} else
				UIHint.Get.ShowHint(TextConst.S(558), Color.red);
		}
	}

	public void OnCrafting () {
		if(GameData.DSkillData.ContainsKey(mUICard.skillCard.Skill.ID)) {
			if(GameData.DSkillData[mUICard.skillCard.Skill.ID].EvolutionSkill == 0) {
				UIHint.Get.ShowHint(TextConst.S(7654), Color.red);
			} else {
				UISkillFormation.Get.IsEvolution = true;
				UISkillFormation.Get.DoFinish();
			}
		}
	}

	public void OnUpgrade() {
		if(GameData.DSkillData.ContainsKey(mUICard.skillCard.Skill.ID)) {
			if(mUICard.skillCard.Skill.Lv >= GameData.DSkillData[mUICard.skillCard.Skill.ID].MaxStar ) {
				UIHint.Get.ShowHint(TextConst.S(553), Color.red);
			} else {
				UISkillFormation.Get.IsReinforce = true;
				UISkillFormation.Get.DoFinish();
			}
		}
	}

	public TUICard MyUICard{
		get{return mUICard;}
	}

	public bool IsEquip {
		get {return isAlreadyEquip;}
	}
}

