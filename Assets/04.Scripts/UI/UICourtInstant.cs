using UnityEngine;

public class UICourtInstant : UIBase {
	private static UICourtInstant instance = null;
	private const string UIName = "UICourtInstant";

	private UILabel labelInstant;
	
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
					RemoveUI(UIName);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}
	
	public static UICourtInstant Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UICourtInstant;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow, string text = ""){
		if(isShow) {
			Get.labelInstant.text = text;
			Get.Show((!string.IsNullOrEmpty(text)));
			Get.labelInstant.gameObject.SetActive(false);
			Get.labelInstant.gameObject.SetActive(true);
		}else 
		if(instance) {
			Get.Show(false);
		}
	}
	
	protected override void InitCom() {
		labelInstant = GameObject.Find (UIName + "/Top/InstantLabel").GetComponent<UILabel>();
	}
}
