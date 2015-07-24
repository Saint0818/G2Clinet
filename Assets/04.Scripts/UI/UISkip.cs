using UnityEngine;
using System.Collections;

public enum ESkipSituation{
	Loading,
	Game
}

public class UISkip : UIBase {
	private static UISkip instance = null;
	private const string UIName = "UISkip";

	private GameObject cover;
	private ESkipSituation sSituation;

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
	
	public static void UIShow(bool isShow, ESkipSituation skipSituation = ESkipSituation.Loading) {
		if(isShow) {
			Get.Show(isShow);
			Get.sSituation = skipSituation;
			if(skipSituation == ESkipSituation.Loading)
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
			case ESkipSituation.Loading:
				UIShow(false);
				SceneMgr.Get.ChangeLevel(SceneName.Court_0);
				break;
			case ESkipSituation.Game:
				GameController.Get.SkipShow();
				UIShow(false);
				break;
		}
	}
}
