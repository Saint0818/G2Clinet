
using System;
using GamePlayStruct;

public class AITools
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="posIndex"> 0:C, 1:F, 2:G. </param>
    /// <param name="data"> true: 取得戰術成功. </param>
    /// <returns></returns>
    public static bool RandomTactical(ETactical tactical, int posIndex, out TTacticalData data)
    {
        return RandomTactical(convert(tactical, posIndex), out data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="data"></param>
    /// <returns> true: 取得戰術成功. </returns>
    public static bool RandomTactical(ETactical tactical, out TTacticalData data)
    {
        int randomValue = UnityEngine.Random.Range(0, TacticalMgr.Ins.GetCount(tactical));
        return TacticalMgr.Ins.GetData(tactical, randomValue, out data);
    }

    /// <summary>
    /// <para> 這部份我並沒有做錯誤檢查, 所以使用時必須小心, 必須要傳正確的數值. </para>
    /// <para> 此方法是亂數找出對應的戰術. 比如: Inbounds 和 InboundsDefence 的戰術. </para>
    /// </summary>
    /// <param name="attTactical"></param>
    /// <param name="defTactical"></param>
    /// <param name="posIndex"></param>
    /// <param name="attData"></param>
    /// <param name="defData"></param>
    /// <returns></returns>
    public static bool RandomCorrespondingTactical(ETactical attTactical, ETactical defTactical, int posIndex,
                                                   out TTacticalData attData, out TTacticalData defData)
    {
        int randomValue = UnityEngine.Random.Range(0, TacticalMgr.Ins.GetCount(attTactical));
        bool attStatus = TacticalMgr.Ins.GetData(convert(attTactical, posIndex), randomValue, out attData);
        bool defStatus = TacticalMgr.Ins.GetData(convert(defTactical, posIndex), randomValue, out defData);
        return attStatus && defStatus;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="posIndex"> 0:C, 1:F, 2:G </param>
    /// <returns></returns>
    private static ETactical convert(ETactical tactical, int posIndex)
    {
        switch (tactical)
        {
            case ETactical.Inbounds:
                if (posIndex == 0)
                    return ETactical.InboundsCenter;
                if (posIndex == 1)
                    return ETactical.InboundsForward;
                if (posIndex == 2)
                    return ETactical.InboundsGuard;
                break;
    
            case ETactical.InboundsDefence:
                if (posIndex == 0)
                    return ETactical.InboundsDefenceCenter;
                if (posIndex == 1)
                    return ETactical.InboundsDefenceForward;
                if (posIndex == 2)
                    return ETactical.InboundsDefenceGuard;
                break;
    
            case ETactical.HalfInbounds:
                if (posIndex == 0)
                    return ETactical.HalfInboundsCenter;
                if (posIndex == 1)
                    return ETactical.HalfInboundsForward;
                if (posIndex == 2)
                    return ETactical.HalfInboundsGuard;
                break;
    
            case ETactical.HalfInboundsDefence:
                if (posIndex == 0)
                    return ETactical.HalfInboundsDefenceCenter;
                if (posIndex == 1)
                    return ETactical.HalfInboundsDefenceForward;
                if (posIndex == 2)
                    return ETactical.HalfInboundsDefenceGuard;
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
}
