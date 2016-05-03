using GameEnum;
using UnityEngine;

public static class UIStageHelper
{
    /// <summary>
    /// 找出玩家該關卡還可以打幾次.
    /// </summary>
    /// <param name="stageData"></param>
    /// <returns></returns>
    public static int FindPlayerRemainDailyCount(TStageData stageData)
    {
        int remainDailyCount = stageData.DailyChallengeNum - GameData.Team.Player.GetStageChallengeNum(stageData.ID);
        return Mathf.Max(0, remainDailyCount); // 強迫數值大於等於 0.
    }

    public static string FindRewardTitle(TStageData stageData)
    {
        if(!GameData.DPlayers.ContainsKey(GameData.Team.Player.ID))
            return TextConst.S(9304);

        EPlayerPostion pos = (EPlayerPostion)GameData.DPlayers[GameData.Team.Player.ID].BodyType;
        if((isMainStageFirstPass(stageData) || isInstanceFirstPass(stageData)) && 
           stageData.HasSurelyRewards(pos))
            return TextConst.S(9310); // 文字是:必給獎勵.

        // 文字是:亂數獎勵.
        return TStageData.IsMainStage(stageData.ID) ? TextConst.S(9304) : TextConst.S(9804);
    }

    private static bool isMainStageFirstPass(TStageData stageData)
    {
        return TStageData.IsMainStage(stageData.ID) && GameData.Team.Player.NextMainStageID == stageData.ID;
    }

    private static bool isInstanceFirstPass(TStageData stageData)
    {
        return TStageData.IsInstance(stageData.ID) && 
               GameData.Team.Player.GetNextInstanceID(stageData.Chapter) == stageData.ID;
    }
}