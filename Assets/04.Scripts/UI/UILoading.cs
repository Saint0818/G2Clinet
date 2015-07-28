using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UILoading : UIBase {
	private static UILoading instance = null;
	private const string UIName = "UILoading";

	private GameObject windowLoading;
	private GameObject windowGame;
	private GameObject[] pageOn = new GameObject[3];
	private GameObject[] viewLoading = new GameObject[3];
	private Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();
//	private GameObject buttonSkip;
	private GameObject loadingPic;

	private UITexture uiLoadingProgress;

	private UITexture uiBG;
	private UITexture uiGameProgress;
	
	private ELoadingGamePic loadingKind;
	private int PicNo = -1;
	private bool isCloseUI = false;
	public float CloseTime = 0;
	//	private UILabel Hint;

	private int pageLoading = 0;
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
				Get.uiBG.mainTexture = Get.loadTexture("Textures/LoadingPic/Loading" + Get.PicNo.ToString());
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
		pageOn[0] = GameObject.Find (UIName + "/WindowGame/Pages/P1Button/Onpage");
		pageOn[1] = GameObject.Find (UIName + "/WindowGame/Pages/P2Button/Onpage");
		pageOn[2] = GameObject.Find (UIName + "/WindowGame/Pages/P3Button/Onpage");
		viewLoading [0] = GameObject.Find (UIName + "/WindowGame/Loading1"); 
		viewLoading [1] = GameObject.Find (UIName + "/WindowGame/Loading2"); 
		viewLoading [2] = GameObject.Find (UIName + "/WindowGame/Loading3"); 
//		buttonSkip = GameObject.Find (UIName + "WindowGame/SkipButton");
		loadingPic = GameObject.Find (UIName + "/WindowGame/LoadingPic");

		uiLoadingProgress = GameObject.Find (UIName + "/WindowLoading/LoadingPic/UIProgressBar").GetComponent<UITexture>();

		uiBG = GameObject.Find (UIName + "/WindowGame/BG").GetComponent<UITexture>();
		uiGameProgress = GameObject.Find (UIName + "/WindowGame/LoadingPic/UIProgressBar").GetComponent<UITexture>();

		SetBtnFun(UIName + "/WindowGame/Pages/P1Button", Point1);
		SetBtnFun(UIName + "/WindowGame/Pages/P2Button", Point2);
		SetBtnFun(UIName + "/WindowGame/Pages/P3Button", Point3);

		UIEventListener.Get(uiBG.gameObject).onDrag = PanelDrag;
		UIEventListener.Get(uiBG.gameObject).onPress = PanelPress;

		windowGame.SetActive(false);
		windowLoading.SetActive(false);
		loadingPic.SetActive(true);
		pageOn[1].SetActive(false);
		pageOn[2].SetActive(false);
		viewLoading[1].SetActive(false);
		viewLoading[2].SetActive(false);
//		buttonSkip.SetActive(false);
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
			yield return new WaitForSeconds (2);
//			buttonSkip.SetActive(true);
			UISkip.UIShow(true, ESkipSituation.Loading);
			loadingPic.SetActive(false);
			yield return new WaitForSeconds (10);
			UIShow(false);
			UISkip.UIShow(false, ESkipSituation.Loading);
			CameraMgr.Get.SetCameraSituation(ECameraSituation.Show);
//			SceneMgr.Get.ChangeLevel(SceneName.Court_0);
			break;
		}
	}
	
	public void UpdateProgress (){
		float b = FileManager.DownlandCount;
		float a = FileManager.AlreadyDownlandCount;
		uiLoadingProgress.fillAmount = (float)(a / b);
	}

	private Texture loadTexture(string path) {
		if (textureCache.ContainsKey(path)) {
			return textureCache [path];
		}else {
			Texture2D obj = Resources.Load(path) as Texture2D;
			if (obj) {
				textureCache.Add(path, obj);
				return obj;
			} else {
				//download form server
				return null;
			}
		}
	}

	private void loadSelectRole(){
		UIShow(false);
		CameraMgr.Get.SetSelectRoleCamera();
		UISelectRole.UIShow(true);
		UI3DSelectRole.UIShow(true);
	}

	private void showPage (int page) {
		for (int i=0; i<viewLoading.Length; i++) {
			bool show = (i == page) ?true:false;
			viewLoading[i].SetActive(show); 
			pageOn[i].SetActive(show);
		}
		uiBG.mainTexture = Get.loadTexture("Textures/LoadingPic/Loading" + (page+1).ToString());
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
		if(pageLoading < 2) {
			pageLoading ++;
			showPage(pageLoading);
		} 
	}
	
	public void DoLeft() {
		if(pageLoading > 0) {
			pageLoading --;
			showPage(pageLoading);
		}
	}

	public void Point1(){
		pageLoading = 0;
		showPage(pageLoading);
	}

	public void Point2(){
		pageLoading = 1;
		showPage(pageLoading);
	}

	public void Point3(){
		pageLoading = 2;
		showPage(pageLoading);
	}

	public float ProgressValue{
		get{return uiLoadingProgress.fillAmount;}
	}

	public bool DownloadDone{
		get{return uiLoadingProgress.fillAmount >= 1;}
	}
}
