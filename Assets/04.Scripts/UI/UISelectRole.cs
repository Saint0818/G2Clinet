using UnityEngine;
using System.Collections;

public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";
	
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
		if(instance)
			instance.Show(isShow);
		else
			if(isShow)
				Get.Show(isShow);
	}
	
	public static UISelectRole Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISelectRole;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}
}

