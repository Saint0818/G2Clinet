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

    private static readonly StageTable INSTANCE = new StageTable();
    public static StageTable Ins
    {
        get { return INSTANCE; }
    }

    /// <summary>
    /// 主線關卡企劃資料中, 目前最大的章節.
    /// </summary>
    public int MainStageMaxChapter { get; private set; }

    /// <summary>
    /// key: StageID. 主線全部小關卡.
    /// </summary>
    private readonly Dictionary<int, TStageData> mStageByIDs = new Dictionary<int, TStageData>();

    /// <summary>
    /// key: 章節, 1: 第一章, 2 第二章. 某個章節的小關卡.
    /// </summary>
    private readonly Dictionary<int, List<TStageData>> mStageByChapters = new Dictionary<int, List<TStageData>>();

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
            if(mStageByIDs.ContainsKey(stage.ID))
            {
                Debug.LogErrorFormat("Stage ID repeat. {0}", stage.ID);
                continue;
            }
            mStageByIDs.Add(stage.ID, stage);

            if(!mStageByChapters.ContainsKey(stage.Chapter))
                mStageByChapters.Add(stage.Chapter, new List<TStageData>());
            mStageByChapters[stage.Chapter].Add(stage);

            if(MainStageMaxChapter < stage.Chapter)
                MainStageMaxChapter = stage.Chapter;
        }

        Debug.Log("[stage parsed finished.] ");
    }

    private void clear()
    {
        mStageByIDs.Clear();
        mStageByChapters.Clear();
        MainStageMaxChapter = 0;
    }

    private readonly TStageData mEmptyStage = new TStageData();
//    private readonly List<StageData> mEmptyStages = new List<StageData>();

    public bool HasByChapter(int chapter)
    {
        return mStageByChapters.ContainsKey(chapter);
    }

    public void GetByChapterRange(int minChapter, int maxChapter, ref List<TStageData> data)
    {
        Assert.IsTrue(maxChapter >= minChapter, string.Format("range error:[{0}, {1}]", minChapter, maxChapter));

        data.Clear();
        for(int chapter = minChapter; chapter <= maxChapter; chapter++)
        {
            if(mStageByChapters.ContainsKey(chapter))
                data.AddRange(mStageByChapters[chapter]);
        }
    }

    public bool HasByID(int id)
    {
        return mStageByIDs.ContainsKey(id);
    }

    public TStageData GetByID(int id)
    {
        if(mStageByIDs.ContainsKey(id))
            return mStageByIDs[id];

        mEmptyStage.Clear();
        return mEmptyStage;
    }
}