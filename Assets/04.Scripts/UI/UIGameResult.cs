using UnityEngine;
using System.Collections;
using GameStruct;
using GamePlayEnum;

public class UIGameResult : UIBase {
	private static UIGameResult instance = null;
	private const string UIName = "UIGameResult";

	private TGameRecord gameRecord;

	private GameObject ViewResult;
	private GameObject ResultDetail;
	private GameObject buttonResume;
	private GameObject UISelect;
	private GameObject UIWin;
	private GameObject UILose;
	private GameObject uiLimitScore;
	private UILabel labelLimiteScore;
	private UITextList[] recordTextList = new UITextList[0];

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

	public static UIGameResult Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGameResult;
			
			return instance;
		}
	} 

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else
		if (isShow)
			Get.Show(isShow);
	}
	
	protected override void InitCom() {
		SetBtnFun(UIName + "/Bottom/ButtonAgain", OnAgain);
		SetBtnFun(UIName + "/Bottom/ButtonResume", OnResume);
		SetBtnFun(UIName + "/Bottom/ButtonReturnSelect", OnReturn);
		SetBtnFun(UIName + "/Center/ViewResult/ButtonNext", OnDetail);
		SetBtnFun(UIName + "/Center/ResultDetail/ButtonPrev", OnInfo);
		SetBtnFun(UIName + "/Center/ViewResult/PlayerMe/ButtonMe", OnPlayerInfo);
		SetBtnFun(UIName + "/Center/ViewResult/PlayerA/ButtonA", OnPlayerInfo);
		SetBtnFun(UIName + "/Center/ViewResult/PlayerB/ButtonB", OnPlayerInfo);

		ViewResult = GameObject.Find(UIName + "/Center/ViewResult");
		ResultDetail = GameObject.Find(UIName + "/Center/ResultDetail");
		buttonResume = GameObject.Find(UIName + "/Bottom/ButtonResume");
		UISelect = GameObject.Find(UIName + "/Center/ViewResult/Select");
		UIWin = GameObject.Find(UIName + "/Bottom/Result/SpriteWin");
		UIWin.SetActive(false);
		UILose = GameObject.Find(UIName + "/Bottom/Result/SpriteLose");
		UILose.SetActive(false);

		uiLimitScore = GameObject.Find(UIName + "/Center/ViewResult/LimitScore");
		labelLimiteScore = GameObject.Find(UIName + "/Center/ViewResult/LimitScore/TargetScore").GetComponent<UILabel>();
		if(GameStart.Get.WinMode == EWinMode.Score) {
			labelLimiteScore.text = GameStart.Get.GameWinValue.ToString();
		} else {
			uiLimitScore.SetActive(false);
		}
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void OnDetail() {
		ViewResult.SetActive(false);
		ResultDetail.SetActive(true);
		SetLabel(UIName + "Center/Title/LabelFinalScore", "");
	}

	public void OnInfo() {
		ViewResult.SetActive(true);
		ResultDetail.SetActive(false);
		SetLabel(UIName + "Center/Title/LabelFinalScore", string.Format("[c30000]{0}[-] : [0bf9d7]{1}[-]", gameRecord.Score2, gameRecord.Score1));
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
		UIGame.Get.UIState(EUISituation.Continue);
	}

	public void OnAgain() {
		Time.timeScale = GameController.Get.RecordTimeScale;
		UIGame.Get.UIState(EUISituation.Reset);
		UIShow(false);
	}

	public void OnPlayerInfo() {
		if (UIButton.current.name == "ButtonMe") 
			setInfo(0, ref gameRecord);
		else
		if (UIButton.current.name == "ButtonA") 
			setInfo(1, ref gameRecord);
		else
		if (UIButton.current.name == "ButtonB") 
			setInfo(2, ref gameRecord);
	}

	private string getInfoString(ref TGamePlayerRecord player) {
		int pts = player.FGIn * 2 + player.FG3In * 3;
		float fg = 0;
		if (player.FG > 0)
			fg = Mathf.Round(player.FGIn * 100 / player.FG);

		float fg3 = 0;
		if (player.FG3 > 0)
			fg3 = Mathf.Round(player.FG3In * 100 / player.FG3);

		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/PTS/LabelValue", pts.ToString());
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/FG/LabelValue", fg.ToString() + "%");
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/3FG/LabelValue", fg3.ToString() + "%");
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/REB/LabelValue", player.Rebound.ToString());
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/AST/LabelValue", player.Assist.ToString());
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/STL/LabelValue", player.Steal.ToString());
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/BLK/LabelValue", player.Block.ToString());
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/TOV/LabelValue", player.BeIntercept.ToString());
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/PUSH/LabelValue", player.Push.ToString());
		SetLabel(UIName + "Center/ViewResult/PlayerMe/GameAttribute/KNOCK/LabelValue", player.Knock.ToString());

		return string.Format("{0}\n{1}%\n{2}%\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n", 
		                     pts, fg, fg3, player.Rebound, player.Assist, player.Steal, player.Block, player.BeIntercept, player.Push, player.Knock);
	}

	private void addDetailString(ref TGamePlayerRecord player, ref UITextList list) {
		list.Clear();

		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "FG", player.FG));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "FGIn", player.FGIn));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "FG3", player.FG3));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "FG3In", player.FG3In));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "ShotError", player.ShotError));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Fake", player.Fake));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "BeFake", player.BeFake));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "ReboundLaunch", player.ReboundLaunch));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "Rebound", player.Rebound));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Assist", player.Assist));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "BeIntercept", player.BeIntercept));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Pass", player.Pass));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "StealLaunch", player.StealLaunch));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Steal", player.Steal));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "BeSteal", player.BeSteal));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Intercept", player.Intercept));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "BlockLaunch", player.BlockLaunch));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Block", player.Block));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "BeBlock", player.BeBlock));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "PushLaunch", player.PushLaunch));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "Push", player.Push));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "BePush", player.BePush));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "ElbowLaunch", player.ElbowLaunch));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Elbow", player.Elbow));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "BeElbow", player.BeElbow));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Knock", player.Knock));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "BeKnock", player.BeKnock));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "AlleyoopLaunch", player.AlleyoopLaunch));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "Alleyoop", player.Alleyoop));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "TipinLaunch", player.TipinLaunch));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "Tipin", player.Tipin));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "DunkLaunch", player.DunkLaunch));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "Dunk", player.Dunk));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "SaveBallLaunch", player.SaveBallLaunch));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "SaveBall", player.SaveBall));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "AngerAdd", player.AngerAdd));
		list.Add(string.Format("[FFFFFF]{0}:{1}[-]", "PassiveSkill", player.PassiveSkill));
		list.Add(string.Format("[0BF9FF]{0}:{1}[-]", "Skill", player.Skill));
	}

	public void AddDetailString(ref TPlayerAttribute Attr, int index) {
		if (index >= 0 && index < recordTextList.Length) {
			recordTextList[index].Add("[FF0000]Attribute-------------[-]");
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "PointRate2", Attr.PointRate2));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "PointRate3", Attr.PointRate3));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "StealRate", Attr.StealRate));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "DunkRate", Attr.DunkRate));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "TipInRate", Attr.TipInRate));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "AlleyOopRate", Attr.AlleyOopRate));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "StrengthRate", Attr.StrengthRate));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "BlockPushRate", Attr.BlockPushRate));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "ElbowingRate", Attr.ElbowingRate));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "ReboundRate", Attr.ReboundRate));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "BlockRate", Attr.BlockRate));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "FaketBlockRate", Attr.FaketBlockRate));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "JumpBallRate", Attr.JumpBallRate));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "PushingRate", Attr.PushingRate));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "PassRate", Attr.PassRate));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "AlleyOopPassRate", Attr.AlleyOopPassRate));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "ReboundHeadDistance", Attr.ReboundHeadDistance));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "ReboundHandDistance", Attr.ReboundHandDistance));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "BlockDistance", Attr.BlockDistance));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "DefDistance", Attr.DefDistance));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "SpeedValue", Attr.SpeedValue));
			recordTextList[index].Add(string.Format("[0BF9FF]{0}:{1}[-]", "StaminaValue", Attr.StaminaValue));
			recordTextList[index].Add(string.Format("[FFFFFF]{0}:{1}[-]", "AutoFollowTime", Attr.AutoFollowTime));
		}
	}

	private void setInfo(int index, ref TGameRecord record) {
		if (index >= 0 && index < record.PlayerRecords.Length) {
			getInfoString(ref record.PlayerRecords[index]);

			switch (index) {
			case 0:
				UISelect.transform.localPosition = new Vector3(0, 210, 0);
				break;
			case 1:
				UISelect.transform.localPosition = new Vector3(270, 210, 0);
				break;
			case 2:
				UISelect.transform.localPosition = new Vector3(-270, 210, 0);
				break;
			}
		}
	} 

	private void setDetail(ref TGameRecord record) {
		for (int i = 0; i < recordTextList.Length; i ++) 
			addDetailString(ref record.PlayerRecords[i], ref recordTextList[i]);
	}

	public void SetGameRecord(ref TGameRecord record) {
		gameRecord = record;
		UIShow(true);
		SetLabel(UIName + "Center/Title/LabelFinalScore", string.Format("[c30000]{0}[-] : [0bf9d7]{1}[-]", record.Score2, record.Score1));

		if (record.Done) {
			buttonResume.SetActive(false);
			if (record.Score1 > record.Score2) {
				UIWin.SetActive(true);
				UILose.SetActive(false);
			} else {
				UIWin.SetActive(false);
				UILose.SetActive(true);
			}
		} else {
			buttonResume.SetActive(true);
			UIWin.SetActive(false);
			UILose.SetActive(false);
		}

		if (recordTextList.Length != record.PlayerRecords.Length) {
			int num = record.PlayerRecords.Length;
			if (num > 3)
				num = 3;

			recordTextList = new UITextList[num];

			string[] positionPicName = {"L_namecard_CENTER", "L_namecard_FORWARD", "L_namecard_GUARD"};

			for (int i = 0; i < recordTextList.Length; i ++) {
				string name = "";
				string faceName = "";
				string positionName = "";

				switch (i) {
				case 0:
					name = "/Center/ResultDetail/LabelMe/TextList";
					faceName = "/Center/ViewResult/PlayerMe/ButtonMe/PlayerFace/MyFace";
					positionName = "/Center/ViewResult/PlayerMe/ButtonMe/PlayerNameMe/SpriteTypeMe";
					break;
				case 1:
					name = "/Center/ResultDetail/LabelA/TextList";
					faceName = "/Center/ViewResult/PlayerA/ButtonA/PlayerFace/AFace";
					positionName = "/Center/ViewResult/PlayerA/ButtonA/PlayerNameA/SpriteTypeA";
					break;
				case 2:
					name = "/Center/ResultDetail/LabelB/TextList";
					faceName = "/Center/ViewResult/PlayerB/ButtonB/PlayerFace/BFace";
					positionName = "/Center/ViewResult/PlayerB/ButtonB/PlayerNameB/SpriteTypeB";
					break;
				}

				recordTextList[i] = GameObject.Find(UIName + name).GetComponent<UITextList>();
				int id = record.PlayerRecords[i].ID;
				if (GameData.DPlayers.ContainsKey(id)) {
					GameObject.Find(UIName + faceName).GetComponent<UISprite>().spriteName = GameData.PlayerName (id);
					if (GameData.DPlayers[id].BodyType >= 0 && GameData.DPlayers[id].BodyType < 3)
						GameObject.Find(UIName + positionName).GetComponent<UISprite>().spriteName = positionPicName[GameData.DPlayers[id].BodyType];
				}
			}
		}

		setInfo(0, ref record);
		setDetail(ref record);
		
		ResultDetail.SetActive(false);
	}

	public bool isStage {
		get {return GameData.DStageData.ContainsKey(GameData.StageID); }
	}
}
