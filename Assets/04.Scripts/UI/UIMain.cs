using UnityEngine;
using System.Collections;

public class UIMain : UIBase {
	private static UIMain instance = null;
	private const string UIName = "UIMain";
	private GameObject[] EffectSwitch = new GameObject[2];
	
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
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		}
		else
			if (isShow)
				Get.Show(isShow);
	}
	
	public static UIMain Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMain;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		SetBtnFun(UIName + "/Center/ButtonReset", OnCourt);
		UIEventListener.Get (GameObject.Find (UIName + "/TopLeft/ButtonEffect")).onClick = DoEffectSwitch;

		EffectSwitch [0] = GameObject.Find (UIName + "/TopLeft/ButtonEffect/LabelON");
		EffectSwitch [1] = GameObject.Find (UIName + "/TopLeft/ButtonEffect/LabelOff");

		EffectSwitch [0].SetActive (GameData.Setting.Effect);
		EffectSwitch [1].SetActive (!GameData.Setting.Effect);
	}

	public void DoEffectSwitch(GameObject obj)
	{
		GameData.Setting.Effect = !GameData.Setting.Effect;
		EffectSwitch [0].SetActive (GameData.Setting.Effect);
		EffectSwitch [1].SetActive (!GameData.Setting.Effect);

		int index = 0;

		if (GameData.Setting.Effect)
			index = 1;

		CourtMgr.Get.EffectEnable (GameData.Setting.Effect);

		PlayerPrefs.SetInt (SettingText.Effect, index);
		PlayerPrefs.Save ();
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void OnCourt() {

		SceneMgr.Get.ChangeLevel(SceneName.Court_0);
	}
}
