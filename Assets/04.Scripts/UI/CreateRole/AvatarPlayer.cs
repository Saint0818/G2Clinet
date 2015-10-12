using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角中的 3D 球員.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> new instance. </item>
/// <item> Call UpdateXXX() 更新球員穿戴品. </item>
/// <item> (Optional) 用 Visible 控制要不要顯示. </item>
/// <item> (Optional) Call SetBall() and SetBallVisible() 控制球員手上拿的球 </item>
/// <item> (Optional) Call PlayAnimation() 撥動作. </item>
/// <item> (Optional) Call EnableSelfRotationByTouch() 開啟觸控旋轉(預設關閉). </item>
/// <item> 使用完畢後, 呼叫 Destroy(). </item>
/// </list>
/// </remarks>
public class AvatarPlayer
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

            if(mShadow != null)
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
    [CanBeNull]private readonly GameObject mShadow;
    private readonly int mPlayerID;

    [CanBeNull]private Animator mAnimator;
    [CanBeNull]private Transform mDummy; // Ball 要放在此 GameObject 下.
    [CanBeNull]private GameObject mBall;
    private bool mEnableSelfRotation;

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
    public AvatarPlayer(Transform parent, [CanBeNull]GameObject shadow, string name, 
                        int playerID, EPlayerPostion pos)
    {
        mParent = parent;
        mName = name;
        mPlayerID = playerID;
        mShadow = shadow;
        if(mShadow != null)
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
    public AvatarPlayer(Transform parent, [CanBeNull]GameObject shadow, string name, int playerID, 
        int bodyItemID, int hairItemID, int clothItemID, int pantsItemID, int shoesItemID)
    {
        mParent = parent;
        mName = name;
        mPlayerID = playerID;
        mShadow = shadow;
        if(mShadow != null)
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

    public bool UpdatePart(UICreateRoleStyleView.EEquip equip, int itemID)
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

        return UpdateParts(bodyItemID, hairItemID, clothItemID, pantsItemID, shoesItemID);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bodyItemID"></param>
    /// <param name="hairItemID"></param>
    /// <param name="clothItemID"></param>
    /// <param name="pantsItemID"></param>
    /// <param name="shoesItemID"></param>
    /// <returns> true:真的有做更新; false:資料都一樣, 所以沒有真的更新. </returns>
    public bool UpdateParts(int bodyItemID, int hairItemID, int clothItemID, int pantsItemID, 
        int shoesItemID)
    {
        if(mItemIDs[UICreateRoleStyleView.EEquip.Body] == bodyItemID &&
           mItemIDs[UICreateRoleStyleView.EEquip.Hair] == hairItemID &&
           mItemIDs[UICreateRoleStyleView.EEquip.Cloth] == clothItemID &&
           mItemIDs[UICreateRoleStyleView.EEquip.Pants] == pantsItemID &&
           mItemIDs[UICreateRoleStyleView.EEquip.Shoes] == shoesItemID)
            return false; // 沒有任何部件需要更新.

        mItemIDs[UICreateRoleStyleView.EEquip.Body] = bodyItemID;
        mItemIDs[UICreateRoleStyleView.EEquip.Hair] = hairItemID;
        mItemIDs[UICreateRoleStyleView.EEquip.Cloth] = clothItemID;
        mItemIDs[UICreateRoleStyleView.EEquip.Pants] = pantsItemID;
        mItemIDs[UICreateRoleStyleView.EEquip.Shoes] = shoesItemID;

//        Destroy();

        if(mModel == null)
            mModel = UICreateRole.CreateModel(mParent, mName, mPlayerID, bodyItemID, hairItemID,
                                          clothItemID, pantsItemID, shoesItemID);
        else
            UICreateRole.UpdateModel(mModel, mPlayerID, bodyItemID, hairItemID,
                                          clothItemID, pantsItemID, shoesItemID);
        mModel.AddComponent<SelectEvent>(); // 避免發生 Error.
        mAnimator = mModel.GetComponent<Animator>();
        mDummy = mModel.transform.FindChild("DummyBall");

        EnableSelfRotationByTouch(mEnableSelfRotation);

        return true;
    }

    public void EnableSelfRotationByTouch(bool enable)
    {
        if(mModel == null)
            return;

        mEnableSelfRotation = enable;

        if(mEnableSelfRotation)
        {
            if(mModel.GetComponent<SpinWithMouse>() == null)
                mModel.AddComponent<SpinWithMouse>();
        }
        else
        {
            var spin = mModel.GetComponent<SpinWithMouse>();
            if(spin != null)
                Object.Destroy(spin);
        }
    }
}