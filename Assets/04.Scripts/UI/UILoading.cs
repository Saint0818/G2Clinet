using UnityEngine;
using System;
using System.Collections;

public class UILoading : UIBase {
	private static UILoading instance = null;
	private const string UIName = "UILoading";

	private GameObject windowLoading;
	private GameObject windowGame;

	private UITexture uiLoadingProgress;
	private GameObject uiLoadingLight1;
	private GameObject uiLoadindLight2;
	private GameObject uiLoadingWord;

	private UITexture uiBG;
	private UITexture uiGameProgress;
	private GameObject uiGameLight1;
	private GameObject uiGameLight2;
	private GameObject uiGameWord;

//	public UILabel LabelVersion;

	private int PicNo = -1;
	private bool isCloseUI = false;
	public float CloseTime = 0;
//	private UILabel Hint;

	public static bool Visible{
		get{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

//	public static void UIShow(bool isShow){
//		if(instance) {
//			if (!isShow)
//				RemoveUI(UIName);
//			else
//				instance.Show(isShow);
//		} else
//		if(isShow)
//			Get.Show(isShow);
//	}

	public static void UIShow(bool isShow, ELoadingGamePic Kind = ELoadingGamePic.None, string hint=""){
//		Get.isCloseUI = isShow;
		if(isShow) {
			Get.Show(isShow);
			Get.CloseTime = 3;
			
			if (Get.PicNo != Kind.GetHashCode()) {
				Get.windowGame.SetActive(true);
				Get.windowLoading.SetActive(false);
				if (Get.uiBG.mainTexture) {
					Get.uiBG.mainTexture = null;
					Resources.UnloadUnusedAssets();
				}
				
				Get.PicNo = Kind.GetHashCode();
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

	public static UILoading Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UILoading;
			
			return instance;
		}
	}

	void FixedUpdate () {
		if (CloseTime > 0) {
			CloseTime -= Time.deltaTime;
			
			if (CloseTime <=0 && !isCloseUI) {
				UIShow(false);
			}
		}
		if(windowGame != null) {
			if(windowGame.activeInHierarchy) {
				uiGameLight1.transform.Rotate(new Vector3(0,0,1));
				uiGameLight2.transform.Rotate(new Vector3(0,0,-1));
				uiGameWord.transform.Rotate(new Vector3(0,0,3f));
			} else {
				uiLoadingLight1.transform.Rotate(new Vector3(0,0,1));
				uiLoadindLight2.transform.Rotate(new Vector3(0,0,-1));
				uiLoadingWord.transform.Rotate(new Vector3(0,0,3f));
			}
		}
	}
	
	protected override void InitCom() {
		windowLoading = GameObject.Find (UIName + "/WindowLoading");
		windowGame = GameObject.Find (UIName + "/WindowGame");

//		LabelVersion = GameObject.Find(UIName + "/Window/Version").GetComponent<UILabel>();

		uiLoadingProgress = GameObject.Find (UIName + "/WindowLoading/LoadingPic/UIProgressBar").GetComponent<UITexture>();
		uiLoadingLight1 = GameObject.Find (UIName + "/WindowLoading/LoadingPic/UILight1");
		uiLoadindLight2 = GameObject.Find (UIName + "/WindowLoading/LoadingPic/UILight2");
		uiLoadingWord = GameObject.Find (UIName + "/WindowLoading/LoadingPic/UIWord");

		uiBG = GameObject.Find (UIName + "/WindowGame/BG").GetComponent<UITexture>();
		uiGameProgress = GameObject.Find (UIName + "/WindowGame/LoadingPic/UIProgressBar").GetComponent<UITexture>();
		uiGameLight1 = GameObject.Find (UIName + "/WindowGame/LoadingPic/UILight1");
		uiGameLight2 = GameObject.Find (UIName + "/WindowGame/LoadingPic/UILight2");
		uiGameWord = GameObject.Find (UIName + "/WindowGame/LoadingPic/UIWord");
	}

	protected override void InitData() {
		uiLoadingProgress.fillAmount = 0;
		uiGameProgress.fillAmount = 0;
	}

	protected override void OnShow(bool isShow){
//		if(isShow) {
//			LabelVersion.text = BundleVersion.version.ToString();
//			uiProgress.fillAmount = 0;
//			if (!LoadingPic.mainTexture)
//				LoadingPic.mainTexture = (Texture)Resources.Load("Textures/GameLoading5", typeof(Texture));
//		} else {
//			LoadingPic.mainTexture = null;
//			Resources.UnloadUnusedAssets();
//			GC.Collect();
//		}
	}

	public float ProgressValue{
		get{return uiLoadingProgress.fillAmount;}
	}

	public bool DownloadDone{
		get{return uiLoadingProgress.fillAmount >= 1;}
	}

	public void UpdateProgress (){
		float b = FileManager.DownlandCount;
		float a = FileManager.AlreadyDownlandCount;
		uiLoadingProgress.fillAmount = (float)(a / b);
	}
}
