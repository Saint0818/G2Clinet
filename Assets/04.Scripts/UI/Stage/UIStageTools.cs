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
        int remainDailyCount = stageData.DailyChallengeNum - GameData.Team.Player.GetStageChallengeNum(stageData.ID);
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

        if(!verifyPlayerChallengeOnlyOnce(stageData))
        {
            errMsg = TextConst.S(9250);
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

    private static bool verifyPlayerChallengeOnlyOnce(TStageData stageData)
    {
        if(!stageData.ChallengeOnlyOnce)
            return true;

        if(stageData.IDKind == TStageData.EKind.Instance)
        {
            if(GameData.Team.Player.NextInstanceIDs == null ||
               !GameData.Team.Player.NextInstanceIDs.ContainsKey(stageData.Chapter))
                return true;

            return GameData.Team.Player.NextInstanceIDs[stageData.Chapter] <= stageData.ID;
        }

        if(stageData.IDKind == TStageData.EKind.MainStage)
            return GameData.Team.Player.NextMainStageID <= stageData.ID;

        throw new NotImplementedException(stageData.ToString());
    }
}