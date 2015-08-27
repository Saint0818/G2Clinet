using UnityEngine;
using System.Collections;

public class UISimple : UIBase {
	private static UISimple instance = null;
	private const string UIName = "UISimple";

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

	public static UISimple Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISimple;
			
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
