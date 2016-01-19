using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

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

    /// <summary>
    /// 副本的 ID 範圍.
    /// </summary>
    public const int MinInstanceID = 2001;
    public const int MaxInstanceID = 4000;

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

            if(MinMainStageID <= stage.ID && stage.ID <= MaxMainStageID)
                addMainStage(stage);
            else if(MinInstanceID <= stage.ID && stage.ID <= MaxInstanceID)
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

    public void GetInstanceByChapterRange(int minChapter, int maxChapter, ref List<TStageData> data)
    {
        Assert.IsTrue(maxChapter >= minChapter, string.Format("range error:[{0}, {1}]", minChapter, maxChapter));

        data.Clear();
        for(int chapter = minChapter; chapter <= maxChapter; chapter++)
        {
            if(mInstanceByChapter.ContainsKey(chapter))
                data.AddRange(mInstanceByChapter[chapter]);
        }
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