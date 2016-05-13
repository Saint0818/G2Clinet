using System;
using GameStruct;

/// <summary>
/// 負責主線關卡介面驗證資訊.
/// </summary>
public static class UIStageVerification
{
    public enum EErrorCode
    {
        Pass,
        CannotEnter, // 關卡進度不足.
        NoPower, 
        NoDailyChallenge, // 每日挑戰次數用完, 但是每日重置次數還有(所以可以重置).
        NoDailyChallengeNoDiamond, // 每日挑戰次數用完, 但是每日重置次數還有(所以可以重置), 但是鑽石不夠.
        NoResetDailyChallenge, // 每日重置挑戰次數用完.
        NoChallengeAgain, // 已經打過, 而且不能再打.
        MissionNoPass
    }

    /// <summary>
    /// 檢查玩家是否可以進入遊戲.
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="errMsg"></param>
    /// <returns></returns>
    public static EErrorCode VerifyQualification(TStageData stageData, out string errMsg)
    {
        if(!VerifyNextMainStageID(stageData, out errMsg))
            return EErrorCode.CannotEnter;

        if(!VerifyCost(stageData, out errMsg))
            return EErrorCode.NoPower;

        if(!VerifyPlayerChallengeOnlyOnce(stageData, out errMsg))
            return EErrorCode.NoChallengeAgain;

        if(!VerifyPlayerMission(stageData, out errMsg))
            return EErrorCode.MissionNoPass;

        if(!VerifyDailyChallenge(stageData, out errMsg) && !VerifyResetDialyChallenge(stageData, out errMsg))
            return EErrorCode.NoResetDailyChallenge;

        if(!VerifyDailyChallenge(stageData, out errMsg))
        {
            var diamondsData = DiamondsTable.Ins.Get(TDiamondData.EKind.ResetDailyChallenge);
            int resetNum = GameData.Team.Player.GetResetStageChallengeNum(stageData.ID);
            int requireDiamond = diamondsData.GetReviseNum(resetNum);
            if(GameData.Team.Diamond >= requireDiamond)
                return EErrorCode.NoDailyChallenge;

            errMsg = TextConst.S(233);
            return EErrorCode.NoDailyChallengeNoDiamond;
        }

        errMsg = String.Empty;
        return EErrorCode.Pass;
    }

    public static bool VerifyNextMainStageID(TStageData stageData, out string errMsg)
    {
        errMsg = String.Empty;
        return GameData.Team.Player.NextMainStageID >= stageData.ID;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="errMsg"></param>
    /// <returns> true: 進入關卡的體力足夠; false: 不可進入關卡. </returns>
    public static bool VerifyCost(TStageData stageData, out string errMsg)
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

    public static bool VerifyDailyChallenge(TStageData stageData, out string errMsg)
    {
        var isCanPlay = UIStageHelper.FindPlayerRemainDailyCount(stageData) > 0;
        errMsg = isCanPlay ? String.Empty : TextConst.S(231);
        return isCanPlay;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="errMsg"></param>
    /// <returns> true: 可重置. </returns>
    public static bool VerifyResetDialyChallenge(TStageData stageData, out string errMsg)
    {
        var isPass = stageData.MaxResetDailyChallengeNum > GameData.Team.Player.GetResetStageChallengeNum(stageData.ID);
        errMsg = isPass ? String.Empty : TextConst.S(9314);
        return isPass;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="errMsg"></param>
    /// <returns> true: 驗證通過, 關卡可能可以打; false: 關卡不能打. </returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="errMsg"></param>
    /// <returns> true: 驗證通過, 關卡可能可以打; false: 關卡不能打. </returns>
    public static bool VerifyPlayerMission(TStageData stageData, out string errMsg)
    {
        errMsg = String.Empty;
        if(!GameData.DMissionData.ContainsKey(stageData.MissionLimit))
            return true; // 沒有任務限制.

        TMission mission = GameData.DMissionData[stageData.MissionLimit];
        bool missionFinished = GameData.Team.MissionFinished(ref mission);
        if(!missionFinished)
            errMsg = TextConst.S(541);

        return missionFinished;
    }
}