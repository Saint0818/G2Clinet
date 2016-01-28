
using System;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using UnityEngine;

public static class UIInstanceBuilder
{
    public static UIInstanceChapter.Data Build(ChapterData data, List<TStageData> normalStages, 
                                               TStageData bossStage)
    {
        return new UIInstanceChapter.Data
        {
            Title = data.Name,
            Desc = data.Explain,
            BossName = GameData.DPlayers[bossStage.PlayerID[0]].Name,
            Model = buildModel(bossStage.PlayerID[0]),
            NormalStages = buildNormalStages(normalStages),
            BossStage = buildStage(bossStage)
        };
    }

    private static UIInstanceStage.Data[] buildNormalStages(List<TStageData> normalStages)
    {
        List<UIInstanceStage.Data> uiNormalStages = new List<UIInstanceStage.Data>();

        foreach(TStageData stage in normalStages)
        {
            uiNormalStages.Add(buildStage(stage));
        }

        return uiNormalStages.ToArray();
    }

    private static UIInstanceStage.Data buildStage(TStageData stageData)
    {
        var data = new UIInstanceStage.Data
        {
            ID = stageData.ID,
            Title = stageData.Name,
            Money = stageData.Money,
            Exp = stageData.Exp,
            Stamina = stageData.CostValue,
            RemainDailyCount = string.Format(TextConst.S(9312), UIStageTools.FindPlayerRemainDailyCount(stageData)),
            ShowClear = GameData.Team.Player.NextInstanceIDs != null && 
                        GameData.Team.Player.NextInstanceIDs.ContainsKey(stageData.Chapter) &&
                        GameData.Team.Player.NextInstanceIDs[stageData.Chapter] > stageData.ID,
//            ShowMask = GameData.Team.Player.GetNextInstanceID(stageData.Chapter) < stageData.ID
            ShowMask = UIStageTools.VerifyPlayerProgress(stageData)
        };

        string errMsg;
        data.StartEnable = UIStageTools.VerifyPlayer(stageData, out errMsg);
        data.ErrorMsg = errMsg;

        data.RewardItems.AddRange(FindRewardItems(stageData));

        return data;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stageData"></param>
    /// <returns></returns>
    public static List<TItemData> FindRewardItems(TStageData stageData)
    {
        List<TItemData> foundRewardItems = new List<TItemData>();

        if(!GameData.DPlayers.ContainsKey(GameData.Team.Player.ID))
            return foundRewardItems;

        var pos = (EPlayerPostion)GameData.DPlayers[GameData.Team.Player.ID].BodyType;

        int[] items;
        if(stageData.HasSurelyRewards(pos))
        {
            // 打最新進度的關卡, 並且有必給獎勵, 那就要顯示必給獎勵.
            items = stageData.GetSurelyRewards(pos);
        }
        else
            // 不是打最新進度, 顯示亂數獎勵.
            items = stageData.Rewards;

        for(int i = 0; i < 3; i++) // 和企劃約定好, 僅顯示前 3 個.
        {
            if(items != null && items.Length > i && GameData.DItemData.ContainsKey(items[i]))
                foundRewardItems.Add(GameData.DItemData[items[i]]);
        }
        return foundRewardItems;
    }

    private static GameObject buildModel(int playerID)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        GameObject model = new GameObject { name = "BossModel" };
        ModelManager.Get.SetAvatar(ref model, player.Avatar, GameData.DPlayers[playerID].BodyType,
                                   EAnimatorType.AvatarControl, false);

        changeLayersRecursively(model.transform, "UI");

        return model;
    }

    private static void changeLayersRecursively(Transform current, string name)
    {
        current.gameObject.layer = LayerMask.NameToLayer(name);
        foreach(Transform child in current)
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
            changeLayersRecursively(child, name);
        }
    }
}
