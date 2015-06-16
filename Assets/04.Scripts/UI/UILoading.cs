using UnityEngine;
using System;
using System.Collections;

public class UILoading : UIBase {
	private static UILoading instance = null;
	private const string UIName = "UILoading";

	private UITexture LoadingPic;
	private UISlider UIProgress;
	public UILabel LabelVersion;
	private UILabel UILoadingHint;
	private UILabel LabelTitle;

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

	public static UILoading Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UILoading;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		LoadingPic = GameObject.Find("UILoading/Window/LoadingPicture").GetComponent<UITexture>();
		UIProgress = GameObject.Find("UILoading/Window/Progress").GetComponent<UISlider>();
		LabelVersion = GameObject.Find("UILoading/Window/Version").GetComponent<UILabel>();
		LabelTitle = GameObject.Find("UILoading/Window/Title").GetComponent<UILabel>();
	}

	protected override void InitData() {
		UIProgress.value = 0;
	}

	public string Title{
		set{
			LabelTitle.text = value;
		}
	}

	protected override void OnShow(bool isShow){
		if(isShow) {
			LabelVersion.text = BundleVersion.version.ToString();
			UIProgress.value = 0;
			if (!LoadingPic.mainTexture)
				LoadingPic.mainTexture = (Texture)Resources.Load("Textures/GameLoading5", typeof(Texture));
		} else {
			LoadingPic.mainTexture = null;
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
	}

	public float ProgressValue{
		get{return UIProgress.value;}
	}

	public bool DownloadDone{
		get{return UIProgress.value >= 1;}
	}

	public void UpdateProgress (){
		float b = FileManager.DownlandCount;
		float a = FileManager.AlreadyDownlandCount;
		UIProgress.value = (float)(a / b);
	}
}
