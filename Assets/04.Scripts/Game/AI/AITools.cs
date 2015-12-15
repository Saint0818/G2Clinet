using System;

public static class AITools
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="pos"></param>
    /// <param name="data"> true: 取得戰術成功. </param>
    /// <returns></returns>
    public static bool RandomTactical(ETacticalAuto tactical, EPlayerPostion pos, out TTacticalData data)
    {
        return RandomTactical(convert(tactical, pos), out data);
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
    /// <param name="attKind"></param>
    /// <param name="defKind"></param>
    /// <param name="pos"></param>
    /// <param name="attData"></param>
    /// <param name="defData"></param>
    /// <returns></returns>
    public static bool RandomCorrespondingTactical(ETacticalAuto attKind, ETacticalAuto defKind, 
                                 EPlayerPostion pos, out TTacticalData attData, out TTacticalData defData)
    {
        ETacticalKind att = convert(attKind, pos);
        ETacticalKind def = convert(defKind, pos);

        // 假如編輯的資料不足, 比如進攻有 4 筆, 防守只有 2 筆, 那麼在找戰術的時候,
        // 進攻就會忽略 2 筆資料.
        int randomAtt = UnityEngine.Random.Range(0, TacticalTable.Ins.GetCount(att));
        int randomDef = UnityEngine.Random.Range(0, TacticalTable.Ins.GetCount(def));
        int randomValue = Math.Min(randomAtt, randomDef);

        bool attStatus = TacticalTable.Ins.GetData(att, randomValue, out attData);
        bool defStatus = TacticalTable.Ins.GetData(def, randomValue, out defData);
        return attStatus && defStatus;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tactical"></param>
    /// <param name="pos"> 0:C, 1:F, 2:G </param>
    /// <returns></returns>
    private static ETacticalKind convert(ETacticalAuto tactical, EPlayerPostion pos)
    {
        switch(tactical)
        {
            case ETacticalAuto.AttackNormal:
                if (pos == EPlayerPostion.C)
                    return ETacticalKind.AttackNormalC;
                if (pos == EPlayerPostion.F)
                    return ETacticalKind.AttackNormalF;
                if (pos == EPlayerPostion.G)
                    return ETacticalKind.AttackNormalG;
                break;

            case ETacticalAuto.Inbounds:
                if(pos == EPlayerPostion.C)
                    return ETacticalKind.InboundsC;
                if (pos == EPlayerPostion.F)
                    return ETacticalKind.InboundsF;
                if (pos == EPlayerPostion.G)
                    return ETacticalKind.InboundsG;
                break;
    
            case ETacticalAuto.InboundsDef:
                if (pos == EPlayerPostion.C)
                    return ETacticalKind.InboundsDefC;
                if (pos == EPlayerPostion.F)
                    return ETacticalKind.InboundsDefF;
                if (pos == EPlayerPostion.G)
                    return ETacticalKind.InboundsDefG;
                break;
    
            case ETacticalAuto.HalfInbounds:
                if (pos == EPlayerPostion.C)
                    return ETacticalKind.HalfInboundsC;
                if (pos == EPlayerPostion.F)
                    return ETacticalKind.HalfInboundsF;
                if (pos == EPlayerPostion.G)
                    return ETacticalKind.HalfInboundsG;
                break;
    
            case ETacticalAuto.HalfInboundsDef:
                if (pos == EPlayerPostion.C)
                    return ETacticalKind.HalfInboundsDefC;
                if (pos == EPlayerPostion.F)
                    return ETacticalKind.HalfInboundsDefF;
                if (pos == EPlayerPostion.G)
                    return ETacticalKind.HalfInboundsDefG;
                break;
    
            case ETacticalAuto.MoveFrontCourt:
                if (pos == EPlayerPostion.C)
                    return ETacticalKind.MoveFrontCourtC;
                if (pos == EPlayerPostion.F)
                    return ETacticalKind.MoveFrontCourtF;
                if (pos == EPlayerPostion.G)
                    return ETacticalKind.MoveFrontCourtG;
                break;
        }

        throw new NotImplementedException(String.Format("Tactical:{0}, Pos:{1}", tactical, pos));
    }
}
