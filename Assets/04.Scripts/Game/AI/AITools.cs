
using GamePlayStruct;
using UnityEngine;

public class AITools
{
    public static void RandomTactical(ETactical poskind, ref TTactical tactical)
    {
        randomTactical(poskind.GetPosNameIndex(), ref tactical);
    }

    public static void RandomTactical(ETactical poskind, int index, ref TTactical tactical)
    {
        randomTactical(poskind.GetPosNameIndex(index), ref tactical);
    }

    /// <summary>
    /// 亂數找出一個戰術.
    /// </summary>
    /// <param name="tacticalIndex"> 哪一類的戰術. </param>
    /// <param name="tactical"></param>
    private static void randomTactical(int tacticalIndex, ref TTactical tactical)
    {
        if(tactical.PosAy1 == null)
            tactical = new TTactical (false);

        tactical.FileName = "";

        if(tacticalIndex >= 0 && tacticalIndex < GameConst.TacticalDataName.Length)
        {
            if(GameData.SituationPosition[tacticalIndex].Length > 0)
            {
                int randomValue = Random.Range(0, GameData.SituationPosition[tacticalIndex].Length);
                int i = GameData.SituationPosition[tacticalIndex][randomValue];
                tactical = GameData.TacticalData[i];
            }
        }
    }
}
