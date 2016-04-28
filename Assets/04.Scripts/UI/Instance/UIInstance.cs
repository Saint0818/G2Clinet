using System;
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

        mMain.StageStartListener += (stageID, errorCode, errorMsg) =>
        {
            switch(errorCode)
            {
                case UIStageVerification.EErrorCode.Pass:
                    enterSelectRole(stageID);
                    break;
                case UIStageVerification.EErrorCode.NoPower:
                    OnBuyPower();
                    break;
                case UIStageVerification.EErrorCode.NoDailyChallenge:
                    var protocol = new ResetStageDailyChallengeProtocol();
                    protocol.Send(stageID, ok => 
                    {
                        if(ok)
                        {
                            ShowByStageID(stageID);
                            UIMainLobby.Get.UpdateUI();
                        }
                    });
                    break;
                default:
                    Debug.LogWarning(errorMsg);
                        UIHint.Get.ShowHint(errorMsg, Color.red);
                    break;
            }
        };
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

        Statistic.Ins.LogScreen(10);
        Statistic.Ins.LogEvent(62);
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
        UIMainLobby.Get.Hide(false);
        UIResource.Get.Show();
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

        Statistic.Ins.LogScreen(10);
        Statistic.Ins.LogEvent(62);
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