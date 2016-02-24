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

    private readonly Dictionary<int, List<TLimitData>> mLimitsByLv = new Dictionary<int, List<TLimitData>>();

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

            if(!mLimitsByLv.ContainsKey(limit.Lv))
                mLimitsByLv.Add(limit.Lv, new List<TLimitData>());
            mLimitsByLv[limit.Lv].Add(limit);
        }

        Debug.Log("[limit parsed finished.] ");
    }

    private void clear()
    {
        mLimitsByOpenID.Clear();
    }

    public bool HasByOpenID(EOpenID id)
    {
        return mLimitsByOpenID.ContainsKey(id);
    }

    [CanBeNull]
    public TLimitData GetByOpenID(EOpenID id)
    {
        return mLimitsByOpenID.ContainsKey(id) ? mLimitsByOpenID[id] : null;
    }

    public bool HasOpenIDByLv(int lv)
    {
        return mLimitsByLv.ContainsKey(lv);
    }

    public int GetLv(EOpenID id)
    {
        return mLimitsByOpenID.ContainsKey(id) ? mLimitsByOpenID[id].Lv : 0;
    }

    public int GetDiamond(EOpenID id)
    {
        return mLimitsByOpenID.ContainsKey(id) ? mLimitsByOpenID[id].Diamond : 0;
	}

	public int GetVisibleLv (EOpenID id)
	{
		return mLimitsByOpenID.ContainsKey(id) ? mLimitsByOpenID[id].VisibleLv : 0;
	}
}