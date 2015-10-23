using GameEnum;
using UnityEngine;

public class UIUpdateVersion : UIBase {
	private static UIUpdateVersion instance = null;
	private const string UIName = "UIUpdateVersion";

	private GameObject uiGooglePlay;
	private GameObject uiAppleStore;
	private GameObject uiOfficialWebsite;
	private GameObject uiMyGamez;

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
		SetBtnFun (UIName + "/Window/Background/Download", NiceMarket);
		SetBtnFun (UIName + "/Window/Background/Googleplay", GooglePaly);
		SetBtnFun (UIName + "/Window/Background/AppStore", AppStore);
		SetBtnFun (UIName + "/Window/Background/MyGamez/Offline", OnOffline);
		SetBtnFun (UIName + "/Window/Background/MyGamez/Quit", OnQuit);

		uiGooglePlay = GameObject.Find(UIName + "/Window/Background/Googleplay");
		uiOfficialWebsite = GameObject.Find(UIName + "/Window/Background/Download");
		uiAppleStore = GameObject.Find(UIName + "/Window/Background/AppStore");
		uiMyGamez = GameObject.Find(UIName + "/Window/Background/MyGamez");

		uiGooglePlay.SetActive(false);
		uiOfficialWebsite.SetActive(false);
		uiAppleStore.SetActive(false);
		uiMyGamez.SetActive(false);

		if (GameData.OS == EOS.IOS) 
			uiAppleStore.SetActive(true);
		else 
		if (GameData.Company == ECompany.MyGamez) {
			uiMyGamez.SetActive(true);
		} else {
			uiGooglePlay.SetActive(true);
			uiOfficialWebsite.SetActive(true);
		}
	}

	protected override void InitText(){
		SetLabel (UIName + "/Window/Background/Title", TextConst.S(14) + GameData.ServerVersion);
		SetLabel (UIName + "/Window/Background/Googleplay/Label", TextConst.S(20209));
		SetLabel (UIName + "/Window/Background/Download/Label", TextConst.S(20210));
		SetLabel (UIName + "/Window/Background/AppStore/Label", TextConst.S(1043));
		SetLabel (UIName + "/Window/Background/MyGamez/Quit/Label", TextConst.S(1081));
		SetLabel (UIName + "/Window/Background/MyGamez/Offline/Label", TextConst.S(1082));
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

	public void OnOffline(){
		UIShow(false);
	}

	public void OnQuit(){

	}
}

