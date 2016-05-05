﻿using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;
using GameEnum;

public struct ReinEvoTab {

	private GameObject selected;
	private GameObject redPoint;
	private GameObject unUse;

	public void Init (GameObject obj, UIEventListener.VoidDelegate delegateCall) {
		selected = obj.transform.Find("Selected").gameObject;
		redPoint = obj.transform.Find("RedPoint").gameObject;
		unUse = obj.transform.Find("UnUseLabel").gameObject;

		UIEventListener.Get(obj).onClick = delegateCall;
	}

	public bool CheckRedPoint {
		get {return redPoint.activeSelf;}
		set {redPoint.SetActive(value);}
	}

	public bool CheckSelected {
		get {return selected.activeSelf;}
		set {selected.SetActive(value);}
	}

	public bool CheckUnUse {
		get {return unUse.activeSelf;}
		set {unUse.SetActive(value);}
	}
}

public class UISkillReinforce : UIBase {
	private static UISkillReinforce instance = null;
	private const string UIName = "UISkillReinforce";

	//Send Value
	private int targetIndex;
	private int[] removeIndexs;

	private TSkill mOldSkill;
	private TSkill mSkill;

	private GameObject itemCardEquipped;
	private GameObject itemCardReinforce;

	private GameObject[] windows = new GameObject[2];
	private ReinEvoTab[] reinEvoTabs = new ReinEvoTab[2];
	private GameObject tabLock;

	//LeftView
	private TActiveSkillCard skillCard;
	private TExpView expView;
	private TCostView costView;
	private TEnergyView energyView;

	//CenterView
	private MaterialSlot[] materialSlots;
	private TReinforceInfo reinForceInfo;

	//RightView
	private GameObject scrollView;
	private UIScrollView uiScrollView;
	private UIButton buttonReinforce;
	private UILabel labelPrice;
	private UIReinForceGrid reinforceGrid;

	private List<GameObject> queueMaterial;
	private List<GameObject> queueBuff;
	private List<GameObject> queuePassive;
	private List<GameObject> queueActive;
	private List<GameObject> reinforceGoCards;

	private int reinforceMoney;
	private int originalExp;
	private int reinforceExp;

	private int oldCardLv;
	private int newCardLv;
	private int recordGreenExp;
	private bool isRunExp = false;
	private bool isNeedShowLevelUp = false;
	private int addInterVal = 10;

	private Dictionary<string, TPassiveSkillCard> passiveSkillCards;
	//card Right
	private List<TPassiveSkillCard> reinforceCards;
	//item Center
	private Dictionary<string, GameObject> reinforceItems;

	private Animator reinforceAnimator;

	private bool isEquiped = false;

	private UISkillEvolution skillEvolution;

	private bool isInReinforce = false;
	private bool isInEvolution = false;

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
                    RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static UISkillReinforce Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillReinforce;

			return instance;
		}
	}

	void OnDestroy() {
		reinforceCards.Clear();
		reinforceItems.Clear();
		reinforceGoCards.Clear();
		queueMaterial.Clear();
		queueBuff.Clear();
		queuePassive.Clear();
		queueActive.Clear();
		passiveSkillCards.Clear();
	}

	void FixedUpdate () {
		runExp ();
	}

	protected override void InitCom() {
		itemCardEquipped = Resources.Load(UIPrefabPath.ItemCardEquipped) as GameObject;
		itemCardReinforce = Resources.Load(UIPrefabPath.ItemAwardGroup) as GameObject;
		reinforceAnimator = GetComponent<Animator>();

		windows[0] = GameObject.Find(UIName + "/Window");
		windows[1] = GameObject.Find(UIName + "/Window2");

		for (int i=0; i<2; i++) {
			reinEvoTabs[i].Init(GameObject.Find(UIName + "/Window3/Center/Tabs/" + i.ToString()), OnTab);
		}

		tabLock = GameObject.Find(UIName + "/Window3/Center/Tabs/1/Lock");

		skillEvolution = new UISkillEvolution();
		skillEvolution.InitCom(this, UIName);

		//Left View
		skillCard = new TActiveSkillCard();
		skillCard.Init(GameObject.Find(UIName + "/Window/Center/LeftView/ItemSkillCard"));
		expView = new TExpView();
		expView.Init(transform);
		costView = new TCostView();
		costView.Init(transform);
		energyView = new TEnergyView();
		energyView.Init(transform);

		//CenterView
		materialSlots = new MaterialSlot[6];
		for (int i=0; i<materialSlots.Length; i++) {
			materialSlots[i] =  GameObject.Find(UIName + "/Window/Center/CenterView/SlotGroup/MaterialSlot"+i.ToString()).GetComponent<MaterialSlot>();
			SetBtnFun(ref materialSlots[i].RemoveBtn, RemoveChooseItem);
			materialSlots[i].RemoveBtn.name = i.ToString();
		}

		reinForceInfo = new TReinforceInfo();
		reinForceInfo.Init(GameObject.Find(UIName + "/Window/Center/CenterView/ReinforceInfo").transform);

		//RightView
		scrollView = GameObject.Find(UIName + "/Window/Center/RightView/ScrollView/Grid");
		reinforceGrid = GameObject.Find(UIName + "/Window/Center/RightView/ScrollView/Grid").GetComponent<UIReinForceGrid>();
		uiScrollView = GameObject.Find(UIName + "/Window/Center/RightView/ScrollView").GetComponent<UIScrollView>();
		uiScrollView.onDragFinished = ScrollViewDragFinish;
		buttonReinforce = GameObject.Find(UIName + "/Window/Center/RightView/ReinforceBtn").GetComponent<UIButton>();
		labelPrice = GameObject.Find(UIName + "/Window/Center/RightView/ReinforceBtn/PriceLabel").GetComponent<UILabel>();

		reinforceCards = new List<TPassiveSkillCard>();
		reinforceGoCards = new List<GameObject>();
		queueMaterial = new List<GameObject>();
		queueBuff = new List<GameObject>();
		queuePassive = new List<GameObject>();
		queueActive = new List<GameObject>();
		reinforceItems = new Dictionary<string, GameObject>();
		passiveSkillCards = new Dictionary<string, TPassiveSkillCard>();

		SetBtnFun(UIName + "/Window/Center/RightView/ReinforceBtn", OnReinforce);
		SetBtnFun(UIName + "/Window3/BottomLeft/BackBtn", OnClose);
	}

	protected override void InitData() {
		buttonReinforce.normalSprite = "button_gray";
		buttonReinforce.pressedSprite = "button_gray2";
	}

	public void SetBtn (string path, EventDelegate.Callback callback) {
		SetBtnFun(path, callback);
	}

	public void OnShowDiamond () {
		if(IsCanClick)
			UIRecharge.Get.ShowView(ERechargeType.Diamond.GetHashCode(), RefreshPriceUI);
	}

	public void OnCoin () {
		if(IsCanClick)
			UIRecharge.Get.ShowView(ERechargeType.Coin.GetHashCode(), RefreshPriceUI);
	}
	/// <summary>
	/// Show the specified skill, index, isAlreadyEquip and showType.
	/// 0: Reinforce 1: Evolution
	/// </summary>
	/// <param name="skill">Skill.</param>
	/// <param name="index">Index.</param>
	/// <param name="isAlreadyEquip">If set to <c>true</c> is already equip.</param>
	/// <param name="showType">Show type.</param>
	public void Show (TSkill skill, int index, bool isAlreadyEquip, int showType) {
		Visible = true;

		UIMainLobby.Get.Hide(false);
        UIResource.Get.Show();

        mSkill = skill;
		mOldSkill = mSkill;
		RefreshView(skill);
		initRightCards ();
		targetIndex = index;
		isEquiped = isAlreadyEquip;
		skillEvolution.ShowView(index, skill, isAlreadyEquip);
		showWindows(showType);
	}

	private void showWindows (int showType) {
		for(int i=0; i<windows.Length; i++) {
			windows[i].SetActive((showType == i));
			reinEvoTabs[i].CheckSelected = (showType == i);
		}
	}

	private void refreshTabRed () {
		//Reinforce
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			//Reinforce
			reinEvoTabs[0].CheckRedPoint = false;
			reinEvoTabs[0].CheckUnUse = (mSkill.Lv == GameData.DSkillData[mSkill.ID].MaxStar);
			//Evolution
			reinEvoTabs[1].CheckRedPoint = ((GameData.Team.IsEnoughMaterial(mSkill)) && GameData.DSkillData[mSkill.ID].EvolutionSkill != 0 && (mSkill.Lv == GameData.DSkillData[mSkill.ID].MaxStar) && 
											 GameData.IsOpenUIEnableByPlayer(EOpenID.SkillEvolution));
			reinEvoTabs[1].CheckUnUse = (GameData.DSkillData[mSkill.ID].EvolutionSkill == 0);

			tabLock.SetActive(!GameData.IsOpenUIEnableByPlayer(EOpenID.SkillEvolution));
		}
	}

	public void OnTab (GameObject go) {
		if(IsCanClick) {
			int result = -1;
			if(int.TryParse(go.name, out result)) {
				if(result == 0) {
					if(mSkill.Lv >= GameData.DSkillData[mSkill.ID].MaxStar)
						UIHint.Get.ShowHint(TextConst.S(553), Color.red);
					else {
						if(GameData.IsOpenUIEnableByPlayer(EOpenID.SkillReinforce))
							showWindows(result);
						else
							UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.SkillReinforce)), LimitTable.Ins.GetLv(EOpenID.SkillReinforce)) , Color.red);			
					}
					
				} else if(result == 1) {
					if(GameData.DSkillData[mSkill.ID].EvolutionSkill == 0) 
						UIHint.Get.ShowHint(TextConst.S(7654), Color.red);
					else {
						if(GameData.IsOpenUIEnableByPlayer(EOpenID.SkillEvolution))
							showWindows(result);
						else
							UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.SkillEvolution)),LimitTable.Ins.GetLv(EOpenID.SkillEvolution)) , Color.red);
					}
				}
			}
		}
	}

	private void initRightCards () {
		if(GameData.Team.SkillCards != null && GameData.Team.SkillCards.Length > 0) {
			int index = 0;
			for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
				if(isCanReinForce(GameData.Team.SkillCards[i])) {
					TPassiveSkillCard obj = null;
					obj = addItem(i, index, GameData.Team.SkillCards[i]);
					if(obj != null && !passiveSkillCards.ContainsKey(obj.Name) && GameData.DSkillData.ContainsKey(GameData.Team.SkillCards[i].ID)) {
						passiveSkillCards.Add(obj.Name, obj);
						if(GameData.DSkillData[GameData.Team.SkillCards[i].ID].Kind == 1) 
							queueMaterial.Add(obj.item);
						else if(GameData.DSkillData[GameData.Team.SkillCards[i].ID].Kind >= 200) 
							queueBuff.Add(obj.item);
						else if(!GameFunction.IsActiveSkill(GameData.Team.SkillCards[i].ID) && GameData.DSkillData[GameData.Team.SkillCards[i].ID].Kind < 200)
							queuePassive.Add(obj.item);
						else if(GameFunction.IsActiveSkill(GameData.Team.SkillCards[i].ID) && GameData.DSkillData[GameData.Team.SkillCards[i].ID].Kind < 200)
							queueActive.Add(obj.item);
						index ++ ;
					} 
				}
			}
		}
		sortCard(ref reinforceGoCards);
		uiScrollView.ResetPosition();
		reinforceGrid.init();
		reinforceGrid.mChildren = reinforceGoCards;
		ScrollViewDragFinish ();
	}

	private string getCardID (string name) {
		string[] names = name.Split("_"[0]);
		return names[0];
	}

	private void bubbleSort (ref List<GameObject> cards) {
		int a = 0;
		int b = 0;
		for(int i=0; i<cards.Count; i++) {
			for (int j=i+1; j<cards.Count; j++){
				if(int.TryParse(getCardID(cards[i].name), out a) && int.TryParse(getCardID(cards[j].name), out b)) {
					if (a >= b){
						GameObject temp = cards[i];
						cards[i] = cards[j];
						cards[j] = temp;
					}
				}
			}
		}
	}
	//我愛籃球黑幫 > Buff > 被動技 > 主動技
	private void sortCard (ref List<GameObject> goCards) {
		bubbleSort(ref queueMaterial);
		bubbleSort(ref queueBuff);
		bubbleSort(ref queuePassive);
		bubbleSort(ref queueActive);

		for(int i=0; i<queueMaterial.Count; i++)
			goCards.Add(queueMaterial[i]);
		for(int i=0; i<queueBuff.Count; i++)
			goCards.Add(queueBuff[i]);
		for(int i=0; i<queuePassive.Count; i++)
			goCards.Add(queuePassive[i]);
		for(int i=0; i<queueActive.Count; i++)
			goCards.Add(queueActive[i]);

		for(int i=0; i<goCards.Count; i++)
			goCards[i].transform.localPosition = new Vector3(0, 200 - 80 * i, 0);
	}

	public void ScrollViewDragFinish () {
		reinforceGrid.WrapContent(false);
	}

	private TPassiveSkillCard addItem (int skillCardIndex, int positionIndex, TSkill skill) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = scrollView.transform;
		obj.transform.name =  skill.ID.ToString() + "_" + skill.SN.ToString() + "_" + skill.Lv.ToString();
        LayerMgr.Get.SetLayerAllChildren(obj, ELayer.TopUI);

		TPassiveSkillCard passiveSkillCard = new TPassiveSkillCard();
		passiveSkillCard.InitReinforce(obj, skillCardIndex);
		passiveSkillCard.UpdateViewReinforce(skill);

//		UIEventListener.Get(obj).onClick = ChooseItem;
		UIButton btn = obj.GetComponent<UIButton>();
		SetBtnFun(ref btn, ChooseItem);

		return passiveSkillCard;
	}

	private GameObject addReinforceCard (Transform parent, TSkill skill) {
		GameObject obj = Instantiate(itemCardReinforce, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.name =  skill.ID.ToString() + "_" + skill.SN.ToString() + "_" + skill.Lv.ToString();
		obj.transform.parent = parent;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;
        LayerMgr.Get.SetLayerAllChildren(obj, ELayer.TopUI);

		obj.GetComponent<ItemAwardGroup>().ShowSkill(skill);
		return obj;
	}

	private bool isUpgradeMaterial (int skillID) {
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) 
		if(GameData.DSkillData[mSkill.ID].UpgradeMaterial != null && GameData.DSkillData[mSkill.ID].UpgradeMaterial.Length > 0)
			for(int i=0; i<GameData.DSkillData[mSkill.ID].UpgradeMaterial.Length; i++)
				if(GameData.DSkillData[mSkill.ID].UpgradeMaterial[i] == skillID)
						return true;
		return false;
	}
		
	private bool isCanReinForce(TSkill skill) {
		if(isUpgradeMaterial(skill.ID)) {
			if(GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 0) {
				for (int i=0; i<GameData.Team.PlayerBank.Length; i++) {
					if(GameData.Team.PlayerBank[i].SkillCardPages != null && GameData.Team.PlayerBank[i].SkillCardPages.Length > 0) {
						for (int j=0; j<GameData.Team.PlayerBank[i].SkillCardPages.Length; j++) {
							int[] SNs = GameData.Team.PlayerBank[i].SkillCardPages[j].SNs;
							if (SNs.Length > 0) {
								for (int k=0; k<SNs.Length; k++)
									if (SNs[k] == skill.SN)
										return false;
							}
						}
					}
				}
			}
			
			if(GameData.Team.Player.SkillCardPages != null && GameData.Team.Player.SkillCardPages.Length > 0) {
				for (int i=0; i<GameData.Team.Player.SkillCardPages.Length; i++) {
					int[] SNs = GameData.Team.Player.SkillCardPages[i].SNs;
					if (SNs.Length > 0) {
						for (int k=0; k<SNs.Length; k++)
							if (SNs[k] == skill.SN)
								return false;
					}
				}
			}
			
			for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) 
				if(GameData.Team.Player.SkillCards[i].SN == skill.SN)
					return false;
			
			if(mSkill.SN == skill.SN)
				return false;

			return true;
		} else 
			return false;
	}

	private int checkLvUp(int addExp) {
		int tempLv = mSkill.Lv;
		int tempExp = mSkill.Exp;
		if(addExp > 0 && GameData.DSkillData.ContainsKey(mSkill.ID)) {

			tempExp += addExp;

			// 更新等級.
			int lvUpExp = GameData.DSkillData[mSkill.ID].GetUpgradeExp(mSkill.Lv);
			while(lvUpExp > 0 && tempExp >= lvUpExp)
			{
				tempLv++;
				tempExp -= lvUpExp;

				lvUpExp = GameData.DSkillData[mSkill.ID].GetUpgradeExp(tempLv);
			}

			if(tempLv >= GameData.DSkillData[mSkill.ID].MaxStar) {
				tempLv = GameData.DSkillData[mSkill.ID].MaxStar;
			}
			return tempLv;
		}
		return tempLv;
	}

	private void addUpgradeView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			reinforceExp += GameData.DSkillData[skill.ID].ExpInlay(skill.Lv);
			expView.SetUpgradeView(mSkill.ID, mSkill.Lv, originalExp, reinforceExp);
			int newLv = checkLvUp(reinforceExp);
			if(newLv > newCardLv)
				skillCard.ShowStarForRein(mSkill.Lv, (newLv - mSkill.Lv));
			
			if((newLv - mSkill.Lv) > 0) {
				newCardLv = newLv;

				costView.UpgradeView(mSkill, newLv);
				energyView.UpgradeView(mSkill, newLv);
				reinForceInfo.UpgradeView(mSkill, newLv);
			}
		}
	}

	private void minusUpgradeView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			reinforceExp -= GameData.DSkillData[skill.ID].ExpInlay(skill.Lv);

			int newLv = checkLvUp(reinforceExp);
			newCardLv = newLv;
			expView.SetUpgradeView(mSkill.ID, mSkill.Lv, originalExp, reinforceExp);
			skillCard.ShowStarForRein(mSkill.Lv, (newLv - mSkill.Lv));

			costView.UpgradeView(mSkill, newLv);
			energyView.UpgradeView(mSkill, newLv);
			reinForceInfo.UpgradeView(mSkill, newLv);
		}
	}

	private void addUpgradeMoney () {
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			reinforceMoney += GameData.DSkillData[mSkill.ID].GetUpgradeMoney(mSkill.Lv);
			RefreshPriceUI ();
		}
	}

	private void minusUpgradeMoney () {
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			reinforceMoney -= GameData.DSkillData[mSkill.ID].GetUpgradeMoney(mSkill.Lv);
			RefreshPriceUI ();
		}
	}

	public void RefreshPriceUI () {
		labelPrice.color = GameData.CoinEnoughTextColor(CheckMoney(reinforceMoney), 1);
		skillEvolution.RefreshPriceUI();
		labelPrice.text = reinforceMoney.ToString();
		if(reinforceMoney > 0) {
			buttonReinforce.normalSprite = "button_orange1";
			buttonReinforce.pressedSprite = "button_orange2";
		} else {
			buttonReinforce.normalSprite = "button_gray";
			buttonReinforce.pressedSprite = "button_gray2";
		}
	}

	private void hideSlotRemoveBtn () {
		for (int i=0; i<materialSlots.Length; i++)
			materialSlots[i].RemoveBtn.gameObject.SetActive(false);
	}

	public void RemoveChooseItem () {
		int index = -1;
		if(int.TryParse(UIButton.current.name, out index)) {
//			ChooseItem(reinforceCards[index].item);
			ChooseItem(true);
		}
	}

	public void ChooseItem (bool isRemove = false) {
//		string name = go.name;
		int result = 0;
		string name = "";
		if(isRemove) {
			if(int.TryParse(UIButton.current.name, out result)) 
				name = reinforceCards[result].item.name;
		} else {
			name = UIButton.current.name;
		} 

		if(!string.IsNullOrEmpty(name)) {
			if(!isRunExp && IsCanClick) {
				if(reinforceCards.Count < 6) {
					if(reinforceItems.ContainsKey(name)) {
						Destroy(reinforceItems[name]);
						passiveSkillCards[name].ChooseReinforce(false);
						reinforceItems.Remove(name);
						reinforceCards.Remove(passiveSkillCards[name]);
						minusUpgradeMoney();
						minusUpgradeView(passiveSkillCards[name].Skill);
					} else {
						materialSlots[reinforceItems.Count].ShowInput();
						reinforceItems.Add(name, addReinforceCard(materialSlots[reinforceItems.Count].View, passiveSkillCards[name].Skill));
						reinforceCards.Add(passiveSkillCards[name]);
						passiveSkillCards[name].ChooseReinforce(true, reinforceCards.Count);
						addUpgradeMoney();
						addUpgradeView(passiveSkillCards[name].Skill);
						AudioMgr.Get.PlaySound(SoundType.SD_Compose);
					}
				} else {
					if(reinforceItems.ContainsKey(name)) {
						Destroy(reinforceItems[name]);
						passiveSkillCards[name].ChooseReinforce(false);
						reinforceItems.Remove(name);
						reinforceCards.Remove(passiveSkillCards[name]);
						minusUpgradeMoney();
						minusUpgradeView(passiveSkillCards[name].Skill);
					} 
				}
				RefreshSlot ();
			}
		}
	}

	public void RefreshView (TSkill skill) {
		//Delete list

		if(reinforceGrid.mTrans != null){
			for(int i=0; i<reinforceGrid.mChildren.Count; i++)
				Destroy(reinforceGrid.mChildren[i]);
		}
		
		if(reinforceItems.Count > 0) {
			foreach(KeyValuePair<string, GameObject> obj in reinforceItems) {
				Destroy(obj.Value);
			}
		}

		for(int i=0 ; i < materialSlots.Length; i++) {
			if(i < reinforceCards.Count) {
				materialSlots[i].HideEatFX();
				materialSlots[i].HideInput();
			}
		}

		mOldSkill = mSkill;
		isRunExp = false;
		isNeedShowLevelUp = false;
		addInterVal = 5;
		skillCard.HideAllGetStar();
		skillCard.HideAllPreviewStar();
		oldCardLv = skill.Lv;
		newCardLv = skill.Lv;
		hideSlotRemoveBtn ();

		reinforceCards.Clear();
		reinforceItems.Clear();
		reinforceGoCards.Clear();
		queueMaterial.Clear();
		queueBuff.Clear();
		queuePassive.Clear();
		queueActive.Clear();
		passiveSkillCards.Clear();
		reinforceMoney = 0;
		labelPrice.text = reinforceMoney.ToString();
		buttonReinforce.normalSprite = "button_gray";
		buttonReinforce.pressedSprite = "button_gray2";

		originalExp = skill.Exp;
		reinforceExp = 0;
		skillCard.UpdateViewFormation(skill, false);
		expView.UpdateView(skill);
		costView.UpdateView(skill);
		energyView.UpdateView(skill);
		reinForceInfo.UpdateView(skill);
		RefreshSlot ();

		skillEvolution.RefreshReinForce(skill, targetIndex);
		refreshTabRed();
	}

	public void RefreshSlot () {
		hideSlotRemoveBtn ();
		if(reinforceCards.Count > 0) {
			for(int i=0; i<reinforceCards.Count; i++) {
				reinforceCards[i].ChooseReinforce(true, i + 1);
				if(i >= 0 && i < materialSlots.Length) {
					materialSlots[i].RemoveBtn.gameObject.SetActive(true);
					reinforceItems[reinforceCards[i].Name].transform.parent = materialSlots[i].View;
					reinforceItems[reinforceCards[i].Name].transform.localPosition = Vector3.zero;
				}
			}
		}
		for(int i=0 ; i < materialSlots.Length; i++) {
			if(i >= reinforceCards.Count) {
				materialSlots[i].HideInput();
			}
		}
	}

	public void OnReinforce () {
		if(!isRunExp && IsCanClick) {
			if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
				if(mSkill.Lv >= GameData.DSkillData[mSkill.ID].MaxStar) {
					UIHint.Get.ShowHint(TextConst.S(556), Color.red);
				} else {
					if(reinforceCards.Count > 0) {
						if(CheckMoney(reinforceMoney, true, "", null, RefreshPriceUI)) {
							removeIndexs = new int[reinforceCards.Count];
							for (int i=0; i<removeIndexs.Length; i++) {
								removeIndexs[i] = reinforceCards[i].CardIndex;
							}
							
							//Bobble Sort
							if(removeIndexs.Length > 1) {
								for(int i=0; i<removeIndexs.Length; i++) {
									for (int j=i+1; j<removeIndexs.Length; j++){
										if (removeIndexs[i] >= removeIndexs[j]){
											int temp = removeIndexs[i];
											removeIndexs[i] = removeIndexs[j];
											removeIndexs[j] = temp;
										}
									}
								}
							}
							
							if(removeIndexs.Length > 0) {
								if(isEquiped)
									SendReinforce(0);
								else
									SendReinforce(1);
							}
						} else {
							AudioMgr.Get.PlaySound (SoundType.SD_Prohibit);
                            UIHint.Get.ShowHint(TextConst.S(510), Color.black);
						}
					} else 
                        UIHint.Get.ShowHint(TextConst.S(557), Color.black);
				}
			}
		}
	}

	public void OnClose () {
		if(IsCanClick) {
			Visible = false;
			UISkillFormation.Get.RefreshFromReinEvo(mSkill.SN);
			UIMainLobby.Get.Hide(false);
            UIResource.Get.Hide();
		}
	} 

	public void OnSearch () {
		if(IsCanClick) {
			Visible = false;
			UISkillFormation.Get.RefreshFromReinEvo(mSkill.SN);
		}
	}

	private void runExp () {
		if(isRunExp) {
			if(GameData.DSkillData.ContainsKey(mSkill.ID) && newCardLv >= oldCardLv && reinforceExp > 0) {
				if( oldCardLv <= GameData.DSkillData[mSkill.ID].MaxStar) {
					expView.SetProgressView(mSkill.ID, 
						oldCardLv, 
						originalExp, 
						recordGreenExp,
						reinforceExp);
					
					if(reinforceExp > 500) 
						addInterVal = 30;
					else if(reinforceExp > 100)
						addInterVal = 10;
					else 
						addInterVal = 5;
					
					if(reinforceExp >= addInterVal)
						reinforceExp -= addInterVal;
					else 
						addInterVal = reinforceExp;
					
					originalExp += addInterVal;
					
					if(originalExp > GameData.DSkillData[mSkill.ID].GetUpgradeExp(oldCardLv)) {
						originalExp -= GameData.DSkillData[mSkill.ID].GetUpgradeExp(oldCardLv);
						oldCardLv ++;
						skillCard.ShowGetStar(oldCardLv);
						recordGreenExp = reinforceExp;
						expView.ShowFull();
						AudioMgr.Get.PlaySound(SoundType.SD_Warning0);
					}
					
					if(reinforceExp <= 0) {
						expView.SetProgressView(mSkill.ID, 
							oldCardLv, 
							recordGreenExp, 
							recordGreenExp,
							GameData.DSkillData[mSkill.ID].GetUpgradeExp(oldCardLv));
						stopRunExp ();
					}
				} else {
					skillCard.ShowGetStar(4); // 5 - 1
					expView.SetTopProgressView();
					stopRunExp ();
				}
			} else  {
				skillCard.ShowGetStar(newCardLv);
				recordGreenExp = reinforceExp;
				expView.ShowFull();
				stopRunExp ();
			}
		}
	}

	private void awakeRunExp () {
		isInReinforce = true;
		recordGreenExp = reinforceExp + originalExp;
		Invoke("delayRunExp", 2);
		reinforceAnimator.SetTrigger("Go");
		skillCard.HideAllPreviewStar();
		for(int i=0 ; i < materialSlots.Length; i++) 
			if(i < reinforceCards.Count)
				materialSlots[i].ShowEatFX();
		
	}

	private void stopRunExp () {
		reinforceExp = 0;
		isRunExp = false;
		finishExp ();
	}

	private void delayRunExp () {
		foreach(KeyValuePair<string, GameObject> obj in reinforceItems) {
			Destroy(obj.Value);
		}
		isRunExp = true;
		isNeedShowLevelUp = (oldCardLv != newCardLv);
	}

	private void finishExp () {
		if(isNeedShowLevelUp) 
			UILevelUp.Get.ShowSkill(mOldSkill, mSkill);
		
		RefreshView(mSkill);
		initRightCards ();
		isInReinforce = false;
		skillEvolution.Refresh(mSkill);
	}

	/// <summary>
	/// Sends the reinforce.
	/// Kind 0:isEquiped(Player.SkillCard) 1:notEquiped(Team.SkillCard)
	/// </summary>
	public void SendReinforce(int kind) {
		WWWForm form = new WWWForm();
		form.AddField("TargetIndex", targetIndex);
		form.AddField("RemoveIndexs", JsonConvert.SerializeObject(removeIndexs));
		form.AddField("Kind", kind);
		SendHttp.Get.Command(URLConst.ReinforceSkillcard, waitReinforce, form);
	}

	private void waitReinforce(bool ok, WWW www) {
		if (ok) {
            TReinforceCallBack result = JsonConvertWrapper.DeserializeObject <TReinforceCallBack>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerSkillCards;
			GameData.Team.InitSkillCardCount();
			GameData.Team.LifetimeRecord = result.LifetimeRecord;
			SetMoney(result.Money);
			UIMainLobby.Get.UpdateUI();

			Statistic.Ins.LogEvent(110, mSkill.ID.ToString(), reinforceMoney);

			if(UISkillFormation.Visible)
				UISkillFormation.Get.RefreshAddCard();

			if(isEquiped)
				mSkill = findNewSkillFromPlayer(mSkill);
			else
				mSkill = findNewSkillFromTeam(mSkill);
			awakeRunExp();

		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

	public void SendEvolution(int kind) {
		isInEvolution = true;
		WWWForm form = new WWWForm();
		form.AddField("RemoveIndex", skillEvolution.SkillIndex);//要進化的對象
		form.AddField("Kind", kind);
		form.AddField("MaterialIndexs", JsonConvert.SerializeObject(skillEvolution.MaterialIndexs));//進化材料
		form.AddField("SkillIndexs", JsonConvert.SerializeObject(skillEvolution.SkillIndexs));//進化卡牌
		SendHttp.Get.Command(URLConst.EvolutionSkillcard, waitEvolutionSkillcard, form);
	}

	private void waitEvolutionSkillcard(bool ok, WWW www) {
		if (ok) {
            TEvolution result = JsonConvertWrapper.DeserializeObject <TEvolution>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerSkillCards;
			GameData.Team.MaterialItems = result.MaterialItems;
			GameData.Team.LifetimeRecord = result.LifetimeRecord;
			GameData.Team.GotItemCount = result.GotItemCount;
			GameData.Team.InitSkillCardCount();
			SetMoney(result.Money);
			UIMainLobby.Get.UpdateUI();

			Statistic.Ins.LogEvent(111, skillEvolution.NextSkill.ID.ToString(), skillEvolution.EvolutionPrice);

			if(UISkillFormation.Visible)
				UISkillFormation.Get.RefreshAddCard();
			
			startEvolutionAni ();

		} else {
			Debug.LogError("text:"+www.text);
		} 
	}


	private TSkill findNewSkillFromTeam(TSkill skill) {
		for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
			if(GameData.Team.SkillCards[i].SN == skill.SN){
				targetIndex = i;
				return GameData.Team.SkillCards[i];
			}
		}

		return skill;
	}

	private TSkill findNewSkillFromPlayer(TSkill skill) {
		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
			if(GameData.Team.Player.SkillCards[i].SN == skill.SN){
				targetIndex = i;
				return GameData.Team.Player.SkillCards[i];
			}
		}

		return skill;
	}

	private void startEvolutionAni () {
		reinforceAnimator.SetTrigger("Evolution");
		Invoke("finishEvolution", 2.6f);
	}

	private void finishEvolution () {
		isInEvolution = false;

		UILevelUp.Get.ShowSkill(skillEvolution.MySkill, skillEvolution.NextSkill);

		if(isEquiped)
			mSkill = findNewSkillFromPlayer(mSkill);
		else
			mSkill = findNewSkillFromTeam(mSkill);

		skillEvolution.Refresh (mSkill);
		RefreshView(mSkill);
		initRightCards ();
	}

	public bool IsCanClick {
		get {return !(isInReinforce || isInEvolution);}
	}
}

