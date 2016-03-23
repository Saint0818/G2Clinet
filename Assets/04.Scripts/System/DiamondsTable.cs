using System.Collections.Generic;
using GameEnum;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 讀取 diamonds.json 的表格, 這個表格是記錄鑽石相關的資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得資料; Call HasXXX 檢查資料. </item>
/// </list>
public class DiamondsTable
{
    private static readonly DiamondsTable INSTANCE = new DiamondsTable();
    public static DiamondsTable Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<TDiamondData.EKind, TDiamondData> mDiamonds = new Dictionary<TDiamondData.EKind, TDiamondData>();

    private DiamondsTable() {}

    public void Load(string jsonText)
    {
        clear();

        // 刪除 ["] 字元.
        jsonText = jsonText.Replace("\"[", "[");
        jsonText = jsonText.Replace("]\"", "]");
        jsonText = jsonText.Replace("\"{", "{");
        jsonText = jsonText.Replace("}\"", "}");

        var diamonds = JsonConvertWrapper.DeserializeObject<TDiamondData[]>(jsonText);
        foreach(TDiamondData element in diamonds)
        {
            mDiamonds.Add(element.Kind, element);
        }

        Debug.Log("[diamonds parsed finished.] ");
    }

    private void clear()
    {
        mDiamonds.Clear();
    }

    public bool Has(TDiamondData.EKind kind)
    {
        return mDiamonds.ContainsKey(kind);
    }

    [CanBeNull]
    public TDiamondData Get(TDiamondData.EKind kind)
    {
        if(Has(kind))
            return mDiamonds[kind];
        return null;
    }
}