using UnityEngine;

public class UIPlayerPotential : UIBase {
	private static UIPlayerPotential instance = null;
	private const string UIName = "UIPlayerPotential";

	private void Awake()
	{

	}
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIPlayerPotential Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIPlayerPotential;
			
			return instance;
		}
	}
	
	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow){
				RemoveUI(UIName);
			}
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}
	
	protected override void InitCom() {


	}

	public void OnReturn()
	{
		UIShow (false);
	}


	protected override void InitData() {

	}


	protected override void OnShow(bool isShow) {

	}
}
