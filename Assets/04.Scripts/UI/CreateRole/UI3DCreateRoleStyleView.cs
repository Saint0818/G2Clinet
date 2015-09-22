using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// �f�t UICreateRoleStyleView �@�_�ϥΪ�����, �M���t�d�y������.
/// </summary>
/// <remarks>
/// �ϥΤ�k:
/// <list type="number">
/// <item> �� UI3DCreateRole.Get.StyleView ���o instance. </item>
/// <item> Call Show() or Hide() ����y���n���n���. </item>
/// </list>
/// </remarks>
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
        mPlayer = new UI3DCreateRoleCommon.Player(mCommon.GetParent(pos), mCommon.GetShadow(pos), 
                                                  "StyleViewPlayer", playerID);
        CamerAnimator.enabled = false;
    }

    public void Hide()
    {
        if(mPlayer != null)
            mPlayer.Visible = false;

        CamerAnimator.enabled = true;
    }
}
