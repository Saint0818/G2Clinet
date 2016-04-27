using System;
using AI;
using GameEnum;
using JetBrains.Annotations;
using UnityEngine;
using GameStruct;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// 大廳主程式.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示某個頁面. </item>
/// <item> Call Hide() 將大廳關閉. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIMainLobby : UIBase
{
    private static UIMainLobby instance;
    private const string UIName = "UIMainLobby";

	public UIMainLobbyView View { get; private set; }

	private const int ThirdQueueDiamonds = 1000;

	private int[] sendIndexs = new int[0];
	private int[] sendBuildIndexs = new int[0];

	private List<int> tempSendIndex = new List<int> ();
	private List<int> tempSendBuild = new List<int> ();

	private TGymQueue[] tempGymQueue = new TGymQueue[3];

	private bool isCheckUpdateOnLoad = false;
	public bool IsCheckUpdateOnLoad {
		get {return isCheckUpdateOnLoad;}
		set {isCheckUpdateOnLoad = value;}
	}

    [UsedImplicitly]
    private void Awake()
    {
		View = GetComponent<UIMainLobbyView>();

		initQueue();

        View.Settings.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => UISetting.UIShow(true)));

        var events = GetComponent<UIMainLobbyEvents>();
        View.RaceButton.onClick.Add(new EventDelegate(events.ShowGameLobby));
        View.SkillButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(events.ShowSkillFormation));
        View.AvatarButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(events.ShowAvatarFitted));
        View.EquipButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(events.ShowEquipment));
        View.SocialButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(events.OnSocial));
        View.ShopButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(events.OnShop));
        View.MallButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(events.OnMall));
    }

    public bool IsVisible
    {
        get { return gameObject.activeSelf; }
    }

    public void Show()
    {
        Show(true);

		UI3DMainLobby.Get.Show();
		if(GameData.Team.GymBuild != null && GameData.Team.GymQueue != null) //因為後台還沒建立所以要判斷
			UIGym.Get.ShowView();
        UpdateUI();

		View.Show();

        UpdateButtonStatus();

        ResetCommands.Get.Run();

        UIResource.Get.Show();

        if(GameData.Team.NeedForSyncRecord)
            SendHttp.Get.SyncDailyRecord();

        Statistic.Ins.LogScreen(2);
    }

    /// <summary>
    /// 更新大廳的全部按鈕狀態.
    /// </summary>
    public void UpdateButtonStatus()
    {
        updateEquipmentButton();
        updateAvatarButton();
        updateShopButton();
        updateSocialButton();
        updateSkillButton();
        updateMissionButton();
        updateMallButton();
		updateAlbumButton ();
        
        View.PlayerNotice = GameData.PotentialNoticeEnable(ref GameData.Team);
//        View.LoginNotice = UIDailyLoginHelper.HasTodayDailyLoginReward() ||
//                           UIDailyLoginHelper.HasLifetimeLoginReward();
    }

	private void updateAlbumButton () {
		View.AlbumNotice = (GameData.IsOpenUIEnableByPlayer(EOpenID.SuitCard) && GameData.Team.SuitCardRedPoint);
	}

    private void updateMallButton()
	{
		View.MallNotice = GameData.Team.IsMallFree && (LimitTable.Ins.HasByOpenID(EOpenID.Mall) && (GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Mall)));
        View.MallButton.CheckEnable();
    }

    private void updateMissionButton()
    {
        bool isEnable = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Mission);
        View.MissionNotice = false;
        View.MissionButton.CheckEnable();
        View.MissionNotice = isEnable && hasMissionAward;
    }

    private void updateSkillButton()
    {
		//合成不須判斷紅點(20160324 GameData.Team.IsExtraCard)
		View.SkillNotice = (GameData.Team.IsSurplusCost || GameData.Team.IsAnyCardReinEvo);
        View.SkillButton.CheckEnable();
    }

    private void updateSocialButton()
    {
        View.SocialNotice = GameData.IsOpenUIEnable(EOpenID.Social) && (GameData.Setting.ShowEvent || GameData.Setting.ShowWatchFriend);
        View.SocialButton.CheckEnable();
    }

    private void updateShopButton()
    {
        View.ShopNotice = false;
        View.ShopButton.CheckEnable();
    }

    private void updateAvatarButton()
    {
//        bool isEnable = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Avatar);
        View.AvatarButton.CheckEnable();
        View.AvatarNotice = View.AvatarButton.IsEnable && GameData.AvatarNoticeEnable();
    }

    private void updateEquipmentButton()
    {
//        bool isEnable = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Equipment);
        View.EquipButton.CheckEnable();
        View.EquipmentNotice = View.EquipButton.IsEnable && 
              (!GameData.Team.IsPlayerAllBestValueItem() || GameData.Team.HasInlayableValueItem() ||
               GameData.Team.HasUpgrableValueItem());
    }

    public void UpdateUI()
    {
        View.Level = GameData.Team.Player.Lv;
        View.PlayerName = GameData.Team.Player.Name;
        View.PlayerIcon = GameData.Team.Player.FacePicture;
		View.PlayerPosition = GameData.Team.Player.PositionPicture;
    }

    public void UpdateText()
	{
		initDefaultText(View.TopLeftGroup);
		initDefaultText(View.BottomGroup);
		RefreshQueue();
	}

    public void Hide(bool playAnimation = true)
	{
		if(GameData.Team.GymBuild != null && GameData.Team.GymQueue != null)
			UIGym.Visible = false;
        UI3DMainLobby.Get.Hide();
        View.Hide(playAnimation);
        ResetCommands.Get.Stop();
		View.QueueGroup.SetActive(false);
    }

    public static UIMainLobby Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIMainLobby;
            }
			
            return instance;
        }
    }

    private bool hasMissionAward
    {
        get
        {
            for(int j = 0; j < GameData.MissionData.Length; j++)
            {
                if(GameData.Team.HaveMissionAward(ref GameData.MissionData[j])) 
                    return true;
            }
            return false;
        }
    }

	void FixedUpdate () {
		if (isCheckUpdateOnLoad) 
			updateQueue();
	}

	//GymQueue

	public void CheckUpdate () {
		tempSendIndex = new List<int> ();
		tempSendBuild = new List<int> ();
		for (int i = 0; i < GameData.Team.GymQueue.Length; i++) {
			if (GameData.Team.GymQueue [i].BuildIndex >= 0 && GameData.Team.GymQueue [i].BuildIndex < GameData.Team.GymBuild.Length) {
				if (GameData.Team.GymBuild [GameData.Team.GymQueue [i].BuildIndex].Time.ToUniversalTime () < DateTime.UtcNow) {
					tempSendIndex.Add (i);
					tempSendBuild.Add (GameData.Team.GymQueue [i].BuildIndex);
				}
			}
		}

		if (tempSendBuild.Count > 0 && tempSendIndex.Count > 0) {
			sendBuildIndexs = new int[tempSendBuild.Count];
			sendIndexs = new int[tempSendIndex.Count];
			for (int i = 0; i < sendBuildIndexs.Length; i++) {
				sendBuildIndexs [i] = tempSendBuild [i];
			}
			for (int i = 0; i < sendIndexs.Length; i++) {
				sendIndexs [i] = tempSendIndex [i];
			}
			SendRefreshQueue ();
		} else {
			isCheckUpdateOnLoad = true;
			tempSendBuild.Clear ();
			tempSendIndex.Clear ();
		}
	}

	private void initQueue () {
		if(GameData.Team.GymQueue != null && GameData.Team.GymQueue.Length > 0 && GameData.Team.GymQueue.Length == 3 && tempGymQueue.Length == GameData.Team.GymQueue.Length) 
			if(LimitTable.Ins.HasByOpenID(EOpenID.OperateQueue)) 
				RefreshQueue();

		UIEventListener.Get(View.QueueButton).onClick = OnClickQueue;
		UIEventListener.Get(View.QueueLockObj).onClick = OnClickLock;
		CheckUpdate ();
		View.QueueButton.SetActive(GameData.IsOpenUIEnable(EOpenID.OperateGym));
		View.QueueGroup.SetActive(false);
		View.QueueLockPrice.text = ThirdQueueDiamonds.ToString();
		RefreshDiamondColor ();
	}

	public void RefreshQueue () {
		if(UIGym.Visible)
			UIGym.Get.RefreshBuild();
		tempGymQueue[0] = GameData.Team.GymQueue[0];
		View.SetQueueBreak(0);
		View.QueueLockObj.SetActive(!GameData.Team.GymQueue[2].IsOpen);
		if(!GameData.Team.GymQueue[1].IsOpen && GameData.Team.GymQueue[2].IsOpen) { //特殊狀況：等級未到，但有購買
			tempGymQueue[1] = GameData.Team.GymQueue[2];
			tempGymQueue[2] = GameData.Team.GymQueue[1];
			View.SetQueueBreak(1);
			View.SetQueueBreak(2);
			View.SetQueueState(2, string.Format(TextConst.S(11018), 3), 0, "", true, string.Format(TextConst.S(11003), LimitTable.Ins.GetLv(EOpenID.OperateQueue)), -2);
		} else {
			tempGymQueue[1] = GameData.Team.GymQueue[1];
			tempGymQueue[2] = GameData.Team.GymQueue[2];
			View.SetQueueBreak(1);
			View.SetQueueBreak(2);
			if(!tempGymQueue[1].IsOpen)
				View.SetQueueState(1, string.Format(TextConst.S(11018), 2), 0, "", true, string.Format(TextConst.S(11003), LimitTable.Ins.GetLv(EOpenID.OperateQueue)), -2);
			
			if(!tempGymQueue[2].IsOpen) 
				View.SetQueueState(2, TextConst.S(11004), 0, "", true, "", -2);
		}

		bubbleList ();
		View.QueueNotice = (GameFunction.GetIdleQueue != 0);
		updateQueue ();
	}

	private void bubbleList () {
		for (int i = 0; i < tempGymQueue.Length; i++) {
			for (int j = 1; j < tempGymQueue.Length; j++) {
				if (tempGymQueue [i].IsOpen && tempGymQueue [i].BuildIndex == -1 &&
					tempGymQueue [j].IsOpen && tempGymQueue [j].BuildIndex != -1) {
					int tempBuildIndex = tempGymQueue [i].BuildIndex;
					tempGymQueue [i].ChangePos (tempGymQueue[j].BuildIndex);
					tempGymQueue [j].ChangePos (tempBuildIndex);
				}
			}
		}
	}
	public void OnClickQueue (GameObject go) {
		View.QueueGroup.SetActive(!View.QueueGroup.activeSelf);
		if(View.QueueGroup.activeSelf)
			Statistic.Ins.LogScreen(18);
	}

	public void OnClickLock (GameObject go) {
		CheckDiamond(ThirdQueueDiamonds, true, string.Format(TextConst.S (11008), ThirdQueueDiamonds), SendBuyQueue, RefreshDiamondColor);
	}

	public void RefreshDiamondColor () {
		View.QueuePriceColor = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(0, ThirdQueueDiamonds), 0);
	}

	private void updateQueue () {
		if(View.IsQueueOpen) 
			if(tempGymQueue != null ) 
				for(int i=0; i<tempGymQueue.Length; i++) 
					if(tempGymQueue[i].IsOpen && tempGymQueue[i].BuildIndex != -1)
						View.SetQueueBuildState(i, tempGymQueue[i].BuildIndex);
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

	private void SendRefreshQueue () {
		WWWForm form = new WWWForm();
		form.AddField("Index", JsonConvert.SerializeObject(sendIndexs));
		form.AddField("BuildIndex", JsonConvert.SerializeObject(sendBuildIndexs));
		SendHttp.Get.Command(URLConst.GymRefreshQueue, waitRefreshQueue, form);
	}

	private void waitRefreshQueue(bool ok, WWW www) {
		if (ok) {
			TGymResult result = JsonConvert.DeserializeObject <TGymResult>(www.text, SendHttp.Get.JsonSetting); 
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.GymBuild = result.GymBuild;
			GameData.Team.GymQueue = result.GymQueue;
			RefreshQueue();
			if(UIGym.Visible)
				UIGym.Get.RefreshBuild();
			UIMainLobby.Get.UpdateUI();
			if (UIGymEngage.Visible)
				UIGymEngage.Get.RefreshUI ();

			isCheckUpdateOnLoad = true;
			tempSendBuild.Clear ();
			tempSendIndex.Clear ();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}
}