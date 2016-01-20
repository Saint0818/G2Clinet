using System.Collections.Generic;
using GameEnum;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角中的 3D 球員.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> new instance. </item>
/// <item> Call ChangeParts() 更新球員穿戴品. </item>
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
    private readonly Dictionary<UICreateRole.EEquip, int> mItemIDs = 
        new Dictionary<UICreateRole.EEquip, int>
        {
            {UICreateRole.EEquip.Body, -1},
            {UICreateRole.EEquip.Hair, -1},
            {UICreateRole.EEquip.Cloth, -1},
            {UICreateRole.EEquip.Pants, -1},
            {UICreateRole.EEquip.Shoes, -1},
            {UICreateRole.EEquip.Head, -1},
            {UICreateRole.EEquip.Hand, -1},
            {UICreateRole.EEquip.Back, -1}
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

        Dictionary<UICreateRole.EEquip, int> itemIDs = new Dictionary<UICreateRole.EEquip, int>
        {
            {UICreateRole.EEquip.Body, CreateRoleTable.Ins.GetBody(pos)[0]},
            {UICreateRole.EEquip.Hair, CreateRoleTable.Ins.GetHairs(pos)[0]},
            {UICreateRole.EEquip.Cloth, CreateRoleTable.Ins.GetCloths(pos)[0]},
            {UICreateRole.EEquip.Pants, CreateRoleTable.Ins.GetPants(pos)[0]},
            {UICreateRole.EEquip.Shoes, CreateRoleTable.Ins.GetShoes(pos)[0]}
        };
        ChangeParts(itemIDs);
    }

    /// <summary>
    /// 給自定套裝.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="shadow"></param>
    /// <param name="name"></param>
    /// <param name="playerID"></param>
    /// <param name="itemIDs"></param>
    public AvatarPlayer(Transform parent, [CanBeNull]GameObject shadow, string name, int playerID, 
                        Dictionary<UICreateRole.EEquip, int> itemIDs)
    {
        mParent = parent;
        mName = name;
        mPlayerID = playerID;
        mShadow = shadow;
        if(mShadow != null)
            mShadow.SetActive(true);

        ChangeParts(itemIDs);
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

    public bool ChangePart(UICreateRole.EEquip equip, int itemID)
    {
        Dictionary<UICreateRole.EEquip, int> itemIDs = new Dictionary<UICreateRole.EEquip, int>
        {
            {equip, itemID}
        };

        return ChangeParts(itemIDs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemIDs"></param>
    /// <returns> true:真的有做更新; false:資料都一樣, 所以沒有真的更新. </returns>
    public bool ChangeParts(Dictionary<UICreateRole.EEquip, int> itemIDs)
    {
        if(!isNeedUpdate(itemIDs))
            return false; // 沒有任何部件需要更新.

        foreach(KeyValuePair<UICreateRole.EEquip, int> pair in itemIDs)
        {
            mItemIDs[pair.Key] = pair.Value;
        }

        if(mModel == null)
            mModel = UICreateRole.CreateModel(mParent, mName, mPlayerID, mItemIDs);
        else
            UICreateRole.UpdateModel(mModel, mPlayerID, mItemIDs);
        mModel.AddComponent<SelectEvent>(); // 避免發生 Error.
        mAnimator = mModel.GetComponent<Animator>();
        mDummy = mModel.transform.FindChild("DummyBall");

        EnableSelfRotationByTouch(mEnableSelfRotation);

        return true;
    }

    private bool isNeedUpdate(Dictionary<UICreateRole.EEquip, int> itemIDs)
    {
        foreach(KeyValuePair<UICreateRole.EEquip, int> pair in itemIDs)
        {
            if(mItemIDs[pair.Key] != pair.Value)
                return true;
        }

        return false;
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