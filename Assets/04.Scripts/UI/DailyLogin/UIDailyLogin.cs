﻿using System;
using UnityEngine;

/// <summary>
/// 每日登入獎勵介面.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示介面. </item>
/// <item> Call Hide() 關閉介面. </item>
/// <item> Year, Month 是目前介面是顯示哪一個月份的資料. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIDailyLogin : UIBase
{
    private static UIDailyLogin instance;
    private const string UIName = "UIDailyLogin";

    private UIDailyLoginMain mMain;
    private void Awake()
    {
        mMain = GetComponent<UIDailyLoginMain>();
        mMain.OnCloseClickListener += () => Hide();

        mMain.OnReceiveListener += onReceive;
    }

    private void onReceive(int year, int month)
    {
        var currentLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        var receviedLoginNum = UIDailyLoginHelper.GetDailyReceiveLoginNum(year, month);
//        Debug.LogFormat("OnReceiveClick:{0}-{1}, CurLoginNum:{2}, ReceivedLoginNum:{3}", year, month, currentLoginNum, receviedLoginNum);

        TDailyData dailyData = DailyTable.Ins.GetByDate(year, month);
        for(var day = receviedLoginNum + 1; day <= currentLoginNum; day++)
        {
            if(dailyData.HasRewardByDay(day))
            {
                TDailyData.Reward reward = dailyData.GetRewardByDay(day);
                UIGetItem.Get.AddItem(reward.ItemID);
            }
        }

        UIDailyLoginHelper.SetDailyReceiveLoginNum(year, month, currentLoginNum);
        Hide();
    }

    /// <summary>
    /// 目前顯示的是哪一個月份的登入獎勵.
    /// </summary>
    public int Year { get { return mMain.Year; } }
    public int Month { get { return mMain.Month; } }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void Show()
    {
        Show(DateTime.Now.Year, DateTime.Now.Month);
    }

    public void Show(int year, int month)
    {
        Show(true);

        buildDailyLoginRewards(year, month);

        selectLastWeek(year, month);
    }

    private void buildDailyLoginRewards(int year, int month)
    {
        mMain.ClearDailyRewards();
        TDailyData dailyData = DailyTable.Ins.GetByDate(year, month);
        if(dailyData == null)
            return;

        int currentLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        int receiveLoginNum = UIDailyLoginHelper.GetDailyReceiveLoginNum(year, month);
        DailyLoginReward.Data[] rewards = new DailyLoginReward.Data[dailyData.Rewards.Length];
        for(var i = 0; i < dailyData.Rewards.Length; i++)
        {
            var itemData = GameData.DItemData[dailyData.Rewards[i].ItemID];
            int day = i + 1;
            DailyLoginReward.EStatus status;
            if(day <= receiveLoginNum)
                status = DailyLoginReward.EStatus.Received;
            else
                status = day <= currentLoginNum ? DailyLoginReward.EStatus.Receivable : DailyLoginReward.EStatus.NoReceive;
            rewards[i] = UIDailyLoginBuilder.BuildDailyReward(day, itemData, status);
        }

        mMain.SetDayReward(year, month, rewards);
    }

    private void selectLastWeek(int year, int month)
    {
        var curLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        if(0 <= curLoginNum && curLoginNum <= 7)
            mMain.ShowWeek(1);
        else if(8 <= curLoginNum && curLoginNum <= 14)
            mMain.ShowWeek(2);
        else if(15 <= curLoginNum && curLoginNum <= 21)
            mMain.ShowWeek(3);
        else if(22 <= curLoginNum && curLoginNum <= 28)
            mMain.ShowWeek(4);
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    public static UIDailyLogin Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIDailyLogin;
            }
			
            return instance;
        }
    }
}