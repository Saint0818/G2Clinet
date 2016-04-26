using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 統計資料(Event).
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得資料; Call HasXXX 檢查資料. </item>
/// </list>
public class StatisticEventTable
{
    private static readonly StatisticEventTable INSTANCE = new StatisticEventTable();
    public static StatisticEventTable Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<int, TStatisticEventData> mEvents = new Dictionary<int, TStatisticEventData>();

    private StatisticEventTable() {}

    public void Load(string jsonText)
    {
        clear();

        // 刪除 ["] 字元.
        jsonText = jsonText.Replace("\"[", "[");
        jsonText = jsonText.Replace("]\"", "]");
        jsonText = jsonText.Replace("\"{", "{");
        jsonText = jsonText.Replace("}\"", "}");

        var allData = JsonConvertWrapper.DeserializeObject<TStatisticEventData[]>(jsonText);
        foreach(var data in allData)
        {
            mEvents.Add(data.ID, data);
        }

        Debug.Log("[StatisticEvent parsed finished.] ");
    }

    private void clear()
    {
        mEvents.Clear();
    }

    [CanBeNull]
    public TStatisticEventData Get(int id)
    {
        return mEvents.ContainsKey(id) ? mEvents[id] : null;
    }

}