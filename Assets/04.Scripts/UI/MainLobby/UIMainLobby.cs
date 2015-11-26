using System.Collections.Generic;
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

    [UsedImplicitly]
    private void Awake()
    {
        Main = GetComponent<UIMainLobbyMain>();
        Main.ChangePlayerNameListener += changePlayerName;
    }

    public void Show()
    {
        Show(true);
        UI3DMainLobby.Get.Show();
        UpdateUI();

        Main.Show();

        Main.EquipmentNotice = !GameData.Team.IsPlayerAllBestValueItem();

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