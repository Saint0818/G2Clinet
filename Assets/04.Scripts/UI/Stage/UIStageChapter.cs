using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 代表關卡中的某個章節.
/// </summary>
[DisallowMultipleComponent]
public class UIStageChapter : MonoBehaviour
{
    [Tooltip("章節數值. 1: 第一章, 2: 第二章.")]
    public int Chapter;

    public string ChapterName
    {
        set { ChapterNameLabel.text = value; }
    }

    public int ChapterValue
    {
        set { ChapterValueLabel.text = string.Format("CHAPTER.{0}", value); }
    }

    public UILabel ChapterNameLabel;
    public UILabel ChapterValueLabel;
    public GameObject Lock;
    public GameObject Open;

    [UsedImplicitly]
	private void Awake()
    {
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

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
