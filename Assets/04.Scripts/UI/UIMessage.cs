using UnityEngine;

public class UIMessage : UIBase {
	private static UIMessage instance = null;
	private const string UIName = "UIMessage";
	private UILabel LabelTitle;
	private UILabel LabelMessage;
	private UIButton YesBtn;
	private UIButton NoBtn;

	private CommonDelegateMethods.Object1 YesFunc;
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

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
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

	public void OnYes()
    {
		if(YesFunc != null)
			YesFunc(mExtraInfo);

		if (YesBtn.onClick.Count > 1)
			YesBtn.onClick.RemoveAt(0);

		UIShow(false);
	}

	public void OnNo(){
		if (NoFunc != null)
			NoFunc ();

		UIShow(false);
	}

	public void ShowMessage(string titleStr, string messageStr, EventDelegate.Callback yesEvent) {
		ShowMessage(titleStr, messageStr);

		YesBtn.onClick.Insert(0, new EventDelegate(yesEvent));
		NoBtn.gameObject.SetActive(false);
	}
	
	public void ShowMessage(string titleStr, string messageStr, CommonDelegateMethods.Object1 yes = null,
                            CommonDelegateMethods.Action no = null, object extraInfo = null)
	{
		UIShow (true);
		NoBtn.gameObject.SetActive(true);

		YesFunc = yes;
		NoFunc = no;
	    mExtraInfo = extraInfo;
		if (LabelTitle != null)
			LabelTitle.text = titleStr;

		if (LabelMessage != null)
			LabelMessage.text = messageStr;
	}
}
