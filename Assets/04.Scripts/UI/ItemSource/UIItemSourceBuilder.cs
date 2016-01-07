
using GameStruct;
using System.Collections.Generic;
using UnityEngine;

public static class UIItemSourceBuilder
{
    public static UIItemSourceElement.Data[] Build(TItemData item)
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
                elements.Add(new UIItemSourceElement.Data
                {
                    KindName = TextConst.S(9152),
                    Name = StageTable.Ins.GetByID(stageID).Name,
                    Action = new OpenStageAction(stageID)
                });
        }

        return elements.ToArray();
    }
}