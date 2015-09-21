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
public class UI3DCreateRolePositionView : MonoBehaviour
{
    public Transform CenterParent;
    public GameObject CenterShadow;

    public Transform ForwardParent;
    public GameObject ForwardShadow;

    public Transform GuardParent;
    public GameObject GuardShadow;

    public Transform SelectSFX;
    public Animator SelectSFXAnimator;

    private class Player
    {
        public int PlayerID
        {
            get { return mPlayerID; }
        }

        public bool Visible
        {
            set
            {
                mModel.SetActive(value);
                mShadow.SetActive(value);
            }
        }

        private readonly GameObject mModel;
        private readonly GameObject mShadow;
        private readonly int mPlayerID;
        private readonly Animator mAnimator;

        public Player(Transform parent, GameObject shadow, string name, int playerID)
        {
            mPlayerID = playerID;
            mShadow = shadow;

            mModel = UICreateRole.CreateModel(parent, name, mPlayerID,
                        CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.G)[0],
                        CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.G)[0],
                        CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.G)[0],
                        CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.G)[0],
                        CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.G)[0]);
            mModel.AddComponent<SelectEvent>(); // 避免發生 Error.

            mAnimator = mModel.GetComponent<Animator>();
        }

        public void PlayAnimation()
        {
            mAnimator.SetTrigger("SelectDown");
        }
    }

    private readonly Dictionary<EPlayerPostion, Transform> mParents = new Dictionary<EPlayerPostion, Transform>();

    private readonly Dictionary<EPlayerPostion, Player> mPlayers = new Dictionary<EPlayerPostion, Player>();
        
    [UsedImplicitly]
	private void Awake()
    {
        mParents.Add(EPlayerPostion.G, GuardParent);
        mParents.Add(EPlayerPostion.F, ForwardParent);
        mParents.Add(EPlayerPostion.C, CenterParent);

        // 現在的版本是讓玩家可以選擇 ID: 1, 2, 3 的角色.
        mPlayers.Add(EPlayerPostion.G, new Player(GuardParent, GuardShadow, "Guard", 1));
        mPlayers.Add(EPlayerPostion.F, new Player(ForwardParent, ForwardShadow, "Forward", 2));
        mPlayers.Add(EPlayerPostion.C, new Player(CenterParent, CenterShadow, "Center", 3));
    }

    public void Show()
    {
        foreach(KeyValuePair<EPlayerPostion, Player> pair in mPlayers)
        {
            pair.Value.Visible = true;
            pair.Value.PlayAnimation();
        }

        SelectSFX.gameObject.SetActive(true);
    }

    public void Hide()
    {
        foreach (KeyValuePair<EPlayerPostion, Player> pair in mPlayers)
        {
            pair.Value.Visible = false;
        }

        SelectSFX.gameObject.SetActive(false);
    }

    public void Select(EPlayerPostion pos)
    {
        SelectSFX.localPosition = mParents[pos].localPosition;
        SelectSFXAnimator.SetTrigger("Start");
    }

    public int GetPlayerID(EPlayerPostion pos)
    {
        return mPlayers[pos].PlayerID;
    }
}
