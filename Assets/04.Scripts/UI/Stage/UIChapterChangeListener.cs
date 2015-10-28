using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// <para> 負責發出 ScrollView 捲動到不同章節的事件. </para>
/// <para>ScrollView 的 local position 就是控制 ScrollView 看了哪些項目. 所以實作是根據 local position
/// 來判斷哪一個章節被選擇.</para>
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 向 OnChangeListener 註冊事件. </item>
/// </list>
public class UIChapterChangeListener : MonoBehaviour
{
    private class Chapter
    {
        private readonly float mMinX;
        private readonly float mMaxX;

        private readonly int mChapter;

        public Chapter(float minX, float maxX, int chapter)
        {
            mMinX = minX;
            mMaxX = maxX;
            mChapter = chapter;
        }

        public int ChapterID
        {
            get { return mChapter; }
        }

        public bool IsInRange(float value)
        {
            return mMinX <= value && value <= mMaxX;
        }

        public override string ToString()
        {
            return string.Format("ChapterID: {2}, Range:({0}, {1})", mMinX, mMaxX, ChapterID);
        }
    }

    private const float SenseRangeInPixel = 600;

    /// <summary>
    /// <para> 呼叫時機: 章節改變時. </para>
    /// <para> int ChapterID: 新章節. </para>
    /// </summary>
    public event CommonDelegateMethods.Action1 OnChangeListener;

    private int mCurrentChapterID = -1;

    private readonly List<Chapter> mChapters = new List<Chapter>();
        
    [UsedImplicitly]
    private void Awake()
    {
        UIStageChapter[] chapters = GetComponentsInChildren<UIStageChapter>();
        float width = chapters[0].GetComponent<UITexture>().width;

        for(int i = 0; i < chapters.Length; i++)
        {
            float minX = -width * i - SenseRangeInPixel * 0.5f;
            float maxX = -width * i + SenseRangeInPixel * 0.5f;
            Chapter ch = new Chapter(minX, maxX, i+1);
            mChapters.Add(ch);
        }
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
        for(int i = 0; i < mChapters.Count; i++)
        {
            if(mChapters[i].IsInRange(transform.localPosition.x) && 
               mCurrentChapterID != mChapters[i].ChapterID)
            {
                fireEvent(mChapters[i].ChapterID);
                mCurrentChapterID = mChapters[i].ChapterID;
                break;
            }
        }
    }

    private void fireEvent(int chapterID)
    {
        if(OnChangeListener != null)
            OnChangeListener(chapterID);
    }
}
