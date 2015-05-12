//#define En
#define zh_TW
using UnityEngine;
using System;
using System.Collections;

public class GameStart : MonoBehaviour {

	public static GameStart Get = null;
	private UnibillDemo unibillDemo = null;

	public GameTest TestMode = GameTest.None;
	public CameraTest TestCameraMode = CameraTest.None;
//	public bool IsOpenIKSystem = true;
	public bool IsSplitScreen = false;
	public float CrossTimeX = 0.5f;
	public float CrossTimeZ = 1;
	public TScoreRate ScoreRate = new TScoreRate(1);

	void Start(){
		#if UNITY_EDITOR
			
			#if En
			ParameterConst.GameLanguage = Language.en;
			#endif
			
			#if zh_TW
			GameConst.GameLanguage = Language.zh_TW;
			#endif
		#else
			#if UNITY_ANDROID
			gameObject.AddComponent(Type.GetType("PushNotificationsAndroid"));
			#endif

			#if UNITY_IOS
			gameObject.AddComponent(Type.GetType("PushNotificationsAndroid"));
			#endif
			switch (Application.systemLanguage) {
			case SystemLanguage.English:
				GameConst.GameLanguage = Language.en;
				break;
			case SystemLanguage.Chinese:
				GameConst.GameLanguage = Language.zh_TW;
				break;
			default:
				GameConst.GameLanguage = Language.en;
				break;
			}

		#endif
		
		if (PlayerPrefs.HasKey ("UserLanguage")) {
			int temp = Convert.ToInt16(PlayerPrefs.GetString("UserLanguage"));
			if(temp == Language.en.GetHashCode())
				GameConst.GameLanguage = Language.en;
			else 
			if(temp == Language.zh_TW.GetHashCode())
				GameConst.GameLanguage = Language.zh_TW;
		}
		
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Get = GameObject.Find("UI2D").GetComponent<GameStart>();
		unibillDemo = gameObject.GetComponent<UnibillDemo>();
		InitCom ();
		//CheckVersion ();
		UIGame.UIShow (true);
	}

	private void InitCom(){
		initResolution();
		TextConst.Init ();
		FacebookAPI.Init();
		ModelManager.Init ();
		SceneMgr.Get.ChangeLevel (3);
		GameData.Init ();
	}

	private void initResolution(){
		GameObject ui2d = GameObject.Find("UI2D");

		if (ui2d != null){
			UIRoot root = ui2d.GetComponent<UIRoot>();

			if (root != null){
				float width = 0;
				float height = 0;
				
				if (Screen.width > Screen.height){
					width = Screen.width;
					height = Screen.height;
				} else{
					width = Screen.height;
					height = Screen.width;
				}
				
				int rate = Mathf.CeilToInt(1.6f * 800f * height / width);
				if (rate > 800)
					root.manualHeight = rate;
			}
		}
	}

	public void CheckVersion(){
		SendHttp.Get.Command (URLConst.Version, waitVersion);
	}

	private void waitVersion(bool Value, WWW www){
		if (Value){
			if (www.text.CompareTo(BundleVersion.version) != 1){
				//Login
				Debug.Log("Login");
			}else{
				//Update APK
				Debug.Log("Update APK");
			}
        }
    }
}
