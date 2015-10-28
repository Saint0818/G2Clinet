using System;
using System.Collections.Generic;
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

    private UICreateRoleFrameView mFrameView;
    private UICreateRolePositionView mPositionView;
    private UICreateRoleStyleView mStyleView;

//    private GameObject mFullScreenBlock;

    public void ShowFrameView()
    {
        Show(true);

        mFrameView.Show();
        mPositionView.Hide();
        mStyleView.Hide();
    }

    public void ShowFrameView([NotNull] UICreateRolePlayerSlot.Data[] data, int selectedIndex, int showNum)
    {
        Show(true);

        mFrameView.Show(data, selectedIndex, showNum);
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
        mFrameView = GetComponent<UICreateRoleFrameView>();
        mFrameView.Hide();

        mPositionView = GetComponent<UICreateRolePositionView>();
        mPositionView.Hide();

        mStyleView = GetComponent<UICreateRoleStyleView>();
        mStyleView.Hide();

//        mFullScreenBlock = GameObject.Find("Window/FullScreenInvisibleWidget");
//        mFullScreenBlock.SetActive(false);

		UILoading.UIShow (false);
		AudioMgr.Get.PlayMusic(EMusicType.MU_game0);
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
                                         Dictionary<EEquip, int> itemIDs)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        extractAvatarData(ref player.Avatar, itemIDs);

        return CreateModel(name, playerID, player.Avatar, parent);
    }

    private static void extractAvatarData(ref TAvatar avatar, Dictionary<EEquip, int> itemIDs)
    {
        if(itemIDs.ContainsKey(EEquip.Body) && GameData.DItemData.ContainsKey(itemIDs[EEquip.Body]))
            avatar.Body = GameData.DItemData[itemIDs[EEquip.Body]].Avatar;

        if(itemIDs.ContainsKey(EEquip.Hair) && GameData.DItemData.ContainsKey(itemIDs[EEquip.Hair]))
            avatar.Hair = GameData.DItemData[itemIDs[EEquip.Hair]].Avatar;

        if(itemIDs.ContainsKey(EEquip.Cloth) && GameData.DItemData.ContainsKey(itemIDs[EEquip.Cloth]))
            avatar.Cloth = GameData.DItemData[itemIDs[EEquip.Cloth]].Avatar;

        if(itemIDs.ContainsKey(EEquip.Pants) && GameData.DItemData.ContainsKey(itemIDs[EEquip.Pants]))
            avatar.Pants = GameData.DItemData[itemIDs[EEquip.Pants]].Avatar;

        if(itemIDs.ContainsKey(EEquip.Shoes) && GameData.DItemData.ContainsKey(itemIDs[EEquip.Shoes]))
            avatar.Shoes = GameData.DItemData[itemIDs[EEquip.Shoes]].Avatar;

        if(itemIDs.ContainsKey(EEquip.Head) && GameData.DItemData.ContainsKey(itemIDs[EEquip.Head]))
            avatar.AHeadDress = GameData.DItemData[itemIDs[EEquip.Head]].Avatar;

        if(itemIDs.ContainsKey(EEquip.Hand) && GameData.DItemData.ContainsKey(itemIDs[EEquip.Hand]))
            avatar.MHandDress = GameData.DItemData[itemIDs[EEquip.Hand]].Avatar;

        if(itemIDs.ContainsKey(EEquip.Back) && GameData.DItemData.ContainsKey(itemIDs[EEquip.Back]))
            avatar.ZBackEquip = GameData.DItemData[itemIDs[EEquip.Back]].Avatar;
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
    public static void UpdateModel(GameObject model, int playerID, Dictionary<EEquip, int> itemIDs)
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

    [CanBeNull]
    public static UICreateRolePlayerSlot.Data[] Convert(TPlayerBank[] playerBanks)
    {
        UICreateRolePlayerSlot.Data[] data = new UICreateRolePlayerSlot.Data[playerBanks.Length];
        for (int i = 0; i < playerBanks.Length; i++)
        {
            if (!GameData.DPlayers.ContainsKey(playerBanks[i].ID))
            {
                Debug.LogErrorFormat("Can't find Player by ID:{0}", playerBanks[i].ID);
                return null;
            }

            data[i].PlayerID = playerBanks[i].ID;
            data[i].RoleIndex = playerBanks[i].RoleIndex;
            data[i].Position = (EPlayerPostion)GameData.DPlayers[playerBanks[i].ID].BodyType;
            data[i].Name = playerBanks[i].Name;
            if(data[i].Name == null)
                data[i].Name = String.Empty;
            data[i].Level = playerBanks[i].Lv;
        }

        return data;
    }

    public enum EEquip
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
        {EPlayerPostion.G, new PosInfo {TextIndex = 21, DescIndex = 18, TextColor = new Color32(57, 94, 204, 255)} },
        {EPlayerPostion.F, new PosInfo {TextIndex = 22, DescIndex = 19, TextColor = new Color32(56, 171, 66, 255)} },
        {EPlayerPostion.C, new PosInfo {TextIndex = 23, DescIndex = 20, TextColor = new Color32(180, 33, 35, 255)} }
    };
}
