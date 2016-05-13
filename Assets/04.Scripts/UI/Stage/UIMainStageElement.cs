using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 小關卡, 也就是關卡介面上的小圓點.
/// </summary>
public class UIMainStageElement : MonoBehaviour
{
    public class Data
    {
        public string BGNormalIcon;
        public string BGPressIcon;

        public bool IsEnable;
        public string ErrMsg;

        public bool IsSelected;

        public bool ShowClear;

        /// <summary>
        /// 要顯示幾顆星.
        /// </summary>
        public int StarNum;

        /// <summary>
        /// 要不要顯示星星.
        /// </summary>
        public bool StarVisible;
    }

    /// <summary>
    /// <para> 呼叫時機: 點擊. </para>
    /// <para> int: StageID. </para>
    /// </summary>
    public event Action<int, UIMainStageInfo.Data> OnClickListener;

    [Tooltip("StageTable 裡面的 ID. 控制要顯示哪一個關卡的資訊.")]
    public int StageID;

    public GameObject SelectedMark;
    public GameObject ClearMark;

    /// <summary>
    /// 撥 Animation 時, 關卡經過幾秒後, 會變成可點選的狀態.
    /// </summary>
    private const float EnableTime = 1.8f;

    public UIMainStageInfo.Data InfoData { get { return mInfoData; } }
    private UIMainStageInfo.Data mInfoData;

    private Data mData;

    private Animator mAnimator;
    private UIButton mButton;

    private UIMainStageStars mStar;

    [UsedImplicitly]
    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        mButton = GetComponent<UIButton>();
        mStar = GetComponent<UIMainStageStars>();

        mButton.onClick.Add(new EventDelegate(() =>
        {
            if(mData.IsEnable)
                fireClickEvent();
            else
                UIHint.Get.ShowHint(mData.ErrMsg, Color.black);
        }));
    }

    private void fireClickEvent()
    {
        if(OnClickListener != null)
            OnClickListener(StageID, mInfoData);
    }

    public void Set(Data data, UIMainStageInfo.Data infoData)
    {
        mData = data;
        mInfoData = infoData;

        mButton.normalSprite = mData.BGNormalIcon;
        mButton.hoverSprite = mData.BGNormalIcon;
        mButton.pressedSprite = mData.BGPressIcon;

        SelectedMark.SetActive(data.IsSelected);
        ClearMark.SetActive(data.ShowClear);

        if(data.StarVisible)
            mStar.Show(data.StarNum);
        else
            mStar.Hide();
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
}
