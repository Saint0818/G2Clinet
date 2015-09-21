using System.Collections;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角介面:選擇位置介面.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRolePositionView : MonoBehaviour
{
    public GameObject Window;
    public Transform ModelPreview;

    public UILabel PosNameLabel;
    public UILabel PosDescriptionLabel;

    public Animator UIAnimator;

    private delegate void Action();

    /// <summary>
    /// 離開此頁面撥動畫的時間, 單位:秒.
    /// </summary>
    private const float HideAnimationTime = 0.8f;

    private EPlayerPostion mCurrentPostion = EPlayerPostion.G;

    [UsedImplicitly]
    private void Awake()
    {
        Visible = false;
    }

    [UsedImplicitly]
    private void Start()
    {
        mCurrentPostion = EPlayerPostion.G;

        updateUI(mCurrentPostion);
    }

    private void updateUI(EPlayerPostion pos)
    {
        switch(pos)
        {
            case EPlayerPostion.G:
                PosNameLabel.text = TextConst.S(15);
                PosDescriptionLabel.text = TextConst.S(18);
                break;
            case EPlayerPostion.F:
                PosNameLabel.text = TextConst.S(16);
                PosDescriptionLabel.text = TextConst.S(19);
                break;
            case EPlayerPostion.C:
                PosNameLabel.text = TextConst.S(17);
                PosDescriptionLabel.text = TextConst.S(20);
                break;

            default:
                throw new InvalidEnumArgumentException(pos.ToString());
        }
    }

    public bool Visible
    {
        set
        {
            Window.SetActive(value);
        }
    }

    public void OnGuardClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.G;
            updateUI(mCurrentPostion);

            UI3DCreateRole.Get.Select(EPlayerPostion.G);
        }
    }

    public void OnForwardClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.F;

            updateUI(mCurrentPostion);

            UI3DCreateRole.Get.Select(EPlayerPostion.F);
        }
    }

    public void OnCenterClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.C;

            updateUI(mCurrentPostion);

            UI3DCreateRole.Get.Select(EPlayerPostion.C);
        }
    }

    public void OnBackClick()
    {
        StartCoroutine(playHideAnimation(showPreviousPage));
    }

    private void showPreviousPage()
    {
        GetComponent<UICreateRole>().ShowFrameView();
    }

    public void OnNextClick()
    {
        StartCoroutine(playHideAnimation(showNextPage));
    }

    private void showNextPage()
    {
        GetComponent<UICreateRole>().ShowStyleView(mCurrentPostion);
    }

    private IEnumerator playHideAnimation(Action action)
    {
        UIAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(HideAnimationTime);

        action();
    }
}