using System;
using UnityEngine;

public class UIMessage : UIBase {
	private static UIMessage instance = null;
	private const string UIName = "UIMessage";
	private UILabel LabelTitle;
	private UILabel LabelMessage;
	private UIButton YesBtn;
	private UIButton NoBtn;
	private UIButton RechargeBtn;
	private int rechargeType;

	private Action<object> YesFunc;
	private Action NoFunc;
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
		RechargeBtn = GameObject.Find("UIMessage/Window/GoRechargeBtn").GetComponent<UIButton>();

		SetBtnFun(ref YesBtn, OnYes);
		SetBtnFun(ref NoBtn, OnNo);
	}

	public void OnYes()
    {
		if(YesFunc != null)
			YesFunc(mExtraInfo);

		if (RechargeBtn.onClick.Count > 1)
			RechargeBtn.onClick.RemoveAt(0);

		AudioMgr.Get.PlaySound(SoundType.SD_Check_Btn);
		UIShow(false);
	}

	public void OnNo(){
		if (NoFunc != null)
			NoFunc ();

		AudioMgr.Get.PlaySound(SoundType.SD_Cancel_Btn);
		UIShow(false);
	}

    public void ShowMessage(string titleStr, string messageStr, EventDelegate.Callback yesEvent, EventDelegate.Callback noEvent = null) {
		UIShow (true);
		YesBtn.gameObject.SetActive(true);
		RechargeBtn.gameObject.SetActive(false);

		if (LabelTitle != null)
			LabelTitle.text = titleStr;

		if (LabelMessage != null)
			LabelMessage.text = messageStr;

		YesBtn.onClick.Insert(0, new EventDelegate(yesEvent));
        if (noEvent != null)
            NoBtn.onClick.Insert(0, new EventDelegate(noEvent));
	}

	public void ShowMessageForBuy (string titleStr, string messageStr, ERechargeType type) {
		UIShow (true);
		YesBtn.gameObject.SetActive(false);
		RechargeBtn.gameObject.SetActive(true);

		if (LabelTitle != null)
			LabelTitle.text = titleStr;

		if (LabelMessage != null)
			LabelMessage.text = messageStr;

		rechargeType = type.GetHashCode();
		SetBtnFun(ref RechargeBtn, OnGoRecharge);
	}
	
	public void ShowMessage(string titleStr, string messageStr, Action<object> yes = null,
                            Action no = null, object extraInfo = null)
	{
		UIShow (true);
		YesBtn.gameObject.SetActive(true);
		RechargeBtn.gameObject.SetActive(false);

		YesFunc = yes;
		NoFunc = no;
	    mExtraInfo = extraInfo;
		if (LabelTitle != null)
			LabelTitle.text = titleStr;

		if (LabelMessage != null)
			LabelMessage.text = messageStr;
	}

	public void OnGoRecharge() {
		UIRecharge.Get.ShowView(rechargeType);
		UIShow(false);
	}
}
