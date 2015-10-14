using UnityEngine;
using System.Collections;
using GameStruct;

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

	private GameObject uiGameTarget;
	private GameObject uiGameResult;
	private GameObject uiHomeButton;
	private GameObject uiAwayButton;
	private GameObject uiButtonRight;
	private GameObject uiButtonLeft;

	private EPauseType pauseType = EPauseType.Target;
	private TGameTarget[] gameTargets = new TGameTarget[5];

	private GameObject uiSelect;

	private string[] positionPicName = {"L_namecard_CENTER", "L_namecard_FORWARD", "L_namecard_GUARD"};
	private TGameRecord gameRecord;
	private GameObject viewAISelect;
	private GameObject viewOption;
	private GameObject[] effectGroup = new GameObject[2];
	private GameObject[] musicGroup = new GameObject[2];
	private UIScrollBar aiLevelScrollBar;
	private bool isShowOption = false;
	private bool isMusicOn = false;
	
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
					RemoveUI(UIName);
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
		uiGameTarget = GameObject.Find(UIName + "/Center/GameTarget");
		uiGameResult = GameObject.Find(UIName + "/Center/GameResult");
		uiHomeButton = GameObject.Find(UIName + "/Center/HomeBtn");
		uiAwayButton = GameObject.Find(UIName + "/Center/AwayBtn");
		uiSelect = GameObject.Find(UIName + "/Center/GameResult/Select");
		uiButtonRight = GameObject.Find(UIName + "/Center/GameResult/ButtonRight");
		uiButtonLeft = GameObject.Find(UIName + "/Center/GameResult/ButtonLeft");

		for(int i=0; i<gameTargets.Length; i++) {
			gameTargets[i].Self = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString());
			gameTargets[i].LabelTargetName = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString()+"/TargetLabel").GetComponent<UILabel>();
			gameTargets[i].LabelTargetValue = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString()+"/BitNumLabel").GetComponent<UILabel>();
			gameTargets[i].LabelValue = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString()+"/BitValueLabel").GetComponent<UILabel>();
			gameTargets[i].LabelCaption = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString()+"/CaptionLabel").GetComponent<UILabel>();
			gameTargets[i].Self.SetActive(false);
		}

		viewAISelect = GameObject.Find (UIName + "/AISelect");
		SetBtnFun (UIName + "/AISelect/ButtonClose", AITimeChange);
		aiLevelScrollBar = GameObject.Find (UIName + "/AISelect/AIControlScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar.onChange.Add(new EventDelegate(changeAIChangeTime));
		viewAISelect.SetActive(false);
		viewOption = GameObject.Find (UIName + "TopRight/ViewTools/ViewOption");
		viewOption.SetActive(isShowOption);
		effectGroup[0] = GameObject.Find (UIName + "/TopRight/ViewTools/ViewOption/ButtonEffect/LabelON");
		effectGroup[1] = GameObject.Find (UIName + "/TopRight/ViewTools/ViewOption/ButtonEffect/LabelOff");
		effectGroup [0].SetActive (GameData.Setting.Effect);
		effectGroup [1].SetActive (!GameData.Setting.Effect);

		musicGroup[0] = GameObject.Find (UIName + "/TopRight/ViewTools/ViewOption/ButtonMusic/LabelON");
		musicGroup[1] = GameObject.Find (UIName + "/TopRight/ViewTools/ViewOption/ButtonMusic/LabelOff");
		musicGroup[0].SetActive(AudioMgr.Get.IsMusicOn);
		musicGroup[1].SetActive(!AudioMgr.Get.IsMusicOn);
		isMusicOn = AudioMgr.Get.IsMusicOn;


		SetBtnFun(UIName + "/Bottom/ButtonAgain", OnAgain);
		SetBtnFun(UIName + "/Bottom/ButtonResume", OnResume);
		SetBtnFun(UIName + "/Bottom/ButtonReturnSelect", OnReturn);
		SetBtnFun(UIName + "/Center/HomeBtn", OnHomeResult);
		SetBtnFun(UIName + "/Center/AwayBtn", OnAwayResult);
		SetBtnFun(UIName + "/Center/GameResult/ButtonRight", OnBackToTarget);
		SetBtnFun(UIName + "/Center/GameResult/ButtonLeft", OnBackToTarget);
		
		SetBtnFun(UIName + "/Center/GameResult/PlayerMe/ButtonMe", OnPlayerInfo);
		SetBtnFun(UIName + "/Center/GameResult/PlayerA/ButtonA", OnPlayerInfo);
		SetBtnFun(UIName + "/Center/GameResult/PlayerB/ButtonB", OnPlayerInfo);

		
		SetBtnFun (UIName + "/TopRight/ViewTools/ButtonOption", OptionSelect);
		SetBtnFun (UIName + "/TopRight/ViewTools/ViewOption/ButtonMusic", MusicSwitch);
		SetBtnFun (UIName + "/TopRight/ViewTools/ViewOption/ButtonMainMenu", BackMainMenu);
		SetBtnFun (UIName + "/TopRight/ViewTools/ViewOption/ButtonEffect", EffectSwitch);
		SetBtnFun (UIName + "/TopRight/ViewTools/ViewOption/ButtonAITime", AITimeChange);
		
		initAiTime();
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	private void initTarget() {
		int index = 0;
		if(GameController.Get.StageHintBit.Length > 0 && GameController.Get.StageHintBit[0] > 0) {
			gameTargets[index].Self.SetActive(true);
			gameTargets[index].LabelCaption.text = setText(index + 1, GameController.Get.StageHintBit[0], 8);
			gameTargets[index].LabelTargetName.text = setText(index + 1, GameController.Get.StageHintBit[0], 9);
			gameTargets[index].LabelTargetValue.text = "/"+  GameController.Get.StageBitNum[0];
			gameTargets[index].LabelValue.text = GameController.Get.GameTime.ToString();
			index ++;
		}

		if(GameController.Get.StageHintBit.Length > 1 && GameController.Get.StageHintBit[1] > 0) {
			gameTargets[index].Self.SetActive(true);
			gameTargets[index].LabelCaption.text = setText(index + 1, GameController.Get.StageHintBit[1], 8);
			gameTargets[index].LabelTargetName.text = setText(index + 1, GameController.Get.StageHintBit[1], 9);
			gameTargets[index].LabelTargetValue.text = "/"+ GameController.Get.StageBitNum[1];
			gameTargets[index].LabelValue.text = UIGame.Get.Scores[(int) ETeamKind.Self].ToString();
			index ++;
		}
		
		if(GameController.Get.StageHintBit.Length > 2 && GameController.Get.StageHintBit[2] > 0) {
			gameTargets[index].Self.SetActive(true);
			gameTargets[index].LabelCaption.text = setText(3, GameController.Get.StageHintBit[2], 8);
			gameTargets[index].LabelTargetName.text = setText(3, GameController.Get.StageHintBit[2], 9);
			gameTargets[index].LabelTargetValue.text = "/"+ GameController.Get.StageBitNum[2];
			gameTargets[index].LabelValue.text = getConditionCount(GameController.Get.StageHintBit[2]).ToString();
			index ++;
		}
		
		if(GameController.Get.StageHintBit.Length > 3 && GameController.Get.StageHintBit[3] > 0) {
			gameTargets[index].Self.SetActive(true);
			gameTargets[index].LabelCaption.text = setText(3, GameController.Get.StageHintBit[3], 8);
			gameTargets[index].LabelTargetName.text = setText(3, GameController.Get.StageHintBit[3], 9);
			gameTargets[index].LabelTargetValue.text = "/"+ GameController.Get.StageBitNum[3];
			gameTargets[index].LabelValue.text = getConditionCount(GameController.Get.StageHintBit[3]).ToString();
			index ++;
		}
	}

	private void initHomeAway (){
		int basemin = 0;
		int basemax = 3;
		if(pauseType == EPauseType.Away) {
			basemin = 3;
			basemax = 6;
		}
		string positionName = "";
		for (int i=0; i<gameRecord.PlayerRecords.Length; i++) {
			if (i>=basemin && i<basemax && GameData.DPlayers.ContainsKey(gameRecord.PlayerRecords[i].ID)) {
//				GameObject.Find(UIName + "Center/GameResult/PlayerMe/ButtonA/PlayerNameA/SpriteTypeA").GetComponent<UISprite>().spriteName = GameData.PlayerName (gameRecord.PlayerRecords[i + baseid].ID);

				switch (i) {
				case 0:
				case 3:
					positionName = "/Center/GameResult/PlayerMe/ButtonMe/PlayerNameMe/SpriteTypeMe";
					break;
				case 1:
				case 4:
					positionName = "/Center/GameResult/PlayerA/ButtonA/PlayerNameA/SpriteTypeA";
					break;
				case 2:
				case 5:
					positionName = "/Center/GameResult/PlayerB/ButtonB/PlayerNameB/SpriteTypeB";
					break;
				}
				if (GameData.DPlayers[gameRecord.PlayerRecords[i].ID].BodyType >= 0 && GameData.DPlayers[gameRecord.PlayerRecords[i].ID].BodyType < 3)
					GameObject.Find(UIName + positionName).GetComponent<UISprite>().spriteName = positionPicName[GameData.DPlayers[gameRecord.PlayerRecords[i].ID].BodyType];
			}
		}
	}

	private int getConditionCount(int type){
		switch (type){
		case 1://two score
			return GameController.Get.Joysticker.GameRecord.FGIn;
		case 2://three score
			return GameController.Get.Joysticker.GameRecord.FG3In;
		case 3://dunk
			return GameController.Get.Joysticker.GameRecord.Dunk;
		case 4://push
			return GameController.Get.Joysticker.GameRecord.Push;
		case 5://steal
			return GameController.Get.Joysticker.GameRecord.Steal;
		case 6://block
			return GameController.Get.Joysticker.GameRecord.Block;
		}
		return 0;
	}
		  
	private string setText (int index, int value, int id){
		int baseValue = 2000000 + (int)(Mathf.Pow(10,index) * value) + id;
		return TextConst.S(baseValue);
	}

	public void SetGameRecord(ref TGameRecord record) {
		gameRecord = record;
		UIShow(true);
		initTarget();
		uiGameTarget.SetActive(true);
		uiGameResult.SetActive(false);
		uiHomeButton.SetActive(true);
		uiAwayButton.SetActive(true);
		uiButtonRight.SetActive(false);
		uiButtonLeft.SetActive(false);
	}

	private void setInfo(int index, ref TGameRecord record) {
		if (index >= 0 && index < record.PlayerRecords.Length) {
			getInfoString(ref record.PlayerRecords[index]);
			initHomeAway ();
			
			switch (index) {
			case 0:
			case 3:
				uiSelect.transform.localPosition = new Vector3(0, 150, 0);
				break;
			case 1:
			case 4:
				uiSelect.transform.localPosition = new Vector3(270, 150, 0);
				break;
			case 2:
			case 5:
				uiSelect.transform.localPosition = new Vector3(-270, 150, 0);
				break;
			}
		}
	} 

	private string getInfoString(ref TGamePlayerRecord player) {
		int pts = player.FGIn * 2 + player.FG3In * 3;
		float fg = 0;
		if (player.FG > 0)
			fg = Mathf.Round(player.FGIn * 100 / player.FG);
		
		float fg3 = 0;
		if (player.FG3 > 0)
			fg3 = Mathf.Round(player.FG3In * 100 / player.FG3);
		
		SetLabel(UIName + "Center/GameResult/GameAttribute/PTS/LabelValue", pts.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/FG/LabelValue", fg.ToString() + "%");
		SetLabel(UIName + "Center/GameResult/GameAttribute/3FG/LabelValue", fg3.ToString() + "%");
		SetLabel(UIName + "Center/GameResult/GameAttribute/REB/LabelValue", player.Rebound.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/AST/LabelValue", player.Assist.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/STL/LabelValue", player.Steal.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/BLK/LabelValue", player.Block.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/TOV/LabelValue", player.BeIntercept.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/PUSH/LabelValue", player.Push.ToString());
		SetLabel(UIName + "Center/GameResult/GameAttribute/KNOCK/LabelValue", player.Knock.ToString());
		
		return string.Format("{0}\n{1}%\n{2}%\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n", 
		                     pts, fg, fg3, player.Rebound, player.Assist, player.Steal, player.Block, player.BeIntercept, player.Push, player.Knock);
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
		UIShow(false);
		if (isStage)
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
		else
			SceneMgr.Get.ChangeLevel (ESceneName.SelectRole, false);
	}
	
	public void OnResume() {
		UIShow(false);
		UIGame.Get.UIState(EUISituation.Continue);
	}
	
	public void OnAgain() {
		Time.timeScale = GameController.Get.RecordTimeScale;
		UIGame.Get.UIState(EUISituation.Reset);
		UIShow(false);
	}

	public void OnHomeResult () {
		pauseType = EPauseType.Home;
		uiHomeButton.SetActive(false);
		uiAwayButton.SetActive(false);
		uiButtonRight.SetActive(false);
		uiButtonLeft.SetActive(true);
		uiGameTarget.SetActive(false);
		uiGameResult.SetActive(true);
		setInfo(0, ref gameRecord);
	}

	public void OnAwayResult () {
		pauseType = EPauseType.Away;
		uiHomeButton.SetActive(false);
		uiAwayButton.SetActive(false);
		uiButtonRight.SetActive(true);
		uiButtonLeft.SetActive(false);
		uiGameTarget.SetActive(false);
		uiGameResult.SetActive(true);
		setInfo(3, ref gameRecord);
	}

	public void OnBackToTarget (){
		pauseType = EPauseType.Target;
		uiHomeButton.SetActive(true);
		uiAwayButton.SetActive(true);
		uiButtonRight.SetActive(false);
		uiButtonLeft.SetActive(false);
		uiGameTarget.SetActive(true);
		uiGameResult.SetActive(false);
	}

	public void BackMainMenu() {
		Time.timeScale = 1;
		SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
	}

	public void EffectSwitch(){
		GameData.Setting.Effect = !GameData.Setting.Effect;
		effectGroup [0].SetActive (GameData.Setting.Effect);
		effectGroup [1].SetActive (!GameData.Setting.Effect);
		
		int index = 0;
		
		if (GameData.Setting.Effect)
			index = 1;
		
		CourtMgr.Get.EffectEnable (GameData.Setting.Effect);
		
		PlayerPrefs.SetInt (SettingText.Effect, index);
		PlayerPrefs.Save ();
	}

	public void OptionSelect(){
		isShowOption = !isShowOption;
		viewOption.SetActive(isShowOption);
	}

	public void MusicSwitch(){
		isMusicOn = !isMusicOn;
		AudioMgr.Get.MusicOn(isMusicOn);
		musicGroup[0].SetActive(isMusicOn);
		musicGroup[1].SetActive(!isMusicOn);
	}

	public void AITimeChange (){
		GameController.Get.Joysticker.SetNoAI();
		viewAISelect.SetActive(!viewAISelect.gameObject.activeInHierarchy);
		if(viewAISelect.gameObject.activeInHierarchy){
			uiHomeButton.SetActive(false);
			uiAwayButton.SetActive(false);
			uiButtonRight.SetActive(false);
			uiButtonLeft.SetActive(false);
			uiGameTarget.SetActive(false);
			uiGameResult.SetActive(false);
		} else {
			pauseType = EPauseType.Target;
			uiHomeButton.SetActive(true);
			uiAwayButton.SetActive(true);
			uiButtonRight.SetActive(false);
			uiButtonLeft.SetActive(false);
			uiGameTarget.SetActive(true);
			uiGameResult.SetActive(false);
		}
		PlayerPrefs.SetFloat(SettingText.AITime, GameData.Setting.AIChangeTime);
	}

	private void initAiTime() {
		float time = PlayerPrefs.GetFloat(SettingText.AITime);
		if(time == 1) {
			aiLevelScrollBar.value = 0;
		} else if(time == 3) {
			aiLevelScrollBar.value = 0.2f;
		}else if(time == 5) {
			aiLevelScrollBar.value = 0.4f;
		}else if(time == 15) {
			aiLevelScrollBar.value = 0.6f;
		}else if(time == 30) {
			aiLevelScrollBar.value = 0.8f;
		}else if(time == 999999) {
			aiLevelScrollBar.value = 1;
		} 
	}

	public void changeAIChangeTime(){
		int level = (int)  Mathf.Round(aiLevelScrollBar.value * 5);
		float time = 1;
		if(level == 0) {
			time = 1;
		} else if(level == 1) {
			time = 3;
		}else if(level == 2) {
			time = 5;
		}else if(level == 3) {
			time = 15;
		}else if(level == 4) {
			time = 30;
		}else if(level == 5) {
			time = 999999;
		}
		GameData.Setting.AIChangeTime = time;
		
	}

	public bool isStage {
		get {return GameData.DStageData.ContainsKey(GameData.StageID); }
	}
}
