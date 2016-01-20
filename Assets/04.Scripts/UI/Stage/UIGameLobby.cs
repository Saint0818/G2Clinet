using System;
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

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIGameLobbyMain>();
        mMain.BackListener += goToMainLobby;
        mMain.MainStageListener += goToMainStage;
        mMain.PvpListener += goToPvp;
        mMain.InstanceListener += goToInstance;
    }

    public void Show()
    {
        Show(true);
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    private void goToMainLobby()
    {
        UIMainLobby.Get.Show();
        Hide();
    }

    private void goToMainStage()
    {
        UIMainStageTools.ClearSelectChapter();
        UIMainStage.Get.Show();
        Hide();
    }

    private void goToPvp()
    {
        UIPVP.UIShow(true);
        Hide();
    }

    private void goToInstance()
    {
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