using UnityEngine;

public class UIAchievement : UIBase {
	private static UIAchievement instance = null;
	private const string UIName = "UIAchievement";
	private UILabel descLabel;
	
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

	private float delayWaitTime = 10;
	void FixedUpdate () {
		if(delayWaitTime > 0) {
			delayWaitTime -= Time.deltaTime;
			if(delayWaitTime <= 0)
				OnReturn(null);
		}
	}

	public void ShowView (int lv){
		UIShow(true);
		if(GameData.DExpData.ContainsKey(lv)) {
			if(GameData.DExpData[lv].UnlockName != 0)
				descLabel.text = TextConst.S(GameData.DExpData[lv].UnlockName);
			else 
				OnReturn(null);
		}
	}

	public void OnReturn (GameObject go) {
        SceneMgr.Get.ChangeLevel(ESceneName.Lobby);

//		if (isStage) {
//			UILoading.AchievementUI(playerLv);
//			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
//		} else
//			SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
	}

	public bool isStage
	{
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
