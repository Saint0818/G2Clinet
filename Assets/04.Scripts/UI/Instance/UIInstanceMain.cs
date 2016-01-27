﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInstanceMain : MonoBehaviour
{
    public Transform ChapterParent;
    public GameObject ChapterView;
    public UIButton ChapterBackButton;

    public Transform StageParent;
    public GameObject StageView;
    public UIButton StageBackButton;

    public UIScrollView ScrollView;

    /// <summary>
    /// <para> 呼叫時機: 關卡的 Start 按鈕按下. </para>
    /// <para>[int stageID]: 進入哪一個關卡. </para>
    /// </summary>
    public event Action<int> StageStartListener;

    /// <summary>
    /// 單位: Pixel.
    /// </summary>
    private const float ChapterInterval = 900;
    private const float StageInterval = 230;

    private readonly List<UIInstanceChapter> mChapters = new List<UIInstanceChapter>();
    private readonly List<UIInstanceStage> mStages = new List<UIInstanceStage>();

    private void Start()
    {
        StageBackButton.onClick.Add(new EventDelegate(ShowChapters));
    }

    public void ClearAllChapters()
    {
        clearAllStages();

        foreach(UIInstanceChapter chapter in mChapters)
            Destroy(chapter.gameObject);
        mChapters.Clear();
    }

    public void AddChapter(UIInstanceChapter.Data data)
    {
        var localPos = new Vector3(mChapters.Count * ChapterInterval, 0, 0);
        var obj = UIPrefabPath.LoadUI(UIPrefabPath.UIInstanceChapter, ChapterParent, localPos);
        obj.name = string.Format("{0}({1})", obj.name, data.Title);
        UIInstanceChapter chapter = obj.GetComponent<UIInstanceChapter>();
        chapter.SetData(data);

        mChapters.Add(chapter);
    }

    public void SelectChapter(int chapter)
    {
        var reviseChapter = chapter;
        if(chapter < 1)
            reviseChapter = 1;
        else if(chapter > mChapters.Count)
            reviseChapter = mChapters.Count;

        // 其實顯示某個章節, 只是移動一整個章節的寬度. 第1章的位置是 (0),
        // 第 2 章的位置是 (-900), 所以這邊才會這樣計算.
        Vector3 targetPos = new Vector3((reviseChapter - 1) * -ChapterInterval, 0, 0);
        Vector3 moveAmount = targetPos - ScrollView.transform.localPosition;
        
        // 從目前 ScrollView 的位置, 移動多少可以到達目標位置.
        ScrollView.MoveRelative(moveAmount);

//        Debug.LogFormat("ScrollView TargetPos:{0}, MoveAmount:{1}, Pos:{2}", 
//            targetPos, moveAmount, ScrollView.transform.localPosition);
    }

    public void ShowChapters()
    {
        StageView.SetActive(false);
        StageBackButton.gameObject.SetActive(false);

        ChapterView.SetActive(true);
        ChapterBackButton.gameObject.SetActive(true);
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

    public void NotifyStageStartClick(int stageID)
    {
        if(StageStartListener != null)
            StageStartListener(stageID);
    }
}
