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
	private GameObject loadingPic;
	private GameObject buttonNext;
	private UITexture uiBG;
	private UITexture uiLoadingProgress;
	private TweenRotation loadingRotation;
	private GameObject[] pageOn = new GameObject[3];
	private GameObject[] viewLoading = new GameObject[3];
	private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

	private ELoadingGamePic loadingKind;
	private int PicNo = -2;
	private int pageLoading = 0;
	private float startTimer = 0;

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
		} else 
		if(instance) {
			Get.Show(false);
			//RemoveUI(UIName);
		}
	}

	protected override void OnShow(bool isShow) {
		if (isShow)
			StartCoroutine(DoLoading(loadingKind));
	}

	private void initLoadingPic(ELoadingGamePic kind = ELoadingGamePic.SelectRole, string hint="") {
		loadingKind = kind;
		startTimer = Time.time;
		if (kind == ELoadingGamePic.Game) {
			windowGame.SetActive(true);
			windowLoading.SetActive(false);
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
		SetBtnFun(UIName + "/WindowGame/Pages/P1Button", OnChangePage);
		SetBtnFun(UIName + "/WindowGame/Pages/P2Button", OnChangePage);
		SetBtnFun(UIName + "/WindowGame/Pages/P3Button", OnChangePage);
		SetBtnFun(UIName + "/WindowGame/Right/Next", OnNext);

		windowLoading = GameObject.Find (UIName + "/WindowLoading");
		windowGame = GameObject.Find (UIName + "/WindowGame");
		pageOn[0] = GameObject.Find (UIName + "/WindowGame/Pages/P1Button/Onpage");
		pageOn[1] = GameObject.Find (UIName + "/WindowGame/Pages/P2Button/Onpage");
		pageOn[2] = GameObject.Find (UIName + "/WindowGame/Pages/P3Button/Onpage");
		viewLoading [0] = GameObject.Find (UIName + "/WindowGame/Loading1"); 
		viewLoading [1] = GameObject.Find (UIName + "/WindowGame/Loading2"); 
		viewLoading [2] = GameObject.Find (UIName + "/WindowGame/Loading3"); 
		buttonNext = GameObject.Find (UIName + "/WindowGame/Right/Next");
		buttonNext.SetActive(false);

		loadingPic = GameObject.Find (UIName + "/LoadingPic");
		uiLoadingProgress = GameObject.Find (UIName + "/LoadingPic/UIProgressBar").GetComponent<UITexture>();
		uiBG = GameObject.Find (UIName + "/WindowGame/BG").GetComponent<UITexture>();
		loadingRotation = GameObject.Find (UIName + "/LoadingPic/UILight1").GetComponent<TweenRotation>();

		windowGame.SetActive(false);
		windowLoading.SetActive(false);
		loadingPic.SetActive(true);
		pageOn[1].SetActive(false);
		pageOn[2].SetActive(false);
		viewLoading[1].SetActive(false);
		viewLoading[2].SetActive(false);
	}

	protected override void InitData() {
		uiLoadingProgress.fillAmount = 0;
	}
	
	IEnumerator DoLoading(ELoadingGamePic kind = ELoadingGamePic.SelectRole) {
		float minWait = 2;
		float maxWait = 4;
		float waitTime = 1;
		yield return new WaitForSeconds (1);

		switch (kind) {
		case ELoadingGamePic.SelectRole:
			AudioMgr.Get.StartGame();
			yield return new WaitForSeconds (0.2f);
			ModelManager.Get.LoadAllSelectPlayer(GameConst.SelectRoleID);

			waitTime = Mathf.Max(minWait, maxWait - Time.time + startTimer);
			yield return new WaitForSeconds (waitTime);
			loadSelectRole();

			break;
		case ELoadingGamePic.Login:

			break;
		case ELoadingGamePic.CreateRole:
			UICreateRole.Get.ShowPositionView();
			UI3DCreateRole.Get.PositionView.PlayDropAnimation();

			waitTime = Mathf.Max(minWait, maxWait - Time.time + startTimer);
			yield return new WaitForSeconds (waitTime);
			UIShow(false);

			break;
		case ELoadingGamePic.Lobby:
			UIMainLobby.Get.Show();

			if (UI3D.Visible)
				UI3D.Get.ShowCamera(false);

			AudioMgr.Get.PlayMusic(EMusicType.MU_game1);
			waitTime = Mathf.Max(minWait, maxWait - Time.time + startTimer);
			yield return new WaitForSeconds (waitTime);
			UIShow(false);

			break;
		case ELoadingGamePic.Game:
			GameController.Get.ChangeSituation(EGameSituation.None);
			yield return new WaitForSeconds (0.2f);
			CourtMgr.Get.InitCourtScene ();
			yield return new WaitForSeconds (0.2f);
			AudioMgr.Get.StartGame();
			yield return new WaitForSeconds (0.2f);
			int rate = UnityEngine.Random.Range(0, 2);
			if(rate == 0)
				AudioMgr.Get.PlayMusic(EMusicType.MU_game0);
			else
				AudioMgr.Get.PlayMusic(EMusicType.MU_game1);

			waitTime = Mathf.Max(minWait, maxWait - Time.time + startTimer);
			yield return new WaitForSeconds (waitTime);

			buttonNext.SetActive(true);
			loadingPic.SetActive(false);

			break;
		case ELoadingGamePic.Stage:
			loadStage();
			break;
		}
	}
	
	public void UpdateProgress (){
		float b = FileManager.DownlandCount;
		float a = FileManager.AlreadyDownlandCount;
		uiLoadingProgress.fillAmount = (float)(a / b);
	}

	private Texture2D loadTexture(string path) {
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

	private void loadSelectRole() {
		CameraMgr.Get.SetSelectRoleCamera();
		UISelectRole.UIShow(true);
		UI3DSelectRole.UIShow(true);

		UIShow(false);
	}

	private void loadStage() {
		UIShow(false);
		CameraMgr.Get.SetSelectRoleCamera();
		UISelectRole.UIShow(true);
		UISelectRole.Get.SelectFriendMode();
		UI3DSelectRole.UIShow(true);
	}

	private void showPage (int page) {
		uiBG.mainTexture = loadTexture("Textures/LoadingPic/Loading" + (page+1).ToString());
		for (int i=0; i < viewLoading.Length; i++) {
			bool show = (i == page) ?true:false;
			viewLoading[i].SetActive(show); 
			pageOn[i].SetActive(show);
		}
	}

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

	public void OnChangePage() {
		int index = -1;
		if (int.TryParse(UIButton.current.name.Substring(1, 1), out index)) {
			pageLoading = index-1;
			showPage(pageLoading);
		}
	}

	public void OnNext() {
		UIShow(false);
		CameraMgr.Get.SetCameraSituation(ECameraSituation.Show);
	}

	public float ProgressValue{
		get{return uiLoadingProgress.fillAmount;}
	}

	public bool DownloadDone{
		get{return uiLoadingProgress.fillAmount >= 1;}
	}
}
