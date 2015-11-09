using UnityEngine;

public class UIPlayerInfo : UIBase {
	private static UIPlayerInfo instance = null;
	private const string UIName = "UIPlayerInfo";
	private GameObject[] PageAy = new GameObject[3];
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIPlayerInfo Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIPlayerInfo;
			
			return instance;
		}
	}
	
	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}
	
	protected override void InitCom() {
	 	UIButton[] BtnAy;
		for (int i = 0; i < PageAy.Length; i++) {
			PageAy[i] = GameObject.Find(string.Format("Page{0}", i));
			BtnAy = PageAy[i].GetComponents<UIButton>();
		}
	}

	public void OnSwitchPage()
	{

	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}
}
