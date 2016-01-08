using UnityEngine;

public class UIAchievement : UIBase {
	private static UIAchievement instance = null;
	private const string UIName = "UIAchievement";
	private UILabel descLabel;
	private int playerLv;
	
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
	
	public static UIAchievement Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIAchievement;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		descLabel = GameObject.Find(UIName + "/Center/BottomView/StageTargetView/DescLabel").GetComponent<UILabel>();
		UIEventListener.Get(GameObject.Find(UIName + "/Center/Mask")).onClick = OnReturn;
	}

	public void Show (int lv){
		UIShow(true);
		playerLv = lv;
		descLabel.text = TextConst.S(GameData.DExpData[lv].UnlockName);
	}

	public void OnReturn (GameObject go) {
		if (isStage) {
			UILoading.AchievementUI(playerLv);
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
		} else
			SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
	}

	public bool isStage
	{
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
