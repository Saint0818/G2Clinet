using System.Collections;
using System.Collections.Generic;
using GameEnum;
using UnityEngine;

public class UILoading : UIBase {
	private static UILoading instance = null;
	private const string UIName = "UILoading";

	private GameObject windowLoading;
	private GameObject windowStage;
	private GameObject loadingPic;
	private GameObject buttonNext;
	private UITexture uiBG;
	private UITexture uiLoadingProgress;
	private UILabel labelLoading;
	private UILabel labelStageTitle;
	private UILabel labelStageExplain;
	private UILabel labelTip;
	private GameObject[] pageOn = new GameObject[3];
	private GameObject[] viewLoading = new GameObject[3];
	private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

	public static EventDelegate.Callback OpenUI = null;
	private ELoading loadingKind;
	private bool closeAfterFinished = false;
	private int pageLoading = 0;
	private float nowProgress;
	private float toProgress;
	private float startTimer = 0;
	private float loadingTimer = 0;
	private float textCount = 0;
	private string loadingText = "";

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

	//Open stage after battle
	public static void OpenStageUI() {
		UIMainStage.Get.Show ();
		UIMainLobby.Get.Hide ();
	}

	public static void UIShow(bool isShow, ELoading kind = ELoading.SelectRole){
		if(isShow) {
			Get.initLoadingPic(kind);
			Get.Show(true);
		} else 
		if(instance) { 
			if (Get.LoadingFinished)
				Get.Show(false);
			else
				Get.closeAfterFinished = true;
			//RemoveUI(UIName);
		}
	}

	void FixedUpdate() {
		if (!LoadingFinished) {
			loadingTimer += Time.deltaTime;
			if (nowProgress < toProgress) {
				nowProgress += Time.deltaTime;
				if (nowProgress > toProgress)
					nowProgress = toProgress;

				uiLoadingProgress.fillAmount = nowProgress;
			}

			if (loadingTimer >= 0.2f) {
				loadingTimer = 0;
				textCount++;
				loadingText = "";
				for (int i = 0; i < textCount; i++)
					loadingText += ".";

				labelLoading.text = TextConst.S(10106) + loadingText;

				if (textCount > 2)
					textCount = 0;
			}

			if (closeAfterFinished)
				UIShow(false);
		} else 
		if (!string.IsNullOrEmpty(labelLoading.text))
			labelLoading.text = "";
	}

	protected override void OnShow(bool isShow) {
		if (isShow)
			StartCoroutine(doLoading(loadingKind));
	}

	private void initLoadingPic(ELoading kind = ELoading.SelectRole) {
		loadingKind = kind;
		startTimer = Time.time;
		closeAfterFinished = false;
		nowProgress = 0;
		ProgressValue = 0;
		if (kind == ELoading.Game) {
			windowStage.SetActive(true);
			windowLoading.SetActive(false);

			TStageData data = StageTable.Ins.GetByID(GameData.StageID);
			labelStageTitle.text = data.Name;
			labelStageExplain.text = data.Explain;
			labelTip.text = TextConst.S(UnityEngine.Random.Range(301, 303));
		} else {
			windowStage.SetActive(false);
			windowLoading.SetActive(true);
		}
	}

	protected override void InitCom() {
		loadingPic = GameObject.Find (UIName + "/LoadingPic");
		uiLoadingProgress = GameObject.Find (UIName + "/LoadingPic/UIProgressBar").GetComponent<UITexture>();
		labelLoading = GameObject.Find (UIName + "/LoadingPic/UIWord").GetComponent<UILabel>();
		windowLoading = GameObject.Find (UIName + "/WindowLoading");
		windowStage = GameObject.Find (UIName + "/StageInfo");
		labelTip = GameObject.Find (UIName + "/StageInfo/Bottom/Tip").GetComponent<UILabel>();
		labelStageTitle = GameObject.Find (UIName + "/StageInfo/Center/SingalStage/StageNameLabel").GetComponent<UILabel>();
		labelStageExplain = GameObject.Find (UIName + "/StageInfo/Center/SingalStage/StageExplainLabel").GetComponent<UILabel>();
		uiBG = GameObject.Find (UIName + "/StageInfo/Center/StageKindTexture").GetComponent<UITexture>();
		buttonNext = GameObject.Find (UIName + "/StageInfo/Right/Next");
		buttonNext.SetActive(false);
		windowStage.SetActive(false);
		windowLoading.SetActive(false);
		loadingPic.SetActive(true);
		/*
		SetBtnFun(UIName + "/WindowGame/Pages/P1Button", OnChangePage);
		SetBtnFun(UIName + "/WindowGame/Pages/P2Button", OnChangePage);
		SetBtnFun(UIName + "/WindowGame/Pages/P3Button", OnChangePage);
		pageOn[0] = GameObject.Find (UIName + "/WindowGame/Pages/P1Button/Onpage");
		pageOn[1] = GameObject.Find (UIName + "/WindowGame/Pages/P2Button/Onpage");
		pageOn[2] = GameObject.Find (UIName + "/WindowGame/Pages/P3Button/Onpage");
		viewLoading [0] = GameObject.Find (UIName + "/WindowGame/Loading1"); 
		viewLoading [1] = GameObject.Find (UIName + "/WindowGame/Loading2"); 
		viewLoading [2] = GameObject.Find (UIName + "/WindowGame/Loading3");
		loadingRotation = GameObject.Find (UIName + "/LoadingPic/UILight1").GetComponent<TweenRotation>();
		pageOn[1].SetActive(false);
		pageOn[2].SetActive(false);
		viewLoading[1].SetActive(false);
		viewLoading[2].SetActive(false);
		*/
	}
	
	IEnumerator doLoading(ELoading kind = ELoading.SelectRole) {
		float minWait = 2;
		float maxWait = 4;
		float waitTime = 1;

		if (kind != ELoading.Login)
			ProgressValue = 0.3f;

		yield return new WaitForSeconds (1);

		switch (kind) {
		case ELoading.SelectRole:
			AudioMgr.Get.StartGame();
			yield return new WaitForSeconds (0.2f);
			ProgressValue = 1;

			waitTime = Mathf.Max(minWait, maxWait - Time.time + startTimer);
			yield return new WaitForSeconds (waitTime);
			loadSelectRole();

			break;
		case ELoading.Login:
			ProgressValue = 1;
			break;
		case ELoading.CreateRole:
			UICreateRole.Get.ShowPositionView();
			UI3DCreateRole.Get.PositionView.PlayDropAnimation();
			ProgressValue = 0.7f;

			waitTime = Mathf.Max(minWait, maxWait - Time.time + startTimer);
			yield return new WaitForSeconds (waitTime);

			UIShow(false);

			break;
		case ELoading.Lobby:
			ProgressValue = 1;
			UIMainLobby.Get.Show();

			if (UI3D.Visible)
				UI3D.Get.ShowCamera(false);

			AudioMgr.Get.PlayMusic(EMusicType.MU_game1);
			waitTime = Mathf.Max(minWait, maxWait - Time.time + startTimer);
			yield return new WaitForSeconds (waitTime);

			if (GameData.Team.Player.Lv == 0) {
				UICreateRole.Get.ShowPositionView();
				UI3DCreateRole.Get.PositionView.PlayDropAnimation();
				UIMainLobby.Get.Hide ();
			} else 
			if (OpenUI != null) {
				OpenUI();
				OpenUI = null;
			}

			UIShow(false);

			break;
		case ELoading.Game:
			GameController.Get.ChangeSituation(EGameSituation.None);
			ProgressValue = 0.6f;
			yield return new WaitForSeconds (0.2f);
			ProgressValue = 1;
			CourtMgr.Get.InitCourtScene ();
			AudioMgr.Get.StartGame();
			waitTime = Mathf.Max(minWait, maxWait - Time.time + startTimer);
			yield return new WaitForSeconds (waitTime);

			if (GameStart.Get.TestMode == EGameTest.None) {
				GameController.Get.LoadStage(GameData.StageID);
			} else {
				CourtMgr.Get.ShowEnd();
				GameController.Get.LoadStage(1);
				GameController.Get.InitIngameAnimator();
				GameController.Get.SetBornPositions();
				GameController.Get.ChangeSituation(EGameSituation.JumpBall);
	            AIController.Get.ChangeState(EGameSituation.JumpBall);
	            CameraMgr.Get.ShowPlayerInfoCamera (true);
			}

			yield return new WaitForSeconds (0.2f);

			UIShow(false);
			//buttonNext.SetActive(true);
			//loadingPic.SetActive(false);

			break;
		case ELoading.Stage:
			ProgressValue = 1;
			loadStage();
			break;
		}

		ProgressValue = 1;
	}
	
	public void UpdateProgress (){
		float b = FileManager.DownlandCount;
		float a = FileManager.AlreadyDownlandCount;
		ProgressValue = (float)(a / b);
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
		UISelectRole.Get.InitFriend();
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

	public float ProgressValue{
		get{
			return uiLoadingProgress.fillAmount;
		}
		set{toProgress = value;}
	}

	public bool LoadingFinished{
		get{return uiLoadingProgress.fillAmount >= 1;}
	}
}
