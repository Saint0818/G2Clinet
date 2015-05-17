using UnityEngine;
using System.Collections;

public class UIMain : UIBase {
	private static UIMain instance = null;
	private const string UIName = "UIMain";
	
	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		}
		else
			if (isShow)
				Get.Show(isShow);
	}
	
	public static UIMain Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMain;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		SetBtnFun(UIName + "/Center/ButtonReset", OnCourt);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void OnCourt() {
		SceneMgr.Get.ChangeLevel(SceneName.Court_0);
	}
}
