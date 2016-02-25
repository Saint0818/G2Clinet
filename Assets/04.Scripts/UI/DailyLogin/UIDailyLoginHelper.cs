using System;
using UnityEngine;

public static class UIDailyLoginHelper
{
    private const string DailyReceiveKey = "UIDailyLoginReceiveNum";
    private const string LifetimeReceiveKey = "UILifetimeLoginReceiveNum";

    /// <summary>
    /// DailyReceiveLoginNum = 0 表示沒有領取; 1 表示已經領取第 1 天的獎勵, 2 表示已經領取第 2 天的獎勵.
    /// 以此類推.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="num"></param>
    public static void SetDailyReceiveLoginNum(int year, int month, int num)
    {
        PlayerPrefs.SetInt(getDailyKey(year, month), num);
    }

    public static int GetDailyReceiveLoginNum(int year, int month)
    {
        string key = getDailyKey(year, month);
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : 0;
    }

    private static string getDailyKey(int year, int month)
    {
        return string.Format("{0}({1:0000}-{2:00})", DailyReceiveKey, year, month);
    }

    public static void SetLifetimeReceiveLoginNum(int value)
    {
        PlayerPrefs.SetInt(LifetimeReceiveKey, value);
    }

    public static int GetLifetimeReceiveLoginNum()
    {
        return PlayerPrefs.HasKey(LifetimeReceiveKey) ? PlayerPrefs.GetInt(LifetimeReceiveKey) : 0;
    }

    public static bool HasTodayDailyLoginReward()
    {
        return HasDailyLoginReward(DateTime.Now.Year, DateTime.Now.Month);
    }

    public static bool HasDailyLoginReward(int year, int month)
    {
        int currentLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        int receiveLoginNum = GetDailyReceiveLoginNum(year, month);

        TDailyData data = DailyTable.Ins.GetByDate(year, month);
        // +1 是已經收獎勵的下一天, 如果下一天沒獎勵, 就表示沒有每日獎勵了.
        return data != null && currentLoginNum > receiveLoginNum && data.HasRewardByDay(receiveLoginNum + 1);
    }

    public static bool HasLifetimeLoginReward()
    {
        var currentLoginNum = GameData.Team.LifetimeRecord.LoginNum;
        var receiveLoginNum = GetLifetimeReceiveLoginNum();

        var currentIndex = LifetimeTable.Ins.FindIndex(currentLoginNum);
        // +1 是下一個未領取的獎勵.
        var receiveIndex = LifetimeTable.Ins.FindIndex(receiveLoginNum) + 1;

        return currentIndex >= receiveIndex;
    }
}
