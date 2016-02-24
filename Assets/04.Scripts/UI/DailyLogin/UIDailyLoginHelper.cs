using UnityEngine;

public static class UIDailyLoginHelper
{
    private const string ReceiveKey = "UIDailyLoginReceiveNum";

    /// <summary>
    /// DailyReceiveLoginNum = 0 表示沒有領取; 1 表示已經領取第 1 天的獎勵, 9 表示已經領取第 9 天的獎勵.
    /// 以此類推.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="num"></param>
    public static void SetDailyReceiveLoginNum(int year, int month, int num)
    {
        PlayerPrefs.SetInt(getKey(year, month), num);
    }

    public static int GetDailyReceiveLoginNum(int year, int month)
    {
        string key = getKey(year, month);
        if(PlayerPrefs.HasKey(key))
            return PlayerPrefs.GetInt(key);
        return 0;
    }

    private static string getKey(int year, int month)
    {
        return string.Format("{0}({1:0000}-{2:00})", ReceiveKey, year, month);
    }
}
