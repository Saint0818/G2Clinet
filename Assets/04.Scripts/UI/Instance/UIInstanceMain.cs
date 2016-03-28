﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 副本主介面行為.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 ClearAllChapters, AddChapter 控制有哪些章節. </item>
/// <item> ShowChapters or ShowStages 控制要顯示章節 or 關卡. </item>
/// <item> Call SelectChapter 控制 ScrollView 顯示哪個章節. </item>
/// <item> Call MoveToChapter 控制 ScrollView 捲動到哪個章節. </item>
/// <item> Call SelectStage 控制 ScrollView 顯示哪個關卡. </item>
/// </list>
public class UIInstanceMain : MonoBehaviour
{
    public Transform ChapterParent;
    public GameObject ChapterView;
    public UIButton ChapterBackButton;

    public Transform StageParent;
    public GameObject StageView;
    public UIButton StageBackButton;

    public UIButton PreviousChapterButton;
    public UIButton NextChapterButton;

    public UIScrollView ChapterScrollView;
    public UIScrollView StageScrollView;

    public GameObject FullScreenBlock;

    /// <summary>
    /// <para> 呼叫時機: 關卡的 Start 按鈕按下. </para>
    /// <para>[int stageID]: 進入哪一個關卡. </para>
    /// <para>[EErrorCode errorCode]:  </para>
    /// <para>[string errorMsg]:  </para>
    /// </summary>
    public event Action<int, UIStageVerification.EErrorCode, string> StageStartListener;

    /// <summary>
    /// 單位: Pixel.
    /// </summary>
    private const float ChapterInterval = 900;
    private const float StageInterval = 230;

    /// <summary>
    /// key: Chapter, 1: 第 1 章, 2: 第 2 章, 以此類推.
    /// </summary>
    private readonly Dictionary<int, UIInstanceChapter> mChapters = new Dictionary<int, UIInstanceChapter>();
    private readonly List<UIInstanceStage> mStages = new List<UIInstanceStage>();

    private UIMoveScrollView mMoveScrollView;

    private void Awake()
    {
        mMoveScrollView = GetComponent<UIMoveScrollView>();
    }

    private void Start()
    {
        StageBackButton.onClick.Add(new EventDelegate(ShowChapters));

        NextChapterButton.onClick.Add(new EventDelegate(() => MoveToChapter(CurrentChapter + 1)));
        PreviousChapterButton.onClick.Add(new EventDelegate(() => MoveToChapter(CurrentChapter - 1)));

        FullScreenBlock.SetActive(false);
    }

    public void ClearAllChapters()
    {
        clearAllStages();

        foreach(KeyValuePair<int, UIInstanceChapter> pair in mChapters)
            Destroy(pair.Value.gameObject);
        mChapters.Clear();
    }

    public void AddChapter(int chapter, UIInstanceChapter.Data data)
    {
        var localPos = new Vector3(mChapters.Count * ChapterInterval, 0, 0);
        var obj = UIPrefabPath.LoadUI(UIPrefabPath.UIInstanceChapter, ChapterParent, localPos);
        obj.name = string.Format("{0}({1})", obj.name, chapter);
        UIInstanceChapter uiChapter = obj.GetComponent<UIInstanceChapter>();
        uiChapter.SetData(data);

        var focus = obj.AddComponent<UIInstanceChapterFocus>();
        focus.Init(uiChapter.Focus, ChapterScrollView.transform, getChapterTargetPos(chapter));

        mChapters.Add(chapter, uiChapter);
    }

    public void SelectChapter(int chapter)
    {
        var reviseChapter = getReviseChapter(chapter);

        // 其實顯示某個章節, 只是移動一整個章節的寬度. 第1章的位置是 (0),
        // 第 2 章的位置是 (-900), 所以這邊才會這樣計算.
        Vector3 targetPos = getChapterTargetPos(reviseChapter);
        Vector3 moveAmount = targetPos - ChapterScrollView.transform.localPosition;
        
        // 從目前 ScrollView 的位置, 移動多少可以到達目標位置.
        ChapterScrollView.MoveRelative(moveAmount);

//        Debug.LogFormat("ScrollView TargetPos:{0}, MoveAmount:{1}, Pos:{2}", 
//            targetPos, moveAmount, ScrollView.transform.localPosition);
    }

    private int getReviseChapter(int chapter)
    {
        if(chapter < 1)
            return 1;
        if(chapter > mChapters.Count)
            return mChapters.Count;
        return chapter;
    }

    /// <summary>
    /// 移動到某個章節.
    /// </summary>
    /// <param name="chapter"></param>
    public void MoveToChapter(int chapter)
    {
        var reviseChapter = getReviseChapter(chapter);

        Vector3 targetPos = getChapterTargetPos(reviseChapter);

        FullScreenBlock.SetActive(true);
        mMoveScrollView.Move(ChapterScrollView, targetPos, () => FullScreenBlock.SetActive(false));
    }

    /// <summary>
    /// 目前介面顯示哪一個章節. 1: 第一章, 2: 第二章.
    /// </summary>
    public int CurrentChapter
    {
        get
        {
            // 其實這部份只是根據 ScrollView 的 Local Position 來判斷現在顯示哪一章.
            // 當位置是 -450 ~ 450 時, 表示是選擇第 1 章; 
            // 當位置是 451 ~ 900 時, 表示是選擇第 2 章;
            // 當位置是 901 ~ 1350 時, 表示是選擇第 3 章. 以此類推.(目前章節的寬度是 900)

            float minX = -450;
            float maxX = 450;
            int chapter = 1;
            int currentPosX = (int)Mathf.Abs(ChapterScrollView.transform.localPosition.x);
            while (chapter <= mChapters.Count)
            {
                if(minX <= currentPosX && currentPosX <= maxX)
                    return chapter;

                ++chapter;
                minX += ChapterInterval;
                maxX += ChapterInterval;
            }

            return 0;

//            return (int)Mathf.Abs(ChapterScrollView.transform.localPosition.x / ChapterInterval) + 1;
        }
    }

//    private void OnGUI()
//    {
//        GUI.Label(new Rect(100, 100, 600, 100), string.Format("Current Chapter:{0}", CurrentChapter));
//    }

    private Vector3 getChapterTargetPos(int chapter)
    {
        return new Vector3((chapter - 1) * -ChapterInterval, 0, 0);
    }

    public void ShowChapters()
    {
        StageView.SetActive(false);
        StageBackButton.gameObject.SetActive(false);

        ChapterView.SetActive(true);
        ChapterBackButton.gameObject.SetActive(true);
    }

    public void ShowStages(int chapter)
    {
        if(!mChapters.ContainsKey(chapter))
            return;

        var uiChapter = mChapters[chapter];
        ShowStages(uiChapter.NormalStages, uiChapter.BossStages);
    }

    public void ShowStages(UIInstanceStage.Data[] oneChapterNormalStages, UIInstanceStage.Data bossStage)
    {
        ChapterView.SetActive(false);
        ChapterBackButton.gameObject.SetActive(false);

        StageView.SetActive(true);
        StageBackButton.gameObject.SetActive(true);

        clearAllStages();
        addStages(oneChapterNormalStages, bossStage);
    }

    public void SelectStage(int index)
    {
        var reviseIndex = index;
        if(index < 0)
            reviseIndex = 0;
        if(index >= mStages.Count)
            reviseIndex = mStages.Count - 1;

        // 其實顯示某個關卡, 只是移動一個關卡的高度. 第1個關卡的位置是 (-21.5, 150, 0),
        // 第 2 個關卡是 (-21.5, 380, 0), 所以這邊才會這樣計算.
        Vector3 targetPos = new Vector3(-21.5f, 150 + reviseIndex * StageInterval, 0);
        Vector3 moveAmount = targetPos - StageScrollView.transform.localPosition;

//        Debug.LogFormat("ScrollView Pos:{0}", StageScrollView.transform.localPosition);

        // 從目前 ScrollView 的位置, 移動多少可以到達目標位置.
        StageScrollView.MoveRelative(moveAmount);

//        Debug.LogFormat("ScrollView TargetPos:{0}, MoveAmount:{1}, Pos:{2}", 
//                    targetPos, moveAmount, StageScrollView.transform.localPosition);
    }

    private void addStages(UIInstanceStage.Data[] oneChapterNormalStages, UIInstanceStage.Data bossStage)
    {
        Vector3 localPos = Vector3.zero;
        foreach(UIInstanceStage.Data data in oneChapterNormalStages)
        {
            mStages.Add(addStage(UIPrefabPath.UIInstanceStage0, data, localPos));
            localPos.y -= StageInterval;
        }
        mStages.Add(addStage(UIPrefabPath.UIInstanceStage9, bossStage, localPos));
    }

    private UIInstanceStage addStage(string prefabName, UIInstanceStage.Data data, Vector3 localPos)
    {
        var obj = UIPrefabPath.LoadUI(prefabName, StageParent, localPos);
        obj.name = string.Format("{0}({1})", obj.name, data.ID);
        UIInstanceStage uiStage = obj.GetComponent<UIInstanceStage>();
        uiStage.SetData(data);
        return uiStage;
    }

    private void clearAllStages()
    {
        foreach(UIInstanceStage stage in mStages)
            Destroy(stage.gameObject);
        mStages.Clear();
    }

    private void FixedUpdate()
    {
        PreviousChapterButton.gameObject.SetActive(CurrentChapter != 1);
        NextChapterButton.gameObject.SetActive(CurrentChapter != mChapters.Count);
    }

    /// <summary>
    /// 內部使用...
    /// </summary>
    /// <param name="stageID"></param>
    /// <param name="errorCode"></param>
    /// <param name="errorMsg"></param>
    public void NotifyStageStartClick(int stageID, UIStageVerification.EErrorCode errorCode, string errorMsg)
    {
        if(StageStartListener != null)
            StageStartListener(stageID, errorCode, errorMsg);
    }
}
