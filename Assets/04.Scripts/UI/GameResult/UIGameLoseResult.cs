using UnityEngine;
using GameStruct;
using Newtonsoft.Json;

public class TPVPObj {
	public Animator objAnimator;
	public UISprite PVPRankIcon;
	public UILabel LabelRankName;
	public UILabel LabelMinusPoint;
	public UILabel LabelNowPoint;
	public UILabel LabelNextPoint;
	public UISlider SliderBar;
}

public struct TPVPRank {
	public int BeforeLv;
	public int AfterLv;
	public int BeforeScore;
	public int AfterScore;
	public string BeforeName;
	public string AfterName;
	public int BeforeLowScore;
	public int AfterLowScore;
	public int BeforeHighScore;
	public int AfterHighScore;
}

public class UIGameLoseResult : UIBase {
	private static UIGameLoseResult instance = null;
	private const string UIName = "UIGameLoseResult";

	private GameStageTargetLose[] mTargets;
	private const float finishInterval = 0.2f;
	private int hintIndex;
	private int hintCount;
	private bool isShowFinish = false;
	private float finishTime = 0;

	//PVP
	private const string PVPNext = "Next";
	private const string PVPDownRank = "DownRank";
	private TPVPObj pvpObj = new TPVPObj();
	private TPVPRank pvpRank = new TPVPRank();
	private bool isShowRank = false;

	public int minusValue;
	public int nowMin;
	public int nowMax;
	public int nowValue;

	private TPVPResult beforeTeam = new TPVPResult();
	private TPVPResult afterTeam = new TPVPResult();
	
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

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else
			if (isShow)
				Get.Show(isShow);
	}
	
	public static UIGameLoseResult Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGameLoseResult;
			
			return instance;
		}
	}

	void FixedUpdate () {
		//Show StageHint
		if(isShowFinish && hintIndex >= -1) {
			finishTime -= Time.deltaTime;
			if(finishTime <= 0) {
				if(hintIndex == -1) {
					isShowFinish = false;
				} else {
					if(hintIndex > 0 && hintIndex < mTargets.Length) {
						if(mTargets[hintCount - hintIndex].IsComplete)
							AudioMgr.Get.PlaySound(SoundType.SD_ResultCount);
						mTargets[hintCount - hintIndex].UpdateFin(mTargets[hintCount - hintIndex].IsComplete);
					}

					finishTime = finishInterval;
					hintIndex --;
				}
			}
		}

		if(isShowRank && minusValue > 0) {
			if(nowValue >= pvpRank.BeforeLowScore) {
				minusValue --;
				nowValue -- ;
				pvpObj.LabelNowPoint.text = nowValue.ToString();
				pvpObj.SliderBar.value = GameFunction.GetPercent(nowValue, nowMin, nowMax);
			} else {
				isShowRank = false;
				deflation ();
			}
		}
	}
	
	protected override void InitCom() {
		mTargets = GetComponentsInChildren<GameStageTargetLose>();
		pvpObj.objAnimator = transform.GetComponent<Animator>();
		pvpObj.PVPRankIcon = GameObject.Find(UIName + "/RankView/PvPRankIcon").GetComponent<UISprite>();
		pvpObj.LabelRankName = GameObject.Find(UIName + "/RankView/PvPRankIcon/RankNameLabel").GetComponent<UILabel>();
		pvpObj.LabelMinusPoint = GameObject.Find(UIName + "/RankView/RankPoint/GetPointLabel").GetComponent<UILabel>();
		pvpObj.LabelNowPoint = GameObject.Find(UIName + "/RankView/NowPoint").GetComponent<UILabel>();
		pvpObj.LabelNextPoint = GameObject.Find(UIName + "/RankView/NextPoint").GetComponent<UILabel>();
		pvpObj.SliderBar = GameObject.Find(UIName + "/RankView/ProgressBar").GetComponent<UISlider>();

		UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/StatsNextLabel")).onClick = OnReturn;
	}


	public void Init () {
		if (GameData.IsPVP) {
			WWWForm form = new WWWForm();
			form.AddField("Score1", UIGame.Get.Scores [0]);
			form.AddField("Score2", UIGame.Get.Scores [1]);
			SendHttp.Get.Command(URLConst.PVPEnd, waitPVPEnd, form, false);
			GameData.PVPEnemyMembers[0].Identifier = string.Empty;
		} else {
			setEndData ();
		}
	}

	private void waitPVPEnd(bool ok, WWW www)
	{
		if (ok) {
			beforeTeam.PVPLv = GameData.Team.PVPLv;
			beforeTeam.PVPIntegral = GameData.Team.PVPIntegral;
			beforeTeam.PVPCoin = GameData.Team.PVPCoin;
			TPVPResult reslut = JsonConvert.DeserializeObject <TPVPResult>(www.text, SendHttp.Get.JsonSetting); 
			afterTeam.PVPLv = reslut.PVPLv;
			afterTeam.PVPIntegral = reslut.PVPIntegral;
			afterTeam.PVPCoin = reslut.PVPCoin;

			GameData.Team.PVPLv = reslut.PVPLv;
			GameData.Team.PVPIntegral = reslut.PVPIntegral;
			GameData.Team.PVPCoin = reslut.PVPCoin;
			GameData.Team.LifetimeRecord = reslut.LifetimeRecord;
			setData(beforeTeam, afterTeam);
		}
	}

	private void setEndData () {
		hintCount = UIStageHintManager.UpdateHintLose(GameData.StageID, ref mTargets);
		hintIndex = hintCount;
		Invoke("showFinish", 3);
	}

	private void setData (TPVPResult before, TPVPResult after) {
		if(GameData.DPVPData.ContainsKey(before.PVPLv) && GameData.DPVPData.ContainsKey(after.PVPLv)) {
			setEndData ();
			pvpRank.BeforeLv = before.PVPLv;
			pvpRank.BeforeName = GameData.DPVPData[before.PVPLv].Name;
			pvpRank.BeforeScore = before.PVPIntegral;
			pvpRank.BeforeLowScore = GameData.DPVPData[before.PVPLv].LowScore;
			pvpRank.BeforeHighScore = GameData.DPVPData[before.PVPLv].HighScore;
			pvpRank.AfterLv = after.PVPLv;
			pvpRank.AfterName = GameData.DPVPData[after.PVPLv].Name;
			pvpRank.AfterScore = after.PVPIntegral;
			pvpRank.AfterLowScore = GameData.DPVPData[after.PVPLv].LowScore;
			pvpRank.AfterHighScore = GameData.DPVPData[after.PVPLv].HighScore;

			minusValue = Mathf.Abs(pvpRank.AfterScore- pvpRank.BeforeScore);
			nowValue = pvpRank.BeforeScore;
			nowMin = pvpRank.BeforeLowScore;
			nowMax = pvpRank.BeforeHighScore;

			pvpObj.PVPRankIcon.spriteName = GameFunction.PVPRankIconName(pvpRank.BeforeLv);
			pvpObj.LabelRankName.text = pvpRank.BeforeName ;
			pvpObj.LabelMinusPoint.text = minusValue.ToString();
			pvpObj.LabelNowPoint.text = nowValue.ToString();
			pvpObj.LabelNextPoint.text = pvpRank.BeforeHighScore.ToString();
			pvpObj.SliderBar.value = GameFunction.GetPercent(pvpRank.BeforeScore, pvpRank.BeforeLowScore, pvpRank.BeforeHighScore);
		}
	}

	private void showFinish () {
		isShowFinish = true;
		finishTime = finishInterval;
	}

	private void pvpNext () {
		pvpObj.objAnimator.SetTrigger(PVPNext);
		Invoke("showRank", 2f);
	}

	private void showRank () {
		isShowRank = true;
	}

	private void deflation () {
		pvpObj.objAnimator.SetTrigger(PVPDownRank);
		Invoke("showFinalRank", 0.5f);
	}

	private void showAfterRank () {
		pvpObj.PVPRankIcon.spriteName = GameFunction.PVPRankIconName(pvpRank.AfterLv);
		if(GameData.DPVPData.ContainsKey(pvpRank.AfterLv))
			pvpObj.LabelRankName.text = GameData.DPVPData[pvpRank.AfterLv].Name;

		nowMax = pvpRank.AfterHighScore;
		nowMin = pvpRank.AfterLowScore;
		pvpObj.LabelNextPoint.text = nowMax.ToString();
		showRank();
	}

	public void OnReturn (GameObject go) {
		Time.timeScale = 1;
		if(GameData.IsMainStage)
		{
			UIShow(false);
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenStageUI;
		}
		else if(GameData.IsInstance)
		{
			UIShow(false);
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenInstanceUI;
		}
		else if (GameData.IsPVP)
		{
			if(!isShowRank)
				pvpNext ();
			else {
				UIShow(false);
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
	            UILoading.OpenUI = UILoading.OpenPVPUI;
			}
		}
		else
		{
			UIShow(false);
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			UILoading.OpenUI = UILoading.OpenStageUI;
		}
	}
}
