
using UnityEngine;

public static class UIInstanceHelper
{
    public static bool IsMainStagePass(int chapter)
    {
        if(!StageTable.Ins.HasMainStageByChapter(chapter))
            return false;

        TStageData lastStage = StageTable.Ins.GetLastMainStageByChapter(chapter);
        return GameData.Team.Player.NextMainStageID > lastStage.ID;
    }

    private const string DefaultSelectChapterKey = "InstanceDefaultSelectChapterKey";
    public static int DefaultSelectChapter
    {
        set { PlayerPrefs.SetInt(DefaultSelectChapterKey, value); }
        get
        {
            if(PlayerPrefs.HasKey(DefaultSelectChapterKey))
                return PlayerPrefs.GetInt(DefaultSelectChapterKey);
            return 1;
        }
    }
}
