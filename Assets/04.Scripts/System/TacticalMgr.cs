
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
/// <item> 用 TacticalMgr.Ins 取得實體. </item>
/// <item> Call RandomTactical 取得戰術. </item>
/// </list>
/// 
/// How to use:
/// <list type="number">
/// <item> use TacticalMgr.Ins to get instance. </item>
/// <item> call RandomTactical method to find tactical data. </item>
/// </list>
/// 
/// 程式碼的壞味道:
/// <para> 每次新增新的戰術類型時, 需要修改很多地方. 不過目前暫時先不改, 後續真的很常新增戰術類型時再改. </para>
/// 
/// Bad Smell:
/// <para> Shotgun Surgery: when add tactical. </para>
/// </remarks>
public class TacticalMgr
{
    private static readonly TacticalMgr INSTANCE = new TacticalMgr();
    public static TacticalMgr Ins
    {
        get { return INSTANCE; }
    }

//    private TTacticalData[] mTacticals;

//    /// <summary>
//    /// <para> key: GameConst.mTacticalPrefixNames 的索引值. </para>
//    /// <para> Value: GameData.TacticalData 的索引值. </para>
//    /// <para> 原作者的用意是用 [a][b] 的方式取出 TacticalData 的某筆戰術. 這個意思就是亂數找出某個情況下的戰術. </para>
//    /// <para> a: 會是遊戲中的某個情況, 比如跳球, 邊界發球等等. </para>
//    /// <para> b 通常都是亂數. </para>
//    /// </summary>
//    private readonly Dictionary<int, int[]> mSituationPosition = new Dictionary<int, int[]>();

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

        {"teehalf0", ETactical.HalfTeeCenter}, 
        {"teehalf1", ETactical.HalfTeeForward},
        {"teehalf2", ETactical.HalfTeeGuard}, 

        {"teedefencehalf0", ETactical.HalfTeeDefenceCenter}, 
        {"teedefencehalf1", ETactical.HalfTeeDefenceForward},
        {"teedefencehalf2", ETactical.HalfTeeDefenceGuard}, 
    };

    public void Load(string jsonText)
    {
        clear();

        TTacticalData[] tacticals = (TTacticalData[])JsonConvert.DeserializeObject(jsonText, typeof(TTacticalData[]));
        foreach(TTacticalData data in tacticals)
        {
            mTacticals[data.Tactical].Add(data);
        }

//        for(int i = 0; i < mTacticalPrefixNames.Length; i++)
//        {
//            if (!mSituationPosition.ContainsKey(i))
//            {
//                List<int> TacticalDataList = new List<int>();
//                for (int j = 0; j < tacticals.Length; j++)
//                {
//                    if(tacticals[j].FileName.Contains(mTacticalPrefixNames[i]))
//                        TacticalDataList.Add(j);
//                }
//
//                mSituationPosition.Add(i, TacticalDataList.ToArray());
//            }
//        }

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="posIndex"> 0:C, 1:F, 2:G </param>
    /// <returns></returns>
    private ETactical convert(ETactical tactical, int posIndex)
    {
        switch(tactical)
        {
            case ETactical.None:
            case ETactical.Attack:
            case ETactical.Center:
            case ETactical.Forward:
            case ETactical.Guard:
                break;

            case ETactical.Inbounds:
                if(posIndex == 0)
                    return ETactical.InboundsCenter;
                if(posIndex == 1)
                    return ETactical.InboundsForward;
                if(posIndex == 2)
                    return ETactical.InboundsGuard;
                break;

            case ETactical.InboundsDefence:
                if(posIndex == 0)
                    return ETactical.InboundsDefenceCenter;
                if (posIndex == 1)
                    return ETactical.InboundsDefenceForward;
                if (posIndex == 2)
                    return ETactical.InboundsDefenceGuard;
                break;

            case ETactical.HalfTee:
                if (posIndex == 0)
                    return ETactical.HalfTeeCenter;
                if (posIndex == 1)
                    return ETactical.HalfTeeForward;
                if (posIndex == 2)
                    return ETactical.HalfTeeGuard;
                break;

            case ETactical.HalfTeeDefence:
                if (posIndex == 0)
                    return ETactical.HalfTeeDefenceCenter;
                if (posIndex == 1)
                    return ETactical.HalfTeeDefenceForward;
                if (posIndex == 2)
                    return ETactical.HalfTeeDefenceGuard;
                break;

            case ETactical.Fast:
                if (posIndex == 0)
                    return ETactical.FastCenter;
                if (posIndex == 1)
                    return ETactical.FastForward;
                if (posIndex == 2)
                    return ETactical.FastGuard;
                break;

            default:
                throw new ArgumentOutOfRangeException(String.Format("Tactical:{0}, PosIndex:{1}", tactical, posIndex));
        }

        throw new NotImplementedException(String.Format("Tactical:{0}", tactical));
    }

    public void RandomTactical(ETactical tactical, int index, out TTacticalData data)
    {
        RandomTactical(convert(tactical, index), out data);
    }

    public void RandomTactical(ETactical tactical, out TTacticalData data)
    {
        int randomValue = UnityEngine.Random.Range(0, mTacticals[tactical].Count);
        data = mTacticals[tactical][randomValue];
    }

//    /// <summary>
//    /// 亂數找出一個戰術.
//    /// </summary>
//    /// <param name="tacticalIndex"> 哪一類的戰術. </param>
//    /// <param name="tactical"></param>
//    private void randomTactical(int tacticalIndex, ref TTacticalData tactical)
//    {
//        if(tactical.PosAy1 == null)
//            tactical = new TTacticalData();
//
//        tactical.FileName = "";
//
//        if(tacticalIndex >= 0 && tacticalIndex < PrefixNames.Length)
//        {
//            if(mSituationPosition[tacticalIndex].Length > 0)
//            {
//                int randomValue = Random.Range(0, mSituationPosition[tacticalIndex].Length);
//                int i = mSituationPosition[tacticalIndex][randomValue];
//                tactical = mTacticals[i];
//            }
//        }
//    }
}
