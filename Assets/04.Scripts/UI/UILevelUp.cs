using GameStruct;
using UnityEngine;
using GameItem;
using GameEnum;

public struct TPlayerLevelUp {
	private TPlayerInGameBtn[] playerIcon;
	private UILabel labelLevelUp;
	private UILabel labelBeforeLevel;
	private UILabel labelAfterLevel;
	private UILabel labelGetPotential;

	public void Init (GameObject obj) {
		playerIcon = new TPlayerInGameBtn[2];
		playerIcon[0] = new TPlayerInGameBtn();
		playerIcon[1] = new TPlayerInGameBtn();
		playerIcon[0].Init(obj.transform.Find("LevelGroup/BeforeLevel/PlayerInGameBtn").gameObject);
		playerIcon[1].Init(obj.transform.Find("LevelGroup/AfterLevel/PlayerInGameBtn").gameObject);
		labelLevelUp = obj.transform.Find("LevelUpLabel").GetComponent<UILabel>();
		labelBeforeLevel = obj.transform.Find("LevelGroup/BeforeLevel/PlayerInGameBtn/LevelGroup").GetComponent<UILabel>();
		labelAfterLevel = obj.transform.Find("LevelGroup/AfterLevel/PlayerInGameBtn/LevelGroup").GetComponent<UILabel>();
		labelGetPotential = obj.transform.Find("GetPotentialLabel").GetComponent<UILabel>();
	}

	public void UpdateView (TPlayer beforePlayer, TPlayer afterPlayer) {
		afterPlayer.BodyType = beforePlayer.BodyType;
		playerIcon[0].UpdateView(beforePlayer);
		playerIcon[1].UpdateView(afterPlayer);
		labelLevelUp.text = afterPlayer.Lv.ToString();
		labelBeforeLevel.text = beforePlayer.Lv.ToString();
		labelAfterLevel.text = afterPlayer.Lv.ToString();
		labelGetPotential.text = string.Format(labelGetPotential.text, GameConst.PreLvPotential);
	}
}

public struct TEquipmentGroup {
	public GameObject[] AttrView;
	public UISprite[] AttrKind;
	public UILabel[] AttrKindLabel;
	public UILabel[] ValueLabel0;
	public UILabel[] ValueLabel1;

	public void Init (Transform t, UIEventListener.VoidDelegate listener) {
		AttrView = new GameObject[3];
		AttrKind = new UISprite[3];
		AttrKindLabel = new UILabel[3];
		ValueLabel0 = new UILabel[3];
		ValueLabel1 = new UILabel[3];

		for(int i=0; i<AttrView.Length; i++) {
			AttrView[i] = t.Find("AttrValue" + i.ToString()).gameObject;
			AttrKind[i] = AttrView[i].transform.Find("AttrKind").GetComponent<UISprite>();
			AttrKindLabel[i] = AttrView[i].transform.Find("AttrKind/KindLabel").GetComponent<UILabel>();
			ValueLabel0[i] = AttrView[i].transform.Find("ValueLabel0").GetComponent<UILabel>();
			ValueLabel1[i] = AttrView[i].transform.Find("ValueLabel1").GetComponent<UILabel>();

			UIEventListener.Get(AttrView[i]).onClick = listener;
		}
	}

	private void showAllView () {
		for(int i=0; i<AttrView.Length; i++) {
			AttrView[i].SetActive(true);
			AttrKind[i].gameObject.SetActive(true);
			AttrKindLabel[i].gameObject.SetActive(true);
			ValueLabel0[i].gameObject.SetActive(true);
			ValueLabel1[i].gameObject.SetActive(true);

		}
	}

	private void valueShow (int index, int value1, int value2, int kind) {
		AttrKind[index].spriteName = "AttrKind_" + kind;
		AttrKindLabel[index].text = TextConst.S(10500 + kind);
		AttrView[index].name = kind.ToString();
		ValueLabel0[index].text = value1.ToString();
		ValueLabel1[index].text = value2.ToString();
		if(value2 != value1) {
			ValueLabel1[index].gameObject.SetActive(true);
		} else {
			ValueLabel1[index].gameObject.SetActive(false);
		}
	}

	private void valueHide (int index) {
		AttrKind[index].gameObject.SetActive(false);
		AttrKindLabel[index].gameObject.SetActive(false);
		AttrView[index].gameObject.SetActive(false);
		ValueLabel0[index].gameObject.SetActive(false);
		ValueLabel1[index].gameObject.SetActive(false);
	}

	public void UpgradeViewForLevelUp (TItemData beforeItemData, TItemData afterItemData) {
		showAllView ();
		int value1 = 0;
		int value2 = 0;
		if(GameData.DItemData.ContainsKey(beforeItemData.ID) && GameData.DItemData.ContainsKey(afterItemData.ID)) {
			value1 = GameData.DItemData[beforeItemData.ID].AttrValue1;
			value2 = GameData.DItemData[afterItemData.ID].AttrValue1;
			if(value1 > 0) 
				valueShow(0, value1, value2, beforeItemData.AttrKind1.GetHashCode());
			else 
				valueHide(0);
			

			value1 = GameData.DItemData[beforeItemData.ID].AttrValue2;
			value2 = GameData.DItemData[afterItemData.ID].AttrValue2;
			if(value1 > 0) 
				valueShow(1, value1, value2, beforeItemData.AttrKind2.GetHashCode());
			else 
				valueHide(1);


			value1 = GameData.DItemData[beforeItemData.ID].AttrValue3;
			value2 = GameData.DItemData[afterItemData.ID].AttrValue3;
			if(value1 > 0) 
				valueShow(2, value1, value2, beforeItemData.AttrKind3.GetHashCode());
			else 
				valueHide(2);
			
		}
	}
}

public struct TItemLevelUp {
	private ItemAwardGroup[] itemAwardGroup;
	private GameObject goReinforceInfo;
	private TReinforceInfo reinForceInfo;
	private GameObject goEquipmentGroup;
	private TEquipmentGroup equipmentGroup; 

	public void Init (GameObject obj, UIEventListener.VoidDelegate listener) {
		itemAwardGroup = new ItemAwardGroup[2];
		itemAwardGroup[0] = obj.transform.Find("LevelGroup/BeforeLevel/ItemAwardGroup").GetComponent<ItemAwardGroup>();
		itemAwardGroup[1] = obj.transform.Find("LevelGroup/AfterLevel/ItemAwardGroup").GetComponent<ItemAwardGroup>();
		goReinforceInfo = obj.transform.Find("ReinforceInfo").gameObject;
		reinForceInfo = new TReinforceInfo();
		reinForceInfo.Init(obj.transform.Find("ReinforceInfo"));
		goEquipmentGroup = obj.transform.Find("EquipmentGroup").gameObject;
		equipmentGroup = new TEquipmentGroup();
		equipmentGroup.Init(obj.transform.Find("EquipmentGroup"), listener);
	}

	public void UpdateForReinforce (TSkill beforeSkill, TSkill afterSkill) {
		itemAwardGroup[0].ShowSkill(beforeSkill);
		itemAwardGroup[1].ShowSkill(afterSkill);
		goReinforceInfo.SetActive(true);
		goEquipmentGroup.SetActive(false);
		reinForceInfo.UpgradeViewForLevelUp(beforeSkill, afterSkill);
	}

	public void UpdateForEquipment (TItemData beforeItemData, TItemData afterItemData) {
		itemAwardGroup[0].Show(beforeItemData);
		itemAwardGroup[1].Show(afterItemData);
		goReinforceInfo.SetActive(false);
		goEquipmentGroup.SetActive(true);
		equipmentGroup.UpgradeViewForLevelUp(beforeItemData, afterItemData);
	}
}

public class TPVPLevelUp {
	public UISprite BeforeRank;
	public UISprite AfterRank;
	public UILabel BeforeRankName;
	public UILabel AfterRankName;
}

public class UILevelUp : UIBase {
	private static UILevelUp instance = null;
	private const string UIName = "UILevelUp";

	private GameObject[] page;//0:角色升級 1:卡牌，物品升級 2:PVP階級升級


	private TPlayerLevelUp playerLevelUp;
	private TItemLevelUp itemLevelUp;
	private TPVPLevelUp pvpLevelUp;

	private int lv;
	
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
	
	public static void UIShow(bool isShow){
		if (instance)
			if(!isShow)
            RemoveUI(instance.gameObject);
			else
				instance.Show(isShow);
		else
		if (isShow)
			Get.Show(isShow);
	}
	
	public static UILevelUp Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UILevelUp;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		page = new GameObject[3];
		page[0] = GameObject.Find(UIName + "/Window/Center/BottomView/Page0");
		page[1] = GameObject.Find(UIName + "/Window/Center/BottomView/Page1");
		page[2] = GameObject.Find(UIName + "/Window/Center/BottomView/Page2");

		playerLevelUp = new TPlayerLevelUp();
		playerLevelUp.Init(GameObject.Find(UIName + "/Window/Center/BottomView/Page0"));
		itemLevelUp = new TItemLevelUp();
		itemLevelUp.Init(GameObject.Find(UIName + "/Window/Center/BottomView/Page1"), OnClickAttr);
		pvpLevelUp = new TPVPLevelUp();
		pvpLevelUp.BeforeRank = GameObject.Find(UIName + "/Window/Center/BottomView/Page2/LevelGroup/BeforeRank").GetComponent<UISprite>();
		pvpLevelUp.BeforeRankName = GameObject.Find(UIName + "/Window/Center/BottomView/Page2/LevelGroup/BeforeRank/RankNameLabel").GetComponent<UILabel>();
		pvpLevelUp.AfterRank = GameObject.Find(UIName + "/Window/Center/BottomView/Page2/LevelGroup/AfterRank").GetComponent<UISprite>();
		pvpLevelUp.AfterRankName = GameObject.Find(UIName + "/Window/Center/BottomView/Page2/LevelGroup/AfterRank/RankNameLabel").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find(UIName + "/Window/BottomRight/NextLabel")).onClick = OnReturn;
	}

	private float delayWaitTime = 10;
	void FixedUpdate () {
		if(delayWaitTime > 0) {
			delayWaitTime -= Time.deltaTime;
			if(delayWaitTime <= 0)
				OnReturn(null);
		}
	}

	public void OnClickAttr (GameObject go) {
		int result = -1;
		if(int.TryParse(go.name, out result)){
			UIAttributeHint.Get.UpdateView(result);
		}	
	}

	public void OnReturn (GameObject go) {
		if(SceneMgr.Get.IsCourt) {
            UILoading.LvUpUI(lv);
			if(GameData.DExpData.ContainsKey(lv) && LimitTable.Ins.HasOpenIDByLv(lv)) {
				UIShow(false);
				UIAchievement.Get.ShowView(lv);
			} else {
				if(GameData.IsMainStage)
				{
					SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
					UILoading.OpenUI = UILoading.OpenStageUI;
				}
				else if(GameData.IsInstance)
				{
					SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
					UILoading.OpenUI = UILoading.OpenInstanceUI;
				}
				else if (GameData.IsPVP)
				{
					SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
					UILoading.OpenUI = UILoading.OpenPVPUI;
				}
				else
				{
					SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
					UILoading.OpenUI = UILoading.OpenStageUI;
				}

			}
        } else {
			UIShow(false);
            if (GameData.DExpData.ContainsKey(lv)) {
                int tid = GameData.DExpData[lv].TutorialID;
                if (GameData.DTutorial.ContainsKey(tid* 100 + 1))
                {
                    if (!GameData.Team.HaveTutorialFlag(tid)) {
                        UIMission.Visible = false;
                        UIGetItem.Visible = false;
                        UIMainLobby.Get.Show();
                        UITutorial.Get.ShowTutorial(tid, 1);
                    }
                }
            }
        }
	}

	public void Show (TPlayer beforePlayer, TPlayer afterPlayer) {
		AudioMgr.Get.PlaySound(SoundType.SD_UpgradePlayer);
		UIShow(true);
		page[0].SetActive(true);
		page[1].SetActive(false);
		page[2].SetActive(false);
		lv = afterPlayer.Lv;
		playerLevelUp.UpdateView(beforePlayer, afterPlayer);
	}

	public void ShowEquip (TItemData beforeItemData, TItemData afterItemData) {
		AudioMgr.Get.PlaySound(SoundType.SD_UpgradeItems);
		UIShow(true);
		page[0].SetActive(false);
		page[1].SetActive(true);
		page[2].SetActive(false);
		itemLevelUp.UpdateForEquipment(beforeItemData, afterItemData);
	}

	public void ShowSkill (TSkill beforeSkill, TSkill afterSkill) {
		AudioMgr.Get.PlaySound(SoundType.SD_UpgradeItems);
		UIShow(true);
		page[0].SetActive(false);
		page[1].SetActive(true);
		page[2].SetActive(false);
		itemLevelUp.UpdateForReinforce(beforeSkill, afterSkill);
	}

	public void ShowRank (int beforeLv, int afterLv) {
		AudioMgr.Get.PlaySound(SoundType.SD_UpgradeItems);
		UIShow(true);
		page[0].SetActive(false);
		page[1].SetActive(false);
		page[2].SetActive(true);
		pvpLevelUp.BeforeRank.spriteName = GameFunction.PVPRankIconName(beforeLv);
		pvpLevelUp.AfterRank.spriteName = GameFunction.PVPRankIconName(afterLv);
		if(GameData.DPVPData.ContainsKey(beforeLv) && GameData.DPVPData.ContainsKey(afterLv)) {
			pvpLevelUp.BeforeRankName.text = GameData.DPVPData[beforeLv].Name;
			pvpLevelUp.AfterRankName.text = GameData.DPVPData[afterLv].Name;
		}
	}

	public bool isStage
	{
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
