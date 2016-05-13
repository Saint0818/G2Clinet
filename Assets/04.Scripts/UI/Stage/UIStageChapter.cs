﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 代表關卡中的某個章節.
/// </summary>
[DisallowMultipleComponent]
public class UIStageChapter : MonoBehaviour
{
    /// <summary>
    /// Unlock Animation 撥多久後, 才會顯示全部的關卡. 單位:秒.
    /// </summary>
    private const float ShowAllStagesTime = 1.5f;

    public UILabel LockNameLabel;

    /// <summary>
    /// 章節數值. 1: 第一章, 2: 第二章.
    /// </summary>
    public int Chapter
    {
        set
        {
            mChapter = value;
            ChapterValueLabel.text = string.Format("CHAPTER.{0}", mChapter);

            string path = string.Format(TexturePath, value);
            GetComponent<UITexture>().mainTexture = Resources.Load<Texture2D>(path);
        }
        get { return mChapter; }
    }
    private int mChapter;

    private readonly Vector3 mDefaultStageScale = new Vector3(1, 1, 1);

    public string Title
    {
        set { ChapterNameLabel.text = value; }
    }

    public UILabel ChapterNameLabel;
    public UILabel ChapterValueLabel;
    public GameObject Lock;
    public GameObject Open;

    private readonly string TexturePath = "Textures/Chapter/Chapter_{0}";

    /// <summary>
    /// key: StageID.
    /// </summary>
    private readonly Dictionary<int, UIMainStageElement> mStages = new Dictionary<int, UIMainStageElement>();

    private Animator mAnimator;

    [UsedImplicitly]
	private void Awake()
    {
        LockNameLabel.text = TextConst.S(int.Parse(LockNameLabel.text));
        mAnimator = GetComponentInChildren<Animator>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Lock.SetActive(false);
        Open.SetActive(true);
    }

    public void ShowLock()
    {
        gameObject.SetActive(true);
        Lock.SetActive(true);
        Open.SetActive(false);
    }

    public void AddStage(int stageID, Vector3 localPos, UIMainStageElement.Data elementData, 
                         UIMainStageInfo.Data infoData)
    {
        if(!mStages.ContainsKey(stageID))
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.UIMainStageElement, Open.transform, localPos);
            obj.name = string.Format("StageElement{0}", stageID);
            var stage = obj.GetComponent<UIMainStageElement>();
            stage.StageID = stageID;

//            mStages.Add(stageID, createStage(UIPrefabPath.UIMainStageElement, stageID, localPos));
            mStages.Add(stageID, stage);
        }
        mStages[stageID].Set(elementData, infoData);
    }

    public void AddBossStage(int stageID, Vector3 localPos, UIMainStageElement.Data elementData, 
                             UIMainStageInfo.Data infoData)
    {
        if(!mStages.ContainsKey(stageID))
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.UIMainStageElement9, Open.transform, localPos);
            obj.name = string.Format("StageElement{0}", stageID);
            var stage = obj.GetComponent<UIMainStageElement>();
            stage.StageID = stageID;

//            mStages.Add(stageID, createStage(UIPrefabPath.UIMainStageElement9, stageID, localPos));
            mStages.Add(stageID, stage);
        }
        mStages[stageID].Set(elementData, infoData);
    }

    public bool HasStage(int stageID)
    {
        return mStages.ContainsKey(stageID);
    }

    public UIMainStageElement GetStageByID(int stageID)
    {
        return mStages[stageID];
    }

    public void PlayUnlockAnimation(int unlockStageID)
    {
        if (mAnimator != null)
            mAnimator.SetTrigger("Unlock");

        StartCoroutine(showAllStages(unlockStageID, ShowAllStagesTime));

        // 要經過比較久的時間, 才把 Lock GameObject 關掉. 因為整個流程是
        // UnLock 要繼續撥, 撥的途中, 會出現關卡, 並撥關卡開啟的 Animation.
        StartCoroutine(delayShow(10)); 
    }

    private IEnumerator showAllStages(int unlockStageID, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Open.SetActive(true);

        mStages[unlockStageID].PlayUnlockAnimation();
    }

    private IEnumerator delayShow(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Show();
    }

//    private UIMainStageElement createStage(string path, int stageID, Vector3 localPos)
//    {
//        GameObject obj = Instantiate(Resources.Load<GameObject>(path));
//        obj.transform.parent = Open.transform;
//        obj.transform.localPosition = localPos;
//        obj.transform.localRotation = Quaternion.identity;
//        obj.transform.localScale = mDefaultStageScale;
//        obj.name = string.Format("StageElement{0}", stageID);
//
//        var stage = obj.GetComponent<UIMainStageElement>();
//        stage.StageID = stageID;
//        return stage;
//    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
