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
	public float BasicMoveSpeed = 1;
	public float DefSpeedup = 8.2f;
	public float DefSpeedNormal = 7.2f;
	public float BallOwnerSpeedup = 7.5f;
	public float BallOwnerSpeedNormal = 6.5f;
	public float AttackSpeedup = 8;
	public float AttackSpeedNormal = 7;

	void Start(){
		#if UNITY_EDITOR
		#if En
		ParameterConst.GameLanguage = Language.en;
		#endif
		
		#if zh_TW
		ParameterConst.GameLanguage = Language.zh_TW;
		#endif
		#else
		switch (Application.systemLanguage) {
		case SystemLanguage.English:
			ParameterConst.GameLanguage = Language.en;
			break;
		case SystemLanguage.Chinese:
			ParameterConst.GameLanguage = Language.zh_TW;
			break;
		default:
			ParameterConst.GameLanguage = Language.en;
			break;
		}
		#endif
		
		if (PlayerPrefs.HasKey ("UserLanguage")) {
			int temp = Convert.ToInt16(PlayerPrefs.GetString("UserLanguage"));
			if(temp == Language.en.GetHashCode())
				ParameterConst.GameLanguage = Language.en;
			else if(temp == Language.zh_TW.GetHashCode())
				ParameterConst.GameLanguage = Language.zh_TW;
		}
		
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Get = GameObject.Find("UI2D").GetComponent<GameStart>();
		unibillDemo = gameObject.GetComponent<UnibillDemo>();
		InitCom ();
		//CheckVersion ();
		FileManager.Get.LoadGameData();
		UIGame.UIShow (true);
	}

	private void InitCom(){
		initResolution();
		TextConst.Init ();
		SendHttp.Init ();
		FacebookAPI.Init();
		ModelManager.Init ();
		SceneMgr.Get.ChangeLevel (3);
		ParameterConst.Init ();
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
