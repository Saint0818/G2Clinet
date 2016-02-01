using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 記錄介面開啟限制的相關資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得關卡資料; Call HasXXX 檢查關卡資料. </item>
/// </list>
public class LimitTable
{
    private static readonly LimitTable INSTANCE = new LimitTable();
    public static LimitTable Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<int, TLimitData> mLimits = new Dictionary<int, TLimitData>();

    private LimitTable() {}

    public void Load(string jsonText)
    {
        clear();

        // 刪除 ["] 字元.
        jsonText = jsonText.Replace("\"[", "[");
        jsonText = jsonText.Replace("]\"", "]");
        jsonText = jsonText.Replace("\"{", "{");
        jsonText = jsonText.Replace("}\"", "}");

        var limits = JsonConvertWrapper.DeserializeObject<TLimitData[]>(jsonText);
        foreach(TLimitData limit in limits)
        {
            mLimits.Add(limit.OpenID, limit);
        }

        Debug.Log("[limit parsed finished.] ");
    }

    private void clear()
    {
        mLimits.Clear();
    }
}