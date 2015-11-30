using System.Collections.Generic;
using GameEnum;
using GameStruct;
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

        updateButtons();

        ResetCommands.Get.Run();
    }

    /// <summary>
    /// 更新大廳下面按鈕的狀態.
    /// </summary>
    private void updateButtons()
    {
        updateRedPoints();

        /*
        1.解鎖數值裝
        2.解鎖Avatar
        3.解鎖大廳左一按鈕(Shop)
        4.解鎖大廳左二按鈕(Social)
        5.解鎖技能介面
        */

//        PlayerPrefs.SetInt(ESave.LevelUpFlag.ToString(), 1);
        foreach(KeyValuePair<int, TExpData> pair in GameData.DExpData)
        {
            bool isEnable = GameData.Team.Player.Lv >= pair.Value.Lv;
            switch(pair.Value.OpenIndex)
            {
                case 1:
                    updateButton(Main.EquipButton, isEnable, GameData.Team.Player.Lv == pair.Value.Lv);
                    break;
                case 2:
                    updateButton(Main.AvatarButton, isEnable, GameData.Team.Player.Lv == pair.Value.Lv);
                    break;
                case 3:
                    updateButton(Main.ShopButton, isEnable, GameData.Team.Player.Lv == pair.Value.Lv);
                    break;
                case 4:
                    updateButton(Main.SocialButton, isEnable, GameData.Team.Player.Lv == pair.Value.Lv);
                    break;
                case 5:
                    updateButton(Main.SkillButton, isEnable, GameData.Team.Player.Lv == pair.Value.Lv);
                    break;
            }
        }

        if(PlayerPrefs.HasKey(ESave.LevelUpFlag.ToString()))
        {
            PlayerPrefs.DeleteKey(ESave.LevelUpFlag.ToString());
            PlayerPrefs.Save();
        }
    }

    private void updateRedPoints()
    {
        Main.EquipmentNotice = !GameData.Team.IsPlayerAllBestValueItem();
        Main.AvatarNotice = GameData.AvatarNoticeEnable();

//        PlayerPrefs.SetInt(ESave.NewCardFlag.ToString(), 1);
//        PlayerPrefs.Save();
        Main.SkillNotice = PlayerPrefs.HasKey(ESave.NewCardFlag.ToString());
    }

    private void updateButton(UIMainLobbyButton button, bool isEnable, bool playSFX)
    {
        button.IsEnable = isEnable;
        if(playSFX && PlayerPrefs.HasKey(ESave.LevelUpFlag.ToString()))
            button.PlaySFX();
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
}