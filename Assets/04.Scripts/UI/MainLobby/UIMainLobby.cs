using System.Collections.Generic;
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

    private readonly Dictionary<int, string> mPlayerIcons = new Dictionary<int, string>
    {
        {0, "Hank"}, // 中鋒.
        {1, "Nick"}, // 前鋒.
        {2, "Emma"}, // 後衛.
    };

    public UIMainLobbyMain Main { get; private set; }

    // 目前發現回到大廳的 Loading 頁面實在是太久了, 所以把這個時間拉長.
    private const float AnimDelay = 3f;

    [UsedImplicitly]
    private void Awake()
    {
        Main = GetComponent<UIMainLobbyMain>();
        Main.ChangePlayerNameListener += changePlayerName;
    }

    [UsedImplicitly]
    private void Start()
    {
        GameData.Team.OnMoneyChangeListener += onMoneyChange;
        GameData.Team.OnDiamondChangeListener += onDiamondChange;
        GameData.Team.OnPowerChangeListener += onPowerChange;
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
        GameData.Team.OnMoneyChangeListener -= onMoneyChange;
        GameData.Team.OnDiamondChangeListener -= onDiamondChange;
        GameData.Team.OnPowerChangeListener -= onPowerChange;
    }

    public void Show()
    {
        Show(true);
        UI3DMainLobby.Get.Show();
        UpdateUI();

        Main.Show();

        Main.EquipmentNotice = !GameData.Team.IsPlayerAllBestValueItem();
		Main.AvatarNotice = GameData.AvatarNoticeEnable ();

        playMoneyAnimation(AnimDelay);
        playPowerAnimation(AnimDelay);
        playDiamondAnimation(AnimDelay);

        ResetCommands.Get.Run();
    }

    public void UpdateUI()
    {
        Main.Money = GameData.Team.Money;
        Main.Diamond = GameData.Team.Diamond;
        Main.Power = GameData.Team.Power;
        Main.PlayerName = GameData.Team.Player.Name;
        Main.PlayerIcon = mPlayerIcons[GameData.Team.Player.BodyType];
    }

    public void Hide()
    {
        UI3DMainLobby.Get.Hide();
        Main.Hide();
        ResetCommands.Get.Stop();
//        RemoveUI(UIName);
    }

    public void HideAll()
    {
        UI3DMainLobby.Get.Hide();
        Main.HideAll();
        ResetCommands.Get.Stop();
    }

    private void changePlayerName()
    {
        WWWForm form = new WWWForm();
        form.AddField("NewPlayerName", UIInput.current.value);
        SendHttp.Get.Command(URLConst.ChangePlayerName, waitChangePlayerName, form);
    }

    private void waitChangePlayerName(bool ok, WWW www)
    {
        if (ok)
        {
            GameData.Team.Player.Name = www.text;
            UIHint.Get.ShowHint("Change Name Success!", Color.black);
        }
        else
            UIHint.Get.ShowHint("Change Player Name fail!", Color.red);

        UpdateUI();
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
        if (PlayerPrefs.HasKey(ESave.DiamondChange.ToString()))
        {
            Main.PlayDiamondAnimation(delay);
            PlayerPrefs.DeleteKey(ESave.DiamondChange.ToString());
            PlayerPrefs.Save();
        }
    }

    private void playPowerAnimation(float delay = 0)
    {
        if (PlayerPrefs.HasKey(ESave.PowerChange.ToString()))
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
}