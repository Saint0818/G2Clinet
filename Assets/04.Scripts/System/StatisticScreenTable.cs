using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 統計資料(Screen).
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得資料; Call HasXXX 檢查資料. </item>
/// </list>
public class StatisticScreenTable
{
    private static readonly StatisticScreenTable INSTANCE = new StatisticScreenTable();
    public static StatisticScreenTable Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<int, TStatisticScreenData> mScreens = new Dictionary<int, TStatisticScreenData>();

    private StatisticScreenTable() {}

    public void Load(string jsonText)
    {
        clear();

        // 刪除 ["] 字元.
        jsonText = jsonText.Replace("\"[", "[");
        jsonText = jsonText.Replace("]\"", "]");
        jsonText = jsonText.Replace("\"{", "{");
        jsonText = jsonText.Replace("}\"", "}");

        var allData = JsonConvertWrapper.DeserializeObject<TStatisticScreenData[]>(jsonText);
        foreach(var data in allData)
        {
            mScreens.Add(data.ID, data);
        }

        Debug.Log("[StatisticScreen parsed finished.] ");
    }

    private void clear()
    {
        mScreens.Clear();
    }
}