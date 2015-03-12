using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;

public class UIBase: MonoBehaviour
{  
	protected const string UIPrefab = "Prefab/UI/";

	private static GameObject root2D = null;
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
			//Destroy(UIDictionary[uiname]);
			NGUIDebug.DestroyImmediate(UIDictionary[uiname], true);
			UIDictionary.Remove(uiname);
			Resources.UnloadUnusedAssets();

//			if (UITutorialHint.Visible){
//				UITutorialHint.UIShow(false);
//				UITutorial.Get.OnTutorial();
//			}
		}
	}

	protected static UIBase LoadUI(string path)
	{
		if(path != null && path != ""){
			GameObject obj = LoadPrefab(UIPrefab + path);
			if(obj) {
				GameObject obj2 = Instantiate(obj) as GameObject;
				if(obj2) {
					AddUI(path, ref obj2);
					string[] strChars = path.Split(new char[] {'/'}); 
					if(strChars.Length > 0) {
						string UIName = strChars[strChars.Length - 1];                
						obj2.name = UIName;
						UIBase ui = obj2.AddComponent(Type.GetType(UIName)) as UIBase;
//						UIBase ui = obj2.AddComponent(UIName) as UIBase;
						ui.InitText();
						ui.InitCom();
						ui.InitData();

						if (!root2D)
							root2D = GameObject.Find("UI2D");

						if (root2D) {
							obj2.transform.parent = root2D.transform;
							obj2.transform.localPosition = Vector3.zero;
							obj2.transform.localScale = Vector3.one;
							obj2.SetActive(false);
						} else
							Debug.Log("Can not find root UI2D.");

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
		if(path != null && path != ""){
			GameObject obj = LoadPrefab(UIPrefab + path);
			if(obj) {
				GameObject obj2 = Instantiate(obj) as GameObject;
				if(obj2) {
					AddUI(path, ref obj2);
					string[] strChars = path.Split(new char[] {'/'}); 
					if(strChars.Length > 0) {
						string UIName = strChars[strChars.Length - 1];                
						obj2.name = UIName;
						UIBase ui = obj2.AddComponent(Type.GetType(UIName)) as UIBase;
//						UIBase ui = obj2.AddComponent(UIName) as UIBase;
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

	public static void SetBtnFun(ref UIButton btn, EventDelegate.Callback callBack)
	{
		if (btn)
		  btn.onClick.Add(new EventDelegate(callBack));	
	}

	protected bool checkNetwork() {
		#if UNITY_EDITOR
		if (Network.player.ipAddress != "127.0.0.1" && Network.player.ipAddress != "0.0.0.0")
			return true;
		#else
		#if UNITY_IPHONE
		if (iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
			return true;
		#endif
		#if UNITY_ANDROID
		if (iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
			return true;
		#endif
		#if (!UNITY_IPHONE && !UNITY_ANDROID)
		if (Network.player.ipAddress != "127.0.0.1" && Network.player.ipAddress != "0.0.0.0")
			return true;
		#endif
		#endif
		
		UIMessage.Get.ShowMessage("", TextConst.S (93));
		return false;
	}

	bool waitclose = false;
	protected virtual void Show(bool isShow)
	{		    
	    if(gameObject) {
			waitclose = false;

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
  
  protected virtual void InitCom()
  {

  }

  protected virtual void InitData()
  {

  }
}