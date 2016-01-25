using UnityEngine;

public enum UIKind
{
	CreateRole,
	PlayerShow,
	GameResult,
}

public class UI3D : UIBase {
	private static UI3D instance = null;
	private const string UIName = "UI3D";
	private bool following = false;
	private float followTime = 2f;
	private GameObject followPos = null;

	public static void UIShow(bool isShow){
		if(instance)
			instance.Show(isShow);
		else
		if(isShow)
			Get.Show(isShow);
	}

	public static UI3D Get
	{
		get {
			if (!instance) 
				instance = Load3DUI(UIName) as UI3D;
			
			return instance;
		}
	}

	public static bool Visible
	{
		get {
			if (instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
}
