
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
/// <item> 修改 ETactical, PrefixNameTacticalPairs. </item>
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
/// <item> modify ETactical and PrefixNameTacticalPairs. </item>
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

    private readonly Dictionary<ETactical, List<TTacticalData>> mTacticals = new Dictionary<ETactical, List<TTacticalData>>();

    public static readonly Dictionary<string, ETactical> PrefixNameTacticalPairs = new Dictionary<string, ETactical>
    {
        {"jumpball0", ETactical.None},
        {"jumpball1", ETactical.None},

        {"normal", ETactical.Attack},

        {"tee0", ETactical.InboundsCenter},
        {"tee1", ETactical.InboundsForward},
        {"tee2", ETactical.InboundsGuard},

        {"teedefence0", ETactical.InboundsDefenceCenter}, 
        {"teedefence1", ETactical.InboundsDefenceForward},
        {"teedefence2", ETactical.InboundsDefenceGuard}, 

        {"fast0", ETactical.FastCenter}, 
        {"fast1", ETactical.FastForward}, 
        {"fast2", ETactical.FastGuard}, 

        {"center", ETactical.Center}, 
        {"forward", ETactical.Forward}, 
        {"guard", ETactical.Guard}, 

        {"teehalf0", ETactical.HalfInboundsCenter}, 
        {"teehalf1", ETactical.HalfInboundsForward},
        {"teehalf2", ETactical.HalfInboundsGuard}, 

        {"teedefencehalf0", ETactical.HalfInboundsDefenceCenter}, 
        {"teedefencehalf1", ETactical.HalfInboundsDefenceForward},
        {"teedefencehalf2", ETactical.HalfInboundsDefenceGuard}, 
    };

    public void Load(string jsonText)
    {
        clear();

        TTacticalData[] tacticals = (TTacticalData[])JsonConvert.DeserializeObject(jsonText, typeof(TTacticalData[]));
        foreach(TTacticalData data in tacticals)
        {
            mTacticals[data.Tactical].Add(data);
        }

        Debug.Log("[tactical parsed finished.]");
    }

    private void clear()
    {
        mTacticals.Clear();
        foreach(ETactical tactical in Enum.GetValues(typeof(ETactical)))
        {
            mTacticals.Add(tactical, new List<TTacticalData>());
        }
    }

    public int GetCount(ETactical tactical)
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
    public bool GetData(ETactical tactical, int index, out TTacticalData data)
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
