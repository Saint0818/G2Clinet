﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 關卡介面主程式.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call RemoveAllChapters() 將整個關卡介面重置. </item>
/// <item> Call AddXXX() 加入章節和關卡. </item>
/// <item> Call ScrollToChapter() 控制預設顯示哪一個章節. </item>
/// <item> 向 UIStageInfo 註冊事件. </item>
/// </list>
[DisallowMultipleComponent]
public class UIMainStageMain : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 返回按鈕按下時.
    /// </summary>
    public event CommonDelegateMethods.Action BackListener;

    /// <summary>
    /// Index 0: 第一章, Index 2: 第二章.
    /// </summary>
    public UIStageInfo Info
    {
        get { return mInfo ?? (mInfo = GetComponent<UIStageInfo>()); }
    }
    private UIStageInfo mInfo;

    public Transform ChapterParent;
    public UIScrollView ScrollView;

    private readonly Vector3 mChapterStartPos = new Vector3(-70, 0, 0);

    /// <summary>
    /// 單位: Pixel.
    /// </summary>
    private int mChapterWidth;

    /// <summary>
    /// 有幾個章節.
    /// </summary>
    public int ChapterCount
    {
        get { return mChapters.Count; }
    }
    /// <summary>
    /// key: Chapter. (注意: 0 的意思是第 0 章, 但以目前的設計來說, 沒有第 0 章)
    /// </summary>
    private readonly Dictionary<int, UIStageChapter> mChapters = new Dictionary<int, UIStageChapter>();

    [UsedImplicitly]
    private void Awake()
    {
    }

    /// <summary>
    /// 顯示某一個章節.
    /// </summary>
    /// <param name="chapter"> 1: 第一章, 2: 第二章. </param>
    /// <param name="title"></param>
    public void AddChapter(int chapter, string title)
    {
        if(!mChapters.ContainsKey(chapter))
            mChapters.Add(chapter, createChapter(chapter, title));
        mChapters[chapter].Show();

        // 其實每個章節的大小都相同, 所以我這邊用稍微取巧的方法取值.
        if(mChapterWidth <= 0)
            mChapterWidth = mChapters[chapter].GetComponent<UITexture>().width;
    }

    /// <summary>
    /// 將某個章節鎖定.
    /// </summary>
    /// <param name="chapter"> 1: 第一章, 2: 第二章. </param>
    /// <param name="title"></param>
    public void AddLockChapter(int chapter, string title)
    {
        if(!mChapters.ContainsKey(chapter))
            mChapters.Add(chapter, createChapter(chapter, title));
        mChapters[chapter].ShowLock();
    }

    /// <summary>
    /// 目前介面顯示哪一個章節. 1: 第一章, 2: 第二章.
    /// </summary>
    public int CurrentChapter
    {
        get
        {
            // 魔術數字 71 主要是歸零, 並且能夠正確計算出章節的數值(try and error).
            // 其實這部份只是根據 ScrollView 的 Local Position 來判斷現在顯示哪一章.
            // 當位置是 (69.999) 時, 表示是選擇第一章; 當位置是 (-1210) 時, 表示是選擇第二章;
            // 當位置是 (-2490) 時, 表示是選擇第三章. 以此類推.(目前章節的寬度是 1280)
            // +1 也是依據算式而做的調整, 不然明明是第一章, 但是卻回傳 0.
            return (int)Mathf.Abs((ScrollView.transform.localPosition.x - 71f) / mChapterWidth) + 1;
        }
    }

    /// <summary>
    /// 控制介面顯示哪一個章節.
    /// </summary>
    /// <param name="chapter"></param>
    public void ScrollToChapter(int chapter)
    {
        if(chapter <= 0 || chapter > mChapters.Count)
            return;

        ScrollView.ResetPosition();

        // 其實顯示某個章節, 只是移動一整個章節的寬度. 而第1章的位置是 (69.999),
        // 第 2 章的位置是 (-1210), 所以這邊才會這樣麼魔術數字去計算.
        Vector3 chapterPos = new Vector3(-(chapter - 1) * mChapterWidth, 0, 0);
        ScrollView.MoveRelative(chapterPos);

        // 這兩行只是 ScrollView 的行為我無法掌握(沒辦法正確更新).
        // 這樣設定, 就可以確保 ScrollView 的行為會有效(之前是設定後, 完全沒效果).
        // 當 NGUI 更新後, 可以嘗試刪除.(現在用的是 NGUI 3.9.4)
        ScrollView.enabled = false;
        ScrollView.enabled = true;
    }

    public void PlayChapterUnlockAnimation(int chapter, int stageID)
    {
        // 魔術數字 10, 是要保證會捲動到下一頁.
        Vector3 move = new Vector3(-mChapterWidth / 2f - 10, 0, 0);
        ScrollView.MoveRelative(move);

        // 1.1 是 try and error 的數值.
        // 需要大約 1.1 秒的時間, 讓 ScrollView 捲動到新章節的頁面. 
        // 因為這幾乎是固定的時間, 所以我沒有抽出成常數.
        StartCoroutine(playChapterUnlockAnimation(chapter, stageID, 1.1f));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="stageID"></param>
    /// <param name="delayTime"> 幾秒後開始撥章節解鎖 Animation. </param>
    /// <returns></returns>
    private IEnumerator playChapterUnlockAnimation(int chapter, int stageID, float delayTime)
    {
        mChapters[chapter].ShowLock();

        yield return new WaitForSeconds(delayTime);

        mChapters[chapter].PlayUnlockAnimation(stageID);
    }

    private UIStageChapter createChapter(int chapter, string title)
    {
        GameObject obj = UIPrefabPath.LoadUI(UIPrefabPath.StageChapter, ChapterParent);
        obj.transform.localPosition = findChapterPos(chapter, obj.GetComponent<UITexture>().width);

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
    public void AddStage(int chapter, int stageID, Vector3 localPos, UIStageInfo.Data data)
    {
        if(mChapters.ContainsKey(chapter))
            mChapters[chapter].AddStage(stageID, localPos, data);
        else
            Debug.LogErrorFormat("Chapter({0}) don't exist, you need call AddChapter() first.", chapter);
    }

    /// <summary>
    /// 某個小關卡鎖定.
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="stageID"></param>
    /// <param name="localPos"></param>
    /// <param name="kindSpriteName"></param>
    public void AddLockStage(int chapter, int stageID, Vector3 localPos, string kindSpriteName)
    {
        if(mChapters.ContainsKey(chapter))
            mChapters[chapter].AddLockStage(stageID, localPos, kindSpriteName);
        else
            Debug.LogErrorFormat("Chapter({0}) don't exist, you need call AddChapter() first.", chapter);
    }

    public bool HasChapter(int chapter)
    {
        return mChapters.ContainsKey(chapter);
    }

    public UIStageChapter GetChapter(int chapter)
    {
        return mChapters[chapter];
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
    }

    public void OnBackClick()
    {
        if(BackListener != null)
            BackListener();
    }
}