using UnityEngine;
using System.Collections;
using GameStruct;

public class UIAvatarFitted : UIBase {
	private static UIAvatarFitted instance = null;
	private const string UIName = "UIAvatarFitted";
	private const int avatarPartCount = 6;


	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIAvatarFitted Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIAvatarFitted;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}

	void FixedUpdate(){
		
	}

	protected override void InitCom() {
		SetBtnFun (UIName + "/MainView/Left/MainButton/HairBtn", DoHair);
		SetBtnFun (UIName + "/MainView/Left/MainButton/ClothesBtn", DoClosthes);
		SetBtnFun (UIName + "/MainView/Left/MainButton/PantsBtn", DoPants);
		SetBtnFun (UIName + "/MainView/Left/MainButton/ShoesBtn", DoShoes);
		SetBtnFun (UIName + "/MainView/Left/MainButton/HandsBtn", DoHands);
		SetBtnFun (UIName + "/MainView/Left/MainButton/BacksBtn", DoBacks);
		SetBtnFun (UIName + "/MainView/BottomLeft/BackBtn", DoReturn);
		SetBtnFun (UIName + "/MainView/BottomRight/CheckBtn", DoSave);
	}

	private void DoHair()
	{

	}

	private void DoClosthes()
	{

	}

	private void DoPants()
	{

	}

	private void DoShoes()
	{

	}

	private void DoHands()
	{

	}

	private void DoBacks()
	{

	}

	private void DoReturn()
	{
		Show (false);
		UIMain.Visible = true;
	}

	private void DoSave()
	{
		//TODO: save serverdata
		DoReturn ();
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

}
