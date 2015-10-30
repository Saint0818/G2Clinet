using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 關卡介面主程式.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call RemoveAllChapters() 將整個關卡介面重置. </item>
/// <item> Call ShowXXX() 設定顯示哪些章節和關卡. </item>
/// <item> 向 UIStageInfo 註冊事件. </item>
/// </list>
[DisallowMultipleComponent]
public class UIMainStageImpl : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 返回按鈕按下時.
    /// </summary>
    public event CommonDelegateMethods.Action BackListener;

    /// <summary>
    /// Index 0: 第一章, Index 2: 第二章.
    /// </summary>
//    public UIStageChapter[] Chapters;
    public UIStageInfo Info;
//    public UIChapterChangeListener ChapterChangeListener;
    public Transform ChapterParent;

    private readonly Vector3 mChapterStartPos = new Vector3(-70, 0, 0);
    private readonly string ChapterPath = "Prefab/UI/UIStageChapter";
    

    /// <summary>
    /// key: Chapter.
    /// </summary>
    private readonly Dictionary<int, UIStageChapter> mChapters = new Dictionary<int, UIStageChapter>();

//    /// <summary>
//    /// key: StageID.
//    /// </summary>
//    private readonly Dictionary<int, UIStageSmall> mStageSmalls = new Dictionary<int, UIStageSmall>();

    [UsedImplicitly]
    private void Awake()
    {
//        foreach(var chapter in Chapters)
//        {
//            mChapters.Add(chapter.Chapter, chapter);
//        }

//        var stageSmalls = GetComponentsInChildren<UIStageSmall>();
//        foreach(var stageSmall in stageSmalls)
//        {
//            mStageSmalls.Add(stageSmall.StageID, stageSmall);
//        }
    }

    /// <summary>
    /// 顯示某一個章節.
    /// </summary>
    /// <param name="chapter"> 1: 第一章, 2: 第二章. </param>
    /// <param name="title"></param>
    public void ShowChapter(int chapter, string title)
    {
        if(!mChapters.ContainsKey(chapter))
            mChapters.Add(chapter, createChapter(chapter, title));
        mChapters[chapter].Show();
    }

    /// <summary>
    /// 將某個章節鎖定.
    /// </summary>
    /// <param name="chapter"> 1: 第一章, 2: 第二章. </param>
    /// <param name="title"></param>
    public void ShowChapterLock(int chapter, string title)
    {
        if(!mChapters.ContainsKey(chapter))
            mChapters.Add(chapter, createChapter(chapter, title));
        mChapters[chapter].ShowLock();
    }

    private UIStageChapter createChapter(int chapter, string title)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(ChapterPath));
        obj.transform.parent = ChapterParent;
        obj.transform.localPosition = findChapterPos(chapter, obj.GetComponent<UITexture>().width);
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        var stageChapter = obj.GetComponent<UIStageChapter>();
        stageChapter.Chapter = chapter;
        stageChapter.Title = title;
        return stageChapter;
    }

    private Vector3 findChapterPos(int chapter, int chapterWidth)
    {
        var localPos = mChapterStartPos;
        localPos.x = mChapterStartPos.x + (chapter - 1) * chapterWidth;
        return localPos;
    }

    /// <summary>
    /// 顯示某個小關卡.
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="stageID"></param>
    /// <param name="localPos"></param>
    /// <param name="data"></param>
    public void ShowStage(int chapter, int stageID, Vector3 localPos, UIStageInfo.Data data)
    {
        if(mChapters.ContainsKey(chapter))
            mChapters[chapter].ShowStage(stageID, localPos, data);
        else
            Debug.LogErrorFormat("Chapter({0}) don't exist, you need call ShowChapter() first.", chapter);
    }

    /// <summary>
    /// 某個小關卡鎖定.
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="stageID"></param>
    /// <param name="localPos"></param>
    /// <param name="kindSpriteName"></param>
    public void ShowStageLock(int chapter, int stageID, Vector3 localPos, string kindSpriteName)
    {
        if(mChapters.ContainsKey(chapter))
            mChapters[chapter].ShowStageLock(stageID, localPos, kindSpriteName);
        else
            Debug.LogErrorFormat("Chapter({0}) don't exist, you need call ShowChapter() first.", chapter);
    }

    /// <summary>
    /// 全部章節重置.
    /// </summary>
    public void RemoveAllChapters()
    {
        foreach(KeyValuePair<int, UIStageChapter> pair in mChapters)
        {
            Destroy(pair.Value.gameObject);
        }

        mChapters.Clear();
//        mStageSmalls.Clear();
    }

    public void OnBackClick()
    {
        if(BackListener != null)
            BackListener();
    }
}