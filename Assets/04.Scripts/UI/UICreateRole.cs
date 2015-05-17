using UnityEngine;
using System.Collections;

public class UICreateRole : UIBase {
	private static UICreateRole instance = null;
	private const string UIName = "UICreateRole";
	
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
	
	public static UICreateRole Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UICreateRole;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		SetBtnFun (UIName + "/Center/ButtonStart", OnCreateRole);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void OnCreateRole() {
		WWWForm form = new WWWForm();
		form.AddField("PlayerID", 1);
		
		SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
	}

	private void waitCreateRole(bool ok, WWW www) {
		if (ok) {
			UIShow(false);
			LobbyStart.Get.EnterLobby();
		}
	}
}
