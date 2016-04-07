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
	private GameObject goStatsNextLabel;
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
	private bool isDeflation = false;

	public int minusValue;
	public int nowMin;
	public int nowMax;
	public int nowValue;
	
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
			if(nowValue > pvpRank.BeforeLowScore) {
				minusValue --;
				nowValue -- ;
				pvpObj.LabelNowPoint.text = nowValue.ToString();
				pvpObj.SliderBar.value = GameFunction.GetPercent(nowValue, nowMin, nowMax);
			} else {
				if(pvpRank.AfterLv == 1 && pvpRank.BeforeLv == 1) {
					return;
				} else {
					if(!isDeflation)
						deflation ();
				}
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

		goStatsNextLabel = GameObject.Find(UIName + "/BottomRight/StatsNextLabel");
		goStatsNextLabel.SetActive(false);

		UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/StatsNextLabel")).onClick = OnReturn;
	}

	public void SetPVPData (TPVPResult before, TPVPResult after) {
		setData(before, after);
		setEndData ();
	}


	public void Init () {
		setEndData ();
	}

	private void setEndData () {
		hintCount = UIStageHintManager.UpdateHintLose(GameData.StageID, ref mTargets);
		hintIndex = hintCount;
		Invoke("showFinish", 3);
	}

	private void setData (TPVPResult before, TPVPResult after) {
		before.PVPLv = GameFunction.GetPVPLv(before.PVPIntegral);
		after.PVPLv = GameFunction.GetPVPLv(after.PVPIntegral);
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
		goStatsNextLabel.SetActive(true);
	}

	private void pvpNext () {
		pvpObj.objAnimator.SetTrigger(PVPNext);
		Invoke("showRank", 2f);
	}

	private void showRank () {
		isShowRank = true;
		goStatsNextLabel.SetActive(true);
	}

	private void deflation () {
		isDeflation = true;
		pvpObj.objAnimator.SetTrigger(PVPDownRank);
		Invoke("showFinalRank", 0.5f);
	}

	private void showFinalRank () {
		pvpObj.PVPRankIcon.spriteName = GameFunction.PVPRankIconName(pvpRank.AfterLv);
		if(GameData.DPVPData.ContainsKey(pvpRank.AfterLv))
			pvpObj.LabelRankName.text = GameData.DPVPData[pvpRank.AfterLv].Name;

		nowMax = pvpRank.AfterHighScore;
		nowMin = pvpRank.AfterLowScore;
		pvpRank.BeforeLowScore = nowMin;
		pvpObj.LabelNextPoint.text = nowMax.ToString();
		showRank();
	}

	public void OnReturn (GameObject go) {
		Time.timeScale = 1;
		goStatsNextLabel.SetActive(false);
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
