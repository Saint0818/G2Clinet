using System.Collections.Generic;
using GameEnum;
using GameStruct;
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
    private const string PlayerNextMainStageIDKey = "MainStagePlayerNextStageID";

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
        if(!PlayerPrefs.HasKey(PlayerNextMainStageIDKey))
            return false;

        if(GameData.Team.Player.NextMainStageID <= PlayerPrefs.GetInt(PlayerNextMainStageIDKey))
            return false;

        TStageData beforeData = StageTable.Ins.GetByID(PlayerPrefs.GetInt(PlayerNextMainStageIDKey));
        if(!beforeData.IsValid())
            return false;

        TStageData newData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
        if(!newData.IsValid())
            return false;

        return beforeData.Chapter < newData.Chapter;
    }

    public static bool HasNewStage()
    {
        if(!PlayerPrefs.HasKey(PlayerNextMainStageIDKey))
            return false;

        return PlayerPrefs.GetInt(PlayerNextMainStageIDKey) < GameData.Team.Player.NextMainStageID;
    }

    public static void ClearStageFlag()
    {
        if(PlayerPrefs.HasKey(PlayerNextMainStageIDKey))
        {
            PlayerPrefs.DeleteKey(PlayerNextMainStageIDKey);
            PlayerPrefs.Save();
        }
    }

    public static void Record(int chapter)
    {
        PlayerPrefs.SetInt(SelectChapterKey, chapter);
        PlayerPrefs.SetInt(PlayerNextMainStageIDKey, GameData.Team.Player.NextMainStageID);
        PlayerPrefs.Save();
    }

    public static void SetDebugNewChapter()
    {
        // 第一章通關, 第二張解鎖.
//        GameData.Team.Player.NextMainStageID = 105;
//        PlayerPrefs.SetInt(PlayerNextMainStageIDKey, 104);
//        PlayerPrefs.SetInt(SelectChapterKey, 1);

        // 主線關卡 106 過關.
        GameData.Team.Player.NextMainStageID = 107;
        PlayerPrefs.SetInt(PlayerNextMainStageIDKey, 106);
        PlayerPrefs.SetInt(SelectChapterKey, 2);
    }

    public static string FindRewardTitle(TStageData stageData)
    {
        if (!GameData.DPlayers.ContainsKey(GameData.Team.Player.ID))
            return TextConst.S(9304);

        EPlayerPostion pos = (EPlayerPostion)GameData.DPlayers[GameData.Team.Player.ID].BodyType;
        if (GameData.Team.Player.NextMainStageID == stageData.ID && stageData.HasSurelyRewards(pos))
            return TextConst.S(9310); // 文字是:必給獎勵.

        return TextConst.S(9304); // 文字是:亂數獎勵.
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stageData"></param>
    /// <returns></returns>
    public static List<TItemData> FindRewardItems(TStageData stageData)
    {
        List<TItemData> foundRewardItems = new List<TItemData>();

        if(!GameData.DPlayers.ContainsKey(GameData.Team.Player.ID))
            return foundRewardItems;

        EPlayerPostion pos = (EPlayerPostion)GameData.DPlayers[GameData.Team.Player.ID].BodyType;

        int[] items;
        if (GameData.Team.Player.NextMainStageID == stageData.ID && stageData.HasSurelyRewards(pos))
        {
            // 打最新進度的關卡, 並且有必給獎勵, 那就要顯示必給獎勵.
            items = stageData.GetSurelyRewards(pos);
        }
        else
            // 不是打最新進度, 顯示亂數獎勵.
            items = stageData.Rewards;

        for (int i = 0; i < 3; i++) // 和企劃約定好, 僅顯示前 3 個.
        {
            if (items != null && items.Length > i && GameData.DItemData.ContainsKey(items[i]))
                foundRewardItems.Add(GameData.DItemData[items[i]]);
        }
        return foundRewardItems;
    }
}