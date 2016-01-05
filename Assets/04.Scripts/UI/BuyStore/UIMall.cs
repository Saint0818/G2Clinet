using UnityEngine;
using System.Collections;

public class UIMall : UIBase {
	private static UIMall instance = null;
	private const string UIName = "UIMall";

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
//			instance.Show(isShow);
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);	
	}

	public static UIMall Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMall;

			return instance;
		}
	}

	protected override void InitCom() {

	}
}
