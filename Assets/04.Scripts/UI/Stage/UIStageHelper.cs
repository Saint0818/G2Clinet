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
}