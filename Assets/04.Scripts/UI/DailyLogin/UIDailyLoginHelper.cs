using UnityEngine;

public static class UIDailyLoginHelper
{
    private const string DailyReceiveKey = "UIDailyLoginReceiveNum";
    private const string LifetimeReceiveKey = "UILifetimeLoginReceiveNum";

    /// <summary>
    /// DailyReceiveLoginNum = 0 表示沒有領取; 1 表示已經領取第 1 天的獎勵, 9 表示已經領取第 9 天的獎勵.
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
}
