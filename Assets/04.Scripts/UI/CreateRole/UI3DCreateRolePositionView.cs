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
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DCreateRolePositionView : MonoBehaviour
{
    public Transform SelectSFX;
    public Animator SelectSFXAnimator;

    private readonly Dictionary<EPlayerPostion, UI3DCreateRoleCommon.Player> mPlayers = new Dictionary<EPlayerPostion, UI3DCreateRoleCommon.Player>();

    private UI3DCreateRoleCommon mCommon;

    [UsedImplicitly]
	private void Awake()
    {
        mCommon = GetComponent<UI3DCreateRoleCommon>();

        // 現在的版本是讓玩家可以選擇 ID: 1, 2, 3 的角色.
        mPlayers.Add(EPlayerPostion.G, new UI3DCreateRoleCommon.Player(mCommon.GuardParent, mCommon.GuardShadow, "Guard", 1, EPlayerPostion.G));
        mPlayers.Add(EPlayerPostion.F, new UI3DCreateRoleCommon.Player(mCommon.ForwardParent, mCommon.ForwardShadow, "Forward", 2, EPlayerPostion.F));
        mPlayers.Add(EPlayerPostion.C, new UI3DCreateRoleCommon.Player(mCommon.CenterParent, mCommon.CenterShadow, "Center", 3, EPlayerPostion.C));
    }

    public void Show()
    {
        foreach(KeyValuePair<EPlayerPostion, UI3DCreateRoleCommon.Player> pair in mPlayers)
        {
            pair.Value.Visible = true;
            pair.Value.PlayAnimation("SelectDown");
        }

        SelectSFX.gameObject.SetActive(true);
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
}
