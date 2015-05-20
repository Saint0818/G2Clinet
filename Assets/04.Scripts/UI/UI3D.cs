using UnityEngine;
using System.Collections;

public enum UIKind
{
	CreateRole,
	PlayerShow,
	GameResult,
}

public class UI3D : UIBase {
	private static UI3D instance = null;
	private const string UIName = "UI3D";
	private Camera Camera3D;
	private bool following = false;
	private float followTime = 2f;
	private GameObject followPos;

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

	protected override void InitCom() {
		Camera3D = GameObject.Find(UIName + "/3DCamera").GetComponent<Camera>();
	}

	public void Open3DUI(UIKind kind)
	{
		ShowCamera(true);

		switch (kind) {
			case UIKind.CreateRole:
 				transform.eulerAngles = new Vector3(0, 90, 0);
//				UICreateRole.UIShow(true);
//				gameObject.transform.position = new Vector3 (-12, 4, -4.6f);
				gameObject.transform.position = new Vector3 (0, 4, 0);
				UICreateRole.Get.gameObject.transform.localEulerAngles = Vector3.zero;

				Camera3D.transform.localPosition = new Vector3(0, 0, -700);
				Camera3D.cullingMask = (1 << LayerMask.NameToLayer("UI3D")) | (1 << LayerMask.NameToLayer("Default"));
				Camera3D.fieldOfView = 60;
				break;
			case UIKind.PlayerShow:
				gameObject.transform.position = Vector3.zero;
				transform.eulerAngles = new Vector3(0, 90, 0);
				
				Camera3D.transform.localPosition = new Vector3(0, 0, -700);
				Camera3D.cullingMask = (1 << LayerMask.NameToLayer("UI3D"));
				Camera3D.fieldOfView = 60;
				break;
			case UIKind.GameResult:
				Camera3D.transform.localPosition = Vector3.zero;
				Camera3D.gameObject.SetActive(true);
				Camera3D.transform.localEulerAngles = Vector3.zero;
				Camera3D.cullingMask = (1 << LayerMask.NameToLayer("UI3D")) | (1 << LayerMask.NameToLayer("Default"));
				Camera3D.fieldOfView = 30;
		
				break;
		}
	}

	public void ShowCamera(bool isShow)
	{
		UIShow(isShow);
		Camera3D.gameObject.SetActive (isShow);
	}

	private Vector3 rotateTo;

	void Update()
	{
		if (following && followPos) 
		{
			followTime -= Time.deltaTime;

			if (gameObject.transform.position != followPos.transform.position)
				gameObject.transform.position = Vector3.Slerp (gameObject.transform.position, followPos.transform.position, 0.1f);

			if (gameObject.transform.eulerAngles != followPos.transform.eulerAngles)
				gameObject.transform.localEulerAngles = Vector3.Slerp (gameObject.transform.localEulerAngles, followPos.transform.eulerAngles, 0.3f);

			if(followTime <= 0)
			{
				following = false;
				gameObject.transform.position = followPos.transform.position;
				gameObject.transform.localEulerAngles = followPos.transform.eulerAngles;
			}
		}
	}

}
