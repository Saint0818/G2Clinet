using System;

public static class AITools
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="posIndex"> 0:C, 1:F, 2:G. </param>
    /// <param name="data"> true: 取得戰術成功. </param>
    /// <returns></returns>
    public static bool RandomTactical(ETacticalKind tactical, EPlayerPostion posIndex, out TTacticalData data)
    {
        return RandomTactical(convert(tactical, posIndex), out data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="data"></param>
    /// <returns> true: 取得戰術成功. </returns>
    public static bool RandomTactical(ETacticalKind tactical, out TTacticalData data)
    {
        int randomValue = UnityEngine.Random.Range(0, TacticalTable.Ins.GetCount(tactical));
        return TacticalTable.Ins.GetData(tactical, randomValue, out data);
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
    public static bool RandomCorrespondingTactical(ETacticalKind attTactical, ETacticalKind defTactical, 
                                                   EPlayerPostion posIndex,
                                                   out TTacticalData attData, out TTacticalData defData)
    {
        ETacticalKind convertAtt = convert(attTactical, posIndex);
        ETacticalKind convertDef = convert(defTactical, posIndex);

        // 假如編輯的資料不足, 比如進攻有 4 筆, 防守只有 2 筆, 那麼在找戰術的時候,
        // 進攻就會忽略 2 筆資料.
        int randomAtt = UnityEngine.Random.Range(0, TacticalTable.Ins.GetCount(convertAtt));
        int randomDef = UnityEngine.Random.Range(0, TacticalTable.Ins.GetCount(convertDef));
        int randomValue = Math.Min(randomAtt, randomDef);

        bool attStatus = TacticalTable.Ins.GetData(convertAtt, randomValue, out attData);
        bool defStatus = TacticalTable.Ins.GetData(convertDef, randomValue, out defData);
        return attStatus && defStatus;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="posIndex"> 0:C, 1:F, 2:G </param>
    /// <returns></returns>
    private static ETacticalKind convert(ETacticalKind tactical, EPlayerPostion posIndex)
    {
        switch (tactical)
        {
            case ETacticalKind.Inbounds:
                if(posIndex == EPlayerPostion.C)
                    return ETacticalKind.InboundsC;
                if (posIndex == EPlayerPostion.F)
                    return ETacticalKind.InboundsF;
                if (posIndex == EPlayerPostion.G)
                    return ETacticalKind.InboundsG;
                break;
    
            case ETacticalKind.InboundsDefence:
                if (posIndex == EPlayerPostion.C)
                    return ETacticalKind.InboundsDefC;
                if (posIndex == EPlayerPostion.F)
                    return ETacticalKind.InboundsDefF;
                if (posIndex == EPlayerPostion.G)
                    return ETacticalKind.InboundsDefG;
                break;
    
            case ETacticalKind.HalfInbounds:
                if (posIndex == EPlayerPostion.C)
                    return ETacticalKind.HalfInboundsC;
                if (posIndex == EPlayerPostion.F)
                    return ETacticalKind.HalfInboundsF;
                if (posIndex == EPlayerPostion.G)
                    return ETacticalKind.HalfInboundsG;
                break;
    
            case ETacticalKind.HalfInboundsDefence:
                if (posIndex == EPlayerPostion.C)
                    return ETacticalKind.HalfInboundsDefC;
                if (posIndex == EPlayerPostion.F)
                    return ETacticalKind.HalfInboundsDefF;
                if (posIndex == EPlayerPostion.G)
                    return ETacticalKind.HalfInboundsDefG;
                break;
    
            case ETacticalKind.Fast:
                if (posIndex == EPlayerPostion.C)
                    return ETacticalKind.MoveFrontCourtC;
                if (posIndex == EPlayerPostion.F)
                    return ETacticalKind.MoveFrontCourtF;
                if (posIndex == EPlayerPostion.G)
                    return ETacticalKind.MoveFrontCourtG;
                break;
    
            default:
                throw new ArgumentOutOfRangeException(String.Format("Tactical:{0}, PosIndex:{1}", tactical, posIndex));
        }
    
        throw new NotImplementedException(String.Format("Tactical:{0}", tactical));
    }
}
