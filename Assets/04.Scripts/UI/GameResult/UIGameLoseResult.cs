using UnityEngine;
using GameStruct;
using Newtonsoft.Json;

public class TPVPObj {
	public Animator objAnimator;
	public UISprite PVPRankIcon;
	public UILabel LabelRankName;
	public UILabel LabelMinusPoint;
	public UILabel LabelNowPoint;
	public UISlider SliderBar;

	public void SetValue (TPVPRank result, bool isLost = true) {
		PVPRankIcon.spriteName = GameFunction.PVPRankIconName(result.BeforeLv);
		LabelRankName.text = result.BeforeName ;
		LabelMinusPoint.text = Mathf.Abs(result.AfterScore- result.BeforeScore).ToString();
		if(isLost)
			LabelNowPoint.text = string.Format("[FF0000FF]{0}[-]/{1}", result.BeforeScore, result.BeforeHighScore);
		else
			LabelNowPoint.text = string.Format("[00FF00FF]{0}[-]/{1}", result.BeforeScore, result.BeforeHighScore);
		SliderBar.value = GameFunction.GetPercent(result.BeforeScore, result.BeforeLowScore, result.BeforeHighScore);
	}

	public void SetValue (TPVPValue value, bool isLost = true) {
		if(isLost)
			LabelNowPoint.text = string.Format("[FF0000FF]{0}[-]/{1}", value.NowValue, value.NowMax);
		else
			LabelNowPoint.text = string.Format("[00FF00FF]{0}[-]/{1}", value.NowValue, value.NowMax);
		SliderBar.value = GameFunction.GetPercent(value.NowValue, value.NowMin, value.NowMax);
	}

	public void SetFinal (TPVPRank result, string rankName) {
		PVPRankIcon.spriteName = GameFunction.PVPRankIconName(result.AfterLv);
		LabelRankName.text = rankName;
	}
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

public struct TPVPValue {
	public bool IsDeflation;
	public bool IsShowRank;
	public int MinusValue;
	public int NowMin;
	public int NowMax;
	public int NowValue;

	public void SetValue (TPVPRank result) {
		MinusValue = Mathf.Abs(result.AfterScore- result.BeforeScore);
		NowMin = result.BeforeLowScore;
		NowMax = result.BeforeHighScore;
		NowValue = result.BeforeScore;
	}

	public void SetFinal (TPVPRank result) {
		NowValue = result.AfterScore;
		NowMax = result.AfterHighScore;
		NowMin = result.AfterLowScore;
	}

	public bool IsRunRankExp {
		get {
			return (IsShowRank && MinusValue > 0);
		}
	}
}

public struct THintValue {
	public GameStageTargetLose[] LoseTargets;
	public UIStageHintTarget[] WinTargets;
	public float FinishInterval;
	public int HintIndex;
	public int HintCount;
	public bool IsShowFinish ;
	public float FinishTime;

	public void Init () {
		FinishInterval = 0.2f;
	}

	public void SetHintCount (int count) {
		HintCount = count;
		HintIndex = count;
	}

	public void InitFinish () {
		IsShowFinish = true;
		FinishTime = FinishInterval;
	}

	public void CostHint () {
		FinishTime = FinishInterval;
		HintIndex --;
	}

	public void UpdateUI (float time, EventDelegate.Callback oneAchieve, EventDelegate.Callback complete) {
		if(IsRunHint) {
			FinishTime -= time;
			if(FinishTime <= 0) {
				if(HintIndex == -1) {
					IsShowFinish = false;
					complete();
				} else {
					oneAchieve();
					CostHint ();
				}
			}
		}
	}

	public bool IsRunHint{
		get {
			return (IsShowFinish && HintIndex >= -1);
		}
	}

	public int ExtraHint {
		get {return HintCount - HintIndex;}
	}
}

public class UIGameLoseResult : UIBase {
	private static UIGameLoseResult instance = null;
	private const string UIName = "UIGameLoseResult";

	private GameObject goStatsNextLabel;

	private THintValue hintValue = new THintValue();

	//PVP
	private const string PVPNext = "Next";
	private const string PVPDownRank = "DownRank";
	private TPVPObj pvpObj = new TPVPObj();
	private TPVPRank pvpRank = new TPVPRank();
	private TPVPValue pvpValue = new TPVPValue();
	
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
		hintValue.UpdateUI(Time.deltaTime, HintAchieveOne, HintComplete);


		if(pvpValue.IsRunRankExp) {
			if(pvpValue.NowValue > pvpRank.BeforeLowScore) {
				pvpValue.MinusValue --;
				pvpValue.NowValue -- ;
				pvpObj.SetValue(pvpValue);
			} else {
				if(pvpRank.AfterLv == 1 && pvpRank.BeforeLv == 1) {
					return;
				} else {
					if(!pvpValue.IsDeflation)
						deflation ();
				}
			}
		}
	}

	public void HintAchieveOne () {
		if(hintValue.HintIndex > 0 && hintValue.HintIndex < hintValue.LoseTargets.Length) {
			if(hintValue.LoseTargets[hintValue.ExtraHint].IsComplete)
				AudioMgr.Get.PlaySound(SoundType.SD_ResultCount);
			hintValue.LoseTargets[hintValue.ExtraHint].UpdateFin(hintValue.LoseTargets[hintValue.ExtraHint].IsComplete);
		}
	}

	public void HintComplete () {
	}

	
	protected override void InitCom() {
		pvpObj.objAnimator = transform.GetComponent<Animator>();
		pvpObj.PVPRankIcon = GameObject.Find(UIName + "/RankView/PvPRankIcon").GetComponent<UISprite>();
		pvpObj.LabelRankName = GameObject.Find(UIName + "/RankView/PvPRankIcon/RankNameLabel").GetComponent<UILabel>();
		pvpObj.LabelMinusPoint = GameObject.Find(UIName + "/RankView/RankPoint/GetPointLabel").GetComponent<UILabel>();
		pvpObj.LabelNowPoint = GameObject.Find(UIName + "/RankView/NowPoint").GetComponent<UILabel>();
		pvpObj.SliderBar = GameObject.Find(UIName + "/RankView/ProgressBar").GetComponent<UISlider>();

		goStatsNextLabel = GameObject.Find(UIName + "/BottomRight/StatsNextLabel");
		goStatsNextLabel.SetActive(false);

		hintValue.Init();
		hintValue.LoseTargets = GetComponentsInChildren<GameStageTargetLose>();

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
		hintValue.SetHintCount(UIStageHintManager.UpdateHintLose(GameData.StageID, ref hintValue.LoseTargets));
		Invoke("showFinish", GameConst.GameEndWait);
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

			pvpValue.SetValue(pvpRank);
			pvpObj.SetValue(pvpRank);
		}
	}

	private void showFinish () {
		hintValue.InitFinish();
		goStatsNextLabel.SetActive(true);
	}

	private void pvpNext () {
		pvpObj.objAnimator.SetTrigger(PVPNext);
		Invoke("showRank", 2f);
	}

	private void showRank () {
		pvpValue.IsShowRank = true;
		goStatsNextLabel.SetActive(true);
	}

	//降階
	private void deflation () {
		pvpValue.IsDeflation = true;
		pvpObj.objAnimator.SetTrigger(PVPDownRank);
		Invoke("showFinalRank", 0.5f);
	}

	private void showFinalRank () {
		if(GameData.DPVPData.ContainsKey(pvpRank.AfterLv))
			pvpObj.SetFinal(pvpRank, GameData.DPVPData[pvpRank.AfterLv].Name);
		else
			pvpObj.SetFinal(pvpRank, "");

		pvpValue.SetFinal(pvpRank);
		pvpRank.BeforeLowScore = pvpValue.NowMin;
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
			if(!pvpValue.IsShowRank)
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
