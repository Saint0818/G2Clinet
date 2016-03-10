﻿using GameStruct;
using UnityEngine;

public struct TGameTarget {
	public GameObject Self;
	public UILabel LabelTargetName;
	public UILabel LabelTargetValue;
	public UILabel LabelValue;
	public UILabel LabelCaption;
}

public enum EPauseType {
	Target,
	Home,
	Away
}

public class UIGamePause : UIBase {
	private static UIGamePause instance = null;
	private const string UIName = "UIGamePause";
	private string[] positionPicName = {"IconCenter", "IconForward", "IconGuard"};
	private EPauseType pauseType = EPauseType.Target;

	private TGameRecord gameRecord;
	private UIStageHint uiStageHint;
	private GameObject uiGameResult;
	private GameObject uiSelect;
	private GameObject goMakeFriend;
	private GameObject uiRecord;
	private UIButton buttonPlayerInfo;
	private UISprite spriteMakeFriend;
	private UILabel labelMakeFriend;
	private UILabel labelAttr;

	private string playerID;
	private int playerIndex;
	private int selectIndex;
	private bool isFriend;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
		
		set {
			if (instance) {
				if (!value)
                    RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
			if (value)
				Get.Show(value);
		}
	}
	
	public static UIGamePause Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGamePause;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow) {
        if (instance) 
            instance.Show(isShow);
        else
			if (isShow)
				Get.Show(isShow);
	}
	
	protected override void InitCom() {
		uiStageHint = Instantiate(Resources.Load<GameObject>(UIPrefabPath.UIStageHint)).GetComponent<UIStageHint>();
		uiStageHint.transform.parent = GameObject.Find(UIName + "/Center").transform;
		uiStageHint.transform.localPosition = Vector3.zero;
		uiStageHint.transform.localRotation = Quaternion.identity;
		uiStageHint.transform.localScale = Vector3.one;
		uiStageHint.SetInterval(150, 150);

		uiRecord = GameObject.Find(UIName + "/Center/GameResult/GameAttribute");
		uiGameResult = GameObject.Find(UIName + "/Center/GameResult");
		uiSelect = GameObject.Find(UIName + "/Center/GameResult/Select");
		goMakeFriend = GameObject.Find(UIName + "/Center/GameResult/MakeFriend");
		labelMakeFriend = GameObject.Find(UIName + "/Center/GameResult/MakeFriend/Label").GetComponent<UILabel>();
		labelAttr = GameObject.Find(UIName + "/Center/GameResult/AttributeBoard/LabelAttr").GetComponent<UILabel>();
		buttonPlayerInfo = GameObject.Find(UIName + "/Center/GameResult/PlayerInfoBtn").GetComponent<UIButton>();
		spriteMakeFriend = GameObject.Find(UIName + "/Center/GameResult/MakeFriend/Btn").GetComponent<UISprite>();

		SetBtnFun(UIName + "/Center/GameResult/AttributeBoard/NextInfo", OnNextInfo);
//		SetBtnFun(UIName + "/Bottom/ButtonAgain", OnAgain);
		SetBtnFun(UIName + "/Bottom/ButtonResume", OnResume);
		SetBtnFun(UIName + "/Bottom/ButtonReturnSelect", OnReturn);
		SetBtnFun(UIName + "/Center/HomeBtn", OnHomeResult);
		SetBtnFun(UIName + "/Center/AwayBtn", OnAwayResult);
		SetBtnFun(UIName + "/Center/TargetBtn", OnBackToTarget);
		SetBtnFun(UIName + "/Center/GameResult/PlayerMe/ButtonMe", OnPlayerInfo);
		SetBtnFun(UIName + "/Center/GameResult/PlayerA/ButtonA", OnPlayerInfo);
		SetBtnFun(UIName + "/Center/GameResult/PlayerB/ButtonB", OnPlayerInfo);
		SetBtnFun (UIName + "/TopRight/ViewTools/ButtonOption", OptionSelect);
        SetBtnFun (UIName + "/TopRight/ViewTools/StrategyBtn", OnStrategy);
        SetBtnFun (UIName + "/Center/GameResult/PlayerInfoBtn", OnOpenInfo);
        SetBtnFun (UIName + "/Center/GameResult/MakeFriend", OnMakeFriend);

		buttonPlayerInfo.gameObject.SetActive(false);
	}

    protected override void InitText()
    {
        SetLabel(UIName + "/TopRight/ViewTools/StrategyBtn/StrategyLabel", "");
    }

	private void initHomeAway (){
		int basemin = 0;
		int basemax = 3;
		if(pauseType == EPauseType.Away) {
			basemin = 3;
			basemax = 6;
		}
		string positionName = "";
		string positionType = "";
		string spriteMe = "";
		for (int i=0; i<GameController.Get.GamePlayers.Count; i++) {
			if (i>=basemin && i<basemax) {
				switch (i) {
				case 0:
				case 3:
					positionName = "/Center/GameResult/PlayerMe/ButtonMe/PlayerFace/MyFace";
					positionType = "/Center/GameResult/PlayerMe/ButtonMe/PlayerNameMe/SpriteTypeMe";
					spriteMe = "/Center/GameResult/PlayerMe/ButtonMe/LabelValue";
					break;
				case 1:
				case 4:
					positionName = "/Center/GameResult/PlayerA/ButtonA/PlayerFace/AFace";
					positionType = "/Center/GameResult/PlayerA/ButtonA/PlayerNameA/SpriteTypeA";
					spriteMe = "/Center/GameResult/PlayerA/ButtonA/LabelValue";
					break;
				case 2:
				case 5:
					positionName = "/Center/GameResult/PlayerB/ButtonB/PlayerFace/BFace";
					positionType = "/Center/GameResult/PlayerB/ButtonB/PlayerNameB/SpriteTypeB";
					spriteMe = "/Center/GameResult/PlayerB/ButtonB/LabelValue";
					break;
				}
				if (GameController.Get.GamePlayers[i].Attribute.BodyType >= 0 && GameController.Get.GamePlayers[i].Attribute.BodyType < 3) {
					GameObject.Find(UIName + positionName).GetComponent<UISprite>().spriteName = GameController.Get.GamePlayers[i].Attribute.FacePicture;
					GameObject.Find(UIName + positionType).GetComponent<UISprite>().spriteName = positionPicName[GameController.Get.GamePlayers[i].Attribute.BodyType];
					GameObject.Find(UIName + spriteMe).GetComponent<UILabel>().text = GameController.Get.GamePlayers[i].Attribute.Name;
				}
			}
		}
	}

	public void SetGameRecord(ref TGameRecord record) {
		gameRecord = record;
		uiStageHint.Show();
		uiStageHint.UpdateValue(GameController.Get.StageData.ID);
		UIShow(true);
		uiGameResult.SetActive(false);
	}	

	private void setInfo(int index, ref TGameRecord record) {
		if (index >= 0 && index < record.PlayerRecords.Length) {
			selectIndex = index;
			getInfoString(ref record.PlayerRecords[index]);
			initHomeAway ();
			
			switch (index) {
			case 0:
			case 3:
				playerIndex = 0;
				uiSelect.transform.localPosition = new Vector3(0, 105, 0);
				break;
			case 1:
			case 4:
				playerIndex = 1;
				uiSelect.transform.localPosition = new Vector3(270, 105, 0);
				break;
			case 2:
			case 5:
				playerIndex = 2;
				uiSelect.transform.localPosition = new Vector3(-270, 105, 0);
				break;
			}

			if(playerIndex < GameData.TeamMembers.Length) {
				if(GameData.Team.CheckFriend(GameData.TeamMembers[playerIndex].Identifier)) 
					isFriend = true;
				else 
					isFriend = false;
				
				playerID = GameData.TeamMembers[playerIndex].Identifier;
				goMakeFriend.SetActive(!string.IsNullOrEmpty(GameData.TeamMembers[playerIndex].Identifier));//如果是AI就是空值
			} else 
				goMakeFriend.SetActive(false);
				
			CheckFriendLike();
            if (!uiRecord.activeInHierarchy)
                labelAttr.text = GameController.Get.GamePlayers[selectIndex].Attribute.AttrText;
            else
                labelAttr.text = "";
		}
	} 

	private string getInfoString(ref TGamePlayerRecord player) {
		int pts = player.FGIn * 2 + player.FG3In * 3;
		float fg = 0;
		if (player.FG > 0)
			fg = Mathf.Min(Mathf.Round(player.FGIn * 100 / player.FG), 100);
		
		float fg3 = 0;
		if (player.FG3 > 0)
			fg3 = Mathf.Min(Mathf.Round(player.FG3In * 100 / player.FG3), 100);
		
		SetLabel(UIName + "Center/GameResult/GameAttribute/PTS/LabelValue", pts.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/FG/LabelValue", fg.ToString() + "%");
		SetLabel(UIName + "Center/GameResult/GameAttribute/3FG/LabelValue", fg3.ToString() + "%");
		SetLabel(UIName + "Center/GameResult/GameAttribute/REB/LabelValue", player.Rebound.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/AST/LabelValue", player.Assist.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/STL/LabelValue", (player.Steal + player.Intercept).ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/BLK/LabelValue", player.Block.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/TOV/LabelValue", (player.BeIntercept + player.BeSteal).ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/PUSH/LabelValue", player.Push.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/KNOCK/LabelValue", player.Knock.ToString());
		
		return string.Format("{0}\n{1}%\n{2}%\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n", 
		                     pts, fg, fg3, player.Rebound, player.Assist, player.Steal, player.Block, player.BeIntercept, player.Push, player.Knock);
	}

    public void OnMakeFriend() {
		if(!string.IsNullOrEmpty(playerID)) {
			if(isFriend) {
				isFriend = false;
			} else {
				isFriend = true;
				SendHttp.Get.MakeFriend(CheckFriendLike, playerID);
			}
		} 
    }

	public void CheckFriendLike () {
		if(isFriend)
			labelMakeFriend.text = TextConst.S(5023);
		else
			labelMakeFriend.text = TextConst.S(5024);
		spriteMakeFriend.spriteName = ButtonBG(isFriend);
	}

    public void OnOpenInfo() {
        
    }

	public void OnNextInfo() {
		if (uiRecord.activeInHierarchy) {
			uiRecord.SetActive(false);
			labelAttr.text = GameController.Get.GamePlayers[selectIndex].Attribute.AttrText;
		} else {
			uiRecord.SetActive(true);
			labelAttr.text = "";
		}
	}
	
	public void OnPlayerInfo() {
		if(pauseType == EPauseType.Home) {
			if (UIButton.current.name == "ButtonMe") 
				setInfo(0, ref gameRecord);
			else
				if (UIButton.current.name == "ButtonA") 
					setInfo(1, ref gameRecord);
			else
				if (UIButton.current.name == "ButtonB") 
					setInfo(2, ref gameRecord);
		} else {
			if (UIButton.current.name == "ButtonMe") 
				setInfo(3, ref gameRecord);
			else
				if (UIButton.current.name == "ButtonA") 
					setInfo(4, ref gameRecord);
			else
				if (UIButton.current.name == "ButtonB") 
					setInfo(5, ref gameRecord);
		}
	}

	public void OnReturn() {
		Time.timeScale = 1;
		uiStageHint.Hide();
		UIShow(false);
//		if (isStage)
//			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
//		else
//			SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
		if(GameData.IsMainStage)
		{
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenStageUI;
		}
		else if(GameData.IsInstance)
		{
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenInstanceUI;
		}
		else if (GameData.IsPVP)
		{
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenPVPUI;
		}
		else
		{
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenStageUI;
		}
	}
	
	public void OnResume() {
        Time.timeScale = GameController.Get.RecordTimeScale;
		uiStageHint.Hide();
		UIShow(false);
		UIGame.Get.UIState(EUISituation.Continue);
	}
	
	public void OnAgain() {
		Time.timeScale = GameController.Get.RecordTimeScale;
		UIGame.Get.UIState(EUISituation.Reset);
		uiStageHint.Hide();
		UIShow(false);
	}

	public void OnHomeResult () {
		pauseType = EPauseType.Home;
		uiStageHint.Hide();
		uiGameResult.SetActive(true);
		setInfo(0, ref gameRecord);
	}

	public void OnAwayResult () {
		pauseType = EPauseType.Away;
		uiStageHint.Hide();
		uiGameResult.SetActive(true);
		setInfo(3, ref gameRecord);
	}

	public void OnBackToTarget (){
		pauseType = EPauseType.Target;
		uiStageHint.Show();
		uiGameResult.SetActive(false);
	}

    public void OnStrategy() {
        UIStrategy.Visible = true;
        if (UIGame.Visible)
            UIStrategy.Get.LabelStrategy = UIGame.Get.LabelStrategy;
    }

	public void OptionSelect(){
		UISetting.UIShow(true, false);
	}

	public bool isStage
    {
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
