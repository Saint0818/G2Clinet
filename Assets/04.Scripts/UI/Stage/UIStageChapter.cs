using UnityEngine;
using JetBrains.Annotations;

/// <summary>
/// 代表關卡中的某個章節.
/// </summary>
[DisallowMultipleComponent]
public class UIStageChapter : MonoBehaviour
{
    public int Chapter;
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
