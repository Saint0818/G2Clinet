using System.Collections;
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
    private const float UnlockTime = 1.5f;

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

    private readonly string StagePath = "Prefab/UI/UIStageElement";
    private readonly string TexturePath = "Textures/Chapter/Chapter_{0}";

    /// <summary>
    /// key: StageID.
    /// </summary>
    private readonly Dictionary<int, UIStageElement> mStages = new Dictionary<int, UIStageElement>();

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

    /// <summary>
    /// 顯示某個小關卡.
    /// </summary>
    /// <param name="stageID"></param>
    /// <param name="localPos"></param>
    /// <param name="data"></param>
    public void AddStage(int stageID, Vector3 localPos, UIStageInfo.Data data)
    {
        if(!mStages.ContainsKey(stageID))
            mStages.Add(stageID, createStage(stageID, localPos));
        mStages[stageID].Show(data);
    }

    /// <summary>
    /// 某個小關卡鎖定.
    /// </summary>
    /// <param name="stageID"></param>
    /// <param name="localPos"></param>
    /// <param name="kindSpriteName"></param>
    public void AddLockStage(int stageID, Vector3 localPos, string kindSpriteName)
    {
        if(!mStages.ContainsKey(stageID))
            mStages.Add(stageID, createStage(stageID, localPos));
        mStages[stageID].ShowLock(kindSpriteName);
    }

    public bool HasStage(int stageID)
    {
        return mStages.ContainsKey(stageID);
    }

    public UIStageElement GetStageByID(int stageID)
    {
        return mStages[stageID];
    }

    public void PlayUnlockAnimation(int stageID)
    {
        mAnimator.SetTrigger("Unlock");

        StartCoroutine(show(stageID, UnlockTime));
    }

    private IEnumerator show(int stageID, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Show();

        mStages[stageID].PlayOpenAnimation();
    }

    private UIStageElement createStage(int stageID, Vector3 localPos)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(StagePath));
        obj.transform.parent = Open.transform;
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = mDefaultStageScale;
        obj.name = string.Format("Stage{0}", stageID);

        var stage = obj.GetComponent<UIStageElement>();
        stage.StageID = stageID;
        return stage;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
