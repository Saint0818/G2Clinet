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
/// <item> Call UpdateModel() ���ҫ�����. </item>
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
        // todo �ثe�ȮɱN�y���w��b�T�w��m, �]���� Camera ��m���t.
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
