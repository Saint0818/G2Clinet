using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 給 UI3DCreateRoleXXX 使用, 主要是放相同的東西.
/// </summary>
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

        public void PlayAnimation(string animName)
        {
//            mAnimator.SetTrigger("SelectDown");
            mAnimator.SetTrigger(animName);
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