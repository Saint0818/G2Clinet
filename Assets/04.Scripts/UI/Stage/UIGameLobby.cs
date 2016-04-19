﻿using GameEnum;
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
        mMain.MainStageListener += GoToMainStage;
        mMain.PVPListener += GoToPvp;
        mMain.InstanceListener += GoToInstance;
    }

    public void Show()
    {
        Show(true);
        mMain.ReddotEnable = GameFunction.CanGetPVPReward(ref GameData.Team);
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
        UIMainStageTools.ClearSelectChapter();
        UIMainStage.Get.Show();
        Hide();
    }

    public void GoToPvp()
    {
        if(GameData.IsOpenUIEnable(EOpenID.PVP))
        {
            UIPVP.UIShow(true);
            UIMainLobby.Get.Hide(2, false);
            Hide();
        }
        else
			UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.PVP)), LimitTable.Ins.GetLv(EOpenID.PVP)), Color.white);
    }

    public void GoToInstance()
    {
        if(!GameData.IsOpenUIEnable(EOpenID.Instance))
        {
			UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.Instance)), LimitTable.Ins.GetLv(EOpenID.Instance)), Color.white);
            return;
        }

        if(!UIInstanceHelper.IsMainStagePass(1))
        {
            UIHint.Get.ShowHint(TextConst.S(100009), Color.white);
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