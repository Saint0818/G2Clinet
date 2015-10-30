using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 記錄章節的說明資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Ins 取得 instance. </item>
/// <item> Call Get 取得關卡章節資料; Call Has 檢查關卡章節資料. </item>
/// </list>
public class ChapterTable
{
    /// <summary>
    /// 主線章節 ID 範圍.
    /// </summary>
    public const int MinChapterID = 1;
    public const int MaxChapterID = 100;

    private static readonly ChapterTable INSTANCE = new ChapterTable();
    public static ChapterTable Ins
    {
        get { return INSTANCE; }
    }

    /// <summary>
    /// key: Chapter Value. 1: 第一章, 2: 第二章.
    /// </summary>
    private readonly Dictionary<int, ChapterData> mChapters = new Dictionary<int, ChapterData>();

    private ChapterTable() {}

    public void Load(string jsonText)
    {
        clear();

        var chapters = (ChapterData[])JsonConvert.DeserializeObject(jsonText, typeof(ChapterData[]));
        foreach(ChapterData chapter in chapters)
        {
            if(mChapters.ContainsKey(chapter.Chapter))
            {
                Debug.LogErrorFormat("Chapter repeat. {0}", chapter);
                continue;
            }

            if(MinChapterID <= chapter.ID && chapter.ID <= MaxChapterID)
                mChapters.Add(chapter.Chapter, chapter);
            else
                Debug.LogErrorFormat("StageID({0}) out of range!", chapter.ID);
        }

        Debug.Log("[stagechapter parsed finished.] ");
    }

    private void clear()
    {
        mChapters.Clear();
    }

    private readonly ChapterData mEmptyChapter = new ChapterData();

    public bool Has(int chapter)
    {
        return mChapters.ContainsKey(chapter);
    }

    public ChapterData Get(int chapter)
    {
        if(mChapters.ContainsKey(chapter))
            return mChapters[chapter];

        return mEmptyChapter;
    }
}