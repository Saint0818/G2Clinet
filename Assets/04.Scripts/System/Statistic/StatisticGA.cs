﻿
using UnityEngine;

/// <summary>
/// Google Analytics.
/// </summary>
public class StatisticGA : IStatisticService
{
    private readonly GoogleAnalyticsV4 mGA;

    public StatisticGA()
    {
        mGA = Object.Instantiate(Resources.Load<GoogleAnalyticsV4>("GoogleAnalytics/GAv4"));
    }

    public void LogScreen(int id, string name)
    {
        mGA.LogScreen(string.Format("{0}.{1}", id, name));
    }

    public void LogEvent(int id, string category, string action, string label, int value)
    {
        var builder = new EventHitBuilder();
        builder.SetEventCategory(category).SetEventAction(action);
        if(!string.IsNullOrEmpty(label)) // 官方文件說 Label 是可選的項目.
            builder.SetEventLabel(label);
        if(value >= 0) // 可選的項目. 官方文件說不支援負整數.
            builder.SetEventValue(value);
        mGA.LogEvent(builder);
    }
}