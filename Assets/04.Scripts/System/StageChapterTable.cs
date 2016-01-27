using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 記錄章節的說明資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call GetXXX 取得章節資料; Call HasXXX 檢查章節資料. </item>
/// </list>
public class StageChapterTable
{
    /// <summary>
    /// 主線關卡章節 ID 範圍.
    /// </summary>
    public const int MinChapterID = 1;
    public const int MaxChapterID = 100;

    /// <summary>
    /// 副本章節 ID 範圍.
    /// </summary>
    public const int MinInstanceID = 2001;
    public const int MaxInstanceID = 2100;

    private static readonly StageChapterTable INSTANCE = new StageChapterTable();
    public static StageChapterTable Ins
    {
        get { return INSTANCE; }
    }

    /// <summary>
    /// 主線關卡章節. key: Chapter Value. 1: 第一章, 2: 第二章.
    /// </summary>
    private readonly Dictionary<int, ChapterData> mMainChapters = new Dictionary<int, ChapterData>();

    /// <summary>
    /// 副本章節. key: Chapter Value. 1: 第一章, 2: 第二章.
    /// </summary>
    private readonly Dictionary<int, ChapterData> mInstanceChapters = new Dictionary<int, ChapterData>();

    private StageChapterTable() {}

    public void Load(string jsonText)
    {
        clear();

        var chapters = (ChapterData[])JsonConvert.DeserializeObject(jsonText, typeof(ChapterData[]));
        foreach(ChapterData data in chapters)
        {
            if(MinChapterID <= data.ID && data.ID <= MaxChapterID)
                AddMainChapter(data);
            else if(MinInstanceID <= data.ID && data.ID <= MaxInstanceID)
                AddInstance(data);
            else
                Debug.LogErrorFormat("Chapter ID({0}) out of range!", data.ID);
        }

        Debug.Log("[stagechapter parsed finished.] ");
    }

    private void clear()
    {
        mMainChapters.Clear();
    }

    private void AddMainChapter(ChapterData data)
    {
        if (mMainChapters.ContainsKey(data.Chapter))
        {
            Debug.LogErrorFormat("Chapter repeat. {0}", data);
            return;
        }

        mMainChapters.Add(data.Chapter, data);
    }

    private void AddInstance(ChapterData data)
    {
        if(mInstanceChapters.ContainsKey(data.Chapter))
        {
            Debug.LogErrorFormat("Chapter repeat. {0}", data);
            return;
        }

        mInstanceChapters.Add(data.Chapter, data);
    }

    private readonly ChapterData mEmptyChapter = new ChapterData();

    public bool HasMain(int chapter)
    {
        return mMainChapters.ContainsKey(chapter);
    }

    public ChapterData GetMain(int chapter)
    {
        if(mMainChapters.ContainsKey(chapter))
            return mMainChapters[chapter];

        return mEmptyChapter;
    }

    public bool HasInstance(int chapter)
    {
        return mInstanceChapters.ContainsKey(chapter);
    }

    public ChapterData GetInstance(int chapter)
    {
        if(mInstanceChapters.ContainsKey(chapter))
            return mInstanceChapters[chapter];

        return mEmptyChapter;
    }

    public List<ChapterData> GetAllInstance()
    {
        List<ChapterData> data = new List<ChapterData>();
        foreach(KeyValuePair<int, ChapterData> pair in mInstanceChapters)
        {
            data.Add(pair.Value);
        }

        return data;
    }
}