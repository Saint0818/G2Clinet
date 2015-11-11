using UnityEngine;

public class UIMasteries : UIBase {
	private static UIMasteries instance = null;
	private const string UIName = "UIMasteries";

	private void Awake()
	{

	}
	
	public void UpdateAvatarModel(TItem[] items)
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
	
	public static UIMasteries Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMasteries;
			
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
