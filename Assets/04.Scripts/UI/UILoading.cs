using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameEnum;

public class UILoading : UIBase {
	private static UILoading instance = null;
	private const string UIName = "UILoading";

	private GameObject windowLoading;
	private GameObject windowGame;
	private GameObject[] pageOn = new GameObject[3];
	private GameObject[] viewLoading = new GameObject[3];
	private Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();
	private GameObject loadingPic;

	private UITexture uiLoadingProgress;

	private UITexture uiBG;
	private UITexture uiGameProgress;
	
	private ELoadingGamePic loadingKind;
	private int PicNo = -1;
//	private bool isCloseUI = false;
	public float CloseTime = 0;

	private int pageLoading = 0;
//	private int dir = 0;
//	private bool wasDragging = false;

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
		if(isShow) {
			Get.initLoadingPic(kind, hint);
			Get.Show(true);
		}else 
		if(instance) {
			if (Get.CloseTime <= 0) {
				Get.CancelInvoke("ChangePage");
				instance.Show(isShow);
				RemoveUI(UIName);
			}
		}
	}

	protected override void OnShow(bool isShow) {
		if (isShow)
			StartCoroutine(DoLoading(loadingKind));
	}

	private void initLoadingPic(ELoadingGamePic kind = ELoadingGamePic.SelectRole, string hint="") {
		loadingKind = kind;
		
		if (PicNo != kind.GetHashCode()) {
			windowGame.SetActive(true);
			windowLoading.SetActive(false);
			
			PicNo = kind.GetHashCode();
			Texture2D txt = loadTexture("Textures/LoadingPic/Loading" + PicNo.ToString()) as Texture2D;
			if (txt) {
				uiBG.mainTexture = txt;
				//uiBG.width = txt.width;
				//uiBG.height = txt.height;
			}

		} else {
			windowGame.SetActive(false);
			windowLoading.SetActive(true);
		}
		
		//			if (hint != "")
		//				Hint.text = hint;
		//			else
		//				Hint.text = "";
	}

	protected override void InitCom() {
		windowLoading = GameObject.Find (UIName + "/WindowLoading");
		windowGame = GameObject.Find (UIName + "/WindowGame");
		pageOn[0] = GameObject.Find (UIName + "/WindowGame/Pages/P1Button/Onpage");
		pageOn[1] = GameObject.Find (UIName + "/WindowGame/Pages/P2Button/Onpage");
		pageOn[2] = GameObject.Find (UIName + "/WindowGame/Pages/P3Button/Onpage");
		viewLoading [0] = GameObject.Find (UIName + "/WindowGame/Loading1"); 
		viewLoading [1] = GameObject.Find (UIName + "/WindowGame/Loading2"); 
		viewLoading [2] = GameObject.Find (UIName + "/WindowGame/Loading3"); 
		loadingPic = GameObject.Find (UIName + "/WindowGame/LoadingPic");

		uiLoadingProgress = GameObject.Find (UIName + "/WindowLoading/LoadingPic/UIProgressBar").GetComponent<UITexture>();

		uiBG = GameObject.Find (UIName + "/WindowGame/BG").GetComponent<UITexture>();
		uiGameProgress = GameObject.Find (UIName + "/WindowGame/LoadingPic/UIProgressBar").GetComponent<UITexture>();

		SetBtnFun(UIName + "/WindowGame/Pages/P1Button", Point1);
		SetBtnFun(UIName + "/WindowGame/Pages/P2Button", Point2);
		SetBtnFun(UIName + "/WindowGame/Pages/P3Button", Point3);

//		UIEventListener.Get(uiBG.gameObject).onDrag = PanelDrag;
//		UIEventListener.Get(uiBG.gameObject).onPress = PanelPress;

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
			yield return new WaitForEndOfFrame();
			ModelManager.Get.LoadAllSelectPlayer(GameConst.SelectRoleID);
			yield return new WaitForSeconds (1);
			loadSelectRole();
			break;
		case ELoadingGamePic.Game:
			Invoke("ChangePage", 2);
			yield return new WaitForEndOfFrame();
			UISkip.UIShow(true, ESkipSituation.Loading);
			loadingPic.SetActive(false);
			yield return new WaitForSeconds (10);
			UIShow(false);
			UISkip.UIShow(false, ESkipSituation.Loading);
			CameraMgr.Get.SetCameraSituation(ECameraSituation.Show);
			break;
		case ELoadingGamePic.Stage:
			yield return new WaitForSeconds (2);
			loadStage();
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

		if (GameStart.Get.OpenGameMode) 
			UIGameMode.UIShow (true);
		else {
			CameraMgr.Get.SetSelectRoleCamera();
			UISelectRole.UIShow(true);
			UI3DSelectRole.UIShow(true);
		}
	}

	private void loadStage() {
		UIShow(false);
		CameraMgr.Get.SetSelectRoleCamera();
		UISelectRole.UIShow(true);
		UISelectRole.Get.SelectFriendMode();
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

//	public void PanelDrag(GameObject go, Vector2 delta) {
//		wasDragging = true;
//		if(delta.x > 0)
//			dir = -1;
//		else
//			dir = 1;
//	}
//
//	public void PanelPress(GameObject go, bool pressed) {
//		if (!pressed && wasDragging) {
//			wasDragging = false;
//			if(dir == 1)
//				DoRight();
//			else
//				DoLeft();
//			dir = 0;
//		}
//	}

	public void ChangePage(){
		if(pageLoading < 2) {
			pageLoading++;
		} else {
			pageLoading = 0;
		}
		showPage(pageLoading);
		Invoke("ChangePage", 2);
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
