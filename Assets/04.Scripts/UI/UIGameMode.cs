using UnityEngine;
using System.Collections;
using GamePlayEnum;

public class UIGameMode: UIBase {
	private static UIGameMode instance = null;
	private const string UIName = "UIGameMode";

	private const float maxScore = 60;
	private const float maxTime = 600;

	private UIScrollBar timeScrollBar;
	private UIScrollBar scoreScrollBar;

	private UILabel timeLabel;
	private UILabel scoreLabel;

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

	public static UIGameMode Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGameMode;
			
			return instance;
		}
	}

	public void FixedUpdate() {
		int s = (int)(timeScrollBar.value * maxTime);
		if (s <= 1)
			s = 1;

		timeLabel.text = s.ToString();

		s = (int)(scoreScrollBar.value * maxScore);
		if (s <= 1)
			s = 1;

		scoreLabel.text = s.ToString();
	}

	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
		if (isShow)
			Get.Show(isShow);
	}

	protected override void InitCom() {
		SetBtnFun (UIName + "/Center/CourtMode/FullCourtBt", OnCourtMode);
		SetBtnFun (UIName + "/Center/CourtMode/HalfCourtBt", OnCourtMode);

		SetBtnFun (UIName + "/Center/TeamMode/1Bt", OnTeamMode);
		SetBtnFun (UIName + "/Center/TeamMode/2Bt", OnTeamMode);
		SetBtnFun (UIName + "/Center/TeamMode/3Bt", OnTeamMode);
		SetBtnFun (UIName + "/Center/TeamMode/4Bt", OnTeamMode);
		SetBtnFun (UIName + "/Center/TeamMode/5Bt", OnTeamMode);

		SetBtnFun (UIName + "/Center/GameMode/Time/ButtonTime", OnWinMode);
		SetBtnFun (UIName + "/Center/GameMode/Score/ButtonScore", OnWinMode);

		SetBtnFun (UIName + "/BottomRight/ButtonNext", OnNext);

		timeLabel = GameObject.Find (UIName + "/Center/GameMode/Time/TimeSlider/Label1").GetComponent<UILabel>();
		scoreLabel = GameObject.Find (UIName + "/Center/GameMode/Score/ScoreSlider/Label1").GetComponent<UILabel>();
		timeScrollBar = GameObject.Find (UIName + "/Center/GameMode/Time/TimeSlider").GetComponent<UIScrollBar>();
		scoreScrollBar = GameObject.Find (UIName + "/Center/GameMode/Score/ScoreSlider").GetComponent<UIScrollBar>();

		timeScrollBar.value = 120f / maxTime;
		scoreScrollBar.value = 13f / maxScore;
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
//		if (UIButton.current.name == "ButtonTime")
//			GameStart.Get.WinMode = EWinMode.Time;
//		else
//		if (UIButton.current.name == "ButtonScore")
//			GameStart.Get.WinMode = EWinMode.Score;
		
	}

	public void OnTeamMode() {
		if (UIButton.current.name == "1Bt")
			GameStart.Get.FriendNumber = 1;
		else
		if (UIButton.current.name == "2Bt")
			GameStart.Get.FriendNumber = 2;
		else
		if (UIButton.current.name == "3Bt")
			GameStart.Get.FriendNumber = 3;
		else
		if (UIButton.current.name == "4Bt")
			GameStart.Get.FriendNumber = 4;
		else
		if (UIButton.current.name == "5Bt")
			GameStart.Get.FriendNumber = 5;
	}

	public void OnNext() {
		int s = (int)(timeScrollBar.value * maxTime);
		GameStart.Get.GameWinTimeValue = s;

//		if (GameStart.Get.WinMode == EWinMode.Score)
//			s = (int)(scoreScrollBar.value * maxScore);

		if (s <= 1)
			s = 1;
//		if (GameStart.Get.WinMode == EWinMode.Score)
//			GameStart.Get.GameWinValue = s;

		UIShow (false);
		CameraMgr.Get.SetSelectRoleCamera();
		UISelectRole.UIShow(true);
		UI3DSelectRole.UIShow(true);
		UI3D.Get.ShowCamera(true);
	}
}
