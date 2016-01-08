using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 小關卡, 也就是關卡介面上的小圓點, 點擊後, 玩家可以進入關卡.
/// </summary>
public class UIStageElement : MonoBehaviour
{
    [Tooltip("StageTable 裡面的 ID. 控制要顯示哪一個關卡的資訊.")]
    public int StageID;

    public UISprite KindSprite;
    public GameObject SelectedMark;

    /// <summary>
    /// 撥 Animation 時, 關卡經過幾秒後, 會變成可點選的狀態.
    /// </summary>
    private const float EnableTime = 1.8f;

    public UIStageInfo.Data Data { get { return mData; } }
    private UIStageInfo.Data mData;

    private Animator mAnimator;
    private UIButton mButton;

    private UIMainStageMain Main
    {
        get { return mMain ?? (mMain = GetComponentInParent<UIMainStageMain>()); }
    }
    private UIMainStageMain mMain;

    [UsedImplicitly]
    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        mButton = GetComponent<UIButton>();
    }

    public void Show(UIStageInfo.Data data, bool selected)
    {
        mData = data;
        mButton.isEnabled = false;
        
        KindSprite.spriteName = mData.KindSpriteName;

        SelectedMark.SetActive(selected);

        changeEnable();
    }

    public void PlayOpenAnimation()
    {
        if(!gameObject.activeInHierarchy)
            return;

        mButton.isEnabled = false;
        StartCoroutine(playAnimation(EnableTime));
    }

    private IEnumerator playAnimation(float enableTime)
    {
        mAnimator.SetTrigger("Open");

        yield return new WaitForSeconds(enableTime);

        changeEnable();
    }

    private void changeEnable()
    {
        mButton.isEnabled = true;
    }

    public void ShowLock(string kindSpriteName)
    {
        KindSprite.spriteName = kindSpriteName;

        mButton.isEnabled = false;

        SelectedMark.SetActive(false);
    }

    public void NotifyClick()
    {
        Main.Info.Show(StageID, mData);
    }
}
