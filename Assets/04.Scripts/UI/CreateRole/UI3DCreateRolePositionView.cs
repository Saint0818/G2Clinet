using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;

/// <summary>
/// 搭配 UICreateRolePositionView 一起使用的介面, 專門負責 3D 場景內的互動操作.
/// </summary>
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
            set { mModel.SetActive(value); }
        }

        private readonly GameObject mModel;
        private readonly int mPlayerID;
        private readonly Animator mAnimator;

        public Player(Transform parent, string name, int playerID)
        {
            mPlayerID = playerID;

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
//
//    private readonly Dictionary<EPlayerPostion, GameObject> mModels = new Dictionary<EPlayerPostion, GameObject>();
//    private readonly Dictionary<EPlayerPostion, int> mPlayerIDs = new Dictionary<EPlayerPostion, int>
//    {
//        {EPlayerPostion.G, 1},
//        {EPlayerPostion.F, 2},
//        {EPlayerPostion.C, 3}
//    };

    private readonly Dictionary<EPlayerPostion, Player> mPlayers = new Dictionary<EPlayerPostion, Player>();
        
    [UsedImplicitly]
	private void Awake()
    {
        mParents.Add(EPlayerPostion.G, GuardParent);
        mParents.Add(EPlayerPostion.F, ForwardParent);
        mParents.Add(EPlayerPostion.C, CenterParent);

        // 現在的版本是讓玩家可以選擇 ID: 1, 2, 3 的角色.
//        var obj = UICreateRole.CreateModel(GuardParent, "GuardModel", 1,
//            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.G)[0],
//            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.G)[0],
//            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.G)[0],
//            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.G)[0],
//            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.G)[0]);
//        obj.AddComponent<SelectEvent>(); // 避免發生 Error.
//        mModels.Add(EPlayerPostion.G, obj);

        mPlayers.Add(EPlayerPostion.G, new Player(GuardParent, "Guard", 1));

        //        obj = UICreateRole.CreateModel(ForwardParent, "ForwardModel", 2,
        //            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.F)[0],
        //            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.F)[0],
        //            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.F)[0],
        //            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.F)[0],
        //            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.F)[0]);
        //        obj.AddComponent<SelectEvent>(); // 避免發生 Error.
        //        mModels.Add(EPlayerPostion.F, obj);

        mPlayers.Add(EPlayerPostion.F, new Player(ForwardParent, "Forward", 2));

        //        obj = UICreateRole.CreateModel(CenterParent, "CenterModel", 3,
        //            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.C)[0],
        //            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.C)[0],
        //            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.C)[0],
        //            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.C)[0],
        //            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.C)[0]);
        //        obj.AddComponent<SelectEvent>(); // 避免發生 Error.
        //        mModels.Add(EPlayerPostion.C, obj);

        mPlayers.Add(EPlayerPostion.C, new Player(CenterParent, "Center", 3));
    }

    public void Show()
    {
        foreach(KeyValuePair<EPlayerPostion, Player> pair in mPlayers)
        {
            pair.Value.Visible = true;
            pair.Value.PlayAnimation();
        }

        CenterShadow.SetActive(true);
        ForwardShadow.SetActive(true);
        GuardShadow.SetActive(true);
    }

    public void Hide()
    {
        foreach (KeyValuePair<EPlayerPostion, Player> pair in mPlayers)
        {
            pair.Value.Visible = false;
        }

        CenterShadow.SetActive(false);
        ForwardShadow.SetActive(false);
        GuardShadow.SetActive(false);
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
