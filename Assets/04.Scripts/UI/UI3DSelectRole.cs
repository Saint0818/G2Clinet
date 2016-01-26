public class UI3DSelectRole : UIBase {
	private static UI3DSelectRole instance = null;
	private const string UIName = "UI3DSelectRole";

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow) {
		if (instance) {
            if (!isShow)
				RemoveUI(UIName);
            else
				instance.Show(isShow);
		} else
		if (isShow) 
			Get.Show(isShow);
	}
	
	public static UI3DSelectRole Get {
		get {
            if (!instance)
				instance = Load3DUI(UIName) as UI3DSelectRole;

			return instance;
		}
	}

	protected override void InitCom() {
	
	}


}
