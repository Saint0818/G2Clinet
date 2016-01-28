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
        if(!VerifyPlayerCost(stageData, out errMsg))
            return false;

        if(!VerifyPlayerDailyCount(stageData, out errMsg))
            return false;

        if(!VerifyPlayerChallengeOnlyOnce(stageData, out errMsg))
            return false;

        errMsg = String.Empty;
        return true;
    }

    public static bool VerifyPlayerCost(TStageData stageData, out string errMsg)
    {
        errMsg = String.Empty;

        switch(stageData.CostKind)
        {
            case TStageData.ECostKind.Stamina:
                if(GameData.Team.Power < stageData.CostValue)
                {
                    errMsg = TextConst.S(230);
                    return false;
                }
                break;
            //            case TStageData.ECostKind.Activity:
            //            case TStageData.ECostKind.Challenger:
            default:
                throw new NotImplementedException();
        }
        return true;
    }

    public static bool VerifyPlayerDailyCount(TStageData stageData, out string errMsg)
    {
        var result = FindPlayerRemainDailyCount(stageData) > 0;
        errMsg = result ? TextConst.S(231) : String.Empty;
        return result;
    }

    public static bool VerifyPlayerProgress(TStageData stageData)
    {
        string errMsg;
        return VerifyPlayerProgress(stageData, out errMsg);
    }

    /// <summary>
    /// 檢查關卡可不可以打.
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="errMsg"></param>
    /// <returns> true: 關卡可以打. </returns>
    public static bool VerifyPlayerProgress(TStageData stageData, out string errMsg)
    {
        bool result;
        switch(stageData.IDKind)
        {
            case TStageData.EKind.MainStage:
                result = GameData.Team.Player.NextMainStageID >= stageData.ID;
                break;
            case TStageData.EKind.Instance:
                result = GameData.Team.Player.GetNextInstanceID(stageData.Chapter) < stageData.ID;
                break;
            default:
                throw new NotImplementedException(stageData.IDKind.ToString());
        }

        errMsg = result ? String.Empty : TextConst.S(9251);
        return result;
    }

    public static bool VerifyPlayerChallengeOnlyOnce(TStageData stageData, out string errMsg)
    {
        errMsg = String.Empty;
        if(!stageData.ChallengeOnlyOnce)
            return true;

        bool result;
        if(stageData.IDKind == TStageData.EKind.Instance)
        {
            if(GameData.Team.Player.NextInstanceIDs == null ||
               !GameData.Team.Player.NextInstanceIDs.ContainsKey(stageData.Chapter))
                return true;

            result = GameData.Team.Player.NextInstanceIDs[stageData.Chapter] <= stageData.ID;
        }
        else if(stageData.IDKind == TStageData.EKind.MainStage)
            result = GameData.Team.Player.NextMainStageID <= stageData.ID;
        else
            throw new NotImplementedException(stageData.ToString());

        if(!result)
            errMsg = TextConst.S(9250);

        return result;
    }
}