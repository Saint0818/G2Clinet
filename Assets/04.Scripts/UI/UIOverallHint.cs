using UnityEngine;
using System.Collections;

public class UIOverallHint : UIBase {
	private static UIOverallHint instance = null;
	private const string UIName = "UIOverallHint";

	private UILabel overallHint;

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
					RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static UIOverallHint Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIOverallHint;

			return instance;
		}
	}

	protected override void InitCom() {
		overallHint = GameObject.Find(UIName + "/Window/Bottom/OverallView/OverallLabel").GetComponent<UILabel>();
		//UIEventListener.Get(GameObject.Find(UIName + "/CoverCollider")).onClick = OnClose;
	}

	public void ShowView (string value ) {
		Visible = true;
		overallHint.text = value;
	}

	public void OnClose (GameObject go) {
		Visible = false;
	}
}
