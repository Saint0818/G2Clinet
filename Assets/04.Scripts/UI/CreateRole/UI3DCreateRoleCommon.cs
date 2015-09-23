using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 給 UI3DCreateRoleXXX 使用, 主要是放相同的東西.
/// </summary>
[DisallowMultipleComponent]
public class UI3DCreateRoleCommon : MonoBehaviour
{
    public Transform CenterParent;
    public GameObject CenterShadow;

    public Transform ForwardParent;
    public GameObject ForwardShadow;

    public Transform GuardParent;
    public GameObject GuardShadow;

    public class Player
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

        private readonly Transform mParent;
        private readonly string mName;
        private GameObject mModel;
        private readonly GameObject mShadow;
        private readonly int mPlayerID;
        private Animator mAnimator;

        /// <summary>
        /// 給預設套裝.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="shadow"></param>
        /// <param name="name"></param>
        /// <param name="playerID"></param>
        /// <param name="pos"></param>
        public Player(Transform parent, GameObject shadow, string name, int playerID, EPlayerPostion pos)
        {
            mParent = parent;
            mName = name;
            mPlayerID = playerID;
            mShadow = shadow;

            UpdateParts(
                CreateRoleDataMgr.Ins.GetBody(pos)[0],
                CreateRoleDataMgr.Ins.GetHairs(pos)[0],
                CreateRoleDataMgr.Ins.GetCloths(pos)[0],
                CreateRoleDataMgr.Ins.GetPants(pos)[0],
                CreateRoleDataMgr.Ins.GetShoes(pos)[0]);
        }

        /// <summary>
        /// 給自定套裝.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="shadow"></param>
        /// <param name="name"></param>
        /// <param name="playerID"></param>
        /// <param name="bodyItemID"></param>
        /// <param name="hairItemID"></param>
        /// <param name="clothItemID"></param>
        /// <param name="pantsItemID"></param>
        /// <param name="shoesItemID"></param>
        public Player(Transform parent, GameObject shadow, string name, int playerID, 
                      int bodyItemID, int hairItemID, int clothItemID, int pantsItemID, int shoesItemID)
        {
            mParent = parent;
            mName = name;
            mPlayerID = playerID;
            mShadow = shadow;

            UpdateParts(bodyItemID, hairItemID, clothItemID, pantsItemID, shoesItemID);
        }

        public void PlayAnimation(string animName)
        {
            mAnimator.SetTrigger(animName);
        }

        public void UpdateParts(int bodyItemID, int hairItemID, int clothItemID, int pantsItemID, 
                                int shoesItemID)
        {
            mModel = UICreateRole.CreateModel(mParent, mName, mPlayerID, bodyItemID, hairItemID,
                                              clothItemID, pantsItemID, shoesItemID
//                CreateRoleDataMgr.Ins.GetBody(),
//                CreateRoleDataMgr.Ins.GetHairs(mPosition)[0],
//                CreateRoleDataMgr.Ins.GetCloths(mPosition)[0],
//                CreateRoleDataMgr.Ins.GetPants(mPosition)[0],
//                CreateRoleDataMgr.Ins.GetShoes(mPosition)[0]
                );
            mModel.AddComponent<SelectEvent>(); // 避免發生 Error.
            mAnimator = mModel.GetComponent<Animator>();
        }
    }

    private readonly Dictionary<EPlayerPostion, GameObject> mShadows = new Dictionary<EPlayerPostion, GameObject>();
    private readonly Dictionary<EPlayerPostion, Transform> mParents = new Dictionary<EPlayerPostion, Transform>();

    [UsedImplicitly]
    private void Awake()
    {
        mShadows.Add(EPlayerPostion.G, GuardShadow);
        mShadows.Add(EPlayerPostion.F, ForwardShadow);
        mShadows.Add(EPlayerPostion.C, CenterShadow);

        mParents.Add(EPlayerPostion.G, GuardParent);
        mParents.Add(EPlayerPostion.F, ForwardParent);
        mParents.Add(EPlayerPostion.C, CenterParent);
    }

//    public void ShowShadow(EPlayerPostion pos)
//    {
//        foreach(KeyValuePair<EPlayerPostion, GameObject> pair in mShadows)
//        {
//            pair.Value.SetActive(pair.Key == pos);
//        }
//    }
//
//    public void SetAllShadowVisible(bool visible)
//    {
//        foreach (KeyValuePair<EPlayerPostion, GameObject> pair in mShadows)
//        {
//            pair.Value.SetActive(visible);
//        }
//    }

    public Transform GetParent(EPlayerPostion pos)
    {
        return mParents[pos];
    }

    public GameObject GetShadow(EPlayerPostion pos)
    {
        return mShadows[pos];
    }
}