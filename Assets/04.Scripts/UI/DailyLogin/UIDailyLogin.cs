using System;
using UnityEngine;

/// <summary>
/// 副本介面.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示副本. </item>
/// <item> Call Hide() 關閉副本. </item>
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
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void Show()
    {
        Show(DateTime.Now.Year, DateTime.Now.Month);
    }

    public void Show(int year, int month)
    {
        Show(true);

        buildDailyLoginRewards(year, month);
    }

    private void buildDailyLoginRewards(int year, int month)
    {
        mMain.ClearDailyRewards();
        TDailyData dailyData = DailyTable.Ins.GetByDate(year, month);
        if(dailyData == null)
            return;

        int currentLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        int receiveLoginNum = UIDailyLoginHelper.GetDailyReceiveLoginNum(year, month);
        IDailyLoginReward.Data[] rewards = new IDailyLoginReward.Data[dailyData.Rewards.Length];
        for(var i = 0; i < dailyData.Rewards.Length; i++)
        {
            var itemData = GameData.DItemData[dailyData.Rewards[i].ItemID];
            int day = i + 1;
            IDailyLoginReward.EStatus status;
            if(day <= receiveLoginNum)
                status = IDailyLoginReward.EStatus.Received;
            else
                status = day <= currentLoginNum ? IDailyLoginReward.EStatus.Receivable : IDailyLoginReward.EStatus.NoReceive;
            rewards[i] = UIDailyLoginBuilder.BuildDailyReward(day, itemData, status);
        }

        mMain.SetDayReward(rewards);
        mMain.ShowWeek(1);
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