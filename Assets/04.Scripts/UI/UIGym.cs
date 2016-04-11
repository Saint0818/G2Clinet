using UnityEngine;
using System;
using System.Collections;
using GameStruct;
using GameEnum;
using Newtonsoft.Json;

public struct TGymResult {
	public int Money;
	public int Diamond;
	public TGymBuild[] GymBuild;
	public TGymQueue[] GymQueue;
}

public struct TGymObj {
	public GameObject Obj;
	public UILabel NameLabel;
	public UILabel LevelLabel;
	public UISlider CDBar;
	public UILabel TimeLabel;
}

public struct TGymQueueObj {
	public UILabel NameLabel;
	public UISlider CDBar;
	public UILabel TimeLabel;
}

public class UIGym : UIBase {
	private static UIGym instance = null;
	private const string UIName = "UIGym";

	private const int ThirdQueueDiamonds = 1000;

	private TGymQueue[] tempGymQueue = new TGymQueue[3];

	private GameObject gymCenter;

	private TGymObj[] gymObj = new TGymObj[9];
	private TGymQueueObj[] gymQueueObj = new TGymQueueObj[3];

	private GameObject goRedPoint;
	private GameObject goQueueGroup;

	private GameObject goLock;
	private UILabel labelPrice;

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
		updateQueue();
	}

	protected override void InitCom() {
		gymCenter = GameObject.Find(UIName + "/Window/Center");
		for (int i=0; i<gymObj.Length; i++) {
			gymObj[i].Obj = GameObject.Find(UIName + "/Window/Center/" + i.ToString());
			gymObj[i].NameLabel = GameObject.Find(UIName + "/Window/Center/" + i.ToString() + "/NameLabel").GetComponent<UILabel>();
			gymObj[i].LevelLabel = GameObject.Find(UIName + "/Window/Center/" + i.ToString() + "/LevelLabel").GetComponent<UILabel>();
			gymObj[i].CDBar = GameObject.Find(UIName + "/Window/Center/" + i.ToString() + "/CDBar").GetComponent<UISlider>();
			gymObj[i].TimeLabel  = GameObject.Find(UIName + "/Window/Center/" + i.ToString() + "/CDBar/TimeLabel").GetComponent<UILabel>();
			SetBtnFun (UIName + "/Window/Center/" + i.ToString(), OnClickBuild);
		}

		for (int i=0; i<gymQueueObj.Length; i++) {
			gymQueueObj[i].NameLabel = GameObject.Find(UIName + "/Window/Left/QueueGroup/" + i.ToString() + "/NameLabel").GetComponent<UILabel>();
			gymQueueObj [i].CDBar = GameObject.Find (UIName + "/Window/Left/QueueGroup/" + i.ToString () + "/CDBar").GetComponent<UISlider> ();
			gymQueueObj[i].TimeLabel = GameObject.Find (UIName + "/Window/Left/QueueGroup/" + i.ToString () + "/CDBar/TimeLabel").GetComponent<UILabel> ();
		}
		SetBtnFun (UIName + "/Window/Left/GymQueue", OnClickQueue);

		goRedPoint = GameObject.Find (UIName + "/Window/Left/GymQueue/AvailableIcon");
		goQueueGroup = GameObject.Find (UIName + "/Window/Left/QueueGroup");

		goLock = GameObject.Find (UIName + "/Window/Left/QueueGroup/Lock");
		UIEventListener.Get(goLock).onClick = OnClickLock;
		labelPrice = GameObject.Find (UIName + "/Window/Left/QueueGroup/Lock/PriceLabel").GetComponent<UILabel>();
		labelPrice.text = ThirdQueueDiamonds.ToString();
		RefreshDiamondColor ();
	}

	public void OnClickBuild () {
		int result = 0;
		if (int.TryParse (UIButton.current.name, out result)) {
			if(UI3DMainLobby.Visible)
				UI3DMainLobby.Get.mImpl.OnSelect(result);
			
			if(goQueueGroup.activeSelf)
				goQueueGroup.SetActive(false);
			
			CenterVisible = false;
			StartCoroutine(ShowEngage(result));
		}
	}

	private IEnumerator ShowEngage (int index) {
		yield return new WaitForSeconds(1);
		UIGymEngage.Get.ShowView(index);
	}

	public void OnClickQueue () {
		goQueueGroup.SetActive(!goQueueGroup.activeSelf);
	}
		
	public void OnClickLock (GameObject go) {
		CheckDiamond(ThirdQueueDiamonds, true, string.Format(TextConst.S (11008), ThirdQueueDiamonds), SendBuyQueue, RefreshDiamondColor);
	}

	public void RefreshDiamondColor () {
		labelPrice.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(0, ThirdQueueDiamonds), 0);
	}

	private void refreshRedPoint () {
		goRedPoint.SetActive(GameFunction.GetIdleQueue != 0);
	}

	public void ShowView() {
		Visible = true;
		goQueueGroup.SetActive(false);
		refreshBuild ();
		for (int i=0; i<gymObj.Length; i++) {
			gymObj[i].Obj.SetActive(isCanShow(i));
		}
		initQueue ();
	}

	private void refreshBuild () {
		for (int i=0; i<gymObj.Length; i++) {
			gymObj[i].NameLabel.text = GameFunction.GetBuildName(i);
			gymObj[i].LevelLabel.text = string.Format(TextConst.S(11021), GameFunction.GetBuildLv(i));
			gymObj[i].CDBar.gameObject.SetActive(false);
		}
	}

	private void updateBuildCD () {
		if(tempGymQueue != null ) {
			for(int i=0; i<tempGymQueue.Length; i++) {
				if(tempGymQueue[i].IsOpen && tempGymQueue[i].BuildIndex != -1){
					gymObj[i].CDBar.value = TextConst.DeadlineStringPercent(GameFunction.GetOriTime(tempGymQueue[i].BuildIndex, GameFunction.GetBuildLv(tempGymQueue[i].BuildIndex) - 1, GameFunction.GetBuildTime(tempGymQueue[i].BuildIndex).ToUniversalTime()) ,GameFunction.GetBuildTime(tempGymQueue[i].BuildIndex).ToUniversalTime());
					gymObj[i].TimeLabel.text = TextConst.SecondString((int)(new System.TimeSpan(GameData.Team.GymBuild[tempGymQueue[i].BuildIndex].Time.ToUniversalTime().Ticks - DateTime.UtcNow.Ticks).TotalSeconds));
				}
			}
		}
	}

	private void initQueue () {
		if(GameData.Team.GymQueue != null && GameData.Team.GymQueue.Length > 0 && GameData.Team.GymQueue.Length == 3 && tempGymQueue.Length == GameData.Team.GymQueue.Length) {
			if(LimitTable.Ins.HasByOpenID(EOpenID.OperateQueue)) {
				if(GameData.Team.GymQueue[1].IsOpen != (GameData.Team.HighestLv >= LimitTable.Ins.GetLv(EOpenID.OperateQueue)))  //假如Client跟Server不同步，會先檢查Server再回傳
					SendCheckQueueLv();
				else 
					RefreshQueue();
			}
		}
	}

	public void RefreshQueue () {
		refreshBuild ();
		tempGymQueue[0] = GameData.Team.GymQueue[0];
		setQueueBreak(0);
		goLock.SetActive(!GameData.Team.GymQueue[2].IsOpen);
		if(!GameData.Team.GymQueue[1].IsOpen && GameData.Team.GymQueue[2].IsOpen) { //特殊狀況：等級未到，但有購買
			tempGymQueue[1] = GameData.Team.GymQueue[2];
			tempGymQueue[2] = GameData.Team.GymQueue[1];
			setQueueBreak(1);
			setQueueBreak(2);
			gymQueueObj[2].NameLabel.text = string.Format(TextConst.S(11003), LimitTable.Ins.GetLv(EOpenID.OperateQueue));

		} else {
			tempGymQueue[1] = GameData.Team.GymQueue[1];
			tempGymQueue[2] = GameData.Team.GymQueue[2];
			setQueueBreak(1);
			setQueueBreak(2);
			if(!tempGymQueue[1].IsOpen)
				gymQueueObj[1].NameLabel.text = string.Format(TextConst.S(11003), LimitTable.Ins.GetLv(EOpenID.OperateQueue));

			if(!tempGymQueue[2].IsOpen) 
				gymQueueObj[2].NameLabel.text = TextConst.S(11004);
		}
		refreshRedPoint ();
		updateQueue ();
	}

	private void setQueueBreak (int index) {
		if(index >= 0 && index < gymQueueObj.Length) {
			gymQueueObj[index].NameLabel.text = TextConst.S(11002);
			gymQueueObj[index].CDBar.value = 0;
			gymQueueObj[index].TimeLabel.text = "";
		}
	}

	private void updateQueue () {
		if(isQueueOpen) {
			if(tempGymQueue != null ) {
				for(int i=0; i<tempGymQueue.Length; i++) {
					if(tempGymQueue[i].IsOpen && tempGymQueue[i].BuildIndex != -1){
						gymQueueObj[i].NameLabel.text = GameFunction.GetBuildName(tempGymQueue[i].BuildIndex);
						gymQueueObj[i].CDBar.value = TextConst.DeadlineStringPercent(GameFunction.GetOriTime(tempGymQueue[i].BuildIndex, GameFunction.GetBuildLv(tempGymQueue[i].BuildIndex) - 1, GameFunction.GetBuildTime(tempGymQueue[i].BuildIndex).ToUniversalTime()) ,GameFunction.GetBuildTime(tempGymQueue[i].BuildIndex).ToUniversalTime());
						gymQueueObj[i].TimeLabel.text = TextConst.SecondString((int)(new System.TimeSpan(GameData.Team.GymBuild[tempGymQueue[i].BuildIndex].Time.ToUniversalTime().Ticks - DateTime.UtcNow.Ticks).TotalSeconds));
					}
				}
			}
		}
	}

	//目前要展示的功能
	private bool isCanShow (int index) {
		if(LimitTable.Ins.HasByOpenID((EOpenID)(index + 50)) && GameData.Team.HighestLv >= LimitTable.Ins.GetLv((EOpenID)(index + 50)))
			return true;
		
		return false;
	}

	private bool isQueueOpen {
		get {return goQueueGroup.activeSelf;}
	}

	private int getQueueOpen {
		get {
			int count = 0;
			for(int i=0; i<tempGymQueue.Length; i++) 
				if(tempGymQueue[i].IsOpen)
					count ++;
			return count;
		}
	}
	
	public bool CenterVisible {
		get {return gymCenter.activeSelf;}
		set {gymCenter.SetActive(value);}
	}

	private void SendCheckQueueLv () {
		WWWForm form = new WWWForm();
		SendHttp.Get.Command(URLConst.GymCheckQueueLv, waitCheckQueueLv, form);
	}

	private void waitCheckQueueLv(bool ok, WWW www) {
		if (ok) {
			TGymResult result = JsonConvert.DeserializeObject <TGymResult>(www.text, SendHttp.Get.JsonSetting); 
			GameData.Team.GymQueue = result.GymQueue;
			RefreshQueue();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

	private void SendBuyQueue () {
		WWWForm form = new WWWForm();
		SendHttp.Get.Command(URLConst.GymBuyQueue, waitBuyQueue, form);
	}

	private void waitBuyQueue(bool ok, WWW www) {
		if (ok) {
			TGymResult result = JsonConvert.DeserializeObject <TGymResult>(www.text, SendHttp.Get.JsonSetting); 
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.GymQueue = result.GymQueue;
			RefreshQueue();
			UIMainLobby.Get.UpdateUI();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

//	private void SendRefreshQueue () {
//		WWWForm form = new WWWForm();
//		SendHttp.Get.Command(URLConst.GymBuyQueue, waitBuyQueue, form);
//	}
//
//	private void waitBuyQueue(bool ok, WWW www) {
//		if (ok) {
//			TGymResult result = JsonConvert.DeserializeObject <TGymResult>(www.text, SendHttp.Get.JsonSetting); 
//			GameData.Team.Diamond = result.Diamond;
//			GameData.Team.GymQueue = result.GymQueue;
//			RefreshQueue();
//			UIMainLobby.Get.UpdateUI();
//		} else {
//			Debug.LogError("text:"+www.text);
//		} 
//	}
}
