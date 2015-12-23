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

    /// <summary>
    /// 背景圖片.
    /// </summary>
    private const string OpenSpriteName = "StageButton01";

    /// <summary>
    /// 撥 Animation 時, 關卡經過幾秒後, 會變成可點選的狀態.
    /// </summary>
    private const float EnableTime = 1.8f;

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

    public void Show(UIStageInfo.Data data)
    {
        mData = data;
        mButton.isEnabled = false;
        
        KindSprite.spriteName = mData.KindSpriteName;

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
        GetComponent<UISprite>().spriteName = OpenSpriteName;
        // 如果不加上這行, 當我滑鼠滑過圖片時, 圖片會變掉. 我認為這應該是 UIButton 的 Bug. 
        // 目前的解決辦法是以下程式碼.
        mButton.normalSprite = OpenSpriteName;

        mButton.isEnabled = true;
    }

    public void ShowLock(string kindSpriteName)
    {
        KindSprite.spriteName = kindSpriteName;

        // 如果不加上這行, 當我滑鼠滑過圖片時, 圖片會變掉. 我認為這應該是 UIButton 的 Bug. 
        // 目前的解決辦法是以下程式碼.
//        mButton.normalSprite = LockSpriteName;

//        GetComponent<BoxCollider>().enabled = false;

        mButton.isEnabled = false;
    }

    public void NotifyClick()
    {
        Main.Info.Show(StageID, mData);
    }
}
