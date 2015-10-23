using UnityEngine;

public class UIUpdate : UIBase {
	private static UIUpdate instance = null;
	private const string UIName = "UIUpdate";
	private UILabel UpdateText;
	
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
	
	public static UIUpdate Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIUpdate;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		UpdateText = GameObject.Find (UIName + "/Center/ButtonUpdate/Label").GetComponent<UILabel>();
		SetBtnFun (UIName + "/Center/ButtonUpdate", DoUpdate);
	}
	
	protected override void InitData() {
		#if UNITY_EDITOR
		UpdateText.text = TextConst.S(10);
		#else
		#if UNITY_IOS
		UpdateText.text = TextConst.S(11);
		#endif
		#if UNITY_ANDROID
		UpdateText.text = TextConst.S(10);
		#endif
		#if (!UNITY_IOS && !UNITY_ANDROID)
		UpdateText.text = TextConst.S(10);
		#endif
		#endif
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void DoUpdate()
	{
		Application.OpenURL ("https://play.google.com/store/apps/details?id=com.nicemarket.nbaa");
	}
}

