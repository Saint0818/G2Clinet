using System;
using AI;
using GameEnum;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// <para> 玩家的資源(鑽石, 體力, 錢, 社群幣, PVP 幣) </para>
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示資源. </item>
/// <item> Call Hide() 隱藏資源. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIResource : UIBase
{
    private static UIResource instance;
    private const string UIName = "UIResource";

    // 目前發現回到大廳的 Loading 頁面實在是太久了, 所以把這個時間拉長.
    private const float AnimDelay = 3f;

    private UIResourceView View { get; set; }

    private readonly CountDownTimer mCountDownTimer = new CountDownTimer(1);

	//給recharge判斷
	private EMode currentmode;
	public EMode CurrentMode {
		get {return currentmode;}
	}

    [UsedImplicitly]
    private void Awake()
    {
        View = GetComponent<UIResourceView>();
    }

    [UsedImplicitly]
    private void Start()
    {
        GameData.Team.OnMoneyChangeListener += onMoneyChange;
        GameData.Team.OnDiamondChangeListener += onDiamondChange;
        GameData.Team.OnPowerChangeListener += onPowerChange;
        GameData.Team.OnPVPCoinChangeListener += onPVPChange;
        GameData.Team.OnSocialCoinChangeListener += onSocialChange;

        mCountDownTimer.TimeUpListener += updateCountDownPower;

        View.MoneyObj.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
        {
            if(!UIRecharge.Visible)
                UIRecharge.Get.ShowView(ERechargeType.Coin.GetHashCode(), false);
				
			UIMail.SetFocus (false);
        }));
        View.DiamondObj.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
        {
            if (!UIRecharge.Visible)
                UIRecharge.Get.ShowView(ERechargeType.Diamond.GetHashCode(), false);

			UIMail.SetFocus (false);
        }));
        View.PowerObj.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
        {
            if (!UIRecharge.Visible)
                UIRecharge.Get.ShowView(ERechargeType.Power.GetHashCode(), false);

			UIMail.SetFocus (false);
        }));
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
        GameData.Team.OnMoneyChangeListener -= onMoneyChange;
        GameData.Team.OnDiamondChangeListener -= onDiamondChange;
        GameData.Team.OnPowerChangeListener -= onPowerChange;
        GameData.Team.OnPVPCoinChangeListener -= onPVPChange;
        GameData.Team.OnSocialCoinChangeListener -= onSocialChange;

        mCountDownTimer.TimeUpListener -= updateCountDownPower;
    }

    public enum EMode
    {
        Basic, // 僅顯示鑽石和金錢.
        Power, PVP, PvpSocial
    }
	public void Show(EMode mode = EMode.Power , bool isRecord = true)
    {
        Show(true);
		if(isRecord)
			currentmode = mode;
        updateUI();

        View.Show(mode);

        playMoneyAnimation(AnimDelay);
        playPowerAnimation(AnimDelay);
        playDiamondAnimation(AnimDelay);

        View.PowerCountDownVisible = ResetCommands.Get.IsRunning;
        if(ResetCommands.Get.IsRunning)
        {
            updateCountDownPower();
            mCountDownTimer.StartAgain(true);
        }
        else
            mCountDownTimer.Stop();
    }

    private void updateCountDownPower()
    {
        DateTime utcFuture = GameData.Team.PowerCD.ToUniversalTime().AddSeconds(GameConst.AddPowerTimeInSeconds);
        TimeSpan timeInterval = utcFuture.Subtract(DateTime.UtcNow);

        View.PowerCountDown = GameFunction.GetTimeString(timeInterval);
        View.PowerCountDownVisible = GameData.Team.Power < GameConst.Max_Power;
    }

    private void updateUI()
    {
        View.Money = GameData.Team.Money;
        View.Diamond = GameData.Team.Diamond;
        View.Power = GameData.Team.Power;
        View.PVPCoin = GameData.Team.PVPCoin;
        View.SocialCoin = GameData.Team.SocialCoin;
    }

    private void Update()
    {
        mCountDownTimer.Update(Time.deltaTime);
    }

    public void Hide()
    {
        View.Hide();

        mCountDownTimer.Stop();
        View.PowerCountDownVisible = false;
    }

    private void onMoneyChange(int money)
    {
        View.Money = money;
        playMoneyAnimation();
    }

    private void onDiamondChange(int diamond)
    {
        View.Diamond = diamond;
        playDiamondAnimation();
    }

    private void onPowerChange(int power)
    {
        View.Power = power;
        playPowerAnimation();
    }

    private void onPVPChange(int pvpCoin)
    {
        View.PVPCoin = pvpCoin;
        playPVPAnimation();
    }

    private void onSocialChange(int socialCoin)
    {
        View.SocialCoin = socialCoin;
        playSocialAnimation();
    }

    private void playMoneyAnimation(float delay = 0)
    {
        if(PlayerPrefs.HasKey(ESave.MoneyChange.ToString()))
        {
            View.PlayMoneyAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.MoneyChange.ToString());
            PlayerPrefs.Save();
        }
    }

    private void playDiamondAnimation(float delay = 0)
    {
        if(PlayerPrefs.HasKey(ESave.DiamondChange.ToString()))
        {
            View.PlayDiamondAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.DiamondChange.ToString());
            PlayerPrefs.Save();
        }
    }

    private void playPVPAnimation(float delay = 0)
    {
        if (PlayerPrefs.HasKey(ESave.PVPCoinChange.ToString()))
        {
            View.PlayPVPAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.PVPCoinChange.ToString());
            PlayerPrefs.Save();
        }
    }

    private void playSocialAnimation(float delay = 0)
    {
        if (PlayerPrefs.HasKey(ESave.SocialCoinChange.ToString()))
        {
            View.PlaySocialAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.SocialCoinChange.ToString());
            PlayerPrefs.Save();
        }
    }

    private void playPowerAnimation(float delay = 0)
    {
        if(PlayerPrefs.HasKey(ESave.PowerChange.ToString()))
        {
            View.PlayPowerAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.PowerChange.ToString());
            PlayerPrefs.Save();
        }
    }

    public static UIResource Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIResource;
            }
			
            return instance;
        }
    }

    public static bool Visible
    {
        get { return instance && instance.gameObject.activeInHierarchy; }
    }
}