using DG.Tweening;
using GameStruct;
using UnityEngine;
using GameEnum;

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
	private UILabel labelSkillCardName;
	private SkillCardStar[] skillStars;
	private UISprite spriteSkillKind;
	private UISprite spriteSkillKindBg;
	private UILabel labelSkillKind;
	private UILabel labelSkillInfoKind4;

	private UISprite goSuitCard;
	private UISprite suitCardFinish;
	private UISprite goSuitItem;
	private UISprite suitItemStarBg;
	private GameObject[] suitItemFinish = new GameObject[7];

	//TopRight
	private UILabel labelEquip;
	private GameObject btnEquip;
	private GameObject btnCrafting;
	private GameObject btnUpgrade;

	private GameObject goEquipUnuse;
	private GameObject goCraftUnuse;
	private GameObject goUpgradeUnuse;

	private GameObject goEquipRedPoint;
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

		goEquipRedPoint = GameObject.Find (UIName + "/Center/TopRight/EquipBtn/RedPoint");
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
		labelSkillKind = GameObject.Find (UIName + "/Center/Left/SkillInfo/SkillKind").GetComponent<UILabel>();
		
		//Buff Ability
		labelSubhead = GameObject.Find (UIName + "/Center/Left/BuffAbility/LabelSubhead").GetComponent<UILabel>();
		buffViews = GetComponentsInChildren<BuffView>();
		
		//Explain
		labelSkillExplain = GameObject.Find (UIName + "/Center/Left/Explain/SkillArea/SkillExplain").GetComponent<UILabel>();
		
		//Card
		spriteSkillCard = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillCard").GetComponent<UISprite>();
		textureSkillPic = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillPic").GetComponent<UITexture>();
		labelSkillCardName = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillName").GetComponent<UILabel>();
		skillStars = new  SkillCardStar[5];
		for(int i=0; i<skillStars.Length; i++) 
			skillStars[i] = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillStar/StarBG" + i.ToString()).GetComponent<SkillCardStar>();
		spriteSkillKind = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillKind").GetComponent<UISprite>();
		spriteSkillKindBg  = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SkillKind/KindBg").GetComponent<UISprite>();

		goSuitCard = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SuitCard").GetComponent<UISprite>();
		suitCardFinish = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SuitCard/SuitFinish").GetComponent<UISprite>();
		goSuitItem = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SuitItem").GetComponent<UISprite>();
		suitItemStarBg = GameObject.Find (UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SuitItem/ItemBottom").GetComponent<UISprite>();
		for (int i=0; i<suitItemFinish.Length; i++)
			suitItemFinish[i] = GameObject.Find(UIName + "/Center/Left/BtnMediumCard/ItemSkillCard/SuitItem/Light/" + i.ToString()).gameObject;

		UIEventListener.Get(goSuitCard.gameObject).onClick = OnSuitCard;
		UIEventListener.Get(goSuitItem.gameObject).onClick = OnSuitItem;

		SetBtnFun(UIName + "/Center/BG", OnClose);
		SetBtnFun(UIName + "/Center/BottomRight/BackBtn", OnClose);
		SetBtnFun(UIName + "/Center/TopRight/EquipBtn", OnEquip);
		SetBtnFun(UIName + "/Center/TopRight/CraftingBtn", OnCrafting);
		SetBtnFun(UIName + "/Center/TopRight/UpgradeBtn", OnUpgrade);
		SetBtnFun(UIName + "/Center/Left/BtnMediumCard/ItemSkillCard", OpenCard);
	}

	public void OnSuitCard (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			UISkillFormation.Get.ClickTab(1);
			UISkillFormation.Get.SuitCard.MoveToID(result);
			Visible = false;
		}
	}

	public void OnSuitItem (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			UISuitAvatar.Get.ShowView(result);
			Visible = false;
		}
	}
	
	public void ShowFromSkill (TUICard uicard, bool isEquip, bool isMaskOpen) {
		Visible = true;
		UIMainLobby.Get.HideAll(false);
		isAlreadyEquip = isEquip;
		btnEquip.SetActive(true);
		btnUpgrade.SetActive(LimitTable.Ins.HasByOpenID(EOpenID.SkillReinforce) && GameData.Team.Player.Lv >= LimitTable.Ins.GetVisibleLv(EOpenID.SkillReinforce));
		btnCrafting.SetActive(LimitTable.Ins.HasByOpenID(EOpenID.SkillEvolution) && GameData.Team.Player.Lv >= LimitTable.Ins.GetVisibleLv(EOpenID.SkillEvolution));

		if(isEquip)
			labelEquip.text = TextConst.S(7215);
		else
			labelEquip.text = TextConst.S(7214);

		if(GameData.DSkillData.ContainsKey(uicard.skillCard.Skill.ID)) {
			TSkillData skillData = GameData.DSkillData[uicard.skillCard.Skill.ID];
			RefreshUICard(uicard);
			goSuitCard.gameObject.SetActive((GameData.DSuitCard.ContainsKey(GameData.DSkillData[uicard.skillCard.Skill.ID].SuitCard)));
			goSuitItem.gameObject.SetActive((GameData.DSuitCard.ContainsKey(GameData.DSkillData[uicard.skillCard.Skill.ID].Suititem)));
		}
	}

	public void ShowFromNewCard (TSkill skill) {
		Visible = true;
		btnEquip.SetActive(false);
		btnUpgrade.SetActive(false);
		btnCrafting.SetActive(false);
		Refresh(skill, -1);

		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			goSuitCard.gameObject.SetActive((GameData.DSuitCard.ContainsKey(GameData.DSkillData[skill.ID].SuitCard)));
			goSuitItem.gameObject.SetActive((GameData.DSuitCard.ContainsKey(GameData.DSkillData[skill.ID].Suititem)));
		}
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
				goEquipUnuse.SetActive((mUICard.Cost > UISkillFormation.Get.ExtraCostSpace) && UISkillFormation.Get.CheckCardnoInstallIgnoreSelf(mUICard.Card.name));
				goCraftUnuse.SetActive((skillData.EvolutionSkill == 0));
				goUpgradeUnuse.SetActive((skill.Lv == skillData.MaxStar));

				goEquipRedPoint.SetActive(!isAlreadyEquip && (mUICard.Cost <= UISkillFormation.Get.ExtraCostSpace) && UISkillFormation.Get.CheckCardnoInstallIgnoreSelf(mUICard.Card.name));
				goCraftRedPoint.SetActive((GameData.Team.IsEnoughMaterial(skill)) && (skillData.EvolutionSkill != 0) && (skill.Lv == skillData.MaxStar) && LimitTable.Ins.HasByOpenID(EOpenID.SkillEvolution) && GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SkillEvolution));
				goUpgradeRedPoint.SetActive((skill.Lv < skillData.MaxStar) && UISkillFormation.Get.CheckCardnoInstallIgnoreSelf(mUICard.Card.name)&& LimitTable.Ins.HasByOpenID(EOpenID.SkillReinforce) && GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SkillReinforce));
			}

			//MediumCard
			spriteSkillCard.spriteName =  GameFunction.CardLevelName(skill.ID);
			textureSkillPic.mainTexture = GameData.CardTexture(skill.ID);
			labelSkillCardName.text = GameData.DSkillData[skill.ID].Name;
			GameFunction.ShowStar(ref skillStars, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);
			if(GameFunction.IsActiveSkill(skill.ID)) {
				spriteSkillKind.spriteName = "ActiveIcon";
				labelSkillInfoKind4.text = TextConst.S(7207);
				labelSkillKind.text = TextConst.S(7002);
			} else {
				spriteSkillKind.spriteName = "PasstiveIcon";
				labelSkillInfoKind4.text = TextConst.S(7206);
				labelSkillKind.text = TextConst.S(7003);
			}
			spriteSkillKindBg.spriteName = "APIcon" + GameData.DSkillData[skill.ID].Quality.ToString();

			if(GameData.DSkillData[skill.ID].SuitCard > 0) {
				goSuitCard.gameObject.SetActive(true);
				goSuitCard.spriteName = GameFunction.CardLevelBallName(skill.ID);
				goSuitCard.gameObject.name = GameData.DSkillData[skill.ID].SuitCard.ToString();
				suitCardFinish.spriteName = GameFunction.CardSuitLightName(GameData.Team.SuitCardCompleteCount(GameData.DSkillData[skill.ID].SuitCard));
			} else
				goSuitCard.gameObject.SetActive(false);

			if(GameData.DSkillData[skill.ID].Suititem > 0 && GameData.DSuitItem.ContainsKey(GameData.DSkillData[skill.ID].Suititem)) {
				goSuitItem.gameObject.SetActive(true);
				goSuitItem.spriteName = GameFunction.CardLevelBallName(skill.ID);
				goSuitItem.gameObject.name = GameData.DSkillData[skill.ID].Suititem.ToString();
				suitItemStarBg.spriteName = GameFunction.CardSuitItemStarBg(GameData.DSuitItem[GameData.DSkillData[skill.ID].Suititem].Items.Length);
				GameFunction.CardSuitItemStar(ref suitItemFinish, GameData.DSuitItem[GameData.DSkillData[skill.ID].Suititem].Items.Length, GameData.Team.SuitItemCompleteCount(GameData.DSkillData[skill.ID].Suititem));
			} else 
				goSuitItem.gameObject.SetActive(false);

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
				labelSkillDemandValue.text = skillData.MaxAnger(skill.Lv).ToString();
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

		goSuitCard.gameObject.SetActive(false);
		goSuitItem.gameObject.SetActive(false);
	}

	private void closeCardTurn (){
		isOpen = false;
		btnMedium.transform.DOKill();
		btnMediumTop.SetActive(false);
		btnMedium.transform.DOLocalMoveX(325, openCardSpeed);
		btnMedium.transform.DOLocalMoveY(0, openCardSpeed);
		btnMedium.transform.DOScale(new Vector3(1.35f, 1.35f, 1), openCardSpeed);
		btnMedium.transform.DOLocalRotate(Vector3.zero, openCardSpeed);

		if(GameData.DSkillData.ContainsKey(mUICard.skillCard.Skill.ID)) {
			goSuitCard.gameObject.SetActive((GameData.DSuitCard.ContainsKey(GameData.DSkillData[mUICard.skillCard.Skill.ID].SuitCard)));
			goSuitItem.gameObject.SetActive((GameData.DSuitCard.ContainsKey(GameData.DSkillData[mUICard.skillCard.Skill.ID].Suititem)));
		}
	}

	public void OnClose() {
		Visible = false;
		if(UIGameResult.Visible && UIGameResult.Get.IsShowFirstCard) {
			UIGameResult.Get.ShowBonusItem();
		}

		UIMainLobby.Get.Hide(3, false);
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
				if(LimitTable.Ins.HasByOpenID(EOpenID.SkillEvolution) && GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SkillEvolution)) {
					UISkillFormation.Get.IsEvolution = true;
					UISkillFormation.Get.DoFinish();
				} else
					UIHint.Get.ShowHint(string.Format(TextConst.S(512),LimitTable.Ins.GetLv(EOpenID.SkillEvolution)) , Color.red);
			}
		}
	}

	public void OnUpgrade() {
		if(GameData.DSkillData.ContainsKey(mUICard.skillCard.Skill.ID)) {
			if(mUICard.skillCard.Skill.Lv >= GameData.DSkillData[mUICard.skillCard.Skill.ID].MaxStar ) {
				UIHint.Get.ShowHint(TextConst.S(553), Color.red);
			} else {
				if(LimitTable.Ins.HasByOpenID(EOpenID.SkillReinforce) && GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SkillReinforce)) {
					UISkillFormation.Get.IsReinforce = true;
					UISkillFormation.Get.DoFinish();
				} else 
					UIHint.Get.ShowHint(string.Format(TextConst.S(512),LimitTable.Ins.GetLv(EOpenID.SkillReinforce)) , Color.red);
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

