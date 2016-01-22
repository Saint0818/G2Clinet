using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    private static readonly StageTable INSTANCE = new StageTable();
    public static StageTable Ins
    {
        get { return INSTANCE; }
    }

    /// <summary>
    /// 記錄全部的關卡資訊.
    /// </summary>
    private readonly Dictionary<int, TStageData> mAllStagesByID = new Dictionary<int, TStageData>();

    /// <summary>
    /// 主線關卡企劃資料中, 目前最大的章節.
    /// </summary>
    public int MainStageMaxChapter { get; private set; }

    /// <summary>
    /// key: StageID. 主線全部小關卡.
    /// </summary>
    private readonly Dictionary<int, TStageData> mMainStagesByID = new Dictionary<int, TStageData>();

    /// <summary>
    /// key: 章節, 1: 第一章, 2 第二章. 主線某個章節的小關卡.
    /// </summary>
    private readonly Dictionary<int, List<TStageData>> mMainStagesByChapter = new Dictionary<int, List<TStageData>>();

    /// <summary>
    /// key: StageID, 副本全部的小關卡.
    /// </summary>
    private readonly Dictionary<int, TStageData> mInstanceByID = new Dictionary<int, TStageData>();

    /// <summary>
    /// key: 章節, 1: 第一章, 2 第二章. 副本某個章節的小關卡.
    /// </summary>
    private readonly Dictionary<int, List<TStageData>> mInstanceByChapter = new Dictionary<int, List<TStageData>>();

    private StageTable() {}

    public void Load(string jsonText)
    {
        clear();

        // 刪除 ["] 字元.
        jsonText = jsonText.Replace("\"[", "[");
        jsonText = jsonText.Replace("]\"", "]");
        jsonText = jsonText.Replace("\"{", "{");
        jsonText = jsonText.Replace("}\"", "}");

        var stages = JsonConvertWrapper.DeserializeObject<TStageData[]>(jsonText);
        foreach(TStageData stage in stages)
        {
            if(mAllStagesByID.ContainsKey(stage.ID))
            {
                Debug.LogErrorFormat("Stage ID repeat. {0}", stage.ID);
                continue;
            }
            mAllStagesByID.Add(stage.ID, stage);

            if(TStageData.MinMainStageID <= stage.ID && stage.ID <= TStageData.MaxMainStageID)
                addMainStage(stage);
            else if(TStageData.MinInstanceID <= stage.ID && stage.ID <= TStageData.MaxInstanceID)
                addInstance(stage);
        }

        Debug.Log("[stage parsed finished.] ");
    }

    private void addMainStage(TStageData stage)
    {
        mMainStagesByID.Add(stage.ID, stage);

        if(!mMainStagesByChapter.ContainsKey(stage.Chapter))
            mMainStagesByChapter.Add(stage.Chapter, new List<TStageData>());
        mMainStagesByChapter[stage.Chapter].Add(stage);

        if(MainStageMaxChapter < stage.Chapter)
            MainStageMaxChapter = stage.Chapter;
    }

    private void addInstance(TStageData stage)
    {
        mInstanceByID.Add(stage.ID, stage);

        if(!mInstanceByChapter.ContainsKey(stage.Chapter))
            mInstanceByChapter.Add(stage.Chapter, new List<TStageData>());
        mInstanceByChapter[stage.Chapter].Add(stage);
    }

    private void clear()
    {
        mMainStagesByID.Clear();
        mMainStagesByChapter.Clear();
        MainStageMaxChapter = 0;
    }

    private readonly TStageData mEmptyStage = new TStageData();

    public bool HasByChapter(int chapter)
    {
        return mMainStagesByChapter.ContainsKey(chapter);
    }

    public void GetMainStageByChapterRange(int minChapter, int maxChapter, ref List<TStageData> data)
    {
        Assert.IsTrue(maxChapter >= minChapter, string.Format("range error:[{0}, {1}]", minChapter, maxChapter));

        data.Clear();
        for(int chapter = minChapter; chapter <= maxChapter; chapter++)
        {
            if(mMainStagesByChapter.ContainsKey(chapter))
                data.AddRange(mMainStagesByChapter[chapter]);
        }
    }

    public List<TStageData> GetInstanceStagesByChapter(int chapter)
    {
        var oneChapterStages = new List<TStageData>();
        if(mInstanceByChapter.ContainsKey(chapter))
            oneChapterStages.AddRange(mInstanceByChapter[chapter]);

        oneChapterStages.Sort((stage1, stage2) => // sort by order.(由低到高)
        {
            if (stage1.Order == stage2.Order)
                return 0;
            if (stage1.Order > stage2.Order)
                return 1;
            return -1;
        });

        return oneChapterStages;
    }

    public List<TStageData> GetInstanceNormalStagesByChapter(int chapter)
    {
        List<TStageData> normalStages = GetInstanceStagesByChapter(chapter);

        if(normalStages.Count >= 1)
            normalStages.RemoveAt(normalStages.Count - 1); // 刪除最後 1 個.

        return normalStages;
    }

    public TStageData GetInstanceBossStage(int chapter)
    {
        List<TStageData> stages = GetInstanceStagesByChapter(chapter);
        if(stages.Count == 0)
            return mEmptyStage;
        return stages[stages.Count - 1]; // 最後一個關卡就是 order 最大的關卡.
    }

    public bool HasByID(int id)
    {
        return mAllStagesByID.ContainsKey(id);
    }

    public TStageData GetByID(int id)
    {
        if(mAllStagesByID.ContainsKey(id))
            return mAllStagesByID[id];

        mEmptyStage.Clear();
        return mEmptyStage;
    }
}