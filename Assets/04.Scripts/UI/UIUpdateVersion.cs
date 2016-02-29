using UnityEngine;

public class UIUpdateVersion : UIBase {
	private static UIUpdateVersion instance = null;
	private const string UIName = "UIUpdateVersion";

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
        if(isShow) {
            GameData.Setting.Language = (GameEnum.ELanguage)PlayerPrefs.GetInt(GameEnum.ESave.UserLanguage.ToString());
			Get.Show(isShow);
        }
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
		SetBtnFun (UIName + "/BottomRight/Notic", OnNotic);
        SetBtnFun (UIName + "/Background/Download", OnDownload);
        
        /*if (GameData.OS == EOS.IOS) 
			uiAppleStore.SetActive(true);
		else {
			uiGooglePlay.SetActive(true);
			uiOfficialWebsite.SetActive(true);
		}*/
	}

	protected override void InitText(){
		SetLabel (UIName + "/Background/Title", TextConst.S(604) + GameData.ServerVersion);

        if (GameData.Company == GameEnum.ECompany.NiceMarket)
            SetLabel (UIName + "/Background/LabelFrom", TextConst.S(606));
        else
            SetLabel (UIName + "/Background/LabelFrom", TextConst.S(605));
	}

    public void OnDownload() {
        Application.OpenURL(URLConst.DownLoadURL);
    }

	public void OnNotic(){
        UINotic.Visible = true;
	}
}

