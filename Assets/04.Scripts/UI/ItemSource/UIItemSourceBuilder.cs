
using System;
using GameStruct;
using System.Collections.Generic;
using UnityEngine;

public static class UIItemSourceBuilder
{
    public static UIItemSourceElement.Data[] Build(TItemData item, Action<bool> startCallback)
    {
        List<UIItemSourceElement.Data> elements = new List<UIItemSourceElement.Data>();
        buildMainStage(item, startCallback, elements);

        return elements.ToArray();
    }

    private static void buildMainStage(TItemData item, Action<bool> startCallback, 
                                       List<UIItemSourceElement.Data> elements)
    {
        foreach(int stageID in item.StageSource)
        {
            if(!StageTable.Ins.HasByID(stageID))
            {
                Debug.LogErrorFormat("Can't find StageID({0}).", stageID);
                continue;
            }

            if(StageTable.MinMainStageID <= stageID && stageID <= StageTable.MaxMainStageID)
            {
                var data = new UIItemSourceElement.Data
                {
                    KindName = TextConst.S(9152),
                    Name = StageTable.Ins.GetByID(stageID).Name,
                    StartEnabled = GameData.Team.Player.NextMainStageID >= stageID,
                    StartWarningMessage = TextConst.S(100009),
                    StartAction = new OpenStageAction(stageID),
                    StartCallback = startCallback
                };
                elements.Add(data);
            }
        }
    }
}