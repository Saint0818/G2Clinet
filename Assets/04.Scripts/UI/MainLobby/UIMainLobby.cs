using System;
using AI;
using GameEnum;
using JetBrains.Annotations;
using UnityEngine;

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

    [UsedImplicitly]
    private void Awake()
    {
        View = GetComponent<UIMainLobbyView>();

//        var lobbyEvents = GetComponent<UIMainLobbyEvents>();
//        View.DailyLoginButton.onClick.Add(new EventDelegate(lobbyEvents.OnDailyLogin));
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
        
        View.PlayerNotice = GameData.PotentialNoticeEnable(ref GameData.Team);
//        View.LoginNotice = UIDailyLoginHelper.HasTodayDailyLoginReward() ||
//                           UIDailyLoginHelper.HasLifetimeLoginReward();
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
	}

    public void Hide(bool playAnimation = true)
	{
		if(GameData.Team.GymBuild != null && GameData.Team.GymQueue != null)
			UIGym.Visible = false;
        UI3DMainLobby.Get.Hide();
        View.Hide(playAnimation);
        ResetCommands.Get.Stop();
//        RemoveUI(UIName);
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
}