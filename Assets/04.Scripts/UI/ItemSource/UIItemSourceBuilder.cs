﻿
using System;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

public static class UIItemSourceBuilder
{
    public static UIItemSourceElement.Data[] Build(TItemData item, Action<bool> startCallback)
    {
        List<UIItemSourceElement.Data> elements = new List<UIItemSourceElement.Data>();
        if(item.StageSource != null)
            buildStages(item.StageSource, startCallback, elements);
        if(item.UISource != null)
            buildUISource(item.UISource, startCallback, elements);

        return elements.ToArray();
    }

    private static void buildStages([NotNull]int[] stageSource, Action<bool> startCallback, 
                                       List<UIItemSourceElement.Data> elements)
    {
        foreach(int stageID in stageSource)
        {
            if(!StageTable.Ins.HasByID(stageID))
            {
                Debug.LogErrorFormat("Can't find StageID({0}).", stageID);
                continue;
            }

            if(TStageData.IsMainStage(stageID))
            {
                var data = new UIItemSourceElement.Data
                {
                    KindName = TextConst.S(9152), // [FFDD33]生涯競賽[-]
                    Name = StageTable.Ins.GetByID(stageID).Name,
                    StartEnabled = GameData.Team.Player.NextMainStageID >= stageID,
                    StartWarningMessage = TextConst.S(100009), // 100009:尚未符合挑戰此關卡的條件
                    StartAction = new OpenMainStageAction(stageID),
                    StartCallback = startCallback
                };
                elements.Add(data);
            }
            else if(TStageData.IsInstance(stageID))
            {
                TStageData instance = StageTable.Ins.GetByID(stageID);

                var data = new UIItemSourceElement.Data
                {
                    KindName = TextConst.S(9154), // [06b225]關主挑戰[-]
                    Name = instance.Name,
                    StartEnabled = GameData.Team.Player.GetNextInstanceID(instance.Chapter) >= stageID &&
                                   UIInstanceHelper.IsMainStagePass(instance.Chapter) ,
                    StartWarningMessage = TextConst.S(100009), // 100009:尚未符合挑戰此關卡的條件
                    StartAction = new OpenInstanceAction(stageID),
                    StartCallback = startCallback
                };
                elements.Add(data);
            }
        }
    }

    private static void buildUISource([NotNull]int[] uiSource, Action<bool> startCallback,
                                      List<UIItemSourceElement.Data> elements)
    {

        Func<string, string, string, UIItemSourceElement.Data> buildMall = 
            (title, desc, warningMsg) => new UIItemSourceElement.Data
            {
                KindName = title,
                Name = desc,
                StartWarningMessage = warningMsg,
                StartEnabled = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Mall),
                StartAction = new OpenMallAction(),
                StartCallback = startCallback
            };
        Func<string, string, string, UIItemSourceElement.Data> buildShopGeneral =
            (title, desc, warningMsg) => new UIItemSourceElement.Data
            {
                KindName = title,
                Name = desc,
                StartWarningMessage = warningMsg,
                StartEnabled = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Shop),
                StartAction = new OpenShopGeneralAction(),
                StartCallback = startCallback
            };
        Func<string, string, string, UIItemSourceElement.Data> buildShopLeague =
            (title, desc, warningMsg) => new UIItemSourceElement.Data
            {
                KindName = title,
                Name = desc,
                StartWarningMessage = warningMsg,
                StartEnabled = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Shop),
                StartAction = new OpenShopLeagueAction(),
                StartCallback = startCallback
            };
        Func<string, string, string, UIItemSourceElement.Data> buildShopSocial =
            (title, desc, warningMsg) => new UIItemSourceElement.Data
            {
                KindName = title,
                Name = desc,
                StartWarningMessage = warningMsg,
                StartEnabled = GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.Shop),
                StartAction = new OpenShopSocialAction(),
                StartCallback = startCallback
            };

        Dictionary<int, Func< string, string, string, UIItemSourceElement.Data>> uiBuilder = new Dictionary<int, Func<string, string, string, UIItemSourceElement.Data>>
        {
            {1, buildMall},
            {10, buildShopGeneral},
            {11, buildShopLeague},
            {12, buildShopSocial}
        };

        foreach(int uiID in uiSource)
        {
            string title = TextConst.S(100000 + uiID * 10);
            string desc = TextConst.S(100000 + uiID * 10 + 1);
            string warningMsg = TextConst.S(100000 + uiID * 10 + 9);

            if(uiBuilder.ContainsKey(uiID))
                elements.Add(uiBuilder[uiID](title, desc, warningMsg));
        }
    }
}