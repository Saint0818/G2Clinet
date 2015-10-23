using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 記錄關卡的相關資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得關卡資料; Call HasXXX 檢查關卡資料. </item>
/// </list>
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
    /// key: StageID. 記錄主線內, 全部的小關卡.
    /// </summary>
    private readonly Dictionary<int, StageData> mStageByIDs = new Dictionary<int, StageData>();

    /// <summary>
    /// [ChapterID, [Order, instance]]. 記錄主線內, 全部的小關卡.
    /// </summary>
    private readonly Dictionary<int, Dictionary<int, StageData>> mStageByChapterOrders = new Dictionary<int, Dictionary<int, StageData>>();

    private StageTable() {}

    public void Load(string jsonText)
    {
        clear();

        var stages = (StageData[])JsonConvert.DeserializeObject(jsonText, typeof(StageData[]));
        foreach(StageData stage in stages)
        {
            if(mStageByIDs.ContainsKey(stage.ID))
            {
                Debug.LogErrorFormat("Stage ID repeat. {0}", stage.ID);
                continue;
            }

            if(MinMainStageID <= stage.ID && stage.ID <= MaxMainStageID)
            {
                if(!mStageByChapterOrders.ContainsKey(stage.Chapter))
                    mStageByChapterOrders.Add(stage.Chapter, new Dictionary<int, StageData>());
                if (mStageByChapterOrders[stage.Chapter].ContainsKey(stage.Order))
                {
                    Debug.LogErrorFormat("Stage Order repeat. {0}", stage);
                    continue;
                }
                mStageByChapterOrders[stage.Chapter].Add(stage.Order, stage);
                mStageByIDs.Add(stage.ID, stage);
            }
            else
                Debug.LogErrorFormat("StageID({0}) out of range!", stage.ID);
        }

        Debug.Log("[stage parsed finished.] ");
    }

    private void clear()
    {
        mStageByIDs.Clear();
        mStageByChapterOrders.Clear();
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
        return mStageByIDs.ContainsKey(id);
    }

    public StageData GetByID(int id)
    {
        if(mStageByIDs.ContainsKey(id))
            return mStageByIDs[id];

        return mEmptyStage;
    }
}