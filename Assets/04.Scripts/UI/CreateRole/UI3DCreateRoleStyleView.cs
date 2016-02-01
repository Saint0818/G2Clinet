using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameEnum;
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
/// <item> Call UpdateModel() or UpdateModels() 做模型換裝. </item>
/// <item> Call SetCamera() 設定 Camera 的位置. </item>
/// <item> Call PlayXXXAnimation() 撥換裝球員的動作. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DCreateRoleStyleView : MonoBehaviour
{
    [Header("3D Camera")]
    public Animator CamerAnimator;

    [CanBeNull] private AvatarPlayer mPlayer;

    private struct CamTransform
    {
        public Vector3 Position;
        public Vector3 EulerAngle;
    }

    /// <summary>
    /// Camera 內差時間, 單位: 秒.
    /// </summary>
    private const float TweenTime = 0.75f;

    private readonly Dictionary<EPlayerPostion, Dictionary<UICreateRole.EPart, CamTransform>> mCameraPos =
        new Dictionary<EPlayerPostion, Dictionary<UICreateRole.EPart, CamTransform>>
	{
		{ EPlayerPostion.G, new Dictionary<UICreateRole.EPart, CamTransform>
			{
				{ UICreateRole.EPart.Body, new CamTransform {Position = new Vector3(-425, -80, 455), EulerAngle = new Vector3(0, 10, 0)} },
				{ UICreateRole.EPart.Hair, new CamTransform {Position = new Vector3(-325, 160, 1100), EulerAngle = new Vector3(0, 10, 0)} },
				{ UICreateRole.EPart.Cloth, new CamTransform {Position = new Vector3(-325, 30, 910), EulerAngle = new Vector3(0, 10, 0)} },
				{ UICreateRole.EPart.Pants, new CamTransform {Position = new Vector3(-370, -250, 755), EulerAngle = new Vector3(0, 10, 0)} },
				{ UICreateRole.EPart.Shoes, new CamTransform {Position = new Vector3(-400, -395, 840), EulerAngle = new Vector3(0, 10, 0)} },
			}},
		{ EPlayerPostion.F, new Dictionary<UICreateRole.EPart, CamTransform>
			{
				{ UICreateRole.EPart.Body, new CamTransform {Position = new Vector3(18, -40, 300), EulerAngle = Vector3.zero} },
				{ UICreateRole.EPart.Hair, new CamTransform {Position = new Vector3(-10, 290, 1080), EulerAngle = Vector3.zero} },
				{ UICreateRole.EPart.Cloth, new CamTransform {Position = new Vector3(0, 115, 755), EulerAngle = Vector3.zero} },
				{ UICreateRole.EPart.Pants, new CamTransform {Position = new Vector3(15, -200, 610), EulerAngle = Vector3.zero} },
				{ UICreateRole.EPart.Shoes, new CamTransform {Position = new Vector3(0, -300, 600), EulerAngle = Vector3.zero} },
			}},
		{ EPlayerPostion.C, new Dictionary<UICreateRole.EPart, CamTransform>
			{
				{ UICreateRole.EPart.Body, new CamTransform {Position = new Vector3(750, 35, 225), EulerAngle = new Vector3(0, -7.5f, 0)} },
				{ UICreateRole.EPart.Hair, new CamTransform {Position = new Vector3(625, 310, 1000), EulerAngle = new Vector3(0, -7.5f, 0)} },
				{ UICreateRole.EPart.Cloth, new CamTransform {Position = new Vector3(680, 95, 670), EulerAngle = new Vector3(0, -7.5f, 0)} },
				{ UICreateRole.EPart.Pants, new CamTransform {Position = new Vector3(680, -160, 585), EulerAngle = new Vector3(0, -7.5f, 0)} },
				{ UICreateRole.EPart.Shoes, new CamTransform {Position = new Vector3(690, -270, 575), EulerAngle = new Vector3(0, -7.5f, 0)} },
			}}
	};

    /// <summary>
    /// 當撥 CreateRoleGetBall 動作後, 要經過幾秒才會顯示球.
    /// </summary>
    private readonly Dictionary<EPlayerPostion, float> mShowBallTime = new Dictionary<EPlayerPostion, float>
    {
        {EPlayerPostion.G, 0.5f},
        {EPlayerPostion.F, 0.5f},
        {EPlayerPostion.C, 1.0f}
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

        mPlayer = new AvatarPlayer(mCommon.GetParent(pos), mCommon.GetShadow(pos), 
                                   "StyleViewPlayer", playerID, pos);
        mPlayer.SetBall(Instantiate(mCommon.SFXBall));
        mPlayer.EnableSelfRotationByTouch(true);
        playGetBallAnimation();
    }

    public void UpdateModel(UICreateRole.EPart part, int itemID)
    {
        if(mPlayer != null && mPlayer.ChangePart(part, itemID))
        {
            mPlayer.SetBall(Instantiate(mCommon.Ball));
            mPlayer.SetBallVisible(true);
        }
    }

    public void UpdateModels(UICreateRole.EPart[] parts, int[] itemIDs)
    {
        if(mPlayer == null)
            return;

        for(int i = 0; i < parts.Length; i++)
        {
            mPlayer.ChangePart(parts[i], itemIDs[i]);
        }

        mPlayer.SetBall(Instantiate(mCommon.Ball));
        mPlayer.SetBallVisible(true);
    }

    public void SetCamera(UICreateRole.EPart part)
    {
        CamerAnimator.transform.DOLocalMove(mCameraPos[mCurrentPos][part].Position, TweenTime);
        CamerAnimator.transform.DOLocalRotate(mCameraPos[mCurrentPos][part].EulerAngle, TweenTime);
    }

    /// <summary>
    /// 一系列的運球動作.
    /// </summary>
    private void playGetBallAnimation()
    {
        if(mPlayer != null)
        {
            mPlayer.PlayAnimation("CreateRoleGetBall");
            StartCoroutine(showBall());
        }
    }

    private IEnumerator showBall()
    {
        yield return new WaitForSeconds(mShowBallTime[mCurrentPos]);
        if(mPlayer != null)
            mPlayer.SetBallVisible(true);
    }

    public void Hide()
    {
        if(mPlayer != null)
            mPlayer.Visible = false;

        CamerAnimator.enabled = true;
    }
}
