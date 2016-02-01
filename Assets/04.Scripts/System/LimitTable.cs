using System.Collections.Generic;
using GameEnum;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 記錄介面開啟限制的相關資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得資料; Call HasXXX 檢查資料. </item>
/// </list>
public class LimitTable
{
    private static readonly LimitTable INSTANCE = new LimitTable();
    public static LimitTable Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<EOpenID, TLimitData> mLimitsByOpenID = new Dictionary<EOpenID, TLimitData>();

    private readonly Dictionary<int, TLimitData> mLimitsByLv = new Dictionary<int, TLimitData>();

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
            mLimitsByOpenID.Add(limit.OpenID, limit);
            mLimitsByLv.Add(limit.Lv, limit);
        }

        Debug.Log("[limit parsed finished.] ");
    }

    private void clear()
    {
        mLimitsByOpenID.Clear();
    }

    public bool HasByID(EOpenID id)
    {
        return mLimitsByOpenID.ContainsKey(id);
    }

    [CanBeNull]
    public TLimitData GetByID(EOpenID id)
    {
        if(mLimitsByOpenID.ContainsKey(id))
            return mLimitsByOpenID[id];
        return null;
    }

    public bool HasOpenIDByLv(int lv)
    {
        return mLimitsByLv.ContainsKey(lv);
    }

    public int GetLv(EOpenID id)
    {
        if(mLimitsByOpenID.ContainsKey(id))
            return mLimitsByOpenID[id].Lv;
        return 0;
    }
}