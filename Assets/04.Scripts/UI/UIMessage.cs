using UnityEngine;
using System.Collections;

public class UIMessage : UIBase {
	private static UIMessage instance = null;
	private const string UIName = "UIMessage";
	private UILabel LabelTitle;
	private UILabel LabelMessage;

	public static void UIShow(bool isShow){
		if(instance) {
            if (!isShow)
                RemoveUI(UIName);
            else
			    instance.Show(isShow);
        }
		else
		if(isShow)
			Get.Show(isShow);
	}

	public static UIMessage Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMessage;
			
			return instance;
		}
	}

	protected override void InitCom() {
		LabelTitle = GameObject.Find("UIMessage/Window/Title").GetComponent<UILabel>();
		LabelMessage = GameObject.Find("UIMessage/Window/Label").GetComponent<UILabel>();
		SetBtnFun("UIMessage/Window/Finish", OnClose);
	}

	protected override void InitText(){
		SetLabel (UIName + "/Window/Title", TextConst.S(139));
		SetLabel (UIName + "/Window/Finish/UILabel", TextConst.S(136));
	}

	public void OnClose() {
//		string text = LabelMessage.text;
//		if (text == TextConst.S (7)) {
//			if (!UILoading.Visible)
//				UIController.Get.OnLogout();
//
//			if (!UIController.Get.VersionChecked)
//				UIController.Get.CheckVersion();
//			else
//				UIController.Get.SendLogin();
//		} else
//		if (text == TextConst.S (33) || text == TextConst.S(93) || text == TextConst.S(101)) {
//			UIController.Get.OnLogout();
//			UIController.Get.CheckVersion();
//		} 

        UIShow(false);
	}

	public void ShowMessage (string title, string text) {
//		UIWaitingHttp.UIShow(false);
//		UIShow (true);
//		LabelMessage.text = text;
//
//		if (title == "")
//			LabelTitle.text = TextConst.S(139);
//		else
//			LabelTitle.text = title;
	}
}
