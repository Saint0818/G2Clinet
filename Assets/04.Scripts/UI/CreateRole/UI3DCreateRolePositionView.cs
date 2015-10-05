using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;

/// <summary>
/// 搭配 UICreateRolePositionView 一起使用的介面, 專門負責球員的互動操作.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 UI3DCreateRole.Get.PositionView 取得 instance. </item>
/// <item> Call Show() or Hide() 控制球員要不要顯示. </item>
/// <item> Call Select() 通知哪位球員被選擇. </item>
/// <item> Call PlayDropAnimation() 撥球員往下跳的動作. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DCreateRolePositionView : MonoBehaviour
{
    public Transform SelectSFX;
    public Animator SelectSFXAnimator;

    private readonly Dictionary<EPlayerPostion, UI3DCreateRoleCommon.Player> mPlayers = new Dictionary<EPlayerPostion, UI3DCreateRoleCommon.Player>();

    /// <summary>
    /// 球員跳入場景的時間, 單位:秒.
    /// </summary>
    private readonly Dictionary<EPlayerPostion, float> mDelayTimes = new Dictionary<EPlayerPostion, float>
    {
        {EPlayerPostion.G, 1.0f },
        {EPlayerPostion.F, 0.0f },
        {EPlayerPostion.C, 2.0f }
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

    [UsedImplicitly]
	private void Awake()
    {
        mCommon = GetComponent<UI3DCreateRoleCommon>();

        // 現在的版本是讓玩家可以選擇 ID: 1, 2, 3 的角色.
        mPlayers.Add(EPlayerPostion.G, new UI3DCreateRoleCommon.Player(mCommon.GuardParent, mCommon.GuardShadow, "Guard", 1, EPlayerPostion.G));
        mPlayers.Add(EPlayerPostion.F, new UI3DCreateRoleCommon.Player(mCommon.ForwardParent, mCommon.ForwardShadow, "Forward", 2, EPlayerPostion.F));
        mPlayers.Add(EPlayerPostion.C, new UI3DCreateRoleCommon.Player(mCommon.CenterParent, mCommon.CenterShadow, "Center", 3, EPlayerPostion.C));

        mPlayers[EPlayerPostion.G].SetBall(Instantiate(mCommon.SFXBall));
        mPlayers[EPlayerPostion.G].SetBallVisible(false);

        mPlayers[EPlayerPostion.F].SetBall(Instantiate(mCommon.SFXBall));
        mPlayers[EPlayerPostion.F].SetBallVisible(false);

        mPlayers[EPlayerPostion.C].SetBall(Instantiate(mCommon.SFXBall));
        mPlayers[EPlayerPostion.C].SetBallVisible(false);
    }

    public void Show()
    {
        foreach(KeyValuePair<EPlayerPostion, UI3DCreateRoleCommon.Player> pair in mPlayers)
        {
            pair.Value.Visible = true;
        }

        SelectSFX.gameObject.SetActive(true);
    }

    /// <summary>
    /// 撥球員往下跳的動作.
    /// </summary>
    public void PlayDropAnimation()
    {
        foreach(KeyValuePair<EPlayerPostion, UI3DCreateRoleCommon.Player> pair in mPlayers)
        {
//            pair.Value.PlayAnimation("SelectDown");
            StartCoroutine(playAnimation(pair.Value, "SelectDown", mDelayTimes[pair.Key]));
        }
    }

    private IEnumerator playAnimation(UI3DCreateRoleCommon.Player player, string animName, float delayTime)
    {
        player.Visible = false;
        yield return new WaitForSeconds(delayTime);
        player.Visible = true;
        player.PlayAnimation(animName);
    } 

    public void Hide()
    {
        foreach (KeyValuePair<EPlayerPostion, UI3DCreateRoleCommon.Player> pair in mPlayers)
        {
            pair.Value.Visible = false;
        }

        SelectSFX.gameObject.SetActive(false);
    }

    public void Select(EPlayerPostion pos)
    {
        SelectSFX.localPosition = mCommon.GetParent(pos).localPosition;
        SelectSFXAnimator.SetTrigger("Start");
    }

    public int GetPlayerID(EPlayerPostion pos)
    {
        return mPlayers[pos].PlayerID;
    }

    /// <summary>
    /// 一系列的運球動作.
    /// </summary>
    public void PlayGetBallAnimation(EPlayerPostion pos)
    {
        mPlayers[pos].PlayAnimation("CreateRoleGetBall");
        StartCoroutine(showBall(pos));
    }

    private IEnumerator showBall(EPlayerPostion pos)
    {
        yield return new WaitForSeconds(mShowBallTime[pos]);
        mPlayers[pos].SetBallVisible(true);
    }
}
