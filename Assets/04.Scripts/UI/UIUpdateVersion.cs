using UnityEngine;
using System.Collections;
using GameEnum;

public class UIUpdateVersion : UIBase {
	private static UIUpdateVersion instance = null;
	private const string UIName = "UIUpdateVersion";

	private GameObject uiGooglePlay;
	private GameObject uiAppleStore;
	private GameObject uiOfficialWebsite;

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
		if(instance) {
			if (!isShow) 
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
		if(isShow)
			Get.Show(isShow);
	}
	
	public static UIUpdateVersion Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIUpdateVersion;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		SetBtnFun (UIName + "/Background/Download", NiceMarket);
		SetBtnFun (UIName + "/Background/Googleplay", GooglePaly);
		SetBtnFun (UIName + "/Background/AppStore", AppStore);
		SetBtnFun (UIName + "/BottomRight/Announcement", OnAnnouncement);

		uiGooglePlay = GameObject.Find(UIName + "/Background/Googleplay");
		uiOfficialWebsite = GameObject.Find(UIName + "/Background/Download");
		uiAppleStore = GameObject.Find(UIName + "/Background/AppStore");

		uiGooglePlay.SetActive(false);
		uiAppleStore.SetActive(false);
		uiOfficialWebsite.SetActive(true);
        
        /*if (GameData.OS == EOS.IOS) 
			uiAppleStore.SetActive(true);
		else {
			uiGooglePlay.SetActive(true);
			uiOfficialWebsite.SetActive(true);
		}*/
	}

	protected override void InitText(){
		SetLabel (UIName + "/Background/Title", TextConst.S(604) + GameData.ServerVersion);
	}

	public void AppStore(){
		Application.OpenURL(URLConst.AppStore);
	}

	public void GooglePaly(){
		Application.OpenURL(URLConst.GooglePlay);
	}

	public void NiceMarket(){
		Application.OpenURL (URLConst.NiceMarketApk);
	}

	public void OnAnnouncement(){
		Application.OpenURL (GameData.UrlAnnouncement);
	}
}

