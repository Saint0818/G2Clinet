using System.Collections;
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

    public UILabel PosNameLabel; // 顯示位置(Guard, Forward, Center)
    public UILabel PosDescriptionLabel;

    public Animator UIAnimator;
    
    public UIToggle ForwardToggle;

    [Tooltip("左下角的前一頁按鈕")]
    public GameObject PreviousButton;

    public Transform HexagonParent;
    private UIAttributes mAttributes;

    private delegate void Action();

    /// <summary>
    /// 離開此頁面撥動畫的時間, 單位:秒.
    /// </summary>
    private const float HideAnimationTime = 0.8f;

    /// <summary>
    /// 屬性全滿的數值.
    /// </summary>
    private const float AttributeMax = 200;

    private EPlayerPostion mCurrentPostion = EPlayerPostion.G;

    [UsedImplicitly]
    private void Awake()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(UIPrefabPath.AttriuteHexagon));
        obj.transform.parent = HexagonParent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        mAttributes = obj.GetComponent<UIAttributes>();

        Hide();

        mCurrentPostion = EPlayerPostion.G;
    }

    [UsedImplicitly]
    private void Start()
    {
    }

    private void updateUI()
    {
        updateAttributes();

        PosNameLabel.text = TextConst.S(UICreateRole.Get.PosInfos[mCurrentPostion].TextIndex);
        PosNameLabel.color = UICreateRole.Get.PosInfos[mCurrentPostion].TextColor;
        PosDescriptionLabel.text = TextConst.S(UICreateRole.Get.PosInfos[mCurrentPostion].DescIndex);
    }

    public void Show(bool showPreviousButton)
    {
        Window.SetActive(true);

        UI3DCreateRole.Get.ShowPositionView();

        updateUI();
        mAttributes.SetVisible(true);
        ForwardToggle.Set(true); // 預設前鋒會被選擇.

        PreviousButton.SetActive(showPreviousButton);
    }

    public void Hide()
    {
        Window.SetActive(false);
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
        StartCoroutine(playExitAnimation(showPreviousPage));
    }

    private void showPreviousPage()
    {
        GetComponent<UICreateRole>().ShowFrameView();
    }

    public void OnNextClick()
    {
        StartCoroutine(playExitAnimation(showNextPage));
    }

    private void showNextPage()
    {
        var playerID = UI3DCreateRole.Get.PositionView.GetPlayerID(mCurrentPostion);
        GetComponent<UICreateRole>().ShowStyleView(mCurrentPostion, playerID);
    }

    private IEnumerator playExitAnimation(Action action)
    {
        UICreateRole.Get.EnableBlock(true);
        UIAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(HideAnimationTime);
        UICreateRole.Get.EnableBlock(false);
        action();
    }

    private void updateAttributes()
    {
        int playerID = UI3DCreateRole.Get.PositionView.GetPlayerID(mCurrentPostion);
        if(GameData.DPlayers.ContainsKey(playerID))
        {
            var player = GameData.DPlayers[playerID];

            var value = player.Strength + player.Block;
            mAttributes.SetValue(UIAttributes.EGroup.StrBlk, value / AttributeMax);

            value = player.Defence + player.Steal;
            mAttributes.SetValue(UIAttributes.EGroup.DefStl, value / AttributeMax);

            value = player.Dribble + player.Pass;
            mAttributes.SetValue(UIAttributes.EGroup.DrbPass, value / AttributeMax);

            value = player.Speed + player.Stamina;
            mAttributes.SetValue(UIAttributes.EGroup.SpdSta, value / AttributeMax);

            value = player.Point2 + player.Point3;
            mAttributes.SetValue(UIAttributes.EGroup.Pt2Pt3, value / AttributeMax);

            value = player.Rebound + player.Dunk;
            mAttributes.SetValue(UIAttributes.EGroup.RebDnk, value / AttributeMax);
        }
        else
            Debug.LogErrorFormat("Can't find Player by ID:{0}", playerID);
    }
}