using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 終生登入獎勵.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得資料; Call HasXXX 檢查資料. </item>
/// </list>
public class LifetimeTable
{
    private static readonly LifetimeTable INSTANCE = new LifetimeTable();
    public static LifetimeTable Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<int, TLifetimeData> mLifetimeByLoginNum = new Dictionary<int, TLifetimeData>();

    /// <summary>
    /// 根據 LoginNum 由小排到大.
    /// </summary>
    private readonly List<TLifetimeData> mLifetimes = new List<TLifetimeData>();

    private LifetimeTable() {}

    public void Load(string jsonText)
    {
        clear();

        // 刪除 ["] 字元.
        jsonText = jsonText.Replace("\"[", "[");
        jsonText = jsonText.Replace("]\"", "]");
        jsonText = jsonText.Replace("\"{", "{");
        jsonText = jsonText.Replace("}\"", "}");

        var lifeTimes = JsonConvertWrapper.DeserializeObject<TLifetimeData[]>(jsonText);
        foreach(TLifetimeData data in lifeTimes)
        {
            mLifetimeByLoginNum.Add(data.LoginNum, data);
            mLifetimes.Add(data);
        }

        // 由小排到大.
        mLifetimes.Sort((d1, d2) =>
        {
            if(d1.LoginNum > d2.LoginNum) return 1;
            if(d1.LoginNum < d2.LoginNum) return -1;
            return 0;
        });

        Debug.Log("[lifetime parsed finished.] ");
    }

    private void clear()
    {
        mLifetimeByLoginNum.Clear();
    }

    public bool Has(int loginNum)
    {
        return mLifetimeByLoginNum.ContainsKey(loginNum);
    }

    [CanBeNull]
    public TLifetimeData Get(int loginNum)
    {
        return Has(loginNum) ? mLifetimeByLoginNum[loginNum] : null;
    }

    /// <summary>
    /// 根據 loginNum 的數值, 找出最大, 但小於 loginNum 的資料索引值.
    /// </summary>
    /// <param name="loginNum"></param>
    /// <returns> -1: 沒找到. </returns>
    public int FindIndex(int loginNum)
    {
        var index = -1;
        for(var i = 0; i < mLifetimes.Count; i++)
        {
            int startLoginNum = mLifetimes[i].LoginNum;
            int endLoginNum = i+1 < mLifetimes.Count ? mLifetimes[i+1].LoginNum : int.MaxValue;
            if(startLoginNum <= loginNum && loginNum < endLoginNum)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public bool HasByIndex(int index)
    {
        return index < mLifetimes.Count;
    }

    [CanBeNull]
    public TLifetimeData GetByIndex(int index)
    {
        return HasByIndex(index) ? mLifetimes[index] : null;
    }
}