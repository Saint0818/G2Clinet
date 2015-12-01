using UnityEngine;

public class UIWaitingHttp : UIBase {
	private static UIWaitingHttp instance = null;
	private const string UIName = "UIWaitingHttp";
	private GameObject UILoadingHint;
	private GameObject SpriteLoading;
	private UILabel labelLoadingHint;
	private GameObject buttonSend;
	private GameObject buttonClose;
	private int resendCount = 0;
	private float sendedTime = 0;
	private string waitingURL;
	private TBooleanWWWObj waitingCallback = null;
	private WWWForm waitingForm = null;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public void ShowResend(string text) {
		if (waitingCallback != null) {
			SpriteLoading.SetActive(false);
			buttonSend.SetActive(true);
			UILoadingHint.SetActive(true);
			labelLoadingHint.text = text;
			resendCount++;
			if (resendCount > 3)
				buttonClose.SetActive(true);
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
				ShowResend(TextConst.S (507));
		}
	}

	public static void UIShow(bool isShow){
		if(isShow) {
			Get.Show(isShow);
		} else 
		if(instance) {
			instance.Show(isShow);
			instance.buttonClose.SetActive(false);
			instance.waitingCallback = null;
			instance.waitingForm = null;
			instance.sendedTime = 0;
			instance.resendCount = 0;
		}
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

		buttonSend = GameObject.Find(UIName + "/Window/Send");
		SetBtnFun(UIName + "/Window/Send", OnResend);
		buttonSend.SetActive(false);
		buttonClose = GameObject.Find(UIName + "/TopRight/NoBtn");
		SetBtnFun(UIName + "/TopRight/NoBtn", OnClose);
		buttonClose.SetActive(false);
	}

	public void OnResend() {
		if (waitingCallback != null) {
			if (SendHttp.Get.CheckNetwork(false)) 
				SendHttp.Get.Command(waitingURL, waitingCallback, waitingForm);
			else
				labelLoadingHint.text = TextConst.S (506);

			resendCount ++;
			if (resendCount >= 3)
				buttonClose.SetActive(true);
		} else
			UIShow(false);
	}

	public void OnClose() {
		UIShow(false);
	}

	public void SaveProtocol(string url, TBooleanWWWObj callback, WWWForm form = null) {
		waitingURL = url;
		waitingCallback = callback;
		waitingForm = form;

		UIShow(true);
		SpriteLoading.SetActive(true);
		UILoadingHint.SetActive(false);
		buttonSend.SetActive(false);
		buttonClose.SetActive(false);
		sendedTime = 15;

		if (UILoading.Visible)
			SpriteLoading.SetActive(false);
	}

	public bool KeepAlive 
	{
		get {
			return sendedTime > 15;
		}
	}
}
