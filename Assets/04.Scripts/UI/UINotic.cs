using System;
using GameEnum;
using UnityEngine;

public class UINotic : UIBase {
	private static UINotic instance = null;
    private const string UIName = "UINotic";

    public GameObject webViewGameObject;
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
		if (UITutorial.Visible)
			return;

		if(instance) {
			if (!isShow) {
                if (Get.webViewGameObject)
                    Destroy(Get.webViewGameObject);
                
				RemoveUI(UIName);
			} else
				instance.Show(isShow);
		} else
		if(isShow)
			Get.Show(isShow);
	}
	
	public static UINotic Get{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UINotic;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
        initWebView();

		SetBtnFun (UIName + "/Window/TopRight/Exit", Close);
		SetBtnFun (UIName + "/Window/BottomLeft/Check", OnNoLongger);

		toggleDaily = GameObject.Find (UIName + "/Window/BottomLeft/Check").GetComponent<UIToggle>();
		toggleDaily.value = false;
		int check = PlayerPrefs.GetInt(ESave.NoticDaily.ToString(), 0);
		if (check == 1)
			toggleDaily.value = true;
	}

    private void initWebView() {
        webViewGameObject = GameObject.Find("WebView");
        if (webViewGameObject == null)
            webViewGameObject = new GameObject("WebView");

        var webView = webViewGameObject.AddComponent<UniWebView>();
        webView.OnLoadComplete += OnLoadComplete;
        webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
        webView.toolBarShow = true;

        string host = "http://nicemarket.com.tw/";
        //string host = "http://localhost:3300/";
        string url = string.Format(host + "notic?game={0}&company={1}&os={2}&language={3}&version={4}", "g2", 
            GameData.Company, GameData.OS, GameData.Setting.Language.ToString(), GameData.SaveVersion);

        Debug.Log (url);
        webView.url = url;
        webView.Load();
    }

    UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation) {
        return new UniWebViewEdgeInsets(60,0,60,0);
    }

    void OnLoadComplete(UniWebView webView, bool success, string errorMessage) {
        if (success) {
            webView.Show();
        } else {
            Debug.Log("Something wrong in webview loading: " + errorMessage);
        }
    }

	public void Close(){
		UIShow (false);
	}

	public void OnNoLongger() {
		int check = 0;
		if (toggleDaily.value)
			check = 1;

		PlayerPrefs.SetInt(ESave.NoticDaily.ToString(), check);
	}
	
	protected override void OnShow(bool isShow) {
		if(isShow){
			int day = DateTime.Now.Day;
			PlayerPrefs.SetInt(ESave.NoticDate.ToString(), day);
		}	
	}
}
