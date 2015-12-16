﻿using UnityEngine;
using System.Collections;

public class UIGameLoseResult : UIBase {
	private static UIGameLoseResult instance = null;
	private const string UIName = "UIGameLoseResult";

	private GameStageTargetLose[] mTargets;
	private const float finishInterval = 0.2f;
	private int hintIndex;
	private int hintCount;
	private bool isShowFinish = false;
	private float finishTime = 0;
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
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
					if(hintIndex > 0 && hintIndex < mTargets.Length)
						mTargets[hintCount - hintIndex].UpdateFin(mTargets[hintCount - hintIndex].IsComplete);

					finishTime = finishInterval;
					hintIndex --;
				}
			}
		}
	}
	
	protected override void InitCom() {
		mTargets = GetComponentsInChildren<GameStageTargetLose>();

		UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/StatsNextLabel")).onClick = OnReturn;
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {

	}

	public void Init () {
		hintCount = UIStageHintManager.UpdateHintLose(GameData.StageID, ref mTargets);
		hintIndex = hintCount;
		Invoke("showFinish", 3);
	}

	private void showFinish () {
		isShowFinish = true;
		finishTime = finishInterval;
	}

	public void OnReturn (GameObject go) {
		Time.timeScale = 1;
		UIShow(false);
		if (GameController.Visible && GameController.Get.StageData.IsTutorial) {
			if (StageTable.Ins.HasByID(GameController.Get.StageData.ID + 1)) {
				GameData.StageID = GameController.Get.StageData.ID + 1;
				int courtNo = StageTable.Ins.GetByID(GameData.StageID).CourtNo;
				SceneMgr.Get.CurrentScene = "";
				SceneMgr.Get.ChangeLevel (courtNo);
			} else {
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			}
		} else {
			UILoading.OpenUI = UILoading.OpenStageUI;
			if (isStage)
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
			else
				SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
		}
	}

	public bool isStage
	{
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
