using UnityEngine;
using System.Collections;

public enum SkipSituation{
	Loading,
	Game
}

public class UISkip : UIBase {
	private static UISkip instance = null;
	private const string UIName = "UISkip";

	private GameObject cover;
	private SkipSituation sSituation;

	public static UISkip Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkip;
			
			return instance;
		}
	}
	
	public static bool Visible {
		get {
			if(instance)
				return (instance.gameObject.activeInHierarchy && instance.gameObject.activeSelf);
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow, SkipSituation skipSituation) {
		if(isShow) {
			Get.Show(isShow);
			Get.sSituation = skipSituation;
			if(skipSituation == SkipSituation.Loading)
				Get.cover.SetActive(false);
		}else 
		if(instance) {
			instance.Show(isShow);
			RemoveUI(UIName);
		}
	}

	protected override void OnShow (bool isShow)
	{

	}

	protected override void InitCom() {
		cover = GameObject.Find (UIName + "/Window/Cover");

		SetBtnFun (UIName + "/Window/SkipButton", SkipEvent);
	}

	public void SkipEvent() {
		switch(sSituation) {
			case SkipSituation.Loading:
				SceneMgr.Get.ChangeLevel(SceneName.Court_0);
				break;
			case SkipSituation.Game:
				break;
		}
	}
}
