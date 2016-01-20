using System.Collections.Generic;
using UnityEngine;

public class UIInstanceMain : MonoBehaviour
{
    public Transform ChapterParent;

    /// <summary>
    /// 單位: Pixel.
    /// </summary>
    private const float ChapterInterval = 900;

    private readonly List<UIInstanceChapter> mChapters = new List<UIInstanceChapter>();

    public void AddChapter(UIInstanceChapter.Data data)
    {
        var localPos = new Vector3(mChapters.Count * ChapterInterval, 0, 0);
        var obj = UIPrefabPath.LoadUI(UIPrefabPath.UIInstanceChapter, ChapterParent, localPos);
        UIInstanceChapter chapter = obj.GetComponent<UIInstanceChapter>();
        chapter.SetData(data);

        mChapters.Add(chapter);
    }
}
