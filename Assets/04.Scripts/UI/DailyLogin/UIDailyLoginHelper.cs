using UnityEngine;

public static class UIDailyLoginHelper
{
    private const string ReceiveKey = "UIDailyLoginReceiveNum";

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
