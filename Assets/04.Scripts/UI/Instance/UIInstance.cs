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
        Show(true);

        buildChapters();
    }

    public void ShowByChapter(int chapter)
    {
        Show(true);

        buildChapters();
        mMain.SelectChapter(chapter);
    }

    private void buildChapters()
    {
        mMain.ClearAllChapters();

        List<ChapterData> allData = StageChapterTable.Ins.GetAllInstance();
        foreach(ChapterData chapterData in allData)
        {
            if(!isMainStagePass(chapterData.Chapter))
                continue;

            List<TStageData> normalStages = StageTable.Ins.GetInstanceNormalStagesByChapter(chapterData.Chapter);
            TStageData bossStage = StageTable.Ins.GetInstanceBossStage(chapterData.Chapter);
            UIInstanceChapter.Data uiData = UIInstanceBuilder.Build(chapterData, normalStages, bossStage);
            mMain.AddChapter(uiData);
        }

        mMain.ShowChapters();
    }

    private bool isMainStagePass(int chapter)
    {
        if(!StageTable.Ins.HasMainStageByChapter(chapter))
            return false;

        TStageData lastStage = StageTable.Ins.GetLastMainStageByChapter(chapter);
        return GameData.Team.Player.NextMainStageID > lastStage.ID;
    }

    private void enterSelectRole(int stageID)
    {
        UISelectRole.Get.LoadStage(stageID);
        Hide();
    }

    public void Hide()
    {
        RemoveUI(UIName);
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