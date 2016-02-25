using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 小關卡, 也就是關卡介面上的小圓點, 點擊後, 玩家可以進入關卡.
/// </summary>
public class UIStageElement : MonoBehaviour
{
    public class Data
    {
        public string BGNormalIcon;
        public string BGPressIcon;

        public bool IsEnable;
        public string ErrMsg;

        public bool IsSelected;

        public bool ShowClear;
    }

    [Tooltip("StageTable 裡面的 ID. 控制要顯示哪一個關卡的資訊.")]
    public int StageID;

    public UISprite KindSprite;
    public GameObject SelectedMark;
    public GameObject ClearMark;

    /// <summary>
    /// 撥 Animation 時, 關卡經過幾秒後, 會變成可點選的狀態.
    /// </summary>
    private const float EnableTime = 1.8f;

    public UIStageInfo.Data InfoData { get { return mInfoData; } }
    private UIStageInfo.Data mInfoData;

    private Data mData;

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

        mButton.onClick.Add(new EventDelegate(() =>
        {
            if(mData.IsEnable)
                Main.Info.Show(StageID, mInfoData);
            else
                UIHint.Get.ShowHint(mData.ErrMsg, Color.white);
        }));
    }

    public void Set(Data data, UIStageInfo.Data infoData)
    {
        mData = data;
        mInfoData = infoData;

        mButton.normalSprite = mData.BGNormalIcon;
        mButton.hoverSprite = mData.BGNormalIcon;
        mButton.pressedSprite = mData.BGPressIcon;

        SelectedMark.SetActive(data.IsSelected);
        ClearMark.SetActive(data.ShowClear);
    }

    public void PlayUnlockAnimation()
    {
        if(!gameObject.activeInHierarchy)
            return;

        StartCoroutine(playUnlockAnimation(EnableTime));
    }

    private IEnumerator playUnlockAnimation(float enableTime)
    {
        mButton.isEnabled = false;

        mAnimator.SetTrigger("Open");

        yield return new WaitForSeconds(enableTime);

        mButton.isEnabled = true;
    }

//    public void ShowLock(string kindSpriteName)
//    {
//        KindSprite.spriteName = kindSpriteName;
//
//        SelectedMark.SetActive(false);
//    }
}
