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
	public GameObject BarFullFX;

	private int currentExp;
	private int maxExp;

	public void Init (Transform t) {
		ExpView = t.FindChild("Window/Center/LeftView/EXPView").gameObject;
		ProgressBar = ExpView.transform.FindChild("ProgressBar").GetComponent<UISlider>();
		ProgressBar2 = ExpView.transform.FindChild("ProgressBar2").GetComponent<UISlider>();
		NextLevelLabel = ExpView.transform.FindChild("NextLevelLabel").GetComponent<UILabel>();
		GetLevelLabel = ExpView.transform.FindChild("GetLevelLabel").GetComponent<UILabel>();
		BarFullFX = ExpView.transform.FindChild("BarFullFX").gameObject;

		if(ExpView == null || ProgressBar == null || ProgressBar2 == null || BarFullFX == null)
			Debug.LogError("TExpStruct not init");
		if(BarFullFX != null)
			BarFullFX.SetActive(false);
	}

	public void UpdateView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			maxExp = GameData.DSkillData[skill.ID].UpgradeExp[skill.Lv];
			if(skill.Lv >= GameData.DSkillData[skill.ID].MaxStar) {
				currentExp = GameData.DSkillData[skill.ID].UpgradeExp[skill.Lv];
				SetTopProgressView ();
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
//				if((originalExp + upgradeExp) >= GameData.DSkillData[id].UpgradeExp[lv])
//					ProgressBar.value = 0;
//				else 
//					ProgressBar.value = (float)currentExp / (float)maxExp;

				GetLevelLabel.text = string.Format(TextConst.S(7408), upgradeExp);
			}
		}
	}

	public void SetProgressView (int id, int lv, int yellowExpValue, int greenExpValue, int getLevelExpValue) {
		if(GameData.DSkillData.ContainsKey(id)) {
			maxExp = GameData.DSkillData[id].UpgradeExp[lv];
			NextLevelLabel.text = string.Format(TextConst.S(7407), maxExp);
			ProgressBar.value = (float)yellowExpValue / (float)maxExp;
			ProgressBar2.value = (float)greenExpValue / (float)maxExp;
			GetLevelLabel.text = string.Format(TextConst.S(7408), getLevelExpValue);
		}
	}

	public void SetTopProgressView () {
		ProgressBar.value = 0;
		ProgressBar2.value = 1;
		NextLevelLabel.text = TextConst.S(7409);
		GetLevelLabel.text = string.Format(TextConst.S(7408), 0);
	}

	public void ShowFull () {
		BarFullFX.SetActive(false);
		BarFullFX.SetActive(true);
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
			AttrView[i] = t.FindChild("AttrView" + i.ToString()).gameObject;
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

	public void UpgradeViewForLevelUp (TSkill skill, int newLv) {
		hideAll ();
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].aniRate > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7404);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].AniRate(newLv).ToString();
				if(GameData.DSkillData[skill.ID].AniRate(newLv) > GameData.DSkillData[skill.ID].AniRate(skill.Lv)) {
					ValueLabel1[index].gameObject.SetActive(true);
					ValueLabel1[index].color = Color.green;
				} else {
					ValueLabel1[index].gameObject.SetActive(false);
				}
				index ++;
			}

			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7405);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].Distance(newLv).ToString();
				if(GameData.DSkillData[skill.ID].Distance(newLv) > GameData.DSkillData[skill.ID].Distance(skill.Lv)) {
					ValueLabel1[index].gameObject.SetActive(true);
					ValueLabel1[index].color = Color.green;
				} else {
					ValueLabel1[index].gameObject.SetActive(false);
				}
				index ++;
			}

			if(GameData.DSkillData[skill.ID].valueBase > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(10500 + GameData.DSkillData[skill.ID].AttrKind);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].Value(newLv).ToString();
				if(GameData.DSkillData[skill.ID].Value(newLv) > GameData.DSkillData[skill.ID].Value(skill.Lv)) {
					ValueLabel1[index].gameObject.SetActive(true);
					ValueLabel1[index].color = Color.green;
				} else {
					ValueLabel1[index].gameObject.SetActive(false);
				}
				index ++;
			}

			if(GameData.DSkillData[skill.ID].lifeTime > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7406);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].LifeTime(newLv).ToString();
				if(GameData.DSkillData[skill.ID].LifeTime(newLv) > GameData.DSkillData[skill.ID].LifeTime(skill.Lv)) {
					ValueLabel1[index].gameObject.SetActive(true);
					ValueLabel1[index].color = Color.green;
				} else {
					ValueLabel1[index].gameObject.SetActive(false);
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

	private TSkill mOldSkill;
	private TSkill mSkill;

	private GameObject itemCardEquipped;
	private GameObject itemCardReinforce;

	//LeftView
	private TActiveSkillCard skillCard;
	private TExpView expView;
	private TCostView costView;
	private TEnergyView energyView;

	//CenterView
	private MaterialSlot[] materialSlots;
//	private GameObject[] MaterialSlots;
//	private GameObject[] MaterialRemoveBtns;
	private TReinforceInfo reinForceInfo;

	//RightView
	private GameObject scrollView;
	private UIScrollView uiScrollView;
	private UIButton buttonReinforce;
	private UILabel labelPrice;

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

	void FixedUpdate () {
		runExp ();
	}

	protected override void InitCom() {
		itemCardEquipped = Resources.Load(UIPrefabPath.ItemCardEquipped) as GameObject;
		itemCardReinforce = Resources.Load(UIPrefabPath.ItemAwardGroup) as GameObject;
		reinforceAnimator = GetComponent<Animator>();

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
		buttonReinforce.normalSprite = "button_gray";
		buttonReinforce.pressedSprite = "button_gray2";
	}

	private void initRightCards () {
		if(GameData.Team.SkillCards != null && GameData.Team.SkillCards.Length > 0) {
			int index = 0;
			for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
				TPassiveSkillCard obj = null;
				obj = addItem(i, index, GameData.Team.SkillCards[i]);
				if(obj != null && !passiveSkillCards.ContainsKey(obj.Name) && GameData.Team.SkillCards[i].ID > 100 && GameData.DSkillData.ContainsKey(GameData.Team.SkillCards[i].ID) && isCanReinForce(GameData.Team.SkillCards[i].SN)) {
					passiveSkillCards.Add(obj.Name, obj);
					index ++ ;
				} else
					Destroy(obj.item);
			}
		}
		uiScrollView.ResetPosition();
//		uiScrollView.MoveRelative(new Vector3(0, -33, 0));
	}

	private TPassiveSkillCard addItem (int skillCardIndex, int positionIndex, TSkill skill) {
		GameObject obj = Instantiate(itemCardEquipped, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = scrollView.transform;
		obj.transform.name =  skill.ID.ToString() + "_" + skill.SN.ToString() + "_" + skill.Lv.ToString();
		obj.transform.localPosition = new Vector3(0, 200 - 80 * positionIndex, 0);
		LayerMgr.Get.SetLayerAllChildren(obj, "TopUI");

		TPassiveSkillCard passiveSkillCard = new TPassiveSkillCard();
		passiveSkillCard.InitReinforce(obj, skillCardIndex);
		passiveSkillCard.UpdateViewReinforce(skill);

		UIEventListener.Get(obj).onClick = ChooseItem;

		return passiveSkillCard;
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

	private void addUpgradeMoney (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			reinforceMoney += GameData.DSkillData[skill.ID].UpgradeMoney[skill.Lv];
			if(CheckMoney(reinforceMoney))
				labelPrice.color = Color.white;
			else
				labelPrice.color = Color.red;
			labelPrice.text = reinforceMoney.ToString();
			if(reinforceMoney > 0) {
				buttonReinforce.normalSprite = "button_orange1";
				buttonReinforce.pressedSprite = "button_orange2";
			} else {
				buttonReinforce.normalSprite = "button_gray";
				buttonReinforce.pressedSprite = "button_gray2";
			}

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
			if(reinforceMoney > 0) {
				buttonReinforce.normalSprite = "button_orange1";
				buttonReinforce.pressedSprite = "button_orange2";
			} else {
				buttonReinforce.normalSprite = "button_gray";
				buttonReinforce.pressedSprite = "button_gray2";
			}
		}
	}

	private void hideSlotRemoveBtn () {
		for (int i=0; i<materialSlots.Length; i++)
			materialSlots[i].RemoveBtn.gameObject.SetActive(false);
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
					materialSlots[reinforceItems.Count].ShowInput();
					reinforceItems.Add(go.name, addReinforceCard(materialSlots[reinforceItems.Count].View, passiveSkillCards[go.name].Skill));
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
		if(reinforceItems.Count > 0) {
			foreach(KeyValuePair<string, GameObject> obj in reinforceItems) {
				Destroy(obj.Value);
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
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			if(mSkill.Lv >= GameData.DSkillData[mSkill.ID].MaxStar) {
				UIHint.Get.ShowHint(TextConst.S(556), Color.red);
			} else {
				if(reinforceCards.Count > 0) {
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
				} else 
					UIHint.Get.ShowHint(TextConst.S(557), Color.white);
			}
		}
	}

	public void OnClose () {
		UIShow(false);
	}

	private void runExp () {
		if(isRunExp && GameData.DSkillData.ContainsKey(mSkill.ID) && newCardLv >= oldCardLv && reinforceExp > 0) {
			if( oldCardLv < GameData.DSkillData[mSkill.ID].MaxStar) {
				expView.SetProgressView(mSkill.ID, 
					oldCardLv, 
					originalExp, 
					recordGreenExp,
					reinforceExp);


				if(reinforceExp >= addInterVal)
					reinforceExp -= addInterVal;
				else 
					addInterVal = reinforceExp;

				originalExp += addInterVal;

				if(originalExp > GameData.DSkillData[mSkill.ID].UpgradeExp[oldCardLv]) {
					originalExp -= GameData.DSkillData[mSkill.ID].UpgradeExp[oldCardLv];
					oldCardLv ++;
					skillCard.ShowGetStar(oldCardLv - 1);
					recordGreenExp = reinforceExp;
					expView.ShowFull();
				}

				if(reinforceExp <= 0) {
					expView.SetProgressView(mSkill.ID, 
						oldCardLv, 
						recordGreenExp, 
						recordGreenExp,
						GameData.DSkillData[mSkill.ID].UpgradeExp[oldCardLv]);
					stopRunExp ();
				}
			} else {
				skillCard.ShowGetStar(4); // 5 - 1
				expView.SetTopProgressView();
				stopRunExp ();
			}
		}
	}

	private void awakeRunExp () {
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
			GameData.Team.InitSkillCardCount();
			SetMoney(result.Money);
			UIMainLobby.Get.UpdateUI();

			if(UISkillFormation.Visible)
				UISkillFormation.Get.RefreshAddCard();

			mSkill = findNewSkillFromTeam(mSkill);
			awakeRunExp();

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
			GameData.Team.InitSkillCardCount();
			SetMoney(result.Money);
			UIMainLobby.Get.UpdateUI();

			if(UISkillFormation.Visible)
				UISkillFormation.Get.RefreshAddCard();

			mSkill = findNewSkillFromPlayer(mSkill);
			awakeRunExp();

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

