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
public class LifeTimeTable
{
    private static readonly LifeTimeTable INSTANCE = new LifeTimeTable();
    public static LifeTimeTable Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<int, TLifeTimeData> mLifeTimeByLoginNum = new Dictionary<int, TLifeTimeData>();

    private LifeTimeTable() {}

    public void Load(string jsonText)
    {
        clear();

        // 刪除 ["] 字元.
        jsonText = jsonText.Replace("\"[", "[");
        jsonText = jsonText.Replace("]\"", "]");
        jsonText = jsonText.Replace("\"{", "{");
        jsonText = jsonText.Replace("}\"", "}");

        var lifeTimes = JsonConvertWrapper.DeserializeObject<TLifeTimeData[]>(jsonText);
        foreach(TLifeTimeData lifeTime in lifeTimes)
        {
            mLifeTimeByLoginNum.Add(lifeTime.LoginNum, lifeTime);
        }

        Debug.Log("[lifetime parsed finished.] ");
    }

    private void clear()
    {
        mLifeTimeByLoginNum.Clear();
    }

    public bool Has(int loginNum)
    {
        return mLifeTimeByLoginNum.ContainsKey(loginNum);
    }

    [CanBeNull]
    public TLifeTimeData Get(int loginNum)
    {
        return Has(loginNum) ? mLifeTimeByLoginNum[loginNum] : null;
    }
}