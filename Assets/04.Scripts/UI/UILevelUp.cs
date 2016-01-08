using GameStruct;
using UnityEngine;

public class UILevelUp : UIBase {
	private static UILevelUp instance = null;
	private const string UIName = "UILevelUp";

	private UILabel labelLevel;

	private UILabel labelBeforeLevel;
	private UILabel labelAfterLevel;

	private UILabel labelGetPotential;

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
		labelLevel = GameObject.Find(UIName + "/Window/Center/BottomView/Page0/LevelUpLabel").GetComponent<UILabel>();
		labelBeforeLevel = GameObject.Find(UIName + "/Window/Center/BottomView/Page0/LevelGroup/BeforeLabel").GetComponent<UILabel>();
		labelAfterLevel = GameObject.Find(UIName + "/Window/Center/BottomView/Page0/LevelGroup/AfterLabel").GetComponent<UILabel>();
		labelGetPotential = GameObject.Find(UIName + "/Window/Center/BottomView/Page0/GetPotentialLabel").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find(UIName + "/Window/BottomRight/NextLabel")).onClick = OnReturn;
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
        base.OnShow(isShow);
		labelGetPotential.text = string.Format(labelGetPotential.text, GameConst.PreLvPotential);
	}

	public void OnReturn (GameObject go) {
		if(UIGameResult.Get.IsExpUnlock) {
			UIShow(false);
			UIAchievement.Get.Show(lv);
		} else {
			UILoading.OpenUI = UILoading.OpenStageUI;
			if (isStage)
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			else
				SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
		}
	}

	public void Show (TPlayer beforePlayer, TPlayer afterPlayer){
		UIShow(true);
		lv = afterPlayer.Lv;
		labelLevel.text = afterPlayer.Lv.ToString();
		labelBeforeLevel.text = beforePlayer.Lv.ToString();
		labelAfterLevel.text = afterPlayer.Lv.ToString();
	}

	public bool isStage
	{
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
