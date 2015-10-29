using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這負責關卡的介面.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call HideAllChapters() 將整個關卡介面重置. </item>
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

//    public UILabel ChapterNumLabel;
//    public UILabel ChapterTitleLabel;
    /// <summary>
    /// Index 0: 第一章, Index 2: 第二章.
    /// </summary>
    public UIStageChapter[] Chapters; 
    public UIStageInfo Info;
//    public UIChapterChangeListener ChapterChangeListener;

    /// <summary>
    /// key: Chapter.
    /// </summary>
    private readonly Dictionary<int, UIStageChapter> mChapters = new Dictionary<int, UIStageChapter>();

    /// <summary>
    /// key: StageID.
    /// </summary>
    private readonly Dictionary<int, UIStageSmall> mStageSmalls = new Dictionary<int, UIStageSmall>();

    [UsedImplicitly]
    private void Awake()
    {
        foreach(var chapter in Chapters)
        {
            mChapters.Add(chapter.Chapter, chapter);
        }

        var stageSmalls = GetComponentsInChildren<UIStageSmall>();
        foreach(var stageSmall in stageSmalls)
        {
            mStageSmalls.Add(stageSmall.StageID, stageSmall);
        }
    }

    /// <summary>
    /// 顯示某一個章節.
    /// </summary>
    /// <param name="chapter"></param>
    public void ShowChapter(int chapter)
    {
        if(mChapters.ContainsKey(chapter))
            mChapters[chapter].Show();
        else
            Debug.LogErrorFormat("Chapter({0}) don't exist!", chapter);
    }

    /// <summary>
    /// 將某個章節鎖定.
    /// </summary>
    /// <param name="chapter"></param>
    public void ShowChapterLock(int chapter)
    {
        if(mChapters.ContainsKey(chapter))
            mChapters[chapter].ShowLock();
        else
            Debug.LogErrorFormat("Chapter({0}) don't exist!", chapter);
    }

    /// <summary>
    /// 顯示某個小關卡.
    /// </summary>
    /// <param name="stageID"></param>
    /// <param name="data"></param>
    public void ShowStage(int stageID, UIStageInfo.Data data)
    {
        if(mStageSmalls.ContainsKey(stageID))
            mStageSmalls[stageID].Show(data);
        else
            Debug.LogErrorFormat("Stage({0}) don't exist!", stageID);
    }

    /// <summary>
    /// 某個小關卡鎖定.
    /// </summary>
    /// <param name="stageID"></param>
    /// <param name="kindSpriteName"></param>
    public void ShowStageLock(int stageID, string kindSpriteName)
    {
        if(mStageSmalls.ContainsKey(stageID))
            mStageSmalls[stageID].ShowLock(kindSpriteName);
        else
            Debug.LogErrorFormat("Stage({0}) don't exist!", stageID);
    }

    /// <summary>
    /// 全部章節重置.
    /// </summary>
    public void HideAllChapters()
    {
        foreach(var chapter in Chapters)
        {
            chapter.Hide();
        }
    }

    public void OnBackClick()
    {
        if(BackListener != null)
            BackListener();
    }
}