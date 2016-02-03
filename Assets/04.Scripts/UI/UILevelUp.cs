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

	public void UpgradeViewForLevelUp (TItemData beforeItemData, TItemData afterItemData) {
		int value1 = 0;
		int value2 = 0;
		if(GameData.DItemData.ContainsKey(beforeItemData.ID) && GameData.DItemData.ContainsKey(afterItemData.ID)) {
			AttrKind[0].spriteName = "AttrKind_" + beforeItemData.AttrKind1.GetHashCode();
			AttrKindLabel[0].text = TextConst.S(10500 + beforeItemData.AttrKind1.GetHashCode());
			AttrView[0].name = beforeItemData.AttrKind1.GetHashCode().ToString();
			value1 = GameData.DItemData[beforeItemData.ID].AttrValue1;
			value2 = GameData.DItemData[afterItemData.ID].AttrValue1;
			ValueLabel0[0].text = value1.ToString();
			ValueLabel1[0].text = value2.ToString();
			if(value2 != value1) {
				ValueLabel1[0].gameObject.SetActive(true);
			} else {
				ValueLabel1[0].gameObject.SetActive(false);
			}

			AttrKind[1].spriteName = "AttrKind_" + beforeItemData.AttrKind2.GetHashCode();
			AttrKindLabel[1].text = TextConst.S(10500 + beforeItemData.AttrKind2.GetHashCode());
			AttrView[1].name = beforeItemData.AttrKind2.GetHashCode().ToString();
			value1 = GameData.DItemData[beforeItemData.ID].AttrValue2;
			value2 = GameData.DItemData[afterItemData.ID].AttrValue2;
			ValueLabel0[1].text = value1.ToString();
			ValueLabel1[1].text = value2.ToString();
			if(value2 != value1) {
				ValueLabel1[1].gameObject.SetActive(true);
			} else {
				ValueLabel1[1].gameObject.SetActive(false);
			}

			AttrKind[2].spriteName = "AttrKind_" + beforeItemData.AttrKind3.GetHashCode();
			AttrKindLabel[2].text = TextConst.S(10500 + beforeItemData.AttrKind3.GetHashCode());
			AttrView[2].name = beforeItemData.AttrKind3.GetHashCode().ToString();
			value1 = GameData.DItemData[beforeItemData.ID].AttrValue3;
			value2 = GameData.DItemData[afterItemData.ID].AttrValue3;
			ValueLabel0[2].text = value1.ToString();
			ValueLabel1[2].text = value2.ToString();
			if(value2 != value1) {
				ValueLabel1[2].gameObject.SetActive(true);
			} else {
				ValueLabel1[2].gameObject.SetActive(false);
			}
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

public class UILevelUp : UIBase {
	private static UILevelUp instance = null;
	private const string UIName = "UILevelUp";

	private GameObject[] page;//0:角色升級 1:卡牌，物品升級


	private TPlayerLevelUp playerLevelUp;
	private TItemLevelUp itemLevelUp;

	private int lv;
	
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
			if(!isShow)
				RemoveUI(UIName);
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
		page = new GameObject[2];
		page[0] = GameObject.Find(UIName + "/Window/Center/BottomView/Page0");
		page[1] = GameObject.Find(UIName + "/Window/Center/BottomView/Page1");

		playerLevelUp = new TPlayerLevelUp();
		playerLevelUp.Init(GameObject.Find(UIName + "/Window/Center/BottomView/Page0"));
		itemLevelUp = new TItemLevelUp();
		itemLevelUp.Init(GameObject.Find(UIName + "/Window/Center/BottomView/Page1"), OnClickAttr);

		UIEventListener.Get(GameObject.Find(UIName + "/Window/BottomRight/NextLabel")).onClick = OnReturn;
	}

	public void OnClickAttr (GameObject go) {
		int result = -1;
		if(int.TryParse(go.name, out result)){
			UIAttributeHint.Get.UpdateView(result);
		}	
	}

	public void OnReturn (GameObject go) {
		if(GameController.Visible) {
			if(GameData.DExpData.ContainsKey(lv) && LimitTable.Ins.HasOpenIDByLv(lv)) {
				UIShow(false);
				UIAchievement.Get.ShowView(lv);
			} else {
				UILoading.OpenUI = UILoading.OpenStageUI;
				if (isStage)
					SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
				else
					SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
			}
		} else 
			UIShow(false);
	}

	public void Show (TPlayer beforePlayer, TPlayer afterPlayer) {
		AudioMgr.Get.PlaySound(SoundType.SD_UpgradePlayer);
		UIShow(true);
		page[0].SetActive(true);
		page[1].SetActive(false);
		lv = afterPlayer.Lv;
		playerLevelUp.UpdateView(beforePlayer, afterPlayer);
	}

	public void ShowEquip (TItemData beforeItemData, TItemData afterItemData) {
		AudioMgr.Get.PlaySound(SoundType.SD_UpgradeItems);
		UIShow(true);
		page[0].SetActive(false);
		page[1].SetActive(true);
		itemLevelUp.UpdateForEquipment(beforeItemData, afterItemData);
	}

	public void ShowSkill (TSkill beforeSkill, TSkill afterSkill) {
		AudioMgr.Get.PlaySound(SoundType.SD_UpgradeItems);
		UIShow(true);
		page[0].SetActive(false);
		page[1].SetActive(true);
		itemLevelUp.UpdateForReinforce(beforeSkill, afterSkill);
	}

	public bool isStage
	{
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
