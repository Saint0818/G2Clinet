﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主線關卡介面主程式.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call RemoveAllChapters() 將整個關卡介面重置. </item>
/// <item> Call AddChapter() 加入章節</item>
/// <item> Call AddStage() 加入章節小關卡. </item>
/// <item> Call SelectChapter() 控制預設顯示哪一個章節. </item>
/// </list>
[DisallowMultipleComponent]
public class UIMainStageView : MonoBehaviour
{
    /// <summary>
    /// ScrollView 捲動幾秒後, 開始撥 Unlock Animation.
    /// </summary>
    private const float PlayUnlockTime = 0.5f;

    /// <summary>
    /// 呼叫時機: 返回按鈕按下時.
    /// </summary>
    public event Action BackListener;

//    /// <summary>
//    /// Index 0: 第一章, Index 2: 第二章.
//    /// </summary>
//    public UIMainStageInfo Info
//    {
//        get { return mInfo ?? (mInfo = GetComponent<UIMainStageInfo>()); }
//    }
//    private UIMainStageInfo mInfo;

    public UIButton BackButton;
    public UIButton PreviousChapterButton;
    public UIButton NextChapterButton;
    public Transform ChapterParent;
    public UIScrollView ScrollView;
    public GameObject FullScreenBlock;
    public bool EnableFullScreenBlock { set { FullScreenBlock.SetActive(value);} }

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

    private UIMoveScrollView mMoveScrollView;

    private void Awake()
    {
        EnableFullScreenBlock = false;

        mMoveScrollView = GetComponent<UIMoveScrollView>();

        BackButton.onClick.Add(new EventDelegate(onBackClick));

        PreviousChapterButton.onClick.Add(new EventDelegate(() => moveToChapter(CurrentChapter - 1)));
        NextChapterButton.onClick.Add(new EventDelegate(() => moveToChapter(CurrentChapter + 1)));
    }

    /// <summary>
    /// 移動到某個章節.
    /// </summary>
    /// <param name="chapter"></param>
    private void moveToChapter(int chapter)
    {
        FullScreenBlock.SetActive(true);

        var reviseChapter = getReviseChapter(chapter);
        Vector3 targetPos = getChapterTargetPos(reviseChapter);
        mMoveScrollView.Move(ScrollView, targetPos, () => FullScreenBlock.SetActive(false));
    }

    /// <summary>
    /// 加入新的 1 章.
    /// </summary>
    /// <param name="chapter"> 1: 第一章, 2: 第二章. </param>
    /// <param name="title"></param>
    /// <returns> instance. </returns>
    public UIStageChapter AddChapter(int chapter, string title)
    {
        if(!mChapters.ContainsKey(chapter))
            mChapters.Add(chapter, createChapter(chapter, title));
        mChapters[chapter].Show();

        // 其實每個章節的大小都相同, 所以我這邊用稍微取巧的方法取值.
        if(mChapterWidth <= 0)
            mChapterWidth = mChapters[chapter].GetComponent<UITexture>().width;

        return mChapters[chapter];
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
            // 當位置是 -640 ~ 640 時, 表示是選擇第 1 章; 
            // 當位置是 641 ~ 1280 時, 表示是選擇第 2 章;
            // 當位置是 1921 ~ 2560 時, 表示是選擇第 3 章. 以此類推.(目前章節的寬度是 1280)

            float minX = -450;
            float maxX = 450;
            int chapter = 1;
            int currentPosX = (int)Mathf.Abs(ScrollView.transform.localPosition.x);
            while (chapter <= mChapters.Count)
            {
                if (minX <= currentPosX && currentPosX <= maxX)
                    return chapter;

                ++chapter;
                minX += mChapterWidth;
                maxX += mChapterWidth;
            }

            return 0;

//            return (int)Mathf.Abs((ScrollView.transform.localPosition.x - 71f) / mChapterWidth) + 1;
        }
    }

    private int getReviseChapter(int chapter)
    {
        int reviseChapter = Math.Max(1, chapter); // >= 1
        return Math.Min(reviseChapter, mChapters.Count); // <= mChapters.Count
    }

    /// <summary>
    /// 控制介面顯示哪一個章節.
    /// </summary>
    /// <param name="chapter"></param>
    public void SelectChapter(int chapter)
    {
        int reviseChapter = getReviseChapter(chapter);

        // 其實顯示某個章節, 只是移動一整個章節的寬度. 而第1章的位置是 (70, 3, 0),
        // 第 2 章的位置是 (-1210, 3, 0), 所以這邊才會這樣麼魔術數字去計算.
        Vector3 targetPos = getChapterTargetPos(reviseChapter);
        Vector3 moveAmount = targetPos - ScrollView.transform.localPosition;
        ScrollView.MoveRelative(moveAmount);
    }

    private Vector3 getChapterTargetPos(int chapter)
    {
        return new Vector3(70 -(chapter - 1) * mChapterWidth, 3, 0);
    }

    public void PlayUnlockChapterAnimation(int unlockChapter, int unlockStageID)
    {
        SelectChapter(unlockChapter - 1);
        // 魔術數字 10, 是要保證會捲動到下一頁.
//        Vector3 move = new Vector3(-mChapterWidth / 2f - 10, 0, 0);
//        ScrollView.MoveRelative(move);

        EnableFullScreenBlock = true;
        mChapters[unlockChapter].ShowLock();
        mMoveScrollView.Move(ScrollView, getChapterTargetPos(unlockChapter), 
            () => StartCoroutine(playUnlockChapterAnimation(unlockChapter, unlockStageID, PlayUnlockTime)));

        // 需要一段很短的時間, 讓 ScrollView 捲動到新章節的頁面. 
//        StartCoroutine(playUnlockChapterAnimation(unlockChapter, stageID, PlayUnlockTime));

        // 4 是 try and error 的數值, 整個 Unlock 特效的時間大概是 4 秒左右撥完, 所以撥完時,
        // 就可以點選了.
        StartCoroutine(disableFullScreenBlock(4)); 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unlockChapter"></param>
    /// <param name="unlockStageID"></param>
    /// <param name="delayTime"> 幾秒後開始撥章節解鎖 Animation. </param>
    /// <returns></returns>
    private IEnumerator playUnlockChapterAnimation(int unlockChapter, int unlockStageID, float delayTime)
    {
//        mChapters[unlockChapter].ShowLock();

        yield return new WaitForSeconds(delayTime);

        mChapters[unlockChapter].PlayUnlockAnimation(unlockStageID);
    }

    private IEnumerator disableFullScreenBlock(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        EnableFullScreenBlock = false;
    }

    private UIStageChapter createChapter(int chapter, string title)
    {
        GameObject obj = UIPrefabPath.LoadUI(UIPrefabPath.StageChapter, ChapterParent);
        obj.transform.localPosition = findChapterPos(chapter, obj.GetComponent<UITexture>().width);
        obj.name = string.Format("Chapter{0}", chapter);

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

    public void AddStage(int chapter, int stageID, Vector3 localPos, UIMainStageElement.Data elementData, 
                         UIMainStageInfo.Data infoData)
    {
        if(mChapters.ContainsKey(chapter))
            mChapters[chapter].AddStage(stageID, localPos, elementData, infoData);
        else
            Debug.LogErrorFormat("Chapter({0}) don't exist, you need call AddChapter() first.", chapter);
    }

    public void AddBossStage(int chapter, int stageID, Vector3 localPos, UIMainStageElement.Data elementData, 
                             UIMainStageInfo.Data data)
    {
        if(mChapters.ContainsKey(chapter))
            mChapters[chapter].AddBossStage(stageID, localPos, elementData, data);
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

    public void ShowStageInfo(int chapter, int stageID)
    {
        if(!mChapters.ContainsKey(chapter) || !mChapters[chapter].HasStage(stageID))
            return;

        UIMainStageElement element = mChapters[chapter].GetStageByID(stageID);
        mChapters[chapter].Info.Show(stageID, element.InfoData);
    }

    private void FixedUpdate()
    {
        PreviousChapterButton.gameObject.SetActive(CurrentChapter != 1);
        NextChapterButton.gameObject.SetActive(CurrentChapter != mChapters.Count);
    }

    private void onBackClick()
    {
        if(BackListener != null)
            BackListener();
    }
}