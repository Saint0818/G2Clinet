using UnityEngine;
using System;
using System.Collections;

public class UILoading : UIBase {
	private static UILoading instance = null;
	private const string UIName = "UILoading";

	private GameObject windowLoading;
	private GameObject windowGame;

	private UITexture uiLoadingProgress;

	private UITexture uiBG;
	private UITexture uiGameProgress;

	private ELoadingGamePic loadingKind;
	private int PicNo = -1;
	private bool isCloseUI = false;
	public float CloseTime = 0;
	//	private UILabel Hint;

	private int page = 1;
	private int dir = 0;
	private bool wasDragging = false;

	public static bool Visible{
		get{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static UILoading Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UILoading;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow, ELoadingGamePic kind = ELoadingGamePic.SelectRole, string hint=""){
//		Get.isCloseUI = isShow;
		if(isShow) {
			Get.loadingKind = kind;
			Get.Show(isShow);
			
			if (Get.PicNo != kind.GetHashCode()) {
				Get.windowGame.SetActive(true);
				Get.windowLoading.SetActive(false);
				if (Get.uiBG.mainTexture) {
					Get.uiBG.mainTexture = null;
					Resources.UnloadUnusedAssets();
				}
				
				Get.PicNo = kind.GetHashCode();
				Get.uiBG.mainTexture = (Texture)Resources.Load("Textures/LoadingPic/Loading" + Get.PicNo.ToString(), typeof(Texture));
			} else {
				Get.windowGame.SetActive(false);
				Get.windowLoading.SetActive(true);
			}

//			if (hint != "")
//				Get.Hint.text = hint;
//			else
//				Get.Hint.text = "";
		}else 
		if(instance) {
			if (Get.CloseTime <= 0) {
				instance.Show(isShow);
				RemoveUI(UIName);
			}
		}
	}

	protected override void OnShow(bool isShow) {
		if (isShow)
			StartCoroutine(DoLoading(loadingKind));
	}

//	void FixedUpdate () {
//		if (CloseTime > 0) {
//			CloseTime -= Time.deltaTime;
//			
//			if (CloseTime <=0 && !isCloseUI) {
//				//UIShow(false);
//			}
//		}
//	}
	
	protected override void InitCom() {
		windowLoading = GameObject.Find (UIName + "/WindowLoading");
		windowGame = GameObject.Find (UIName + "/WindowGame");

		uiLoadingProgress = GameObject.Find (UIName + "/WindowLoading/LoadingPic/UIProgressBar").GetComponent<UITexture>();

		uiBG = GameObject.Find (UIName + "/WindowGame/BG").GetComponent<UITexture>();
		uiGameProgress = GameObject.Find (UIName + "/WindowGame/LoadingPic/UIProgressBar").GetComponent<UITexture>();

		UIEventListener.Get(uiBG.gameObject).onDrag = PanelDrag;
		UIEventListener.Get(uiBG.gameObject).onPress = PanelPress;
	}

	protected override void InitData() {
		uiLoadingProgress.fillAmount = 0;
		uiGameProgress.fillAmount = 0;
	}
	
	IEnumerator DoLoading(ELoadingGamePic kind = ELoadingGamePic.SelectRole) {
		switch (kind) {
		case ELoadingGamePic.SelectRole:
			yield return new WaitForSeconds (1);
			ModelManager.Get.LoadAllBody("Character/");
			ModelManager.Get.LoadAllTexture("Character/");
			yield return new WaitForSeconds (2);
			loadSelectRole();
			break;
		case ELoadingGamePic.Game:

			break;
		}
	}
	
	public void UpdateProgress (){
		float b = FileManager.DownlandCount;
		float a = FileManager.AlreadyDownlandCount;
		uiLoadingProgress.fillAmount = (float)(a / b);
	}
	
	private void loadSelectRole(){
		UIShow(false);
		CameraMgr.Get.SetSelectRoleCamera();
		UISelectRole.UIShow(true);
		UI3DSelectRole.UIShow(true);
	}

	public void PanelDrag(GameObject go, Vector2 delta) {
		wasDragging = true;
		if(delta.x > 0)
			dir = -1;
		else
			dir = 1;
	}

	public void PanelPress(GameObject go, bool pressed) {
		if (!pressed && wasDragging) {
			wasDragging = false;
			if(dir == 1)
				DoRight();
			else
				DoLeft();
			dir = 0;
		}
	}
	
	public void DoRight() {
		if(page == 1) {
			page ++;
		}
	}
	
	public void DoLeft() {
		if(page == 2) {
			page --;
		}
	}

	public float ProgressValue{
		get{return uiLoadingProgress.fillAmount;}
	}

	public bool DownloadDone{
		get{return uiLoadingProgress.fillAmount >= 1;}
	}
}
