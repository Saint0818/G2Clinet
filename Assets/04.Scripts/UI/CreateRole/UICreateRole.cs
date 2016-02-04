using System;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角主程式.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 UICreateRole.Get 取得 instance. </item>
/// <item> Call ShowXXX() 顯示某個頁面. </item>
/// <item> Call Hide() 將整個創角流程關閉. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UICreateRole : UIBase
{
	private static UICreateRole instance;
	private const string UIName = "UICreateRole";

    public UICreateRoleFrameView FrameView { get { return mFrameView; } }
    private UICreateRoleFrameView mFrameView;

    private UICreateRolePositionView mPositionView;
    private UICreateRoleStyleView mStyleView;

    public void ShowFrameView()
    {
        Show(true);

        mFrameView.Show();
        mPositionView.Hide();
        mStyleView.Hide();
    }

    public void ShowFrameView([NotNull] UICreateRolePlayerSlot.Data[] data, int selectedIndex)
    {
        Show(true);

        mFrameView.Show(data, selectedIndex);
        mPositionView.Hide();
        mStyleView.Hide();
    }

    public void ShowPositionView()
    {
        Show(true);

        mFrameView.Hide();
        mPositionView.Show(GameData.Team.Player.Lv > 0); // 大於 0 表示玩家有創角色.
        mStyleView.Hide();
    }

    public void ShowStyleView(EPlayerPostion pos, int playerID)
    {
        Show(true);

        mFrameView.Hide();
        mPositionView.Hide();
        mStyleView.Show(pos, playerID);
    }

    public void Hide()
    {
        UI3DCreateRole.Get.Hide();
        RemoveUI(UIName);
    }

    /// <summary>
    /// Block 的目的是避免使用者點擊任何 UI 元件.(內部使用, 一般使用者不要使用)
    /// </summary>
    /// <param name="enable"></param>
    public void EnableBlock(bool enable)
    {
        mFrameView.FullScreenBlock.SetActive(enable);
    }

	public static UICreateRole Get
	{
		get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UICreateRole;
            }
			
			return instance;
		}
	}

    [UsedImplicitly]
    private void Awake()
    {
    }

    protected override void InitCom()
    {
        base.InitCom();

        mFrameView = GetComponent<UICreateRoleFrameView>();
        mFrameView.Hide();

        mPositionView = GetComponent<UICreateRolePositionView>();
        mPositionView.Hide();

        mStyleView = GetComponent<UICreateRoleStyleView>();
        mStyleView.Hide();

        UILoading.UIShow(false);

        mFrameView.UnlockPlayerListener += unlockDiamond =>
        {
            if(GameData.Team.Diamond >= unlockDiamond)
            {
                var protocol = new AddMaxPlayerBankProtocol();
                protocol.Send(onUnlockPlayer);
            }
            else
                UIHint.Get.ShowHint(TextConst.S(233), Color.red);
        };
    }

    private void onUnlockPlayer(bool ok)
    {
        if(ok)
        {
            var uiData = UICreateRoleBuilder.Build(GameData.Team.PlayerBank);
            ShowFrameView(uiData, GameData.Team.Player.RoleIndex);
        }
    }

    /// <summary>
    /// 內部使用, 一般使用者不要使用. 建立新的球員模型(動作會被重置).
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="playerID"></param>
    /// <param name="itemIDs"></param>
    /// <returns></returns>
    public static GameObject CreateModel(Transform parent, string name, int playerID, 
                                         Dictionary<EPart, int> itemIDs)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        extractAvatarData(ref player.Avatar, itemIDs);

        return CreateModel(name, playerID, player.Avatar, parent);
    }

    private static void extractAvatarData(ref TAvatar avatar, Dictionary<EPart, int> itemIDs)
    {
        if(itemIDs.ContainsKey(EPart.Body) && GameData.DItemData.ContainsKey(itemIDs[EPart.Body]))
            avatar.Body = GameData.DItemData[itemIDs[EPart.Body]].Avatar;

        if(itemIDs.ContainsKey(EPart.Hair) && GameData.DItemData.ContainsKey(itemIDs[EPart.Hair]))
            avatar.Hair = GameData.DItemData[itemIDs[EPart.Hair]].Avatar;

        if(itemIDs.ContainsKey(EPart.Cloth) && GameData.DItemData.ContainsKey(itemIDs[EPart.Cloth]))
            avatar.Cloth = GameData.DItemData[itemIDs[EPart.Cloth]].Avatar;

        if(itemIDs.ContainsKey(EPart.Pants) && GameData.DItemData.ContainsKey(itemIDs[EPart.Pants]))
            avatar.Pants = GameData.DItemData[itemIDs[EPart.Pants]].Avatar;

        if(itemIDs.ContainsKey(EPart.Shoes) && GameData.DItemData.ContainsKey(itemIDs[EPart.Shoes]))
            avatar.Shoes = GameData.DItemData[itemIDs[EPart.Shoes]].Avatar;

        if(itemIDs.ContainsKey(EPart.Head) && GameData.DItemData.ContainsKey(itemIDs[EPart.Head]))
            avatar.AHeadDress = GameData.DItemData[itemIDs[EPart.Head]].Avatar;

        if(itemIDs.ContainsKey(EPart.Hand) && GameData.DItemData.ContainsKey(itemIDs[EPart.Hand]))
            avatar.MHandDress = GameData.DItemData[itemIDs[EPart.Hand]].Avatar;

        if(itemIDs.ContainsKey(EPart.Back) && GameData.DItemData.ContainsKey(itemIDs[EPart.Back]))
            avatar.ZBackEquip = GameData.DItemData[itemIDs[EPart.Back]].Avatar;
    }

    /// <summary>
    /// 內部使用, 一般使用者不要使用. 建立新的球員模型(動作會被重置).
    /// </summary>
    /// <param name="name"></param>
    /// <param name="playerID"></param>
    /// <param name="avatar"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject CreateModel(string name, int playerID, TAvatar avatar, Transform parent)
    {
        GameObject model = new GameObject { name = name };
		ModelManager.Get.SetAvatar(ref model, avatar, GameData.DPlayers[playerID].BodyType, 
                                   EAnimatorType.AvatarControl, false);

        model.transform.parent = parent;
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;
		LayerMgr.Get.SetLayer(model, ELayer.UI3D);

        return model;
    }

    /// <summary>
    /// <para> 內部使用, 一般使用者不要使用. 更新球員模型(動作不會重置). </para>
    /// <para> ItemID = 0 表示該部位沒有任何東西. </para>
    /// </summary>
    /// <param name="model"></param>
    /// <param name="playerID"></param>
    /// <param name="itemIDs"></param>
    /// <returns></returns>
    public static void UpdateModel(GameObject model, int playerID, Dictionary<EPart, int> itemIDs)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        extractAvatarData(ref player.Avatar, itemIDs);

        UpdateModel(model, playerID, player.Avatar);
    }

    /// <summary>
    /// 內部使用, 一般使用者不要使用. 更新球員模型(動作不會重置).
    /// </summary>
    /// <param name="model"></param>
    /// <param name="playerID"></param>
    /// <param name="avatar"></param>
    /// <returns></returns>
    public static void UpdateModel(GameObject model, int playerID, TAvatar avatar)
    {
        ModelManager.Get.SetAvatar(ref model, avatar, GameData.DPlayers[playerID].BodyType,
                                   EAnimatorType.AvatarControl, false);
    }

    public enum EPart
    {
        Body,
        Hair,
        Cloth,
        Pants,
        Shoes,
        Head,
        Hand,
        Back
    }

    public struct PosInfo
    {
        public int TextIndex;
        public int DescIndex;
        public Color32 TextColor;
    }

    public readonly Dictionary<EPlayerPostion, PosInfo> PosInfos = new Dictionary<EPlayerPostion, PosInfo>
    {
        {EPlayerPostion.G, new PosInfo {TextIndex = 21, DescIndex = 2107, TextColor = new Color32(57, 94, 204, 255)} },
        {EPlayerPostion.F, new PosInfo {TextIndex = 22, DescIndex = 2106, TextColor = new Color32(56, 171, 66, 255)} },
        {EPlayerPostion.C, new PosInfo {TextIndex = 23, DescIndex = 2105, TextColor = new Color32(180, 33, 35, 255)} }
    };
}
