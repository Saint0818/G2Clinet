using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 負責送出統計資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call LogXXX 送出資料. </item>
/// </list>
public class Statistic
{
    private static readonly Statistic INSTANCE = new Statistic();
    public static Statistic Ins
    {
        get { return INSTANCE; }
    }

    private readonly List<IStatisticService> mServices = new List<IStatisticService>
    {
        new StatisticGA()
    };

    private Statistic() {}

    public void LogScreen(int id)
    {
        TStatisticScreenData data = StatisticScreenTable.Ins.Get(id);
        if(data == null)
        {
            Debug.LogWarningFormat("ID({0}) don't exist.", id);
            return;
        }

//        Debug.LogFormat("LogScreen:{0}", data);

        for(var i = 0; i < mServices.Count; i++)
        {
            mServices[i].LogScreen(data);
        }
    }

    public void LogEvent(int id)
    {
        TStatisticEventData data = StatisticEventTable.Ins.Get(id);
        if(data == null)
        {
            Debug.LogWarningFormat("ID({0}) don't exist.", id);
            return;
        }

        for (var i = 0; i < mServices.Count; i++)
        {
            mServices[i].LogEvent(data);
        }
    }
}