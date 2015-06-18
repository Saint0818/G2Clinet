using UnityEngine;
using System.Collections;
using GameStruct;

public class UIGameResult : UIBase {
	private static UIGameResult instance = null;
	private const string UIName = "UIGameResult";

	private GameObject ViewResult;
	private GameObject ResultDetail;
	private GameObject UIWin;
	private GameObject UILose;
	private UITextList textListMe;
	private UITextList textListA;
	private UITextList textListB;

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
		} else
		if (isShow)
			Get.Show(isShow);
	}
	
	public static UIGameResult Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGameResult;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		SetBtnFun(UIName + "/Bottom/ButtonAgain", OnAgain);
		SetBtnFun(UIName + "/Bottom/ButtonReturnSelect", OnReturn);
		SetBtnFun(UIName + "/Center/ViewResult/ButtonNext", OnDetail);
		SetBtnFun(UIName + "/Center/ResultDetail/ButtonPrev", OnInfo);

		textListMe = GameObject.Find(UIName + "/Center/ResultDetail/LabelMe/TextList").GetComponent<UITextList>();
		textListA = GameObject.Find(UIName + "/Center/ResultDetail/LabelA/TextList").GetComponent<UITextList>();
		textListB = GameObject.Find(UIName + "/Center/ResultDetail/LabelB/TextList").GetComponent<UITextList>();
		ViewResult = GameObject.Find(UIName + "/Center/ViewResult");
		ResultDetail = GameObject.Find(UIName + "/Center/ResultDetail");
		UIWin = GameObject.Find(UIName + "/Top/Title/SpriteWin");
		UIWin.SetActive(false);
		UILose = GameObject.Find(UIName + "/Top/Title/SpriteLose");
		UILose.SetActive(false);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void OnDetail() {
		ViewResult.SetActive(false);
		ResultDetail.SetActive(true);
	}

	public void OnInfo() {
		ViewResult.SetActive(true);
		ResultDetail.SetActive(false);
	}

	public void OnReturn() {
		Time.timeScale = 1;
		SceneMgr.Get.ChangeLevel (SceneName.SelectRole);
	}

	public void OnAgain() {
		Time.timeScale = 1;
		UIGame.Get.UIState(UISituation.Reset);
		UIShow(false);
	}

	private string getInfoString(ref TGamePlayerRecord player) {
		int pts = player.FGIn * 2 + player.FG3In * 3;
		float fg = 0;
		if (player.FG > 0)
			fg = Mathf.Round(player.FGIn / player.FG * 100) / 100;

		float fg3 = 0;
		if (player.FG3 > 0)
			fg = Mathf.Round(player.FG3In / player.FG3 * 100) / 100;

		return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n", 
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

	private string getDetailString(ref TGamePlayerRecord player) {
		return string.Format("{0}:{1}\n{2}:{3}\n{4}:{5}\n{6}:{7}\n{8}:{9}\n{10}:{11}\n{12}:{13}\n{14}:{15}\n{16}:{17}\n{18}:{19}\n{20}:{21}\n{22}:{23}\n{24}:{25}\n{26}:{27}\n{28}:{29}\n{30}:{31}\n{32}:{33}\n{34}:{35}\n{36}:{37}\n{38}:{39}\n{40}:{41}\n{42}:{43}\n{44}:{45}\n{46}:{47}\n{48}:{49}\n{50}:{51}\n{52}:{53}\n{54}:{55}\n{56}:{57}\n{58}:{59}\n{60}:{61}\n{62}:{63}\n{64}:{65}\n{66}:{67}\n{68}:{69}", 
		                     "FG", player.FG, "FGIn", player.FGIn, "FG3", player.FG3, "FG3In", player.FG3In, "ShotError", player.ShotError, 
		                     "Fake", player.Fake, "BeFake", player.BeFake, "ReboundLaunch", player.ReboundLaunch, "Rebound", player.Rebound,   
		                     "Pass", player.Pass, "StealLaunch", player.StealLaunch, "Steal", player.Steal, "BeSteal", player.BeSteal, 
		                     "Intercept", player.Intercept, "BlockLaunch", player.BlockLaunch, "Block", player.Block, "BeBlock", player.BeBlock, "PushLaunch", player.PushLaunch, 
		                     "Push", player.Push, "BePush", player.BePush, "ElbowLaunch", player.ElbowLaunch, "Elbow", player.Elbow, "BeElbow", player.BeElbow, 
		                     "BeKnock", player.BeKnock, "AlleyoopLaunch", player.AlleyoopLaunch, "Alleyoop", player.Alleyoop, "TipinLaunch", player.TipinLaunch,  
		                     "DunkLaunch", player.DunkLaunch, "Dunk", player.Dunk, "SaveBallLaunch", player.SaveBallLaunch, "SaveBall", player.SaveBall, "AngerAdd", player.AngerAdd, 
		                     "Tipin", player.Tipin, "PassiveSkill", player.PassiveSkill, "Skill", player.Skill);
	}

	private void setInfo(ref TGameRecord record) {
		string str = "";
		if (record.PlayerRecords.Length > 0)
			str = getInfoString(ref record.PlayerRecords[0]);

		SetLabel(UIName + "Center/ViewResult/PlayerMe/LabelValue", str);

		if (record.PlayerRecords.Length > 1)
			str = getInfoString(ref record.PlayerRecords[1]);
		else
			str = "";

		SetLabel(UIName + "Center/ViewResult/PlayerA/LabelValue", str);

		if (record.PlayerRecords.Length > 2)
			str = getInfoString(ref record.PlayerRecords[2]);
		else
			str = "";
		
		SetLabel(UIName + "Center/ViewResult/PlayerB/LabelValue", str);
	} 

	private void setDetail(ref TGameRecord record) {
		addDetailString(ref record.PlayerRecords[0], ref textListMe);
		addDetailString(ref record.PlayerRecords[1], ref textListA);
		addDetailString(ref record.PlayerRecords[2], ref textListB);
	}

	public void SetGameRecord(ref TGameRecord record) {
		UIShow(true);
		SetLabel(UIName + "Top/Title/LabelFinalScore", string.Format("{0} : {1}", record.Score1, record.Score2));

		if (record.Done) {
			if (record.Score1 > record.Score2) {
				UIWin.SetActive(true);
				UILose.SetActive(false);
			} else {
				UIWin.SetActive(false);
				UILose.SetActive(true);
			}
		} else {
			UIWin.SetActive(false);
			UILose.SetActive(false);
		}

		setInfo(ref record);
		setDetail(ref record);
		
		ResultDetail.SetActive(false);
	}
}
