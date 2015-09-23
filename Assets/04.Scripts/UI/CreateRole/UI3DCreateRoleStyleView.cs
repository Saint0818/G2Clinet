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
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DCreateRoleStyleView : MonoBehaviour
{
    public Animator CamerAnimator;

    [CanBeNull]private UI3DCreateRoleCommon.Player mPlayer;
    private UI3DCreateRoleCommon mCommon;

    [UsedImplicitly]
    private void Awake()
    {
        mCommon = GetComponent<UI3DCreateRoleCommon>();
    }

    public void Show(EPlayerPostion pos, int playerID)
    {
        CamerAnimator.enabled = false;

        mPlayer = new UI3DCreateRoleCommon.Player(mCommon.GetParent(pos), mCommon.GetShadow(pos), 
                                                  "StyleViewPlayer", playerID, pos);
    }

    public void Hide()
    {
        if(mPlayer != null)
            mPlayer.Visible = false;

        CamerAnimator.enabled = true;
    }
}
