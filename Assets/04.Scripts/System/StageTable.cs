using System.Collections.Generic;
using GameEnum;
using Newtonsoft.Json;
using UnityEngine;

public struct Stage
{
    public int ID;
    public int Chapter;
    public int Order;

    /// <summary>
    /// 1.傳統
    /// 2.得分
    /// 3.防守
    /// 4.攻擊
    /// 9.挑戰魔王對手
    /// </summary>
    public int Kind;
     
    public string Hint;
    public int Bit0Num;
    public int Bit1Num;
    public int Bit2Num;
    public int Bit3Num;
    public int CourtMode;
    public int WinMode;
    public int WinValue;
    public int FriendNumber;

    public int PlayerID1;
    public int PlayerID2;
    public int PlayerID3;
    public int PlayerID4;
    public int PlayerID5;
    public string NameTW;
    public string NameCN;
    public string NameEN;
    public string NameJP;
    public string ExplainTW;
    public string ExplainCN;
    public string ExplainEN;
    public string ExplainJP;

    public bool IsValid()
    {
        return ID >= 1;
    }

    public override string ToString()
    {
        return string.Format("ID: {0}, Chapter: {1}, Order: {2}", ID, Chapter, Order);
    }

    public int[] HintBit
    {
        get
        {
            return AI.BitConverter.Convert(Hint);
        }
    }

    public string Name
    {
        get
        {
            switch(GameData.Setting.Language)
            {
                case ELanguage.TW: return NameTW;
                case ELanguage.CN: return NameCN;
                case ELanguage.JP: return NameJP;
                default : return NameEN;
            }
        }
    }

    public string Explain
    {
        get
        {
            switch(GameData.Setting.Language)
            {
                case ELanguage.TW: return ExplainTW;
                case ELanguage.CN: return ExplainCN;
                case ELanguage.JP: return ExplainJP;
                default : return ExplainEN;
            }
        }
    }
}

/// <summary>
/// 
/// </summary>
public class StageTable
{
    /// <summary>
    /// 主線關卡 ID 的範圍.
    /// </summary>
    public const int MinMainStageID = 101;
    public const int MaxMainStageID = 2000;

    private static readonly StageTable INSTANCE = new StageTable();
    public static StageTable Ins
    {
        get { return INSTANCE; }
    }

    /// <summary>
    /// key: StageID.
    /// </summary>
    private readonly Dictionary<int, Stage> mStageIDs = new Dictionary<int, Stage>();

    /// <summary>
    /// [ChapterID, [Order, instance]].
    /// </summary>
    private readonly Dictionary<int, Dictionary<int, Stage>> mStageChapters = new Dictionary<int, Dictionary<int, Stage>>();

    private static Stage EmptyStage;
    private readonly List<Stage> EmptyStages = new List<Stage>();

    public void Load(string jsonText)
    {
        clear();

        var stages = (Stage[])JsonConvert.DeserializeObject(jsonText, typeof(Stage[]));
        foreach(Stage stage in stages)
        {
            if(mStageIDs.ContainsKey(stage.ID))
            {
                Debug.LogErrorFormat("Stage ID repeat. {0}", stage.ID);
                continue;
            }
            
            if(!mStageChapters.ContainsKey(stage.Chapter))
                mStageChapters.Add(stage.Chapter, new Dictionary<int, Stage>());
            if(mStageChapters[stage.Chapter].ContainsKey(stage.Order))
            {
                Debug.LogErrorFormat("Stage Order repeat. {0}", stage);
                continue;
            }

            mStageIDs.Add(stage.ID, stage);
            mStageChapters[stage.Chapter].Add(stage.Order, stage);
        }

        Debug.Log("[stage parsed finished.] ");
    }

    private void clear()
    {
        mStageIDs.Clear();
        mStageChapters.Clear();
    }

    public bool HasByChapterOrder(int chapter, int order)
    {
        if(mStageChapters.ContainsKey(chapter) && mStageChapters[chapter].ContainsKey(order))
            return true;

        return false;
    }

    public Stage GetByChapterOrder(int chapter, int order)
    {
        if(!mStageChapters.ContainsKey(chapter) || !mStageChapters[chapter].ContainsKey(order))
            return EmptyStage;

        return mStageChapters[chapter][order];
    }

    public bool HasByChapter(int chapter)
    {
        return mStageChapters.ContainsKey(chapter);
    }

    public List<Stage> GetByChapter(int chapter)
    {
        EmptyStages.Clear();
        if(mStageChapters.ContainsKey(chapter))
        {
            foreach(KeyValuePair<int, Stage> pair in mStageChapters[chapter])
            {
                EmptyStages.Add(pair.Value);
            }
        }
        
        return EmptyStages;
    }

    public bool HasByID(int id)
    {
        return mStageIDs.ContainsKey(id);
    }

    public Stage GetByID(int id)
    {
        if(mStageIDs.ContainsKey(id))
            return mStageIDs[id];

        return EmptyStage;
    }
}