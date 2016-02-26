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

    public UIMainLobbyMain Main { get; private set; }

    // 目前發現回到大廳的 Loading 頁面實在是太久了, 所以把這個時間拉長.
    private const float AnimDelay = 3f;

    private readonly CountDownTimer mCountDownTimer = new CountDownTimer(1);

    [UsedImplicitly]
    private void Awake()
    {
        Main = GetComponent<UIMainLobbyMain>();

        var lobbyEvents = GetComponent<UIMainLobbyEvents>();
        Main.DailyLoginButton.onClick.Add(new EventDelegate(lobbyEvents.OnDailyLogin));
    }

    [UsedImplicitly]
    private void Start()
    {
        GameData.Team.OnMoneyChangeListener += onMoneyChange;
        GameData.Team.OnDiamondChangeListener += onDiamondChange;
        GameData.Team.OnPowerChangeListener += onPowerChange;

        mCountDownTimer.TimeUpListener += updateCountDownPower;
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
        GameData.Team.OnMoneyChangeListener -= onMoneyChange;
        GameData.Team.OnDiamondChangeListener -= onDiamondChange;
        GameData.Team.OnPowerChangeListener -= onPowerChange;

        mCountDownTimer.TimeUpListener -= updateCountDownPower;
    }

    public bool IsVisible
    {
        get { return Main.IsShow; }
    }

    public void Show()
    {
        Show(true);
        UI3DMainLobby.Get.Show();
        UpdateUI();

        Main.Show();

        playMoneyAnimation(AnimDelay);
        playPowerAnimation(AnimDelay);
        playDiamondAnimation(AnimDelay);

        UpdateButtonStatus();

        ResetCommands.Get.Run();

        if(GameData.Team.NeedForSyncRecord)
            SendHttp.Get.SyncDailyRecord();

        updateCountDownPower();
        mCountDownTimer.StartAgain(true);
    }

    private void updateCountDownPower()
    {
        DateTime utcFuture = GameData.Team.PowerCD.ToUniversalTime().AddSeconds(GameConst.AddPowerTimeInSeconds);
        TimeSpan timeInterval = utcFuture.Subtract(DateTime.UtcNow);

        Main.PowerCountDown = GameFunction.GetTimeString(timeInterval);
        Main.PowerCountDownVisible = GameData.Team.Power < GameConst.Max_Power;
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
        
        Main.PlayerNotice = GameData.PotentialNoticeEnable(ref GameData.Team);
        Main.LoginNotice = UIDailyLoginHelper.HasTodayDailyLoginReward() ||
                           UIDailyLoginHelper.HasLifetimeLoginReward();
    }

    private void updateMallButton()
    {
        Main.MallButton.CheckEnable();
    }

    private void updateMissionButton()
    {
        bool isEnable = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Mission);
        Main.MissionNotice = false;
        Main.MissionButton.CheckEnable();
        Main.MissionNotice = isEnable && hasMissionAward;
    }

    private void updateSkillButton()
    {
		Main.SkillNotice = (GameData.Team.IsSurplusCost || GameData.Team.IsAnyCardReinEvo || GameData.Team.IsExtraCard) && (LimitTable.Ins.HasByOpenID(EOpenID.Ability) && (GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Ability))) ;
        Main.SkillButton.CheckEnable();
    }

    private void updateSocialButton()
    {
        Main.SocialNotice = GameData.IsOpenUIEnable(EOpenID.Social) && (GameData.Setting.ShowEvent || GameData.Setting.ShowWatchFriend);
        Main.SocialButton.CheckEnable();
    }

    private void updateShopButton()
    {
        Main.ShopNotice = false;
        Main.ShopButton.CheckEnable();
    }

    private void updateAvatarButton()
    {
        bool isEnable = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Avatar);
        Main.AvatarNotice = false;
        Main.AvatarButton.CheckEnable();
        Main.AvatarNotice = isEnable && GameData.AvatarNoticeEnable();
    }

    private void updateEquipmentButton()
    {
        bool isEnable = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Equipment);
        Main.EquipmentNotice = false;
        Main.EquipButton.CheckEnable();
        Main.EquipmentNotice = isEnable && !GameData.Team.IsPlayerAllBestValueItem();
    }

    public void UpdateUI()
    {
        Main.Level = GameData.Team.Player.Lv;
        Main.Money = GameData.Team.Money;
        Main.Diamond = GameData.Team.Diamond;
        Main.Power = GameData.Team.Power;
        Main.PlayerName = GameData.Team.Player.Name;
        Main.PlayerIcon = GameData.Team.Player.FacePicture;
		Main.PlayerPosition = GameData.Team.Player.PositionPicture;
    }

    private void Update()
    {
        mCountDownTimer.Update(Time.deltaTime);
    }

    public void UpdateText()
	{
		initDefaultText(Main.gameObject);
	}

    public void Hide(int kind = 3, bool playAnimation = true)
    {
        UI3DMainLobby.Get.Hide();
        Main.Hide(kind, playAnimation);
        ResetCommands.Get.Stop();
//        RemoveUI(UIName);

        mCountDownTimer.Stop();
        Main.PowerCountDownVisible = false;
    }

    public void HideAll(bool playAnimation = true)
    {
        UI3DMainLobby.Get.Hide();
        Main.HideAll(playAnimation);
        ResetCommands.Get.Stop();

        mCountDownTimer.Stop();
        Main.PowerCountDownVisible = false;
    }

    private void onMoneyChange(int money)
    {
        Main.Money = money;
        playMoneyAnimation();
    }

    private void onDiamondChange(int diamond)
    {
        Main.Diamond = diamond;
        playDiamondAnimation();
    }

    private void onPowerChange(int power)
    {
        Main.Power = power;
        playPowerAnimation();
    }

    private void playMoneyAnimation(float delay = 0)
    {
        if(PlayerPrefs.HasKey(ESave.MoneyChange.ToString()))
        {
            Main.PlayMoneyAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.MoneyChange.ToString());
            PlayerPrefs.Save();
        }
    }

    private void playDiamondAnimation(float delay = 0)
    {
        if(PlayerPrefs.HasKey(ESave.DiamondChange.ToString()))
        {
            Main.PlayDiamondAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.DiamondChange.ToString());
            PlayerPrefs.Save();
        }
    }

    private void playPowerAnimation(float delay = 0)
    {
        if(PlayerPrefs.HasKey(ESave.PowerChange.ToString()))
        {
            Main.PlayPowerAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.PowerChange.ToString());
            PlayerPrefs.Save();
        }
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