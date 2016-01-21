using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// How to use:
/// <list type="number">
/// <item> Inherit. </item>
/// <item> Make singleton pattern in child class. Singleton instance must be created by LoadUI() or Load3DUI().  </item>
/// <item> Call RemoveUI when UI non-visible. </item>
/// <item> Call SetBtnFun to set callback method. </item>
/// <item> Call Show to control GameObject active flag. </item>
/// </list>
public class UIBase: MonoBehaviour
{  
	protected const string UIPrefab = "Prefab/UI/";
	
	private static GameObject root3D = null;
	private static Dictionary<string, GameObject> UIResources = new Dictionary<string, GameObject>();
	private static Dictionary<string, GameObject> UIDictionary = new Dictionary<string, GameObject>();

	private GameEnum.ELanguage uiLanguage;
	private Dictionary<UILabel, int> labelTextID = new Dictionary<UILabel, int>();

	public static GameObject LoadPrefab(string uiname) {
		if (UIResources.ContainsKey(uiname))
			return UIResources[uiname];
		else {
			GameObject obj = Resources.Load<GameObject>(uiname);
			UIResources.Add(uiname, obj);
			return obj;
		}
	}
	
	public static void AddUI(string uiname, ref GameObject ui) {
		if (!UIDictionary.ContainsKey(uiname))
			UIDictionary.Add(uiname, ui);
	}
	
	public static void RemoveUI(string uiname) {
		if (UIDictionary.ContainsKey(uiname)) {
			NGUIDebug.DestroyImmediate(UIDictionary[uiname], true);
			UIDictionary.Remove(uiname);
			Resources.UnloadUnusedAssets();
		}
	}

	protected static UIBase LoadUI(string path)
	{
		if(!string.IsNullOrEmpty(path))
        {
			UI2D.UIShow(true);
			GameObject obj = LoadPrefab(UIPrefab + path);
			if(obj) {
				GameObject obj2 = Instantiate(obj) as GameObject;
				if(obj2) {
					AddUI(path, ref obj2);
					string[] strChars = path.Split('/'); 
					if(strChars.Length > 0) {
						string UIName = strChars[strChars.Length - 1];                
						obj2.name = UIName;
						UIBase ui = obj2.AddComponent(Type.GetType(UIName)) as UIBase;
						ui.uiLanguage = GameData.Setting.Language;
						ui.initDefaultText(ui.gameObject);
						ui.InitText();
						ui.InitCom();
						ui.InitData();

						obj2.transform.parent = UI2D.Get.gameObject.transform;
						obj2.transform.localPosition = Vector3.zero;
						obj2.transform.localScale = Vector3.one;
						obj2.SetActive(false);

						return ui;
					} else
						Debug.LogError("Split path fail: " + path);
				} else
					Debug.Log("Instantiate fail: " + path);
			} else
				Debug.LogError("Loading prefab fail: " + path);
		}

		return null;
	}

	protected static UIBase Load3DUI(string path)
	{
		if(!string.IsNullOrEmpty(path)){
			GameObject obj = LoadPrefab(UIPrefab + path);
			if(obj) {
				GameObject obj2 = Instantiate(obj) as GameObject;
				if(obj2) {
					AddUI(path, ref obj2);
					string[] strChars = path.Split('/'); 
					if(strChars.Length > 0) {
						string UIName = strChars[strChars.Length - 1];                
						obj2.name = UIName;
						UIBase ui = obj2.AddComponent(Type.GetType(UIName)) as UIBase;
						ui.uiLanguage = GameData.Setting.Language;
						ui.initDefaultText(ui.gameObject);
						ui.InitText();
						ui.InitCom();
						ui.InitData();   

						if (!root3D)
							root3D = GameObject.Find("UI3D");

						if (root3D) {
							obj2.transform.parent = root3D.transform;
							obj2.transform.localEulerAngles = Vector3.zero;
							obj2.transform.localPosition = Vector3.zero;
							obj2.transform.localScale = Vector3.one;
							obj2.SetActive(false);
						} else
							Debug.Log("Can not find root UI3D.");
						return ui;
					} else
						Debug.LogError("Split path fail: " + path);
				} else
					Debug.Log("Instantiate fail: " + path);
			} else
				Debug.LogError("Loading prefab fail: " + path);
		}
		
		return null;
	}

	public static void SetLabel(string path, string text) {
		GameObject obj = GameObject.Find(path);
		if (obj) {
			UILabel lab = obj.GetComponent<UILabel>();
			if (lab) 
				lab.text = text;
			else
				Debug.LogWarning("Can not find component UILabel in " + path);
		} else
			Debug.LogWarning("Can not find path " + path);
	}

	public static void SetBtnFun(string path, EventDelegate.Callback callBack)
	{
		GameObject obj = GameObject.Find(path);
		if (obj) {
			UIButton btn = obj.GetComponent<UIButton>();
			if (btn) 
				btn.onClick.Add(new EventDelegate(callBack));
			else
				Debug.Log("Can not find component UIButton in " + path);
		} else
			Debug.Log("Can not find path " + path);
	}

	public static void SetBtnFunReName(string path, EventDelegate.Callback callBack, string reName = "")
	{
		GameObject obj = GameObject.Find(path);
		if (obj) {
			UIButton btn = obj.GetComponent<UIButton>();
			if (btn) {
				btn.onClick.Add(new EventDelegate(callBack));

				if(reName != string.Empty)
					btn.name = reName;
			}
			else
				Debug.Log("Can not find component UIButton in " + path);
		} else
			Debug.Log("Can not find path " + path);
	}

	public static void SetBtnFun(ref UIButton btn, EventDelegate.Callback callBack)
	{
		if (btn)
		  btn.onClick.Add(new EventDelegate(callBack));	
	}

	protected virtual void Show(bool isShow)
	{		    
	    if(gameObject) {
    	  gameObject.SetActive(isShow);
    	  OnShow(isShow);
		}
  	} 
  
    protected virtual void OnShow(bool isShow)
    {
		if (isShow) {
			if (GameStart.Get.OpenTutorial && GameData.Team.Player.Lv > 0 && GameData.DTutorialUI.ContainsKey(this.name) && !GameData.Team.HaveTutorialFlag(GameData.DTutorialUI[this.name]))
				UITutorial.Get.ShowTutorial(GameData.DTutorialUI[this.name], 1);

			if (this.uiLanguage != GameData.Setting.Language) {
				this.uiLanguage = GameData.Setting.Language;
				this.initDefaultText(this.gameObject);
			}
		}
    }

	public void initDefaultText(GameObject obj) {
		UILabel[] labs = obj.GetComponentsInChildren<UILabel>();
		for (int i = 0; i < labs.Length; i++) {
			int id = 0;
			if (!string.IsNullOrEmpty(labs[i].text) && int.TryParse(labs[i].text, out id) && TextConst.HasText(id)) {
				if (this.labelTextID != null && !this.labelTextID.ContainsKey(labs[i]))
					this.labelTextID.Add(labs[i], id);

				labs[i].text = TextConst.S(id);
			} else
			if (this.labelTextID != null && this.labelTextID.ContainsKey(labs[i]))
				labs[i].text = TextConst.S(this.labelTextID[labs[i]]);
		}
	}

    protected virtual void InitText()
    {
		
    }
  
    /// <summary>
    /// 子類別要設定按鈕的事件.
    /// </summary>
    protected virtual void InitCom()
    {

    }

    protected virtual void InitData()
    {

    }

	/// <summary>
	/// Money is GameCoin
	/// </summary>
	public bool CheckMoney(int money, bool isShowMessage = false, string message = "", EventDelegate.Callback callback = null)
	{
		if(GameData.Team.Money >= money) {
			if (message != null && callback != null)  
				UIMessage.Get.ShowMessage(TextConst.S(249), message, callback);
			return true;
		} else {
			if(isShowMessage)
				UIMessage.Get.ShowMessageForBuy(TextConst.S(237), TextConst.S(239), ERechargeType.Coin);
			return false;
		}
	}

	public void OnBuyMoney() 
	{
		UIRecharge.Get.Show(ERechargeType.Coin.GetHashCode());
		UIMessage.UIShow(false);
	}

	public void SetMoney(int money)
	{
		GameData.Team.Money = money;
	}

	/// <summary>
	/// Diamond
	/// </summary>
    public bool CheckDiamond(int diamond, bool isShowMessage = false, string message = "", EventDelegate.Callback callback = null) 
	{
        if (GameData.Team.Diamond >= diamond) {
            if (message != null && callback != null)  
                UIMessage.Get.ShowMessage(TextConst.S(212), message, callback);
            
			return true;
        } else {
			if(isShowMessage)
				UIMessage.Get.ShowMessageForBuy(TextConst.S(233), TextConst.S(238), ERechargeType.Diamond);
            
			return false;
		}
	}

	public void OnBuyDiamond() {
		UIRecharge.Get.Show(ERechargeType.Diamond.GetHashCode());
		UIMessage.UIShow(false);
	}

	public void SetDiamond(int diamond)
	{
		GameData.Team.Diamond = diamond;
	}
	/// <summary>
	/// Power
	/// </summary>
	public static bool CheckPower(int power, bool isShowMessage = false) 
	{
		if(GameData.Team.Power >= power)
			return true;

        if(isShowMessage)
		    UIMessage.Get.ShowMessageForBuy(TextConst.S(230), TextConst.S(240), ERechargeType.Power);

        return false;
	}

	public void OnBuyPower() {
		UIRecharge.Get.Show(ERechargeType.Power.GetHashCode());
		UIMessage.UIShow(false);
	}

	public void SetPower(int power)
	{
		GameData.Team.Power = power;
	}

    public static string ButtonBG(bool ok) {
        if (ok) 
            return "button_orange1";
        else
            return "button_gray";
    }

    public static Color ButtonTextColor(bool ok) {
        if (ok) 
            return Color.white;
        else
            return Color.red;
    }
}