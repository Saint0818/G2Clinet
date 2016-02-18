using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 每日登入獎勵.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得資料; Call HasXXX 檢查資料. </item>
/// </list>
public class DailyTable
{
    private static readonly DailyTable INSTANCE = new DailyTable();
    public static DailyTable Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<int, Dictionary<int, TDailyData>> mDailyByDate = new Dictionary<int, Dictionary<int, TDailyData>>();

    private DailyTable() {}

    public void Load(string jsonText)
    {
        clear();

        // 刪除 ["] 字元.
        jsonText = jsonText.Replace("\"[", "[");
        jsonText = jsonText.Replace("]\"", "]");
        jsonText = jsonText.Replace("\"{", "{");
        jsonText = jsonText.Replace("}\"", "}");

        var allDaily = JsonConvertWrapper.DeserializeObject<TDailyData[]>(jsonText);
        foreach(TDailyData daily in allDaily)
        {
            if(!mDailyByDate.ContainsKey(daily.Year))
                mDailyByDate.Add(daily.Year, new Dictionary<int, TDailyData>());
            mDailyByDate[daily.Year].Add(daily.Month, daily);
        }

        Debug.Log("[daily parsed finished.] ");
    }

    private void clear()
    {
        mDailyByDate.Clear();
    }

    public bool HasByDate(int year, int month)
    {
        return mDailyByDate.ContainsKey(year) && mDailyByDate[year].ContainsKey(month);
    }

    [CanBeNull]
    public TDailyData GetByDate(int year, int month)
    {
        if(HasByDate(year, month))
            return mDailyByDate[year][month];
        return null;
    }
}