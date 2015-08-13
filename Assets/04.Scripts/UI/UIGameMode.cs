using UnityEngine;
using System.Collections;

public class UIGameMode: UIBase {
	private static UIGameMode instance = null;
	private const string UIName = "UIGameMode";

	private UIScrollBar timeScrollBar;
	private UIScrollBar scoreScrollBar;

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
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		}
		else
		if (isShow)
			Get.Show(isShow);
	}
	
	public static UIGameMode Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGameMode;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		SetBtnFun (UIName + "/Center/CourtMode/FullCourtBt", OnCourtMode);
		SetBtnFun (UIName + "/Center/CourtMode/HalfCourtBt", OnCourtMode);

		SetBtnFun (UIName + "/Center/TeamMode/1Bt", OnTeamMode);
		SetBtnFun (UIName + "/Center/TeamMode/2Bt", OnTeamMode);
		SetBtnFun (UIName + "/Center/TeamMode/3Bt", OnTeamMode);
		SetBtnFun (UIName + "/Center/TeamMode/4Bt", OnTeamMode);
		SetBtnFun (UIName + "/Center/TeamMode/5Bt", OnTeamMode);

		SetBtnFun (UIName + "/BottomRight/ButtonNext", OnNext);

		timeScrollBar = GameObject.Find (UIName + "/Center/GameMode/Time/TimeSlider").GetComponent<UIScrollBar>();
		scoreScrollBar = GameObject.Find (UIName + "/Center/GameMode/Score/ScoreSlider").GetComponent<UIScrollBar>();
	}

	protected override void InitData() {
		
	}

	protected override void OnShow(bool isShow) {
		
	}

	public void OnCourtMode() {
		if (UIButton.current.name == "FullCourtBt")
			GameStart.Get.CourtMode = ECourtMode.Full;
		else
		if (UIButton.current.name == "HalfCourtBt")
			GameStart.Get.CourtMode = ECourtMode.Half;

	}

	public void OnWinMode() {
		if (UIButton.current.name == "ButtonTime")
			GameStart.Get.WinMode = EWinMode.Time;
		else
		if (UIButton.current.name == "ButtonScore")
			GameStart.Get.WinMode = EWinMode.Score;
		
	}

	public void OnTeamMode() {
		if (UIButton.current.name == "1Bt")
			GameStart.Get.PlayerNumber = 1;
		else
		if (UIButton.current.name == "2Bt")
			GameStart.Get.PlayerNumber = 2;
		else
		if (UIButton.current.name == "3Bt")
			GameStart.Get.PlayerNumber = 3;
		else
		if (UIButton.current.name == "4Bt")
			GameStart.Get.PlayerNumber = 4;
		else
		if (UIButton.current.name == "5Bt")
			GameStart.Get.PlayerNumber = 5;
	}

	public void OnNext() {
		int s = (int)(timeScrollBar.value * 600);
		if (GameStart.Get.WinMode == EWinMode.Score)
			s = (int)(scoreScrollBar.value * 60);

		UIShow (false);
		CameraMgr.Get.SetSelectRoleCamera();
		UISelectRole.UIShow(true);
		UI3DSelectRole.UIShow(true);
	}
}
