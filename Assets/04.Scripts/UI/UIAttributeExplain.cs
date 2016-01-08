using UnityEngine;

public class UIAttributeExplain  : UIBase {
	private static UIAttributeExplain instance = null;
	private const string UIName = "UIAttributeExplain";

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

	public static UIAttributeExplain Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIAttributeExplain;

			return instance;
		}
	}

	protected override void InitCom() {
		UIEventListener.Get(GameObject.Find(UIName + "/Center/View/NoBtn")).onClick = OnClose;
		UIEventListener.Get(GameObject.Find(UIName + "/Center/View/CoverCollider")).onClick = OnClose;
	}

	public void OnClose (GameObject go) {
		UIShow(false);
	}
}
