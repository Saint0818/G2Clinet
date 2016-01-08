using UnityEngine;

public class UIAttributeHint  : UIBase {
	private static UIAttributeHint instance = null;
	private const string UIName = "UIAttributeHint";

	private UILabel labelKind;
	private UILabel labelExplain;
	private UISprite spriteAttr;

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

	public static UIAttributeHint Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIAttributeHint;

			return instance;
		}
	}

	protected override void InitCom() {
		UIEventListener.Get(GameObject.Find(UIName + "/Window/CoverCollider")).onClick = OnClose;

		labelKind = GameObject.Find(UIName + "/Window/AttributeView/AttrKind/KindLabel").GetComponent<UILabel>();
		labelExplain = GameObject.Find(UIName + "/Window/AttributeView/AttrKind/ExplainLabel").GetComponent<UILabel>();
		spriteAttr = GameObject.Find(UIName + "/Window/AttributeView/AttrKind/AttrKind").GetComponent<UISprite>();
	}

	public void UpdateView (int kind) {
		UIShow (true);
		labelKind.text = TextConst.S(3300 + kind + 1);
		labelExplain.text = TextConst.S(3320 + kind + 1);
		spriteAttr.spriteName = "AttrKind_" + kind;
	}

	public void OnClose (GameObject go) {
		UIShow(false);
	}
}
