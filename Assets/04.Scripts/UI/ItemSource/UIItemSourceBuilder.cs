
using System;
using GameStruct;
using System.Collections.Generic;
using UnityEngine;

public static class UIItemSourceBuilder
{
    public static UIItemSourceElement.Data[] Build(TItemData item, Action<bool> startCallback)
    {
        List<UIItemSourceElement.Data> elements = new List<UIItemSourceElement.Data>();
        foreach(int stageID in item.StageSource)
        {
            if(!StageTable.Ins.HasByID(stageID))
            {
                Debug.LogErrorFormat("Can't find StageID({0}).", stageID);
                continue;
            }

            if(StageTable.MinMainStageID <= stageID && stageID <= StageTable.MaxMainStageID)
                elements.Add(createData(stageID, startCallback));
        }

        return elements.ToArray();
    }

    private static UIItemSourceElement.Data createData(int stageID, Action<bool> startCallback)
    {
        var data = new UIItemSourceElement.Data
        {
            KindName = TextConst.S(9152),
            Name = StageTable.Ins.GetByID(stageID).Name,
            EnableStartButton = GameData.Team.Player.NextMainStageID >= stageID
        };

        if(data.EnableStartButton)
        {
            data.StartAction = new OpenStageAction(stageID);
            data.StartCallback = startCallback;
        }

        return data;
    }
}