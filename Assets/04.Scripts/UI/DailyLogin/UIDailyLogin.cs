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

        mDailyMain.OnReceiveListener += onDailyReceive;

        mLifetimeMain = GetComponent<UILifetimeLoginMain>();
        mLifetimeMain.OnReceiveListener += onLifetimeReceive;
    }

    private void onLifetimeReceive()
    {
        var currentLoginNum = GameData.Team.LifetimeRecord.LoginNum;
        var receivedLoginNum = UIDailyLoginHelper.GetLifetimeReceiveLoginNum();
        Debug.LogFormat("Current:{0}, Received:{1}", currentLoginNum, receivedLoginNum);

        // +1 才是可領取的獎勵.
        var startIndex = LifetimeTable.Ins.FindIndex(receivedLoginNum) + 1;

        var endIndex = LifetimeTable.Ins.FindIndex(currentLoginNum);
        endIndex = Math.Max(0, endIndex);

        Debug.LogFormat("StartIndex:{0}, EndIndex:{1}", startIndex, endIndex);

        UIGetItem.Get.SetTitle(TextConst.S(3812));
        for(int i = startIndex; i <= endIndex; i++)
        {
            TLifetimeData data = LifetimeTable.Ins.GetByIndex(i);
            if(data == null)
                break;

            foreach(TLifetimeData.Reward reward in data.Rewards)
                UIGetItem.Get.AddItem(reward.ItemID);
        }

        UIDailyLoginHelper.SetLifetimeReceiveLoginNum(currentLoginNum);

        UIMainLobby.Get.UpdateButtonStatus(); // 這只是為了更新大廳登入按鈕的紅點狀態.
        Show(Year, Month); // 刷新介面.
    }

    private void onDailyReceive(int year, int month)
    {
        var currentLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        var receviedLoginNum = UIDailyLoginHelper.GetDailyReceiveLoginNum(year, month);
//        Debug.LogFormat("OnReceiveClick:{0}-{1}, CurLoginNum:{2}, ReceivedLoginNum:{3}", year, month, currentLoginNum, receviedLoginNum);

        UIGetItem.Get.SetTitle(TextConst.S(3812));
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

        UIMainLobby.Get.UpdateButtonStatus(); // 這只是為了更新大廳登入按鈕的紅點狀態.
        Show(Year, Month); // 刷新介面.
    }

    /// <summary>
    /// 目前顯示的是哪一個月份的登入獎勵.
    /// </summary>
    public int Year { get { return mDailyMain.Year; } }
    public int Month { get { return mDailyMain.Month; } }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void Show()
    {
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
        int receiveLoginNum = UIDailyLoginHelper.GetDailyReceiveLoginNum(year, month);
        DailyLoginReward.Data[] rewards = new DailyLoginReward.Data[dailyData.Rewards.Length];
        for(var i = 0; i < dailyData.Rewards.Length; i++)
        {
            var itemData = GameData.DItemData[dailyData.Rewards[i].ItemID];
            int day = i + 1;
            UIDailyLoginMain.EStatus status;
            if(day <= receiveLoginNum)
                status = UIDailyLoginMain.EStatus.Received;
            else
                status = day <= currentLoginNum ? UIDailyLoginMain.EStatus.Receivable : UIDailyLoginMain.EStatus.NoReceive;
            rewards[i] = UIDailyLoginBuilder.BuildDailyReward(day, itemData, status);
        }

        mDailyMain.SetDayReward(year, month, rewards);
    }

    private void selectLastWeek(int year, int month)
    {
        var curLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        if(0 <= curLoginNum && curLoginNum <= 7)
            mDailyMain.ShowWeek(1);
        else if(8 <= curLoginNum && curLoginNum <= 14)
            mDailyMain.ShowWeek(2);
        else if(15 <= curLoginNum && curLoginNum <= 21)
            mDailyMain.ShowWeek(3);
        else if(22 <= curLoginNum && curLoginNum <= 28)
            mDailyMain.ShowWeek(4);
    }

    private void buildLifetimeRewards()
    {
        var receivedLoginNum = UIDailyLoginHelper.GetLifetimeReceiveLoginNum();
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
        
        mLifetimeMain.SetRewards(rewards.ToArray());
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