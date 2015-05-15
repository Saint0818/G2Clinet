using UnityEngine;
using System.Collections;

public class UIWaitingHttp : UIBase {
	private static UIWaitingHttp instance = null;
	private const string UIName = "UIWaitingHttp";
	private GameObject UILoadingHint;
	private GameObject SpriteLoading;
	private UILabel labelLoadingHint;
	private GameObject ButtonSend;
	private int resendCount = 0;
	private float sendedTime = 0;
	private string waitingURL;
	private TBooleanWWWObj waitingCallback = null;
	private WWWForm waitingForm = null;

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

	public void ShowResend() {
		if (waitingCallback != null) {
			SpriteLoading.SetActive(false);
			ButtonSend.SetActive(true);
			UILoadingHint.SetActive(true);
			labelLoadingHint.text = TextConst.S(328);
		}
	}

	public void WaitForCheckSession() {
		if (waitingCallback != null) 
			sendedTime = 30;
		else
			UIShow(false);
	}

	void FixedUpdate() {
		if (Visible && gameObject.activeInHierarchy && sendedTime > 0) {
			sendedTime -= Time.deltaTime;

			if (sendedTime <= 0)
				ShowResend();
		}
	}

	public static void UIShow(bool isShow){
		if(isShow) 
			Get.Show(isShow);
		else 
		if(instance) 
			instance.Show(isShow);
	}

	public void ReleaseUI() {
		RemoveUI(UIName);
	}

	public static UIWaitingHttp Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIWaitingHttp;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		UILoadingHint = GameObject.Find(UIName + "/Window/LoadingHint");
		SpriteLoading = GameObject.Find(UIName + "/Window/SpriteLoading");
		labelLoadingHint = GameObject.Find(UIName + "/Window/LoadingHint").GetComponent<UILabel>();
		labelLoadingHint.text = TextConst.S(138);

		ButtonSend = GameObject.Find(UIName + "/Window/Send");
		SetBtnFun(UIName + "/Window/Send", OnResend);
		ButtonSend.SetActive(false);
	}

	protected override void InitText(){
		SetLabel (UIName + "/Window/LoadingHint", TextConst.S(138));
		SetLabel (UIName + "/Window/Send/UILabel", TextConst.S(20214));
	}

	protected override void Show(bool isShow) {
		if(isShow) {
			ButtonSend.SetActive(isShow);
			SpriteLoading.SetActive(isShow);
			UILoadingHint.SetActive(isShow);
			labelLoadingHint.text = TextConst.S(138);
		} else {
			waitingCallback = null;
			waitingForm = null;
			sendedTime = 0;
			resendCount = 0;
		}
	}

	public void OnResend() {
		if (waitingCallback != null) {
			SendHttp.Get.Command(waitingURL, waitingCallback, waitingForm);
			resendCount ++;
			if (resendCount >= 5)
				Show(false);
		}
		else
			UIShow(false);
	}

	public void SaveProtocol(string url, TBooleanWWWObj callback, WWWForm form = null) {
		waitingURL = url;
		waitingCallback = callback;
		waitingForm = form;

		UIShow(true);
		SpriteLoading.SetActive(true);
		UILoadingHint.SetActive(false);
		ButtonSend.SetActive(false);
		sendedTime = 15;
	}

	public bool KeepAlive 
	{
		get {
			return sendedTime > 15;
		}
	}
}
