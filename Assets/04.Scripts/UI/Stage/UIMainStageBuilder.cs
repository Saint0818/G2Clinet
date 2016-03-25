using System;
using GameStruct;

public static class UIMainStageBuilder
{
    public static UIMainStageInfo.Data BuildInfo(TStageData stageData)
    {
        UIMainStageInfo.Data infoData = new UIMainStageInfo.Data
        {
            Name = stageData.Name,
            BgTextureName = stageData.KindTextIndex.ToString(),
            Description = stageData.Explain,
            KindSpriteName = stageData.KindTextIndex.ToString(),
            KindName = TextConst.S(stageData.KindTextIndex),
            Money = stageData.Money,
            ExpVisible = GameData.Team.Player.NextMainStageID <= stageData.ID,
            Exp = stageData.Exp,
            Stamina = stageData.CostValue,
            ShowCompleted = stageData.ID < GameData.Team.Player.NextMainStageID,
            RemainDailyCount = String.Format(TextConst.S(9312), UIStageHelper.FindPlayerRemainDailyCount(stageData)),
            StartEnable = UIStageVerification.VerifyQualification(stageData),
            RewardTitle = UIMainStageTools.FindRewardTitle(stageData)
        };

        infoData.RewardItems.AddRange(UIMainStageTools.FindRewardItems(stageData));

        infoData.MissionVisible = GameData.DMissionData.ContainsKey(stageData.MissionLimit) &&
                                  !GameData.Team.MissionFinished(stageData.MissionLimit);
        if(infoData.MissionVisible)
        {
            TMission mission = GameData.DMissionData[stageData.MissionLimit];
            infoData.MissionTitle = mission.Name;
            infoData.MissionDesc = mission.Explain;
            infoData.MissionCurrentValue =
                    GameData.Team.GetMissionValue(mission.Kind, mission.TimeKind, mission.TimeValue);
            infoData.MissionGoalValue = mission.GetAppropriateValue(infoData.MissionCurrentValue);
            infoData.MissionAction = () =>
            {
                UIMainStage.Get.Hide();
                UI2D.Get.OpenUI(mission.OpenUI);
            };
        }

        return infoData;
    }

    public static UIMainStageElement.Data BuildElement(TStageData stageData)
    {
        UIMainStageElement.Data elementData = new UIMainStageElement.Data
        {
            IsSelected = stageData.ID == GameData.Team.Player.NextMainStageID
        };
        elementData.IsEnable = UIStageVerification.VerifyPlayerProgress(stageData, out elementData.ErrMsg);
        if (elementData.IsEnable) // 再一次驗證關卡是不是只能打一次.
        {
            elementData.IsEnable = UIStageVerification.VerifyPlayerChallengeOnlyOnce(stageData, out elementData.ErrMsg);
            elementData.ShowClear = !elementData.IsEnable;
        }

        if (stageData.Kind != 9)
        {
            elementData.BGNormalIcon = elementData.IsEnable ? "StageButton01" : "StageButton03";
            elementData.BGPressIcon = elementData.IsEnable ? "StageButton02" : "StageButton03";
        }
        else
        {
            elementData.BGNormalIcon = elementData.IsEnable ? "2000009" : "StageButton09";
            elementData.BGPressIcon = elementData.IsEnable ? "StageButton08" : "StageButton09";
        }

        return elementData;
    }
}