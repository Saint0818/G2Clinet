using UnityEngine;
using System;
using System.Collections;
using GameStruct;
using GameEnum;
using Newtonsoft.Json;
using System.Collections.Generic;

public struct TGymBuildResult {
	public int Diamond;
	public int[] GymOwn;
	public TGymBuild[] GymBuild;
}

public struct TGymResult {
	public int Money;
	public int Diamond;
	public TGymBuild[] GymBuild;
	public TGymQueue[] GymQueue;
	public int StatiumLv;
}

public struct TGymObj {
	public GameObject Obj;
	public UILabel LevelLabel;
	public UISlider CDBar;
	public UILabel TimeLabel;
	public GameObject RedPoint;
}

[System.Serializable]
public struct TGymQueueObj {
	public UILabel NameLabel;
	public UISlider CDBar;
	public UILabel TimeLabel;
	public GameObject GoEmpty;
	public UILabel EmptyLabel; // 閒置中，LV 開啟
	public UISprite ToolSprite;
}

public class UIGym : UIBase {
	private static UIGym instance = null;
	private const string UIName = "UIGym";

	private GameObject window;

	private GameObject gymCenter;
	private TGymObj[] gymObj = new TGymObj[9];

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

	public static UIGym Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGym;

			return instance;
		}
	}

	void FixedUpdate () {
		if(UIMainLobby.Get.IsVisible && UIMainLobby.Get.IsCheckUpdateOnLoad)
			updateBuildCD ();
	}

	protected override void InitCom() {
		window = GameObject.Find(UIName + "/Window");
		gymCenter = GameObject.Find(UIName + "/Window/Center");
		for (int i=0; i<gymObj.Length; i++) {
			gymObj[i].Obj = GameObject.Find(UIName + "/Window/Center/" + i.ToString());
			gymObj[i].LevelLabel = GameObject.Find(UIName + "/Window/Center/" + i.ToString() + "/LevelLabel").GetComponent<UILabel>();
			gymObj[i].CDBar = GameObject.Find(UIName + "/Window/Center/" + i.ToString() + "/CDBar").GetComponent<UISlider>();
			gymObj[i].TimeLabel  = GameObject.Find(UIName + "/Window/Center/" + i.ToString() + "/CDBar/TimeLabel").GetComponent<UILabel>();
			gymObj[i].RedPoint  = GameObject.Find(UIName + "/Window/Center/" + i.ToString() + "/RedPoint");
			SetBtnFun (UIName + "/Window/Center/" + i.ToString(), OnClickBuild);
		}
	}

	public void UpdateText () {
		initDefaultText(window);
	}

	//場景上的建築物從1開始， Array從0開始
	public void OnClickBuild () {
		int result = 0;
		if (int.TryParse (UIButton.current.name, out result)) {
			if (isCanUse(result + 1)) {
				// todo: need to refactor
				Statistic.Ins.LogScreen(24 + result);
				Statistic.Ins.LogEvent(600 + (50 * result) + 1);

				if (result != 8) {
					if (UI3DMainLobby.Visible)
						UI3DMainLobby.Get.Impl.OnSelect (result);
				} else {
					UIMainLobby.Get.View.PlayExitAnimation();
				}

				if (UIMainLobby.Get.IsVisible)
					UIMainLobby.Get.View.QueueGroup.SetActive (false);
				
				CenterVisible = false;
				// todo: need to refactor
				if (result==8)
					UIMail.Visible = true;
				else
					StartCoroutine(ShowEngage(result));
			} else
				UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber(result + 1 + 50)), LimitTable.Ins.GetLv((EOpenID)(result + 1 + 50))), Color.red);

		}
	}

	private void setStatistic (int buildIndex) {
		Statistic.Ins.LogEvent(600 + (50 * buildIndex) + (buildIndex + 1));
	}

	private IEnumerator ShowEngage (int index) {
		yield return new WaitForSeconds(1);
		UIGymEngage.Get.ShowView(index);
		
	}

	public void ShowView() {
		Visible = true;
		for (int i=0; i<gymObj.Length; i++) {
			gymObj[i].Obj.SetActive(isCanShow(i + 1));
		}
		RefreshBuild ();
	}

	//3為球館，其他建築物等級不可以超過
	public void RefreshBuild () {
		for (int i=0; i<gymObj.Length; i++) {
			gymObj[i].LevelLabel.text = string.Format(TextConst.S(11021), GameFunction.GetBuildLv(i));
			gymObj[i].LevelLabel.gameObject.SetActive((i != 8));
			gymObj[i].CDBar.gameObject.SetActive(isBuildRun(i));
			GameFunction.SetGymRedPoint(ref gymObj[i].RedPoint, i);
//			if(i == 3)
//				gymObj[i].RedPoint.SetActive(GameFunction.GetBuildLv(i) < GameData.DBuildHightestLvs[i] && GameFunction.IsBuildLvEnough(i) && (GameFunction.GetIdleQueue != 0) && !GameFunction.IsBuildQueue(i));
//			else { 
//				if(GameFunction.GetBuildLv(i) >= GameFunction.GetGymLv) 
//					gymObj[i].RedPoint.SetActive(false);
//				else 
//					gymObj[i].RedPoint.SetActive(GameFunction.IsBuildLvEnough(i) && (GameFunction.GetIdleQueue != 0) && !GameFunction.IsBuildQueue(i));
//			}
		}
	}

	private bool isBuildRun (int buildIndex) {
		for (int i = 0; i < GameData.Team.GymQueue.Length; i++) 
			if (GameData.Team.GymQueue [i].BuildIndex == buildIndex)
				return true;

		return false;
	}

	private void updateBuildCD () {
		if(GameData.Team.GymQueue != null) {
			for(int i=0; i<GameData.Team.GymQueue.Length; i++) {
				if(GameData.Team.GymQueue[i].IsOpen && GameData.Team.GymQueue[i].BuildIndex != -1 && GameData.Team.GymQueue[i].BuildIndex < gymObj.Length && GameData.Team.GymQueue[i].BuildIndex < GameData.Team.GymBuild.Length) {
					if (GameData.Team.GymBuild [GameData.Team.GymQueue [i].BuildIndex].Time.ToUniversalTime () > DateTime.UtcNow) {
						if(gymObj [GameData.Team.GymQueue [i].BuildIndex].LevelLabel.gameObject.activeSelf)
							gymObj [GameData.Team.GymQueue [i].BuildIndex].LevelLabel.gameObject.SetActive(false);
						gymObj [GameData.Team.GymQueue [i].BuildIndex].CDBar.value = TextConst.DeadlineStringPercent (GameFunction.GetOriTime (GameData.Team.GymQueue [i].BuildIndex, GameFunction.GetBuildLv (GameData.Team.GymQueue [i].BuildIndex) - 1, GameFunction.GetBuildTime (GameData.Team.GymQueue [i].BuildIndex).ToUniversalTime ()), GameFunction.GetBuildTime (GameData.Team.GymQueue [i].BuildIndex).ToUniversalTime ());
						gymObj [GameData.Team.GymQueue [i].BuildIndex].TimeLabel.text = TextConst.SecondString ((int)(new System.TimeSpan (GameData.Team.GymBuild [GameData.Team.GymQueue [i].BuildIndex].Time.ToUniversalTime ().Ticks - DateTime.UtcNow.Ticks).TotalSeconds));
					} else {
						UIMainLobby.Get.IsCheckUpdateOnLoad = false;
						UIMainLobby.Get.CheckUpdate ();
					}
				} 
			}
		}
	}

	//目前要展示的功能
	private bool isCanShow (int index) {
		if(GameData.IsOpenUIVisible((EOpenID)(index + 50)))
			return true;

		return false;
	}

	private bool isCanUse (int index) {
		if(GameData.IsOpenUIEnable((EOpenID)(index + 50)))
			return true;

		return false;
	}

	public bool CenterVisible {
		get {return gymCenter.activeSelf;}
		set {gymCenter.SetActive(value);}
	}
}
