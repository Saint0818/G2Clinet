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

    public UIAttributes Attributes;

    private delegate void Action();

    /// <summary>
    /// 離開此頁面撥動畫的時間, 單位:秒.
    /// </summary>
    private const float HideAnimationTime = 0.8f;

    /// <summary>
    /// 第一次進入此頁面時, 要 delay 多久才播放 Attribute Scale 的動畫. 單位: 秒.
    /// </summary>
    private const float AttributeScaleDelayTime = 1;

    /// <summary>
    /// 屬性全滿的值.
    /// </summary>
    private const float AttributeMax = 200;

    private EPlayerPostion mCurrentPostion = EPlayerPostion.G;

    [UsedImplicitly]
    private void Awake()
    {
        Visible = false;

        mCurrentPostion = EPlayerPostion.G;

        Attributes.ClickVisible = false;
    }

    [UsedImplicitly]
    private void Start()
    {
    }

    private void updateUI()
    {
        updateAttributes();

        switch (mCurrentPostion)
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
                throw new InvalidEnumArgumentException(mCurrentPostion.ToString());
        }
    }

    public bool Visible
    {
        set
        {
            Window.SetActive(value);
            if(value)
            {
                UI3DCreateRole.Get.ShowPositionView();

                updateUI();
                Attributes.PlayScale(AttributeScaleDelayTime);
            }
        }
    }

    public void OnGuardClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.G;

            updateUI();
            
            UI3DCreateRole.Get.PositionView.Select(EPlayerPostion.G);
        }
    }

    public void OnForwardClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.F;

            updateUI();

            UI3DCreateRole.Get.PositionView.Select(EPlayerPostion.F);
        }
    }

    public void OnCenterClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.C;

            updateUI();

            UI3DCreateRole.Get.PositionView.Select(EPlayerPostion.C);
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
        var playerID = UI3DCreateRole.Get.PositionView.GetPlayerID(mCurrentPostion);
        GetComponent<UICreateRole>().ShowStyleView(mCurrentPostion, playerID);
    }

    private IEnumerator playHideAnimation(Action action)
    {
        UIAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(HideAnimationTime);

        action();
    }

    private void updateAttributes()
    {
        int playerID = UI3DCreateRole.Get.PositionView.GetPlayerID(mCurrentPostion);
        if(GameData.DPlayers.ContainsKey(playerID))
        {
            var player = GameData.DPlayers[playerID];

            var value = player.Strength + player.Block;
            Attributes.SetValue(UIAttributes.EAttribute.StrBlk, value / AttributeMax);

            value = player.Defence + player.Steal;
            Attributes.SetValue(UIAttributes.EAttribute.DefStl, value / AttributeMax);

            value = player.Dribble + player.Pass;
            Attributes.SetValue(UIAttributes.EAttribute.DrbPass, value / AttributeMax);

            value = player.Speed + player.Stamina;
            Attributes.SetValue(UIAttributes.EAttribute.SpdSta, value / AttributeMax);

            value = player.Point2 + player.Point3;
            Attributes.SetValue(UIAttributes.EAttribute.Pt2Pt3, value / AttributeMax);

            value = player.Rebound + player.Dunk;
            Attributes.SetValue(UIAttributes.EAttribute.RebDnk, value / AttributeMax);
        }
        else
            Debug.LogErrorFormat("Can't find Player by ID:{0}", playerID);
    }
}