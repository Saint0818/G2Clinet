﻿using System.Collections.Generic;
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

    public GameObject SFXBall;
    public GameObject Ball;

    /// <summary>
    /// 創角中的 3D 球員.
    /// </summary>
    /// <remarks>
    /// 使用方法:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> Call UpdateXXX() 更新球員穿戴品. </item>
    /// <item> Call SetBallVisible(). </item>
    /// <item> 使用完畢後, 呼叫 Destroy(). </item>
    /// </list>
    /// </remarks>
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
                if(mModel != null)
                    mModel.SetActive(value);
                mShadow.SetActive(value);
            }
        }

        public Transform Parent
        {
            get { return mParent; }
        }

        private readonly Transform mParent;
        private readonly string mName;
        [CanBeNull]private GameObject mModel;
        private readonly GameObject mShadow;
        private readonly int mPlayerID;

        [CanBeNull]private Animator mAnimator;
        [CanBeNull]private Transform mDummy; // Ball 要放在此 GameObject 下.
        [CanBeNull]private GameObject mBall;

        /// <summary>
        /// 目前角色裝備的物品.
        /// </summary>
        private readonly Dictionary<UICreateRoleStyleView.EEquip, int> mItemIDs = 
            new Dictionary<UICreateRoleStyleView.EEquip, int>
            {
                {UICreateRoleStyleView.EEquip.Body, -1},
                {UICreateRoleStyleView.EEquip.Hair, -1},
                {UICreateRoleStyleView.EEquip.Cloth, -1},
                {UICreateRoleStyleView.EEquip.Pants, -1},
                {UICreateRoleStyleView.EEquip.Shoes, -1}
            };

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
            mShadow.SetActive(true);

            int bodyItemID = CreateRoleDataMgr.Ins.GetBody(pos)[0];
            int hairItemID = CreateRoleDataMgr.Ins.GetHairs(pos)[0];
            int clothItemID = CreateRoleDataMgr.Ins.GetCloths(pos)[0];
            int pantsItemID = CreateRoleDataMgr.Ins.GetPants(pos)[0];
            int shoesItemID = CreateRoleDataMgr.Ins.GetShoes(pos)[0];
            UpdateParts(bodyItemID, hairItemID, clothItemID, pantsItemID, shoesItemID);
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
            mShadow.SetActive(true);

            UpdateParts(bodyItemID, hairItemID, clothItemID, pantsItemID, shoesItemID);
        }

        public void PlayAnimation(string animName)
        {
            if(mAnimator != null)
                mAnimator.SetTrigger(animName);
        }

        public void SetBall([NotNull]GameObject ball)
        {
            mBall = ball;
            mBall.transform.parent = mDummy;
            mBall.transform.localPosition = Vector3.zero;
            mBall.transform.localRotation = Quaternion.identity;
            mBall.transform.localScale = Vector3.one;
        }

        public void SetBallVisible(bool visible)
        {
            if(mBall != null)
                mBall.SetActive(visible);
        }

        public void Destroy()
        {
            if(mModel)
            {
                Object.Destroy(mModel);
                mModel = null;
                mAnimator = null;
                mDummy = null;
            }
        }

        public void UpdatePart(UICreateRoleStyleView.EEquip equip, int itemID)
        {
            var bodyItemID = mItemIDs[UICreateRoleStyleView.EEquip.Body];
            var hairItemID = mItemIDs[UICreateRoleStyleView.EEquip.Hair];
            var clothItemID = mItemIDs[UICreateRoleStyleView.EEquip.Cloth];
            var pantsItemID = mItemIDs[UICreateRoleStyleView.EEquip.Pants];
            var shoesItemID = mItemIDs[UICreateRoleStyleView.EEquip.Shoes];

            if(equip == UICreateRoleStyleView.EEquip.Body)
                bodyItemID = itemID;
            else if(equip == UICreateRoleStyleView.EEquip.Hair)
                hairItemID = itemID;
            else if (equip == UICreateRoleStyleView.EEquip.Cloth)
                clothItemID = itemID;
            else if (equip == UICreateRoleStyleView.EEquip.Pants)
                pantsItemID = itemID;
            else if (equip == UICreateRoleStyleView.EEquip.Shoes)
                shoesItemID = itemID;

            UpdateParts(bodyItemID, hairItemID, clothItemID, pantsItemID, shoesItemID);
        }

        public void UpdateParts(int bodyItemID, int hairItemID, int clothItemID, int pantsItemID, 
                                int shoesItemID)
        {
            if(mItemIDs[UICreateRoleStyleView.EEquip.Body] == bodyItemID &&
               mItemIDs[UICreateRoleStyleView.EEquip.Hair] == hairItemID &&
               mItemIDs[UICreateRoleStyleView.EEquip.Cloth] == clothItemID &&
               mItemIDs[UICreateRoleStyleView.EEquip.Pants] == pantsItemID &&
               mItemIDs[UICreateRoleStyleView.EEquip.Shoes] == shoesItemID)
                return; // 沒有任何部件需要更新.

            mItemIDs[UICreateRoleStyleView.EEquip.Body] = bodyItemID;
            mItemIDs[UICreateRoleStyleView.EEquip.Hair] = hairItemID;
            mItemIDs[UICreateRoleStyleView.EEquip.Cloth] = clothItemID;
            mItemIDs[UICreateRoleStyleView.EEquip.Pants] = pantsItemID;
            mItemIDs[UICreateRoleStyleView.EEquip.Shoes] = shoesItemID;

            Destroy();

            mModel = UICreateRole.CreateModel(mParent, mName, mPlayerID, bodyItemID, hairItemID,
                                              clothItemID, pantsItemID, shoesItemID);
            mModel.AddComponent<SelectEvent>(); // 避免發生 Error.
            mAnimator = mModel.GetComponent<Animator>();
            mDummy = mModel.transform.FindChild("DummyBall");
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

        SFXBall.SetActive(false);
        Ball.SetActive(false);
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