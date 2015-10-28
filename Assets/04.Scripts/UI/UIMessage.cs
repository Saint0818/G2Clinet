using UnityEngine;

public class UIMessage : UIBase {
	private static UIMessage instance = null;
	private const string UIName = "UIMessage";
	private UILabel LabelTitle;
	private UILabel LabelMessage;
	private UIButton YesBtn;
	private UIButton NoBtn;

	private CommonDelegateMethods.Action3 YesFunc;
	private CommonDelegateMethods.Action NoFunc;
    private object mExtraInfo;

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
		LabelTitle = GameObject.Find("UIMessage/Window/HighlightFrame/TitleLabel").GetComponent<UILabel>();
		LabelMessage = GameObject.Find("UIMessage/Window/HighlightFrame/Contents/ContentsLabel").GetComponent<UILabel>();

		YesBtn = GameObject.Find("UIMessage/Window/CheckBtn").GetComponent<UIButton>();
		NoBtn = GameObject.Find("UIMessage/Window/NoBtn").GetComponent<UIButton>();

		SetBtnFun(ref YesBtn, OnYes);
		SetBtnFun(ref NoBtn, OnNo);
	}

	protected override void InitText(){
		SetLabel (UIName + "/Window/Title", TextConst.S(139));
		SetLabel (UIName + "/Window/Finish/UILabel", TextConst.S(136));
	}

	public void OnYes()
    {
		if(YesFunc != null)
			YesFunc(mExtraInfo);

		UIShow(false);
	}

	public void OnNo(){
		if (NoFunc != null)
			NoFunc ();

		UIShow(false);
	}
	
	public void ShowMessage(string titleStr, string messageStr, CommonDelegateMethods.Action3 yes = null,
                            CommonDelegateMethods.Action no = null, object extraInfo = null)
    {
		YesFunc = yes;
		NoFunc = no;
	    mExtraInfo = extraInfo;

		UIShow (true);

//		if(YesBtn && NoBtn){
//			NoBtn.enabled = NoFunc == null? false : true;
//		}

		if (LabelTitle != null)
			LabelTitle.text = titleStr;

		if (LabelMessage != null)
			LabelMessage.text = messageStr;
	}

	protected override void OnShow(bool isShow) {
		if(isShow){

		}
	}
}
