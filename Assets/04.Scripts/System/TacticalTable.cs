
using System;
using System.Collections.Generic;
using GamePlayStruct;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 TacticalTable.Ins 取得實體. </item>
/// <item> Call GetData 取得戰術. </item>
/// <item> Call GetCount 取得戰術數量. </item>
/// </list>
/// 
/// 新增戰術:
/// <list type="number">
/// <item> 修改 ETacticalKind, PrefixNameTacticalPairs. </item>
/// </list>
/// 
/// How to use:
/// <list type="number">
/// <item> use TacticalTable.Ins to get instance. </item>
/// <item> call GetData method to find tactical data. </item>
/// <item> call GetCount method to know tactical number. </item>
/// </list>
/// 
/// How to add tactical:
/// <list type="number">
/// <item> modify ETacticalKind and PrefixNameTacticalPairs. </item>
/// </list>
/// 
/// 程式碼的壞味道:
/// <para> 每次新增新的戰術類型時, 需要修改很多地方. 不過目前暫時先不改, 後續真的很常新增戰術類型時再改. </para>
/// 
/// Bad Smell:
/// <para> Shotgun Surgery: when add tactical. </para>
/// </remarks>
public class TacticalTable
{
    private static readonly TacticalTable INSTANCE = new TacticalTable();
    public static TacticalTable Ins
    {
        get { return INSTANCE; }
    }

    private static readonly TTacticalData EmptyData = new TTacticalData();

    private readonly Dictionary<ETacticalKind, List<TTacticalData>> mTacticals = new Dictionary<ETacticalKind, List<TTacticalData>>();

    public static readonly Dictionary<string, ETacticalKind> PrefixNameTacticalPairs = new Dictionary<string, ETacticalKind>
    {
        {"jumpball0", ETacticalKind.None},
        {"jumpball1", ETacticalKind.None},

        {"normal", ETacticalKind.Attack},

        {"tee0", ETacticalKind.InboundsCenter},
        {"tee1", ETacticalKind.InboundsForward},
        {"tee2", ETacticalKind.InboundsGuard},

        {"teedefence0", ETacticalKind.InboundsDefenceCenter}, 
        {"teedefence1", ETacticalKind.InboundsDefenceForward},
        {"teedefence2", ETacticalKind.InboundsDefenceGuard}, 

        {"fast0", ETacticalKind.FastCenter}, 
        {"fast1", ETacticalKind.FastForward}, 
        {"fast2", ETacticalKind.FastGuard}, 

        {"center", ETacticalKind.Center}, 
        {"forward", ETacticalKind.Forward}, 
        {"guard", ETacticalKind.Guard}, 

        {"teehalf0", ETacticalKind.HalfInboundsCenter}, 
        {"teehalf1", ETacticalKind.HalfInboundsForward},
        {"teehalf2", ETacticalKind.HalfInboundsGuard}, 

        {"teedefencehalf0", ETacticalKind.HalfInboundsDefenceCenter}, 
        {"teedefencehalf1", ETacticalKind.HalfInboundsDefenceForward},
        {"teedefencehalf2", ETacticalKind.HalfInboundsDefenceGuard}, 
    };

    private TacticalTable()
    {
    }

    public void Load(string jsonText)
    {
        clear();

        TTacticalData[] tacticals = (TTacticalData[])JsonConvert.DeserializeObject(jsonText, typeof(TTacticalData[]));
        foreach(TTacticalData data in tacticals)
        {
            mTacticals[data.Kind].Add(data);
        }

        Debug.Log("[tactical parsed finished.]");
    }

    private void clear()
    {
        mTacticals.Clear();
        foreach(ETacticalKind tactical in Enum.GetValues(typeof(ETacticalKind)))
        {
            mTacticals.Add(tactical, new List<TTacticalData>());
        }
    }

    public int GetCount(ETacticalKind tactical)
    {
        return mTacticals[tactical].Count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="index"></param>
    /// <param name="data"></param>
    /// <returns> true: 資料取得成功. </returns>
    public bool GetData(ETacticalKind tactical, int index, out TTacticalData data)
    {
        if(index < 0 || index >= mTacticals[tactical].Count)
        {
            data = EmptyData;
            return false;
        }

        data = mTacticals[tactical][index];
        return true;
    }
}
