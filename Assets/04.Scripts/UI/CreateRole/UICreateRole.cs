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

    private GameObject mFullScreenBlock;

    public void ShowFrameView()
    {
        Show(true);

        mFrameView.Show();
        mPositionView.Hide();
        mStyleView.Hide();
    }

    public void ShowFrameView([NotNull] UICreateRolePlayerSlot.Data[] playerBanks, int selectedIndex, int showNum)
    {
        Show(true);

        mFrameView.Show(playerBanks, selectedIndex, showNum);
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
        mFullScreenBlock.SetActive(enable);
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

        mFullScreenBlock = GameObject.Find("Window/FullScreenInvisibleWidget");
        mFullScreenBlock.SetActive(false);
    }

    /// <summary>
    /// 內部使用, 一般使用者不要使用. 建立新的球員模型(動作會被重置).
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="playerID"></param>
    /// <param name="bodyItemID"></param>
    /// <param name="hairItemID"></param>
    /// <param name="clothItemID"></param>
    /// <param name="pantsItemID"></param>
    /// <param name="shoesItemID"></param>
    /// <returns></returns>
    public static GameObject CreateModel(Transform parent, string name, int playerID, 
        int bodyItemID, int hairItemID, int clothItemID, int pantsItemID, int shoesItemID)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        player.Avatar.Body = GameData.DItemData[bodyItemID].Avatar;
        player.Avatar.Hair = GameData.DItemData[hairItemID].Avatar;
        player.Avatar.Cloth = GameData.DItemData[clothItemID].Avatar;
        player.Avatar.Pants = GameData.DItemData[pantsItemID].Avatar;
        player.Avatar.Shoes = GameData.DItemData[shoesItemID].Avatar;

        return CreateModel(name, player, parent);
    }

    /// <summary>
    /// 內部使用, 一般使用者不要使用. 建立新的球員模型(動作會被重置).
    /// </summary>
    /// <param name="name"></param>
    /// <param name="player"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject CreateModel(string name, TPlayer player, Transform parent)
    {
        GameObject model = new GameObject { name = name };
		ModelManager.Get.SetAvatar(ref model, player.Avatar, GameData.DPlayers[player.ID].BodyType, 
                                   EAnimatorType.AvatarControl, false);

        model.transform.parent = parent;
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;
		LayerMgr.Get.SetLayer(model, ELayer.UI3D);

        return model;
    }

    /// <summary>
    /// 內部使用, 一般使用者不要使用. 更新球員模型(動作不會重置).
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="playerID"></param>
    /// <param name="bodyItemID"></param>
    /// <param name="hairItemID"></param>
    /// <param name="clothItemID"></param>
    /// <param name="pantsItemID"></param>
    /// <param name="shoesItemID"></param>
    /// <returns></returns>
    public static GameObject UpdateModel(Transform parent, string name, int playerID,
        int bodyItemID, int hairItemID, int clothItemID, int pantsItemID, int shoesItemID)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        player.Avatar.Body = GameData.DItemData[bodyItemID].Avatar;
        player.Avatar.Hair = GameData.DItemData[hairItemID].Avatar;
        player.Avatar.Cloth = GameData.DItemData[clothItemID].Avatar;
        player.Avatar.Pants = GameData.DItemData[pantsItemID].Avatar;
        player.Avatar.Shoes = GameData.DItemData[shoesItemID].Avatar;

        return CreateModel(name, player, parent);
    }

    /// <summary>
    /// 內部使用, 一般使用者不要使用. 更新球員模型(動作不會重置).
    /// </summary>
    /// <param name="model"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public static GameObject UpdateModel(GameObject model, TPlayer player/*, Transform parent*/)
    {
        ModelManager.Get.SetAvatar(ref model, player.Avatar, GameData.DPlayers[player.ID].BodyType,
                                   EAnimatorType.AvatarControl, false);

//        model.transform.parent = parent;
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;
        LayerMgr.Get.SetLayer(model, ELayer.UI3D);

        return model;
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
            data[i].Level = playerBanks[i].Lv;
        }

        return data;
    }
}
