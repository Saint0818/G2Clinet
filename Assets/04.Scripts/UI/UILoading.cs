using UnityEngine;
using System;
using System.Collections;

public class UILoading : UIBase {
	private static UILoading instance = null;
	private const string UIName = "UILoading";

	public UILabel LabelVersion;
	private UILabel UILoadingHint;
	private UILabel LabelTitle;

	private UITexture uiProgress;
	private GameObject uiLight1;
	private GameObject uiLight2;
	private GameObject uiWord;


	private int PicNo = -1;
	private bool isCloseUI = false;
	public float CloseTime = 0;
	private UILabel Hint;

	public static bool Visible{
		get{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if(instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
		if(isShow)
			Get.Show(isShow);
	}

//	public static void UIShow(bool isShow, ELoadingGamePic Kind = ELoadingGamePic.StagePic, string hint=""){
//		Get.isCloseUI = isShow;
//		if(isShow) {
//			Get.Show(isShow);
//			Get.CloseTime = 3;
//			
//			if (Get.PicNo != Kind.GetHashCode()) {
//				if (Get.LoadingPic.mainTexture) {
//					Get.LoadingPic.mainTexture = null;
//					Resources.UnloadUnusedAssets();
//				}
//				
//				Get.PicNo = Kind.GetHashCode();
//				Get.LoadingPic.mainTexture = (Texture)Resources.Load("Textures/GameLoading" + Get.PicNo.ToString(), typeof(Texture));
//			}
//			
//			if (hint != "")
//				Get.Hint.text = hint;
//			else
//				Get.Hint.text = TextConst.S(405);
//		}
//		else 
//		if(instance) {
//			if (Get.CloseTime <= 0) {
//				instance.Show(isShow);
//				RemoveUI(UIName);
//			}
//		}
//	}

	public static UILoading Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UILoading;
			
			return instance;
		}
	}

//	protected override void UpdateUI() {
	void FixedUpdate () {
		if (CloseTime > 0) {
			CloseTime -= Time.deltaTime;
			
			if (CloseTime <=0 && !isCloseUI) {
				UIShow(false);
			}
		}
		uiLight1.transform.Rotate(new Vector3(0,0,-1));
		uiLight2.transform.Rotate(new Vector3(0,0,1));
		uiWord.transform.Rotate(new Vector3(0,0,-2f));
	}
	
	protected override void InitCom() {
//		LabelVersion = GameObject.Find(UIName + "/Window/Version").GetComponent<UILabel>();
//		LabelTitle = GameObject.Find(UIName + "/Window/Title").GetComponent<UILabel>();

		uiProgress = GameObject.Find (UIName + "/Window/LoadingPic/UIProgressBar").GetComponent<UITexture>();
		uiLight1 = GameObject.Find (UIName + "/Window/LoadingPic/UILight1");
		uiLight2 = GameObject.Find (UIName + "/Window/LoadingPic/UILight2");
		uiWord = GameObject.Find (UIName + "/Window/LoadingPic/UIWord");
	}

	protected override void InitData() {
		uiProgress.fillAmount = 0;
	}

	public string Title{
		set{
			LabelTitle.text = value;
		}
	}

	protected override void OnShow(bool isShow){
//		if(isShow) {
//			LabelVersion.text = BundleVersion.version.ToString();
//			UIProgress.value = 0;
//			if (!LoadingPic.mainTexture)
//				LoadingPic.mainTexture = (Texture)Resources.Load("Textures/GameLoading5", typeof(Texture));
//		} else {
//			LoadingPic.mainTexture = null;
//			Resources.UnloadUnusedAssets();
//			GC.Collect();
//		}
	}

	public float ProgressValue{
		get{return uiProgress.fillAmount;}
	}

	public bool DownloadDone{
		get{return uiProgress.fillAmount >= 1;}
	}

	public void UpdateProgress (){
		float b = FileManager.DownlandCount;
		float a = FileManager.AlreadyDownlandCount;
		uiProgress.fillAmount = (float)(a / b);
	}
}
