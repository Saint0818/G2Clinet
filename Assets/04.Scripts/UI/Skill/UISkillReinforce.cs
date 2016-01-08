using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

//LeftView
public struct TExpView {
	public GameObject ExpView;
	public UISlider ProgressBar;
	public UISlider ProgressBar2;
	public UILabel NextLevelLabel;
	public UILabel GetLevelLabel;

	private int currentExp;
	private int maxExp;

	public void Init (Transform t) {
		ExpView = t.FindChild("Window/Center/LeftView/EXPView").gameObject;
		ProgressBar = ExpView.transform.FindChild("ProgressBar").GetComponent<UISlider>();
		ProgressBar2 = ExpView.transform.FindChild("ProgressBar2").GetComponent<UISlider>();
		NextLevelLabel = ExpView.transform.FindChild("NextLevelLabel").GetComponent<UILabel>();
		GetLevelLabel = ExpView.transform.FindChild("GetLevelLabel").GetComponent<UILabel>();

		if(ExpView == null || ProgressBar == null || ProgressBar2 == null)
			Debug.LogError("TExpStruct not init");
	}

	public void UpdateView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			maxExp = GameData.DSkillData[skill.ID].UpgradeExp[skill.Lv];
			if(skill.Lv >= GameData.DSkillData[skill.ID].MaxStar) {
				currentExp = GameData.DSkillData[skill.ID].UpgradeExp[skill.Lv];
				ProgressBar.value = 0;
				ProgressBar2.value = 1;
				NextLevelLabel.text = string.Format(TextConst.S(7407), 0);
			} else {
				currentExp = skill.Exp;
				ProgressBar.value = (float)currentExp / (float)maxExp;
				ProgressBar2.value = (float)currentExp/ (float)maxExp;
				NextLevelLabel.text = string.Format(TextConst.S(7407), GameData.DSkillData[skill.ID].UpgradeExp[skill.Lv]);
			}
			GetLevelLabel.text = string.Format(TextConst.S(7408), 0);
		}
	}

	public void SetUpgradeView (int id, int lv, int originalExp, int upgradeExp) {
		if(GameData.DSkillData.ContainsKey(id)) {
			if(lv < GameData.DSkillData[id].MaxStar) {
				ProgressBar2.value = (float)(originalExp + upgradeExp)/ (float)maxExp;
				if((originalExp + upgradeExp) >= GameData.DSkillData[id].UpgradeExp[lv])
					ProgressBar.value = 0;
				else 
					ProgressBar.value = (float)currentExp / (float)maxExp;

				GetLevelLabel.text = string.Format(TextConst.S(7408), upgradeExp);
			}
		}
	}
}

public struct TCostView {
	public GameObject CostView;
	public UILabel FirstLabel;
	public UILabel SecondLabel;

	public void Init (Transform t) {
		CostView = t.FindChild("Window/Center/LeftView/CostView").gameObject;
		FirstLabel = CostView.transform.FindChild("ValueLabel0").GetComponent<UILabel>();
		SecondLabel = CostView.transform.FindChild("ValueLabel1").GetComponent<UILabel>();
		SecondLabel.color = Color.white;

		if(CostView == null || FirstLabel == null || SecondLabel == null)
			Debug.LogError("CostView not Init");
	}

	public void UpdateView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			FirstLabel.text = GameData.DSkillData[skill.ID].Space(skill.Lv).ToString();
			SecondLabel.text = GameData.DSkillData[skill.ID].Space(skill.Lv).ToString();
			SecondLabel.color = Color.white;
		}
	}

	public void UpgradeView (TSkill skill , int newLv) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(skill.Lv < GameData.DSkillData[skill.ID].MaxStar && newLv <= GameData.DSkillData[skill.ID].MaxStar) {
				SecondLabel.text = GameData.DSkillData[skill.ID].Space(newLv).ToString();
				if(GameData.DSkillData[skill.ID].Space(newLv) > GameData.DSkillData[skill.ID].Space(skill.Lv))
					SecondLabel.color = Color.green;
				else 
					SecondLabel.color = Color.white;
			}
		}
	}
}

public struct TEnergyView {
	public GameObject EnergyView;
	public UILabel TitleLabel;
	public UILabel FirstLabel;
	public UILabel SecondLabel;

	public void Init (Transform t) {
		EnergyView = t.FindChild("Window/Center/LeftView/EnergyView").gameObject;
		TitleLabel = EnergyView.transform.FindChild("GroupLabel").GetComponent<UILabel>();
		FirstLabel = EnergyView.transform.FindChild("ValueLabel0").GetComponent<UILabel>();
		SecondLabel = EnergyView.transform.FindChild("ValueLabel1").GetComponent<UILabel>();
		SecondLabel.color = Color.white;

		if(EnergyView == null || FirstLabel == null || SecondLabel == null)
			Debug.LogError("TEnergyView not Init");
	}

	public void UpdateView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameFunction.IsActiveSkill(skill.ID)) {
				TitleLabel.text = TextConst.S(7207);
				FirstLabel.text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
				SecondLabel.text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
			} else {
				TitleLabel.text = TextConst.S(7206);
				FirstLabel.text = GameData.DSkillData[skill.ID].Rate(skill.Lv).ToString();
				SecondLabel.text = GameData.DSkillData[skill.ID].Rate(skill.Lv).ToString();
			}
			SecondLabel.color = Color.white;
		}
	}

	public void UpgradeView (TSkill skill , int newLv) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(skill.Lv < GameData.DSkillData[skill.ID].MaxStar && newLv <= GameData.DSkillData[skill.ID].MaxStar) {
				if(GameFunction.IsActiveSkill(skill.ID)) {
					SecondLabel.text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
				} else {
					SecondLabel.text = GameData.DSkillData[skill.ID].Rate(newLv).ToString();
					if(GameData.DSkillData[skill.ID].Rate(newLv) > GameData.DSkillData[skill.ID].Rate(skill.Lv))
						SecondLabel.color = Color.green;
					else 
						SecondLabel.color = Color.white;
				}
			}
		}
	}
}

//CenterView
public struct TReinforceInfo {
	public GameObject[] AttrView;
	public UILabel[] GroupLabel;
	public UILabel[] ValueLabel0;
	public UILabel[] ValueLabel1;

	public void Init (Transform t) {
		AttrView = new GameObject[4];
		GroupLabel = new UILabel[4];
		ValueLabel0 = new UILabel[4];
		ValueLabel1 = new UILabel[4];

		for(int i=0; i<AttrView.Length; i++) {
			AttrView[i] = t.FindChild("Window/Center/CenterView/ReinforceInfo/AttrView" + i.ToString()).gameObject;
			GroupLabel[i] = AttrView[i].transform.FindChild("GroupLabel").GetComponent<UILabel>();
			ValueLabel0[i] = AttrView[i].transform.FindChild("ValueLabel0").GetComponent<UILabel>();
			ValueLabel1[i] = AttrView[i].transform.FindChild("ValueLabel1").GetComponent<UILabel>();
			ValueLabel1[i].color = Color.white;
		}
	}

	private void hideAll () {
		for(int i=0; i<AttrView.Length; i++) 
			AttrView[i].SetActive(false);	
	}
	//AttrKindRate  AniRate
	//Distance 
	//AttrKind Value
	//LifeTime 
	public void UpdateView(TSkill skill) {
		hideAll ();
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].aniRate > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7404);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				ValueLabel1[index].color = Color.white;
				index ++;
			}
			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7405);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				ValueLabel1[index].color = Color.white;
				index ++;
			}
			if(GameData.DSkillData[skill.ID].valueBase > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(10500 + GameData.DSkillData[skill.ID].AttrKind);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				ValueLabel1[index].color = Color.white;
				index ++;
			}
			if(GameData.DSkillData[skill.ID].lifeTime > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7406);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				ValueLabel1[index].color = Color.white;
				index ++;
			}
		}
	}

	public void UpgradeView (TSkill skill, int newLv) {
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(skill.Lv < GameData.DSkillData[skill.ID].MaxStar && newLv <= GameData.DSkillData[skill.ID].MaxStar) {
				if(GameData.DSkillData[skill.ID].aniRate > 0) {
					ValueLabel1[index].text = GameData.DSkillData[skill.ID].AniRate(newLv).ToString();
					if(GameData.DSkillData[skill.ID].AniRate(newLv) > GameData.DSkillData[skill.ID].AniRate(skill.Lv)) {
						ValueLabel1[index].color = Color.green;
					} else {
						ValueLabel1[index].color = Color.white;
					}
					index ++;
				}
				if(GameData.DSkillData[skill.ID].distance > 0) {
					ValueLabel1[index].text = GameData.DSkillData[skill.ID].Distance(newLv).ToString();
					if(GameData.DSkillData[skill.ID].Distance(newLv) > GameData.DSkillData[skill.ID].Distance(skill.Lv)) {
						ValueLabel1[index].color = Color.green;
					} else {
						ValueLabel1[index].color = Color.white;
					}
					index ++;
				}
				if(GameData.DSkillData[skill.ID].valueBase > 0) {
					ValueLabel1[index].text = GameData.DSkillData[skill.ID].Value(newLv).ToString();
					if(GameData.DSkillData[skill.ID].Value(newLv) > GameData.DSkillData[skill.ID].Value(skill.Lv)) {
						ValueLabel1[index].color = Color.green;
					} else {
						ValueLabel1[index].color = Color.white;
					}
					index ++;
				}
				if(GameData.DSkillData[skill.ID].lifeTime > 0) {
					ValueLabel1[index].text = GameData.DSkillData[skill.ID].LifeTime(newLv).ToString();
					if(GameData.DSkillData[skill.ID].LifeTime(newLv) > GameData.DSkillData[skill.ID].LifeTime(skill.Lv)) {
						ValueLabel1[index].color = Color.green;
					} else {
						ValueLabel1[index].color = Color.white;
					}
					index ++;
				}
			}
		}
	}
}

public class UISkillReinforce : UIBase {
	private static UISkillReinforce instance = null;
	private const string UIName = "UISkillReinforce";

	//Send Value
	private int targetIndex;
	private int[] removeIndexs;

	private TSkill mSkill;

	private GameObject itemCardEquipped;
	private GameObject itemCardReinforce;

	//LeftView
	private TActiveSkillCard skillCard;
	private TExpView expView;
	private TCostView costView;
	private TEnergyView energyView;

	//CenterView
	private GameObject[] MaterialSlots;
	private GameObject[] MaterialRemoveBtns;
	private TReinforceInfo reinForceInfo;

	//RightView
	private GameObject scrollView;
	private UIScrollView uiScrollView;
	private UIButton buttonReinforce;
	private UILabel labelPrice;

	private int reinforceMoney;
	private int originalExp;
	private int reinforceExp;

	private Dictionary<string, TPassiveSkillCard> passiveSkillCards;
	//card Right
	private List<TPassiveSkillCard> reinforceCards;
	//item Center
	private Dictionary<string, GameObject> reinforceItems;

	private bool isEquiped = false;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else
			if (isShow)
				Get.Show(isShow);
	}

	public static UISkillReinforce Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillReinforce;

			return instance;
		}
	}

	protected override void InitCom() {
		itemCardEquipped = Resources.Load(UIPrefabPath.ItemCardEquipped) as GameObject;
		itemCardReinforce = Resources.Load(UIPrefabPath.ItemAwardGroup) as GameObject;

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
		MaterialSlots = new GameObject[6];
		MaterialRemoveBtns = new GameObject[6];
		for(int i=0; i<MaterialSlots.Length; i++) {
			MaterialSlots[i] = GameObject.Find(UIName + "/Window/Center/CenterView/SlotGroup/MaterialSlot"+i.ToString()+"/View");
			MaterialRemoveBtns[i] =  GameObject.Find(UIName + "/Window/Center/CenterView/SlotGroup/MaterialSlot"+i.ToString()+"/RemoveBtn");
			SetBtnFun(UIName + "/Window/Center/CenterView/SlotGroup/MaterialSlot"+i.ToString()+"/RemoveBtn", RemoveChooseItem);
			MaterialRemoveBtns[i].name = i.ToString();
			MaterialRemoveBtns[i].SetActive(false);
		}
		reinForceInfo = new TReinforceInfo();
		reinForceInfo.Init(transform);

		//RightView
		scrollView = GameObject.Find(UIName + "/Window/Center/RightView/ScrollView");
		uiScrollView = GameObject.Find(UIName + "/Window/Center/RightView/ScrollView").GetComponent<UIScrollView>();
		buttonReinforce = GameObject.Find(UIName + "/Window/Center/RightView/ReinforceBtn").GetComponent<UIButton>();
		labelPrice = GameObject.Find(UIName + "/Window/Center/RightView/ReinforceBtn/PriceLabel").GetComponent<UILabel>();

		reinforceCards = new List<TPassiveSkillCard>();
		reinforceItems = new Dictionary<string, GameObject>();
		passiveSkillCards = new Dictionary<string, TPassiveSkillCard>();

		SetBtnFun(UIName + "/Window/Center/RightView/ReinforceBtn", OnReinforce);
		SetBtnFun(UIName + "/Window/Center/NoBtn", OnClose);
	}

	protected override void InitData() {
		buttonReinforce.isEnabled = false;
	}

	private void initRightCards () {
		if(GameData.Team.SkillCards != null && GameData.Team.SkillCards.Length > 0) {
			int index = 0;
			for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
				TPassiveSkillCard obj = null;
				if(GameData.Team.SkillCards[i].ID > 100 && GameData.DSkillData.ContainsKey(GameData.Team.SkillCards[i].ID) && isCanReinForce(GameData.Team.SkillCards[i].SN)) {
					obj = addItem(i, index, GameData.Team.SkillCards[i]);
					passiveSkillCards.Add(obj.Name, obj);
					index ++ ;
				}
			}
		}
		uiScrollView.ResetPosition();
	}

	private TPassiveSkillCard addItem (int skillCardIndex, int positionIndex, TSkill skill) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = scrollView.transform;
		obj.transform.name =  skill.ID.ToString() + "_" + skill.SN.ToString() + "_" + skill.Lv.ToString();
		obj.transform.localPosition = new Vector3(0, 200 - 80 * positionIndex, 0);
		LayerMgr.Get.SetLayerAllChildren(obj, "TopUI");

		TPassiveSkillCard skillCard = new TPassiveSkillCard();
		skillCard.InitReinforce(obj, skillCardIndex);
		skillCard.UpdateViewReinforce(skill);

		UIEventListener.Get(obj).onClick = ChooseItem;

		return skillCard;
	}

	private GameObject addReinforceCard (Transform parent, TSkill skill) {
		GameObject obj = Instantiate(itemCardReinforce, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.name =  skill.ID.ToString() + "_" + skill.SN.ToString() + "_" + skill.Lv.ToString();
		obj.transform.parent = parent;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;
		LayerMgr.Get.SetLayerAllChildren(obj, "TopUI");

		obj.GetComponent<ItemAwardGroup>().ShowSkill(skill);
		return obj;
	}
		
	private bool isCanReinForce(int sn) {
		if(GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 0) {
			for (int i=0; i<GameData.Team.PlayerBank.Length; i++) {
				if(GameData.Team.PlayerBank[i].SkillCardPages != null && GameData.Team.PlayerBank[i].SkillCardPages.Length > 0) {
					for (int j=0; j<GameData.Team.PlayerBank[i].SkillCardPages.Length; j++) {
						int[] SNs = GameData.Team.PlayerBank[i].SkillCardPages[j].SNs;
						if (SNs.Length > 0) {
							for (int k=0; k<SNs.Length; k++)
								if (SNs[k] == sn)
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
						if (SNs[k] == sn)
							return false;
				}
			}
		}

		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) 
			if(GameData.Team.Player.SkillCards[i].SN == sn)
				return false;

		if(mSkill.SN == sn)
			return false;

		return true;
	}

	private int checkLvUp(int addExp) {
		int tempLv = mSkill.Lv;
		int tempExp = mSkill.Exp;
		if(addExp > 0 && GameData.DSkillData.ContainsKey(mSkill.ID)) {

			tempExp += addExp;

			// 更新等級.
			int lvUpExp = GameData.DSkillData[mSkill.ID].UpgradeExp[mSkill.Lv];
			while(lvUpExp > 0 && tempExp >= lvUpExp)
			{
				tempLv++;
				tempExp -= lvUpExp;

				lvUpExp = GameData.DSkillData[mSkill.ID].UpgradeExp[tempLv];
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
			if((newLv - mSkill.Lv) > 0) {
				skillCard.ShowStarForRein(mSkill.Lv, (newLv - mSkill.Lv));

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
			expView.SetUpgradeView(mSkill.ID, mSkill.Lv, originalExp, reinforceExp);
			skillCard.ShowStarForRein(mSkill.Lv, (newLv - mSkill.Lv));

			costView.UpgradeView(mSkill, newLv);
			energyView.UpgradeView(mSkill, newLv);
			reinForceInfo.UpgradeView(mSkill, newLv);
		}
	}

	private void addUpgradeMoney (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			reinforceMoney += GameData.DSkillData[skill.ID].UpgradeMoney[skill.Lv];
			if(CheckMoney(reinforceMoney))
				labelPrice.color = Color.white;
			else
				labelPrice.color = Color.red;
			labelPrice.text = reinforceMoney.ToString();
			buttonReinforce.isEnabled = (reinforceMoney > 0);
		}
	}

	private void minusUpgradeMoney (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			reinforceMoney -= GameData.DSkillData[skill.ID].UpgradeMoney[skill.Lv];
			if(CheckMoney(reinforceMoney))
				labelPrice.color = Color.white;
			else
				labelPrice.color = Color.red;
			labelPrice.text = reinforceMoney.ToString();
			buttonReinforce.isEnabled = (reinforceMoney > 0);
		}
	}

	private void hideSlotRemoveBtn () {
		for (int i=0; i<MaterialRemoveBtns.Length; i++)
			MaterialRemoveBtns[i].SetActive(false);
	}

	public void RemoveChooseItem () {
		int index = -1;
		if(int.TryParse(UIButton.current.name, out index)) {
			ChooseItem(reinforceCards[index].item);
		}
	}

	public void ChooseItem (GameObject go) {
		if(passiveSkillCards.ContainsKey(go.name)) {
			if(reinforceCards.Count < 6) {
				if(reinforceItems.ContainsKey(go.name)) {
					Destroy(reinforceItems[go.name]);
					passiveSkillCards[go.name].ChooseReinforce(false);
					reinforceItems.Remove(go.name);
					reinforceCards.Remove(passiveSkillCards[go.name]);
					minusUpgradeMoney(passiveSkillCards[go.name].Skill);
					minusUpgradeView(passiveSkillCards[go.name].Skill);
				} else {
					reinforceItems.Add(go.name, addReinforceCard(MaterialSlots[reinforceItems.Count].transform, passiveSkillCards[go.name].Skill));
					reinforceCards.Add(passiveSkillCards[go.name]);
					passiveSkillCards[go.name].ChooseReinforce(true, reinforceCards.Count);
					addUpgradeMoney(passiveSkillCards[go.name].Skill);
					addUpgradeView(passiveSkillCards[go.name].Skill);
				}
			} else {
				if(reinforceItems.ContainsKey(go.name)) {
					Destroy(reinforceItems[go.name]);
					passiveSkillCards[go.name].ChooseReinforce(false);
					reinforceItems.Remove(go.name);
					reinforceCards.Remove(passiveSkillCards[go.name]);
					minusUpgradeMoney(passiveSkillCards[go.name].Skill);
					minusUpgradeView(passiveSkillCards[go.name].Skill);
				} 
			}
			RefreshSlot ();
		}
	}

	public void RefreshView (TSkill skill) {
		//Delete list
		foreach(Transform child in scrollView.transform) {
			Destroy(child.gameObject);
		}

		foreach(KeyValuePair<string, GameObject> obj in reinforceItems) {
			Destroy(obj.Value);
		}
		hideSlotRemoveBtn ();

		reinforceCards.Clear();
		reinforceItems.Clear();
		passiveSkillCards.Clear();
		reinforceMoney = 0;
		labelPrice.text = reinforceMoney.ToString();
		buttonReinforce.isEnabled = false;

		originalExp = skill.Exp;
		reinforceExp = 0;
		skillCard.UpdateViewFormation(skill, false);
		expView.UpdateView(skill);
		costView.UpdateView(skill);
		energyView.UpdateView(skill);
		reinForceInfo.UpdateView(skill);
	}

	public void RefreshSlot () {
		hideSlotRemoveBtn ();
		if(reinforceCards.Count > 0) {
			for(int i=0; i<reinforceCards.Count; i++) {
				reinforceCards[i].ChooseReinforce(true, i + 1);
				if(i >= 0 && i < MaterialSlots.Length) {
					MaterialRemoveBtns[i].SetActive(true);
					reinforceItems[reinforceCards[i].Name].transform.parent = MaterialSlots[i].transform;
					reinforceItems[reinforceCards[i].Name].transform.localPosition = Vector3.zero;
				}
			}
		}
	}

	private bool checkEquiped (int sn) {
		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) 
			if(GameData.Team.Player.SkillCards[i].SN == sn)
				return true;
		
		return false;
	}

	public void Show (TSkill skill, int index, bool isAlreadyEquip) {
		mSkill = skill;
		UIShow (true);
		RefreshView(skill);
		initRightCards ();
		targetIndex = index;
		isEquiped = isAlreadyEquip;
	}

	public void OnReinforce () {
		if(CheckMoney(reinforceMoney)) {
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
					SendReinforcePlayer();
				else
					SendReinforce();
			}
		} else {
			UIHint.Get.ShowHint(TextConst.S(510), Color.white);
		}
	}

	public void OnClose () {
		UIShow(false);
	}

	/// <summary>
	/// Sends the reinforce.
	/// </summary>
	public void SendReinforce() {
		WWWForm form = new WWWForm();
		form.AddField("TargetIndex", targetIndex);
		form.AddField("RemoveIndexs", JsonConvert.SerializeObject(removeIndexs));
		SendHttp.Get.Command(URLConst.ReinforceSkillcard, waitReinforce, form);
	}

	public void SendReinforcePlayer() {
		WWWForm form = new WWWForm();
		form.AddField("TargetIndex", targetIndex);
		form.AddField("RemoveIndexs", JsonConvert.SerializeObject(removeIndexs));
		SendHttp.Get.Command(URLConst.ReinforcePlayerSkillcard, waitReinforcePlayer, form);
	}

	private void waitReinforce(bool ok, WWW www) {
		if (ok) {
			TEquipSkillCardResult result = JsonConvert.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			SetMoney(result.Money);

			if(UISkillFormation.Visible)
				UISkillFormation.Get.RefreshAddCard();

			mSkill = findNewSkillFromTeam(mSkill);
			RefreshView(mSkill);
			initRightCards ();
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

	private void waitReinforcePlayer(bool ok, WWW www) {
		if (ok) {
			TEquipSkillCardResult result = JsonConvert.DeserializeObject <TEquipSkillCardResult>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerCards;
			SetMoney(result.Money);

			if(UISkillFormation.Visible)
				UISkillFormation.Get.RefreshAddCard();

			mSkill = findNewSkillFromPlayer(mSkill);
			RefreshView(mSkill);
			initRightCards ();
		} else {
			Debug.LogError("text:"+www.text);
		} 
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
}

