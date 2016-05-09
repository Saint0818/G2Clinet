using GameStruct;
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
	private EPauseType pauseType = EPauseType.Target;

	private TGameRecord gameRecord;
	private UIStageHint uiStageHint;
	private GameObject uiGameResult;
	private UILabel labelStrategy;

	private UIButton[] btnPlayer = new UIButton[3];
	private GameObject[] goSelect = new GameObject[3];
	private UILabel[] labelPlayerName = new UILabel[3];
	private UISprite[] spritePlayerFace = new UISprite[3];
	private UISprite[] spritePlayerPosition = new UISprite[3];
	private UILabel[] labelPlayerLv = new UILabel[3];

	private GameObject[] tabSelect = new GameObject[3];

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

	public static void UIShow(bool isShow)
	{
	    if (instance) 
            instance.Show(isShow);
        else
			if (isShow)
				Get.Show(isShow);

	    if(isShow)
            Statistic.Ins.LogScreen(9);
    }

    protected override void InitCom() {
		uiStageHint = GameObject.Find(UIName + "/Window/Right/View/UIStageHint").GetComponent<UIStageHint>();

		uiGameResult = GameObject.Find(UIName + "/Window/Right/View/GameResult");

		for(int i=0; i<3; i++) {
			goSelect[i] = GameObject.Find(UIName + "/Window/Right/View/GameResult/Player" + i.ToString() + "/Select");
			btnPlayer[i] = GameObject.Find(UIName + "/Window/Right/View/GameResult/Player" + i.ToString() + "/PlayerInGameBtn").GetComponent<UIButton>();
			SetBtnFun(ref btnPlayer[i], OnOpenInfo);
			labelPlayerName[i] = GameObject.Find(UIName + "/Window/Right/View/GameResult/Player" + i.ToString() + "/PlayerNameLabel").GetComponent<UILabel>();
			spritePlayerFace[i] = GameObject.Find(UIName + "/Window/Right/View/GameResult/Player" + i.ToString() + "/PlayerInGameBtn/PlayerPic").GetComponent<UISprite>();
			spritePlayerPosition[i] = GameObject.Find(UIName + "/Window/Right/View/GameResult/Player" + i.ToString() + "/PlayerInGameBtn/PlayerPic/PositionIcon").GetComponent<UISprite>();
			labelPlayerLv[i] = GameObject.Find(UIName + "/Window/Right/View/GameResult/Player" + i.ToString() + "/PlayerInGameBtn/LevelGroup").GetComponent<UILabel>();
		}

		tabSelect[0] = GameObject.Find(UIName + "/Window/Right/View/HomeBtn/Select");
		tabSelect[1] = GameObject.Find(UIName + "/Window/Right/View/TargetBtn/Select");
		tabSelect[2] = GameObject.Find(UIName + "/Window/Right/View/AwayBtn/Select");

		labelStrategy = GameObject.Find(UIName + "/Window/Bottom/StrategyBtn/StrategyLabel").GetComponent<UILabel>();

		SetBtnFun(UIName + "/Window/Bottom/ButtonResume", OnResume);
		SetBtnFun(UIName + "/Window/Bottom/ButtonReturnSelect", OnReturn);
		SetBtnFun(UIName + "/Window/Right/View/HomeBtn", OnHomeResult);
		SetBtnFun(UIName + "/Window/Right/View/AwayBtn", OnAwayResult);
		SetBtnFun(UIName + "/Window/Right/View/TargetBtn", OnBackToTarget);
		SetBtnFun (UIName + "/Window/TopRight/ViewTools/ButtonOption", OptionSelect);
		SetBtnFun (UIName + "/Window/Bottom/StrategyBtn", OnStrategy);
	}

	private void initHomeAway (){
		int basemin = 0;
		int basemax = 3;
		if(pauseType == EPauseType.Away) {
			basemin = 3;
			basemax = 6;
		}
		int playerIndex = 0;
		for (int i=0; i<GameController.Get.GamePlayers.Count; i++) {
			if (i>=basemin && i<basemax) {
				switch (i) {
				case 0:
				case 3:
					playerIndex = 0;
					break;
				case 1:
				case 4:
					playerIndex = 1;
					break;
				case 2:
				case 5:
					playerIndex = 2;
					break;
				}
				hideSelect ();
				goSelect[0].SetActive(true);
				btnPlayer[playerIndex].gameObject.name = playerIndex.ToString();
				labelPlayerName[playerIndex].text = GameController.Get.GamePlayers[i].Attribute.Name;
				spritePlayerFace[playerIndex].spriteName = GameController.Get.GamePlayers[i].Attribute.FacePicture;
				spritePlayerPosition[playerIndex].spriteName = GameFunction.PositionIcon(GameController.Get.GamePlayers[i].Attribute.BodyType);
				labelPlayerLv[playerIndex].text = GameController.Get.GamePlayers[i].Attribute.Lv.ToString();
				labelPlayerLv[playerIndex].gameObject.SetActive(GameController.Get.GamePlayers[i].Attribute.Lv > 0);
				if(i < gameRecord.PlayerRecords.Length)
					getInfoString(playerIndex, ref gameRecord.PlayerRecords[i]);
			}
		}
	}

	private void setTabSelect (int index) {
		for (int i=0; i<tabSelect.Length; i++) 
			tabSelect[i].SetActive((i == index));
	}

	public void SetGameRecord(ref TGameRecord record) {
		gameRecord = record;
		labelStrategy.text =  GameData.Team.Player.StrategyText;
		uiStageHint.Show();
		uiStageHint.UpdateValue(GameController.Get.StageData.ID);
		UIShow(true);
		uiGameResult.SetActive(false);
		setTabSelect(1);
	}	

	private void hideSelect () {
		for(int i=0; i<goSelect.Length; i++)
			goSelect[i].SetActive(false);
	}

	private void setInfo(int index, ref TGameRecord record) {
		if (index >= 0 && index < record.PlayerRecords.Length) {
			initHomeAway ();
			moveCamera (index);
		}
	} 

	private void  moveCamera (int index) {
		CameraMgr.Get.GamePause();
		CameraMgr.Get.LookatByPause(new Vector3(GameController.Get.GamePlayers[index].Pelvis.transform.position.x , 
			GameController.Get.GamePlayers[index].Pelvis.transform.position.y, 
			GameController.Get.GamePlayers[index].Pelvis.transform.position.z -3),
			new Vector3(GameController.Get.GamePlayers[index].PlayerRefGameObject.transform.position.x - 13, 
				GameController.Get.GamePlayers[index].PlayerRefGameObject.transform.position.y + 12, 
				GameController.Get.GamePlayers[index].PlayerRefGameObject.transform.position.z));
	}

	private void getInfoString(int index, ref TGamePlayerRecord player) {
		int pts = player.FGIn * 2 + player.FG3In * 3;
		float fg = 0;
		if (player.FG > 0)
			fg = Mathf.Min(Mathf.Round(player.FGIn * 100 / player.FG), 100);
		
		float fg3 = 0;
		if (player.FG3 > 0)
			fg3 = Mathf.Min(Mathf.Round(player.FG3In * 100 / player.FG3), 100);
		
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/PTS/LabelValue", pts.ToString());
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/FG/LabelValue", fg.ToString() + "%");
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/3FG/LabelValue", fg3.ToString() + "%");
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/REB/LabelValue", player.Rebound.ToString());
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/AST/LabelValue", player.Assist.ToString());
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/STL/LabelValue", (player.Steal + player.Intercept).ToString());
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/BLK/LabelValue", player.Block.ToString());
//		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/TOV/LabelValue", (player.BeIntercept + player.BeSteal).ToString());
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/PUSH/LabelValue", player.Push.ToString());
		SetLabel(UIName + "/Window/Right/View/GameResult/ScrollView/View/GameAttribute"+ index.ToString() +"/KNOCK/LabelValue", player.Knock.ToString());
		
//		return string.Format("{0}\n{1}%\n{2}%\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n", 
//		                     pts, fg, fg3, player.Rebound, player.Assist, player.Steal, player.Block, player.BeIntercept, player.Push, player.Knock);
	}

    public void OnOpenInfo() {
		int result = 0;
		if(int.TryParse(UIButton.current.name, out result)) {
			hideSelect ();
			goSelect[result].SetActive(true);
			if(pauseType == EPauseType.Away) {
				moveCamera(result + 3);
				if(result >= 0 && result < GameData.EnemyMembers.Length)
					UIGamePlayerInfo.Get.ShowView(GameData.EnemyMembers[result], GameController.Get.GamePlayers[result + 3]);
			}else {
				moveCamera(result);
				if(result >= 0 && result < GameData.TeamMembers.Length)
					UIGamePlayerInfo.Get.ShowView(GameData.TeamMembers[result], GameController.Get.GamePlayers[result]);
			}
		}
    }

	public void OnReturn() {
		Time.timeScale = 1;
		uiStageHint.Hide();
		UIShow(false);
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
		CameraMgr.Get.GameContinue();
        Time.timeScale = GameController.Get.RecordTimeScale;
		UIGame.Get.UIState(EUISituation.Continue);
		uiStageHint.Hide();
		UIShow(false);
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
		setTabSelect(0);
	}

	public void OnAwayResult () {
		pauseType = EPauseType.Away;
		uiStageHint.Hide();
		uiGameResult.SetActive(true);
		setInfo(3, ref gameRecord);
		setTabSelect(2);
	}

	public void OnBackToTarget (){
		pauseType = EPauseType.Target;
		uiStageHint.Show();
		uiGameResult.SetActive(false);
		setTabSelect(1);
	}

    public void OnStrategy() {
        UIStrategy.Visible = true;
        if (UIGame.Visible)
            UIStrategy.Get.LabelStrategy = UIGame.Get.LabelStrategy;

		UIStrategy.Get.PauseLabelStrategy = labelStrategy;
    }

	public void OptionSelect(){
		UISetting.UIShow(true, false);
	}

	public bool isStage
    {
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
