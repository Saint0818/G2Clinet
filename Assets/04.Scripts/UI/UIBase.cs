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
}