using System;
using System.Collections.Generic;
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

    private UIDailyLoginMain mDailyMain;
    private UILifetimeLoginMain mLifetimeMain;
    private void Awake()
    {
        mDailyMain = GetComponent<UIDailyLoginMain>();
        mDailyMain.OnCloseClickListener += () => Hide();

        mDailyMain.OnReceiveClick += (year, month) =>
        {
            var protocol = new ReceiveDailyLoginRewardProtocol();
            protocol.Send(onDailyReceived);
        };

        mLifetimeMain = GetComponent<UILifetimeLoginMain>();
        mLifetimeMain.OnReceiveClick += loginNum =>
        {
            var protocol = new ReceiveLifetimeLoginRewardProtocol();
            protocol.Send(loginNum, onLifetimeReceive);
        };
    }

    private void onLifetimeReceive(bool ok, int loginNum)
    {
        UIGetItem.Get.SetTitle(TextConst.S(3812));
        TLifetimeData data = LifetimeTable.Ins.Get(loginNum);
        if(data == null)
            return;

        foreach (TLifetimeData.Reward reward in data.Rewards)
            UIGetItem.Get.AddItem(reward.ItemID);

        UIMainLobby.Get.UpdateButtonStatus(); // 這只是為了更新大廳登入按鈕的紅點狀態.
        Show(Year, Month); // 刷新介面.
    }

    private void onDailyReceived(bool ok, int year, int month)
    {
        if(ok)
        {
            var rewardDay = GameData.Team.GetReceivedDailyLoginNum(year, month);

            UIGetItem.Get.SetTitle(TextConst.S(3812));
            TDailyData dailyData = DailyTable.Ins.GetByDate(year, month);
            if(dailyData != null && dailyData.HasRewardByDay(rewardDay))
            {
                TDailyData.Reward reward = dailyData.GetRewardByDay(rewardDay);
                UIGetItem.Get.AddItem(reward.ItemID);
            }

            UIMainLobby.Get.UpdateButtonStatus(); // 這只是為了更新大廳登入按鈕的紅點狀態.
            Show(Year, Month); // 刷新介面.
        }
    }

    /// <summary>
    /// 目前顯示的是哪一個月份的登入獎勵.
    /// </summary>
    public int Year { get { return mDailyMain.Year; } }
    public int Month { get { return mDailyMain.Month; } }

    public bool Visible { 
        get { 
            if (gameObject)
                return gameObject.activeSelf; 
            else
                return false;
        } 
    }

    public void Show()
    {
        UIMainLobby.Get.HideAll();
        Show(DateTime.Now.Year, DateTime.Now.Month);
    }

    public void Show(int year, int month)
    {
        Show(true);

        mLifetimeMain.LifetimeLoginNum = GameData.Team.LifetimeRecord.LoginNum;
        buildLifetimeRewards();

        buildDailyLoginRewards(year, month);
        selectLastWeek(year, month);
    }

    private void buildDailyLoginRewards(int year, int month)
    {
        mDailyMain.ClearDailyRewards();
        TDailyData dailyData = DailyTable.Ins.GetByDate(year, month);
        if(dailyData == null)
            return;

        int currentLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        int receivedLoginNum = GameData.Team.GetReceivedDailyLoginNum(year, month);
        DailyLoginReward.Data[] rewards = new DailyLoginReward.Data[dailyData.ReviseRewards.Length];
        for(var i = 0; i < dailyData.ReviseRewards.Length; i++)
        {
            var itemData = GameData.DItemData[dailyData.ReviseRewards[i].ItemID];
            int day = i + 1;

            UIDailyLoginMain.EStatus status;
            if(day <= receivedLoginNum)
                status = UIDailyLoginMain.EStatus.Received;
            else if(day == receivedLoginNum + 1 && currentLoginNum > receivedLoginNum) // +1 是下一天未領取的獎勵.
                status = UIDailyLoginMain.EStatus.Receivable;
            else
                status = UIDailyLoginMain.EStatus.NoReceive;
            rewards[i] = UIDailyLoginBuilder.BuildDailyReward(day, itemData, status);
        }

        mDailyMain.SetDayReward(year, month, rewards);
    }

    private void selectLastWeek(int year, int month)
    {
        var receivedDailyLoginNum = GameData.Team.GetReceivedDailyLoginNum(year, month) + 1;
        if(0 <= receivedDailyLoginNum && receivedDailyLoginNum <= 7)
            mDailyMain.ShowWeek(1);
        else if(8 <= receivedDailyLoginNum && receivedDailyLoginNum <= 14)
            mDailyMain.ShowWeek(2);
        else if(15 <= receivedDailyLoginNum && receivedDailyLoginNum <= 21)
            mDailyMain.ShowWeek(3);
        else if(22 <= receivedDailyLoginNum && receivedDailyLoginNum <= 28)
            mDailyMain.ShowWeek(4);
        else
            mDailyMain.ShowWeek(4);
    }

    private void buildLifetimeRewards()
    {
        var receivedLoginNum = GameData.Team.LifetimeRecord.ReceivedLoginNum;
        var index = LifetimeTable.Ins.FindIndex(receivedLoginNum);
        index = Math.Max(index, 0); // 必須大於 0.

        List<UILifetimeReward.Data> rewards = new List<UILifetimeReward.Data>();
        for(int i = index; i < index + 3; i++) // 介面最多只能顯示 3 個終生獎勵.
        {
            TLifetimeData data = LifetimeTable.Ins.GetByIndex(i);
            if(data == null)
                break;

            UILifetimeReward.Data reward = UIDailyLoginBuilder.BuildLifetimeReward(data);
            rewards.Add(reward);
        }
        
        onlyOneRewardReceivable(rewards);

        mLifetimeMain.SetRewards(rewards.ToArray());
    }

    /// <summary>
    /// 一次只顯示 1 個可領取獎勵.
    /// </summary>
    /// <param name="rewards"></param>
    private static void onlyOneRewardReceivable(List<UILifetimeReward.Data> rewards)
    {
        bool hasReceivableReward = false;
        for(var i = 0; i < rewards.Count; i++)
        {
            if(hasReceivableReward)
            {
                rewards[i].Status = UIDailyLoginMain.EStatus.NoReceive;
                continue;
            }

            if(rewards[i].Status == UIDailyLoginMain.EStatus.Receivable)
                hasReceivableReward = true;
        }
    }

    public void Hide()
    {
        UIMainLobby.Get.Show();
        RemoveUI(instance.gameObject);
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