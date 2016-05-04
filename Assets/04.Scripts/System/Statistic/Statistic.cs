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

    private readonly List<IStatisticService> mServices = new List<IStatisticService>();

    private Statistic()
    {
        if(Application.platform == RuntimePlatform.Android ||
           Application.platform == RuntimePlatform.WindowsEditor ||
           Application.platform == RuntimePlatform.OSXEditor)
        {
            mServices.Add(new StatisticGA());
        }

        mServices.Add(new StatisticNodeJs());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"> statisticscreen.json 表格的 id. </param>
    public void LogScreen(int id)
    {
        TStatisticScreenData data = StatisticScreenTable.Ins.Get(id);
        if(data == null)
        {
            Debug.LogWarningFormat("ID({0}) don't exist.", id);
            return;
        }

        LogScreen(data.ID, data.Name);
    }

    public void LogScreen(int customID, string customName)
    {
//        Debug.LogFormat("LogScreen, ID:{0}, Name:{1}", customID, customName);

        for(var i = 0; i < mServices.Count; i++)
        {
            mServices[i].LogScreen(customID, customName);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"> statisticevent.json 表格的 id. </param>
    public void LogEvent(int id)
    {
        TStatisticEventData data = StatisticEventTable.Ins.Get(id);
        if(data == null)
        {
            Debug.LogWarningFormat("ID({0}) don't exist.", id);
            return;
        }

        logEvent(data.ID, data.Category, data.Action, data.Label, data.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"> statisticevent.json 表格的 id. </param>
    /// <param name="customLabel"> 不使用 statisticevent.json 表格的 Value, 改用這個值. </param>
    public void LogEvent(int id, string customLabel)
    {
        TStatisticEventData data = StatisticEventTable.Ins.Get(id);
        if (data == null)
        {
            Debug.LogWarningFormat("ID({0}) don't exist.", id);
            return;
        }

        logEvent(data.ID, data.Category, data.Action, customLabel, data.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"> statisticevent.json 表格的 id. </param>
    /// <param name="customValue"> 不使用 statisticevent.json 表格的 Value, 改用這個值(要給 >= 0 的數值). </param>
    public void LogEvent(int id, int customValue)
    {
        TStatisticEventData data = StatisticEventTable.Ins.Get(id);
        if (data == null)
        {
            Debug.LogWarningFormat("ID({0}) don't exist.", id);
            return;
        }

        logEvent(data.ID, data.Category, data.Action, data.Label, customValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"> statisticevent.json 表格的 id. </param>
    /// <param name="customLabel"> 不使用 statisticevent.json 表格的 Label, 改用這個值. </param>
    /// <param name="customValue"> 不使用 statisticevent.json 表格的 Value, 改用這個值(要給 >= 0 的數值). </param>
    public void LogEvent(int id, string customLabel, int customValue)
    {
        TStatisticEventData data = StatisticEventTable.Ins.Get(id);
        if(data == null)
        {
            Debug.LogWarningFormat("ID({0}) don't exist.", id);
            return;
        }

        logEvent(data.ID, data.Category, data.Action, customLabel, customValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="category"></param>
    /// <param name="action"></param>
    /// <param name="label"> (Optional) 空字串表示不送此資訊. </param>
    /// <param name="value"> (Optional) 小於 0 表示此參數不送. </param>
    private void logEvent(int id, string category, string action, string label, int value)
    {
//        Debug.LogFormat("LogEvent, ID:{0}, Category:{1}, Action:{2}, CustomLabel:{3}, CustomValue:{4}", id, category, action, label, value);

        for (var i = 0; i < mServices.Count; i++)
        {
            mServices[i].LogEvent(id, category, action, label, value);
        }
    }
}