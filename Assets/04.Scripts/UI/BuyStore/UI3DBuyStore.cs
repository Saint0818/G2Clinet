using UnityEngine;

public class UI3DBuyStore :  UIBase {
	private static UI3DBuyStore instance = null;
	private const string UIName = "UI3DBuyStore";

	private Animator animator3DBuy;

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
			if (isShow) {
				Get.Show(isShow);
			}
	}

	public static UI3DBuyStore Get {
		get {
			if (!instance) 
				instance = Load3DUI(UIName) as UI3DBuyStore;

			return instance;
		}
	}

	protected override void InitCom() {
		animator3DBuy = GetComponent<Animator>();
	}

	public void Show (){
		UIShow(true);
	}

	public void StartRaffle () {
		animator3DBuy.SetTrigger("Go");
	}

	public void AgainRaffle() {
		animator3DBuy.SetTrigger("Again");
	}

}
