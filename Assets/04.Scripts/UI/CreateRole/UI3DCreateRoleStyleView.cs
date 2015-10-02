using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 搭配 UICreateRoleStyleView 一起使用的介面, 專門負責球員換裝.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 UI3DCreateRole.Get.StyleView 取得 instance. </item>
/// <item> Call Show() or Hide() 控制球員要不要顯示. </item>
/// <item> Call UpdateModel() 幫模型換裝. </item>
/// <item> Call SetCamera() 設定 Camera 的位置. </item>
/// <item> Call PlayAnimation() 撥換裝球員的動作. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DCreateRoleStyleView : MonoBehaviour
{
    [Header("3D Camera")]
    public Animator CamerAnimator;

    public GameObject Ball;

    [CanBeNull] private UI3DCreateRoleCommon.Player mPlayer;

    private struct CamTransform
    {
        public Vector3 Position;
        public Vector3 EulerAngle;
    }

    /// <summary>
    /// Camera 內差時間, 單位: 秒.
    /// </summary>
    private const float TweenTime = 0.3f;

    private readonly Dictionary<EPlayerPostion, Dictionary<UICreateRoleStyleView.EEquip, CamTransform>> mCameraPos =
        new Dictionary<EPlayerPostion, Dictionary<UICreateRoleStyleView.EEquip, CamTransform>>
    {
        { EPlayerPostion.G, new Dictionary<UICreateRoleStyleView.EEquip, CamTransform>
        {
            { UICreateRoleStyleView.EEquip.Body, new CamTransform {Position = new Vector3(-400, 0, 380), EulerAngle = new Vector3(0, 10, 0)} },
            { UICreateRoleStyleView.EEquip.Hair, new CamTransform {Position = new Vector3(-290, 220, 1100), EulerAngle = new Vector3(0, 10, 0)} },
            { UICreateRoleStyleView.EEquip.Cloth, new CamTransform {Position = new Vector3(-325, 30, 910), EulerAngle = new Vector3(0, 10, 0)} },
            { UICreateRoleStyleView.EEquip.Pants, new CamTransform {Position = new Vector3(-345, -285, 740), EulerAngle = new Vector3(0, 10, 0)} },
            { UICreateRoleStyleView.EEquip.Shoes, new CamTransform {Position = new Vector3(-330, -445, 820), EulerAngle = new Vector3(0, 10, 0)} },
        }},
        { EPlayerPostion.F, new Dictionary<UICreateRoleStyleView.EEquip, CamTransform>
        {
            { UICreateRoleStyleView.EEquip.Body, new CamTransform {Position = new Vector3(0, -18, 325), EulerAngle = Vector3.zero} },
            { UICreateRoleStyleView.EEquip.Hair, new CamTransform {Position = new Vector3(0, 300, 1110), EulerAngle = Vector3.zero} },
            { UICreateRoleStyleView.EEquip.Cloth, new CamTransform {Position = new Vector3(0, 65, 780), EulerAngle = Vector3.zero} },
            { UICreateRoleStyleView.EEquip.Pants, new CamTransform {Position = new Vector3(0, -200, 650), EulerAngle = Vector3.zero} },
            { UICreateRoleStyleView.EEquip.Shoes, new CamTransform {Position = new Vector3(0, -380, 600), EulerAngle = Vector3.zero} },
        }},
        { EPlayerPostion.C, new Dictionary<UICreateRoleStyleView.EEquip, CamTransform>
        {
            { UICreateRoleStyleView.EEquip.Body, new CamTransform {Position = new Vector3(740, -10, 170), EulerAngle = new Vector3(0, -7.5f, 0)} },
            { UICreateRoleStyleView.EEquip.Hair, new CamTransform {Position = new Vector3(600, 70, 725), EulerAngle = new Vector3(0, -7.5f, 0)} },
            { UICreateRoleStyleView.EEquip.Cloth, new CamTransform {Position = new Vector3(640, -60, 410), EulerAngle = new Vector3(0, -7.5f, 0)} },
            { UICreateRoleStyleView.EEquip.Pants, new CamTransform {Position = new Vector3(640, -270, 410), EulerAngle = new Vector3(0, -7.5f, 0)} },
            { UICreateRoleStyleView.EEquip.Shoes, new CamTransform {Position = new Vector3(720, -365, 435), EulerAngle = new Vector3(0, -7.5f, 0)} },
        }}
    };

    /// <summary>
    /// 當撥動作完畢後, 要經過幾秒才會顯示球.
    /// </summary>
    private readonly Dictionary<EPlayerPostion, float> mShowBallTime = new Dictionary<EPlayerPostion, float>
    {
        {EPlayerPostion.G, 1.2f},
        {EPlayerPostion.F, 1.2f},
        {EPlayerPostion.C, 1.2f},
    };

    private UI3DCreateRoleCommon mCommon;

    private EPlayerPostion mCurrentPos;

    [UsedImplicitly]
    private void Awake()
    {
        mCommon = GetComponent<UI3DCreateRoleCommon>();
    }

    public void Show(EPlayerPostion pos, int playerID)
    {
        mCurrentPos = pos;

        CamerAnimator.enabled = false;

        if(mPlayer != null)
            mPlayer.Destroy();

        mPlayer = new UI3DCreateRoleCommon.Player(mCommon.GetParent(pos), mCommon.GetShadow(pos), 
                                                  "StyleViewPlayer", playerID, pos);
        mPlayer.SetBall(Instantiate(Ball));
    }

    public void UpdateModel(UICreateRoleStyleView.EEquip equip, int itemID)
    {
        if(mPlayer != null)
            mPlayer.UpdatePart(equip, itemID);
    }

    public void SetCamera(UICreateRoleStyleView.EEquip equip)
    {
        CamerAnimator.transform.DOLocalMove(mCameraPos[mCurrentPos][equip].Position, TweenTime);
        CamerAnimator.transform.DOLocalRotate(mCameraPos[mCurrentPos][equip].EulerAngle, TweenTime);
    }

    public void PlayAnimation(string animName)
    {
        if(mPlayer != null)
        {
            mPlayer.PlayAnimation(animName);
            StartCoroutine(showBall());
        }
    }

    private IEnumerator showBall()
    {
        yield return new WaitForSeconds(mShowBallTime[mCurrentPos]);
        if(mPlayer != null)
            mPlayer.ShowBall();
    }

    public void Hide()
    {
        if(mPlayer != null)
            mPlayer.Visible = false;

        CamerAnimator.enabled = true;
    }
}
