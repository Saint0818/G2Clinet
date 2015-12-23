using UnityEngine;

public static class UIMainStageTools
{
    /// <summary>
    /// 記錄介面開啟時, 要顯示哪一個章節. Value(int): 哪一章.
    /// </summary>
    private const string SelectChapterKey = "MainStageSelectChapter";

    /// <summary>
    /// 記錄玩家上次打關卡時, 玩家的關卡進度. Value(int): GameData.Team.Player.NextMainStageID.
    /// </summary>
    private const string PlayerNextStageIDKey = "MainStagePlayerNextStageID";

    public static bool HasSelectChapter()
    {
        return PlayerPrefs.HasKey(SelectChapterKey);
    }

    public static int GetSelectChapter()
    {
        return PlayerPrefs.GetInt(SelectChapterKey);
    }

    /// <summary>
    /// 不要關卡介面顯示上次玩家打的章節, 而是直接顯示最新關卡的章節.
    /// </summary>
    public static void ClearSelectChapter()
    {
        if (PlayerPrefs.HasKey(SelectChapterKey))
        {
            PlayerPrefs.DeleteKey(SelectChapterKey);
            PlayerPrefs.Save();
        }
    }

    public static bool HasNewChapter()
    {
        if(!PlayerPrefs.HasKey(PlayerNextStageIDKey))
            return false;

        if(GameData.Team.Player.NextMainStageID <= PlayerPrefs.GetInt(PlayerNextStageIDKey))
            return false;

        TStageData beforeData = StageTable.Ins.GetByID(PlayerPrefs.GetInt(PlayerNextStageIDKey));
        if(!beforeData.IsValid())
            return false;

        TStageData newData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
        if(!newData.IsValid())
            return false;

        return beforeData.Chapter < newData.Chapter;
    }

    public static bool HasNewStage()
    {
        if(!PlayerPrefs.HasKey(PlayerNextStageIDKey))
            return false;

        return PlayerPrefs.GetInt(PlayerNextStageIDKey) < GameData.Team.Player.NextMainStageID;
    }

    public static void ClearStageFlag()
    {
        if(PlayerPrefs.HasKey(PlayerNextStageIDKey))
        {
            PlayerPrefs.DeleteKey(PlayerNextStageIDKey);
            PlayerPrefs.Save();
        }
    }

    public static void Record(int chapter)
    {
        PlayerPrefs.SetInt(SelectChapterKey, chapter);
        PlayerPrefs.SetInt(PlayerNextStageIDKey, GameData.Team.Player.NextMainStageID);
        PlayerPrefs.Save();
    }

    public static void SetDebugParameters()
    {
        GameData.Team.Player.NextMainStageID = 109;
        PlayerPrefs.SetInt(PlayerNextStageIDKey, 108);
        PlayerPrefs.SetInt(SelectChapterKey, 2);
    }
}