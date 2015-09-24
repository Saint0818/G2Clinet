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
        CamerAnimator.transform.localPosition = new Vector3(0, -18, 325);

        if(mPlayer != null)
            mPlayer.Destroy();

//        mPlayer = new UI3DCreateRoleCommon.Player(mCommon.GetParent(pos), mCommon.GetShadow(pos), 
//                                                  "StyleViewPlayer", playerID, pos);
        // todo 目前暫時將球員定位在固定位置, 也不做 Camera 位置內差.
        mPlayer = new UI3DCreateRoleCommon.Player(mCommon.GetParent(EPlayerPostion.F), mCommon.GetShadow(pos),
                                                  "StyleViewPlayer", playerID, pos);
    }

    public void UpdateModel(UICreateRoleStyleView.EEquip equip, int itemID)
    {
        if(mPlayer != null)
            mPlayer.UpdatePart(equip, itemID);
    }

    public void Hide()
    {
        if(mPlayer != null)
            mPlayer.Visible = false;

        CamerAnimator.enabled = true;
    }
}
