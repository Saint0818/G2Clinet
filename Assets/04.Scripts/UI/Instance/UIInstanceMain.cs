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

    public void AddChapter(UIInstanceChapter.Data data)
    {
        var localPos = new Vector3(mChapters.Count * ChapterInterval, 0, 0);
        var obj = UIPrefabPath.LoadUI(UIPrefabPath.UIInstanceChapter, ChapterParent, localPos);
        obj.name = string.Format("{0}({1})", obj.name, data.Title);
        UIInstanceChapter chapter = obj.GetComponent<UIInstanceChapter>();
        chapter.SetData(data);

        mChapters.Add(chapter);
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

        clearStages();
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

    private void clearStages()
    {
        foreach(UIInstanceStage stage in mStages)
        {
            Destroy(stage.gameObject);
        }
        mStages.Clear();
    }
}
