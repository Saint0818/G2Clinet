using System;
using UnityEngine;

public static class UIStageTools
{
    /// <summary>
    /// 找出玩家該關卡還可以打幾次.
    /// </summary>
    /// <param name="stageData"></param>
    /// <returns></returns>
    public static int FindPlayerRemainDailyCount(TStageData stageData)
    {
        int remainDailyCount = stageData.ChallengeNum - GameData.Team.Player.GetStageChallengeNum(stageData.ID);
        return Mathf.Max(0, remainDailyCount); // 強迫數值大於等於 0.
    }

    public static bool VerifyPlayer(TStageData stageData)
    {
        string errMsg;
        return VerifyPlayer(stageData, out errMsg);
    }

    /// <summary>
    /// 檢查玩家是否可以進入遊戲.
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="errMsg"></param>
    /// <returns></returns>
    public static bool VerifyPlayer(TStageData stageData, out string errMsg)
    {
        if(!verifyPlayerCost(stageData))
        {
            errMsg = TextConst.S(230);
            return false;
        }

        if(!verifyPlayerDailyCount(stageData))
        {
            errMsg = TextConst.S(231);
            return false;
        }

        if(!verifyPlayerLv(stageData))
        {
            errMsg = TextConst.S(232);
            return false;
        }

        errMsg = String.Empty;
        return true;
    }

    private static bool verifyPlayerCost(TStageData stageData)
    {
        switch(stageData.CostKind)
        {
            case TStageData.ECostKind.Stamina:
                if(GameData.Team.Power < stageData.CostValue)
                    return false;
                break;
            //            case TStageData.ECostKind.Activity:
            //            case TStageData.ECostKind.Challenger:
            default:
                throw new NotImplementedException();
        }
        return true;
    }

    private static bool verifyPlayerDailyCount(TStageData stageData)
    {
        return FindPlayerRemainDailyCount(stageData) > 0;
    }

    private static bool verifyPlayerLv(TStageData stageData)
    {
        return GameData.Team.Player.Lv >= stageData.LimitLevel;
    }
}