using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得關卡資料; Call HasXXX 檢查關卡資料. </item>
/// </list>
public class StageTable
{
    /// <summary>
    /// 主線關卡說明的 ID 範圍.
    /// </summary>
    public const int MinMainStageDescID = 1;
    public const int MaxMainStageDescID = 100;

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
    /// key: StageID. 記錄主線內, 全部的關卡(小關卡資訊和章節說明).
    /// </summary>
    private readonly Dictionary<int, StageData> mAllStageByIDs = new Dictionary<int, StageData>();

    /// <summary>
    /// [ChapterID, [Order, instance]]. 記錄主線內, 全部的小關卡.
    /// </summary>
    private readonly Dictionary<int, Dictionary<int, StageData>> mStageByChapterOrders = new Dictionary<int, Dictionary<int, StageData>>();

    /// <summary>
    /// Key: ChapterID. 記錄章節說明.
    /// </summary>
    private readonly Dictionary<int, StageData> mChapterDescs = new Dictionary<int, StageData>();

    private StageTable() {}

    public void Load(string jsonText)
    {
        clear();

        var stages = (StageData[])JsonConvert.DeserializeObject(jsonText, typeof(StageData[]));
        foreach(StageData stage in stages)
        {
            if(mAllStageByIDs.ContainsKey(stage.ID))
            {
                Debug.LogErrorFormat("Stage ID repeat. {0}", stage.ID);
                continue;
            }

            if(MinMainStageID <= stage.ID && stage.ID <= MaxMainStageID)
            {
                // 小關卡.
                if(!mStageByChapterOrders.ContainsKey(stage.Chapter))
                    mStageByChapterOrders.Add(stage.Chapter, new Dictionary<int, StageData>());
                if (mStageByChapterOrders[stage.Chapter].ContainsKey(stage.Order))
                {
                    Debug.LogErrorFormat("Stage Order repeat. {0}", stage);
                    continue;
                }
                mStageByChapterOrders[stage.Chapter].Add(stage.Order, stage);
            }
            else if(MinMainStageDescID <= stage.ID && stage.ID <= MaxMainStageDescID)
            {
                // 章節說明.
                if(mChapterDescs.ContainsKey(stage.Chapter))
                {
                    Debug.LogErrorFormat("Stage Chapter repeat. {0}", stage);
                    continue;
                }
                mChapterDescs.Add(stage.Chapter, stage);
            }
            else
            {
                Debug.LogErrorFormat("StageID({0}) out of range!", stage.ID);
                continue;
            }

            mAllStageByIDs.Add(stage.ID, stage);
        }

        Debug.Log("[stage parsed finished.] ");
    }

    private void clear()
    {
        mAllStageByIDs.Clear();
        mStageByChapterOrders.Clear();
        mChapterDescs.Clear();
    }

    public bool HasByChapterOrder(int chapter, int order)
    {
        if(mStageByChapterOrders.ContainsKey(chapter) && mStageByChapterOrders[chapter].ContainsKey(order))
            return true;

        return false;
    }

    private readonly StageData mEmptyStage = new StageData();
    private readonly List<StageData> mEmptyStages = new List<StageData>();
    public StageData GetByChapterOrder(int chapter, int order)
    {
        if(!mStageByChapterOrders.ContainsKey(chapter) || !mStageByChapterOrders[chapter].ContainsKey(order))
            return mEmptyStage;

        return mStageByChapterOrders[chapter][order];
    }

    public bool HasByChapter(int chapter)
    {
        return mStageByChapterOrders.ContainsKey(chapter);
    }

    public List<StageData> GetByChapter(int chapter)
    {
        mEmptyStages.Clear();
        if(mStageByChapterOrders.ContainsKey(chapter))
        {
            foreach(KeyValuePair<int, StageData> pair in mStageByChapterOrders[chapter])
            {
                mEmptyStages.Add(pair.Value);
            }
        }
        
        return mEmptyStages;
    }

    public bool HasByID(int id)
    {
        return mAllStageByIDs.ContainsKey(id);
    }

    public StageData GetByID(int id)
    {
        if(mAllStageByIDs.ContainsKey(id))
            return mAllStageByIDs[id];

        return mEmptyStage;
    }

    public bool HasChapterDesc(int chapterID)
    {
        return mChapterDescs.ContainsKey(chapterID);
    }

    public StageData GetChapterDesc(int chapterID)
    {
        if(mChapterDescs.ContainsKey(chapterID))
            return mChapterDescs[chapterID];

        return mEmptyStage;
    }
}