using UnityEngine;
using System;
using System.Collections;
using GameEnum;

public class UIAnnouncement : UIBase {
	private static UIAnnouncement instance = null;
	private const string UIName = "UIAnnouncement";

	public GameObject uiAnnouncement;
	private UIToggle toggleDaily;
	
	public static bool Visible{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if(instance) {
			if (!isShow) {
				Destroy(Get.uiAnnouncement);
				RemoveUI(UIName);
			} else
				instance.Show(isShow);
		} else
		if(isShow)
			Get.Show(isShow);
	}
	
	public static UIAnnouncement Get{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIAnnouncement;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		GameObject obj = Resources.Load<GameObject>("Prefab/UniWebViewAnnouncement");
		if (obj) {
			UniWebView view = obj.GetComponent<UniWebView>();
			if (view) {
				view.url = GameData.UrlAnnouncement;
				
				uiAnnouncement = Instantiate(obj) as GameObject;
				uiAnnouncement.transform.parent = gameObject.transform;
				uiAnnouncement.transform.localPosition = Vector3.zero;
				uiAnnouncement.transform.localScale = Vector3.one;
			}
		}

		SetBtnFun (UIName + "/Window/Center/Exit", Close);
		SetBtnFun (UIName + "/Window/Center/Check", OnNoLongger);

		toggleDaily = GameObject.Find (UIName + "/Window/Center/Check").GetComponent<UIToggle>();
		toggleDaily.value = false;
		int check = PlayerPrefs.GetInt(ESave.AnnouncementDaily.ToString(), 0);
		if (check == 1)
			toggleDaily.value = true;
	}

	public void Close(){
		UIShow (false);
	}

	public void OnNoLongger() {
		int check = 0;
		if (toggleDaily.value)
			check = 1;

		PlayerPrefs.SetInt(ESave.AnnouncementDaily.ToString(), check);
	}
	
	protected override void OnShow(bool isShow) {
		if(isShow){
			int day = DateTime.Now.Day;
			PlayerPrefs.SetInt(ESave.AnnouncementDate.ToString(), day);
		}	
	}
}
