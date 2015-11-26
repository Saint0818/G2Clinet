using UnityEngine;
using System.Collections;

public class UIGameLoseResult : UIBase {
	private static UIGameLoseResult instance = null;
	private const string UIName = "UIGameLoseResult";
	
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
	
	public static UIGameLoseResult Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGameLoseResult;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/StatsNextLabel")).onClick = OnReturn;
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void OnReturn (GameObject go) {
		Time.timeScale = 1;
		UIShow(false);
		UILoading.OpenUI = UILoading.OpenStageUI;
		if (isStage)
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
		else
			SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
	}

	public bool isStage
	{
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
