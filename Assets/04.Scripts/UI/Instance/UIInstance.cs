﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 副本介面.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示副本. </item>
/// <item> Call Hide() 關閉副本. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIInstance : UIBase
{
    private static UIInstance instance;
    private const string UIName = "UIInstance";

    public UIInstanceMain Main { get { return mMain; } }
    private UIInstanceMain mMain;

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIInstanceMain>();
        mMain.ChapterBackButton.onClick.Add(new EventDelegate(() =>
            {
                UIGameLobby.Get.Show();
                Hide();
            }));

        mMain.StageStartListener += enterSelectRole;
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void Show()
    {
        ShowByChapter(UIInstanceHelper.DefaultSelectChapter);
    }

    public void ShowByChapter(int chapter)
    {
        Show(true);

        buildChapters();
        mMain.ShowChapters();
        mMain.SelectChapter(chapter);
    }

    /// <summary>
    /// 顯示在玩家上次打的副本關卡.
    /// </summary>
    public void ShowByPlayStageID()
    {
        ShowByStageID(UIInstanceHelper.PlayStageID);
    }

    public void ShowByStageID(int stageID)
    {
        UIMainLobby.Get.Hide(3, false);
        Show(true);

        buildChapters();

        Func<int, int> findUIStageIndex = chapter =>
        {
            var stages = StageTable.Ins.GetInstanceStagesByChapter(chapter);
            for(var i = 0; i < stages.Count; i++)
            {
                if(stages[i].ID == stageID)
                    return i;
            }
            return 0;
        };

        if(StageTable.Ins.HasByID(stageID) && 
           StageTable.Ins.GetByID(stageID).IDKind == TStageData.EKind.Instance)
        {
            TStageData stageData = StageTable.Ins.GetByID(stageID);
            mMain.SelectChapter(stageData.Chapter);
            mMain.ShowStages(stageData.Chapter);
            mMain.SelectStage(findUIStageIndex(stageData.Chapter));
        }
    }

    private void buildChapters()
    {
        mMain.ClearAllChapters();

        List<ChapterData> allData = StageChapterTable.Ins.GetAllInstance();
        foreach(ChapterData chapterData in allData)
        {
            if(!UIInstanceHelper.IsMainStagePass(chapterData.Chapter))
                continue;

            List<TStageData> normalStages = StageTable.Ins.GetInstanceNormalStagesByChapter(chapterData.Chapter);
            TStageData bossStage = StageTable.Ins.GetInstanceBossStage(chapterData.Chapter);
            UIInstanceChapter.Data uiData = UIInstanceBuilder.Build(chapterData, normalStages, bossStage);
            mMain.AddChapter(chapterData.Chapter, uiData);
        }
    }

    private void enterSelectRole(int stageID)
    {
        if(StageTable.Ins.HasByID(stageID))
        {
            TStageData stageData = StageTable.Ins.GetByID(stageID);
            UIInstanceHelper.DefaultSelectChapter = stageData.Chapter;
            UIInstanceHelper.PlayStageID = stageData.ID;
        }

        UISelectRole.Get.LoadStage(stageID);
        Hide();
    }

    public void Hide()
    {
        RemoveUI(instance.gameObject);
    }

    public static UIInstance Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIInstance;
            }
			
            return instance;
        }
    }
}