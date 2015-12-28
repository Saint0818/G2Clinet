using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

//LeftView
public struct TExpView {
	public GameObject ExpView;
	public UISlider ProgressBar;
	public UISlider ProgressBar2;
	public UILabel NextLevelLabel;
	public UILabel GetLevelLabel;

	public void Init (Transform t) {
		ExpView = t.FindChild("Window/Center/LeftView/EXPView").gameObject;
		ProgressBar = ExpView.transform.FindChild("ProgressBar").GetComponent<UISlider>();
		ProgressBar2 = ExpView.transform.FindChild("ProgressBar2").GetComponent<UISlider>();
		NextLevelLabel = ExpView.transform.FindChild("NextLevelLabel").GetComponent<UILabel>();
		GetLevelLabel = ExpView.transform.FindChild("GetLevelLabel").GetComponent<UILabel>();

		if(ExpView == null || ProgressBar == null || ProgressBar2 == null)
			Debug.LogError("TExpStruct not init");
	}

	public void UpdateView (int id, int lv, int exp) {
		if(GameData.DSkillData.ContainsKey(id)) {
			ProgressBar.value = (float)exp / (float)GameData.DSkillData[id].UpgradeExp[lv];
			ProgressBar2.value = (float)exp/ (float)GameData.DSkillData[id].UpgradeExp[lv];
			NextLevelLabel.text = string.Format(TextConst.S(7407), GameData.DSkillData[id].UpgradeExp[lv]);
			GetLevelLabel.gameObject.SetActive(false);
		}
	}

	public void SetUpgradeView () {
		
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

	public void UpdateView (int id, int lv) {
		if(GameData.DSkillData.ContainsKey(id)) {
			FirstLabel.text = GameData.DSkillData[id].Space(lv).ToString();
			SecondLabel.text = GameData.DSkillData[id].Space(lv).ToString();
		}
	}

	public void UpgradeView () {
//		Color.green
	}
}

public struct TEnergyView {
	public GameObject EnergyView;
	public UILabel FirstLabel;
	public UILabel SecondLabel;

	public void Init (Transform t) {
		EnergyView = t.FindChild("Window/Center/LeftView/EnergyView").gameObject;
		FirstLabel = EnergyView.transform.FindChild("ValueLabel0").GetComponent<UILabel>();
		SecondLabel = EnergyView.transform.FindChild("ValueLabel1").GetComponent<UILabel>();
		SecondLabel.color = Color.white;

		if(EnergyView == null || FirstLabel == null || SecondLabel == null)
			Debug.LogError("CostView not Init");
	}

	public void UpdateView (int id, int lv) {
		if(GameData.DSkillData.ContainsKey(id)) {
			FirstLabel.text = GameData.DSkillData[id].Space(lv).ToString();
			SecondLabel.text = GameData.DSkillData[id].Space(lv).ToString();
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
	public void UpdateView(int id, int lv) {
		hideAll ();
		int index = 0;
		if(GameData.DSkillData.ContainsKey(id)) {
			if(GameData.DSkillData[id].aniRate > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7404);
				ValueLabel0[index].text = GameData.DSkillData[id].AniRate(lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[id].AniRate(lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[id].distance > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7405);
				ValueLabel0[index].text = GameData.DSkillData[id].Distance(lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[id].Distance(lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[id].valueBase > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(10500 + GameData.DSkillData[id].AttrKind);
				ValueLabel0[index].text = GameData.DSkillData[id].Value(lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[id].Value(lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[id].lifeTime > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7406);
				ValueLabel0[index].text = GameData.DSkillData[id].LifeTime(lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[id].LifeTime(lv).ToString();
				index ++;
			}
		}
	}

	public void UpdateGradeValue (int id, int lv, int newLv) {
		
	}
}

public class UISkillReinforce : UIBase {
	private static UISkillReinforce instance = null;
	private const string UIName = "UISkillReinforce";

	private int mSn;

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
	private UIButton buttonReinforce;
	private UILabel labelPrice;

	private int reinforceMoney;

	private Dictionary<string, TPassiveSkillCard> passiveSkillCards;
	//card Right
	private List<TPassiveSkillCard> reinforceCards;
	//item Center
	private Dictionary<string, GameObject> reinforceItems;

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
		buttonReinforce = GameObject.Find(UIName + "/Window/Center/RightView/ReinforceBtn").GetComponent<UIButton>();
		labelPrice = GameObject.Find(UIName + "/Window/Center/RightView/ReinforceBtn/PriceLabel").GetComponent<UILabel>();

		reinforceCards = new List<TPassiveSkillCard>();
		reinforceItems = new Dictionary<string, GameObject>();
		passiveSkillCards = new Dictionary<string, TPassiveSkillCard>();

		SetBtnFun(UIName + "/Window/Center/NoBtn", OnClose);
	}

	protected override void InitData() {
		buttonReinforce.isEnabled = false;
	}

	private void init () {
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
				if(GameData.Team.PlayerBank[i].ID != GameData.Team.Player.ID) {
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
		}

		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) 
			if(GameData.Team.Player.SkillCards[i].SN == sn)
				return false;

		if(mSn == sn)
			return false;

		return true;
	}

	private void addUpgradeView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			
		}
	}

	private void minusUpgradeView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {

		}
	}

	private void addUpgradeMoney (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			reinforceMoney += GameData.DSkillData[skill.ID].UpgradeMoney[skill.Lv];
			labelPrice.text = reinforceMoney.ToString();
			buttonReinforce.isEnabled = (reinforceMoney > 0);
		}
	}

	private void minusUpgradMoney (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			reinforceMoney -= GameData.DSkillData[skill.ID].UpgradeMoney[skill.Lv];
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
			Debug.Log("RemoveChooseItem: "+ index);

			ChooseItem(reinforceCards[index].item);
		}
	}

	public void ChooseItem (GameObject go) {
		if(passiveSkillCards.ContainsKey(go.name)) {
			if(reinforceItems.ContainsKey(go.name)) {
				Destroy(reinforceItems[go.name]);
				passiveSkillCards[go.name].ChooseReinforce(false);
				reinforceItems.Remove(go.name);
				reinforceCards.Remove(passiveSkillCards[go.name]);
				minusUpgradMoney(passiveSkillCards[go.name].Skill);
			} else {
				reinforceItems.Add(go.name, addReinforceCard(MaterialSlots[reinforceItems.Count].transform, passiveSkillCards[go.name].Skill));
				reinforceCards.Add(passiveSkillCards[go.name]);
				passiveSkillCards[go.name].ChooseReinforce(true, reinforceCards.Count);
				addUpgradeMoney(passiveSkillCards[go.name].Skill);
			}
			addUpgradeView(passiveSkillCards[go.name].Skill);
			RefreshSlot ();
		}
	}

	public void Refresh () {
		//Delete list
		foreach(Transform child in scrollView.transform) {
			Destroy(child.gameObject);
		}

		foreach(KeyValuePair<string, GameObject> obj in reinforceItems) {
			Destroy(obj.Value);
		}

		reinforceCards.Clear();
		reinforceItems.Clear();
		passiveSkillCards.Clear();
		reinforceMoney = 0;
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

	public void Show (TUICard skill) {
		mSn = skill.CardSN;
		UIShow (true);
		Refresh();
		init ();
		skillCard.UpdateViewFormation(skill.skillCard.Skill, false);
		expView.UpdateView(skill.CardID, skill.CardLV, skill.CardExp);
		costView.UpdateView(skill.CardID, skill.CardLV);
		energyView.UpdateView(skill.CardID, skill.CardLV);
		reinForceInfo.UpdateView(skill.CardID, skill.CardLV);
	}

	public void OnClose () {
		UIShow(false);
	}
}

