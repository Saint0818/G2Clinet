using GameEnum;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 關卡主程式.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示關卡. </item>
/// <item> Call Hide() 關閉關卡. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIGameLobby : UIBase
{
    private static UIGameLobby instance;
    private const string UIName = "UIGameLobby";

    private UIGameLobbyMain mMain;

    public bool Visible
    {
        get { return instance.gameObject.activeSelf; }
    }

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIGameLobbyMain>();
        mMain.BackListener += goToMainLobby;
        mMain.MainStageListener += GoToMainStage;
        mMain.PVPListener += GoToPvp;
        mMain.InstanceListener += GoToInstance;
        mMain.PvpUnlockButton.Event = new EventDelegate(GoToPvp);
        mMain.InstanceUnlockButton.Event = new EventDelegate(GoToInstance);
    }

    public void Show()
    {
        Show(true);
        mMain.ReddotEnable = GameFunction.CanGetPVPReward(ref GameData.Team);

        Statistic.Ins.LogScreen(3);
    }

    public void Hide()
    {
        RemoveUI(instance.gameObject);
    }

    private void goToMainLobby()
    {
        UIMainLobby.Get.Show();
        Hide();
    }

    public void GoToMainStage()
    {
        Statistic.Ins.LogEvent(51);

        UIMainStageTools.ClearSelectChapter();
        UIMainStage.Get.Show();
        Hide();
    }

    public void GoToPvp()
    {
        UIPVP.UIShow(true);

        UIMainLobby.Get.Hide(false);
        UIResource.Get.Show(UIResource.EMode.PVP);

        Hide();
    }

    public void GoToInstance()
    {
        if(!UIInstanceHelper.IsMainStagePass(1))
        {
            UIHint.Get.ShowHint(TextConst.S(100009), Color.black);
            return;
        }

        UIInstance.Get.Show();
        Hide();
    }

    public static UIGameLobby Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIGameLobby;
            }

            return instance;
        }
    }
}