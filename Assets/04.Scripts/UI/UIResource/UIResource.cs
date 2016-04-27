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
public class UIResource : UIBase
{
    private static UIResource instance;
    private const string UIName = "UIResource";

    // 目前發現回到大廳的 Loading 頁面實在是太久了, 所以把這個時間拉長.
    private const float AnimDelay = 3f;

    public UIResourceView View { get; private set; }

    private readonly CountDownTimer mCountDownTimer = new CountDownTimer(1);

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

        mCountDownTimer.TimeUpListener += updateCountDownPower;

        View.MoneyObj.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
        {
            if(!UIRecharge.Visible)
                UIRecharge.Get.ShowView(ERechargeType.Coin.GetHashCode(), null, false);
        }));
        View.DiamondObj.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
        {
            if (!UIRecharge.Visible)
                UIRecharge.Get.ShowView(ERechargeType.Diamond.GetHashCode(), null, false);
        }));
        View.PowerObj.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
        {
            if (!UIRecharge.Visible)
                UIRecharge.Get.ShowView(ERechargeType.Power.GetHashCode(), null, false);
        }));
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
        GameData.Team.OnMoneyChangeListener -= onMoneyChange;
        GameData.Team.OnDiamondChangeListener -= onDiamondChange;
        GameData.Team.OnPowerChangeListener -= onPowerChange;

        mCountDownTimer.TimeUpListener -= updateCountDownPower;
    }

    public void Show(int kind = 3)
    {
        Show(true);
        
        UpdateUI();

        View.Show(kind);

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

    public void UpdateUI()
    {
        View.Money = GameData.Team.Money;
        View.Diamond = GameData.Team.Diamond;
        View.Power = GameData.Team.Power;
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
}