
public static class UIInstanceHelper
{
    public static bool IsMainStagePass(int chapter)
    {
        if(!StageTable.Ins.HasMainStageByChapter(chapter))
            return false;

        TStageData lastStage = StageTable.Ins.GetLastMainStageByChapter(chapter);
        return GameData.Team.Player.NextMainStageID > lastStage.ID;
    }
}
