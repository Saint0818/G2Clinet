
using UnityEngine;

/// <summary>
/// Google Analytics.
/// </summary>
public class StatisticGA : IStatisticService
{
    private readonly GoogleAnalyticsV4 mGA;

    public StatisticGA()
    {
        mGA = Object.FindObjectOfType<GoogleAnalyticsV4>();
    }

    public void LogScreen(TStatisticScreenData data)
    {
        if(!mGA)
        {
            Debug.LogError("Can't find GoogleAnalyticsV4 instance.");
            return;
        }

        mGA.LogScreen(string.Format("{0}.{1}", data.ID, data.Name));
    }

    public void LogEvent(TStatisticEventData data)
    {
        if(!mGA)
        {
            Debug.LogError("Can't find GoogleAnalyticsV4 instance.");
            return;
        }

        var builder = new EventHitBuilder();
        builder.SetEventCategory(data.Category).SetEventAction(data.Action);
        if(!string.IsNullOrEmpty(data.Label)) // 官方文件說 Label 是可選的項目.
            builder.SetEventLabel(data.Label);
        if(data.Value < 0) // 可選的項目. 官方文件說不支援負整數.
            builder.SetEventValue(data.Value);
        mGA.LogEvent(builder);
    }
}