using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRole : UIBase
{
	private static UICreateRole instance;
	private const string UIName = "UICreateRole";

    private UICreateRoleFrameView mFrameView;
    private UICreateRolePositionView mPositionView;
    private UICreateRoleStyleView mStyleView;

    public void ShowFrameView()
    {
        Show(true);

        mFrameView.Show();
        mPositionView.Visible = false;
        mStyleView.Hide();
    }

    public void ShowFrameView([NotNull] UICreateRolePlayerSlot.Data[] playerBanks, int selectedIndex, int showNum)
    {
        Show(true);

        mFrameView.Show(playerBanks, selectedIndex, showNum);
        mPositionView.Visible = false;
        mStyleView.Hide();
    }

    public void ShowPositionView()
    {
        Show(true);

        mFrameView.Hide();
        mPositionView.Visible = true;
        mStyleView.Hide();
    }

    public void ShowStyleView(EPlayerPostion pos, int playerID)
    {
        Show(true);

        mFrameView.Hide();
        mPositionView.Visible = false;
        mStyleView.Show(pos, playerID);
    }

    public void Hide()
    {
        RemoveUI(UIName);
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
        mPositionView.Visible = false;

        mStyleView = GetComponent<UICreateRoleStyleView>();
        mStyleView.Hide();

		GameData.Team.Player.ID = 1;
		GameData.Team.Player.Name = SystemInfo.deviceUniqueIdentifier;
    }

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

    public static GameObject CreateModel(string name, TPlayer player, Transform parent)
    {
        GameObject model = new GameObject { name = name };
		ModelManager.Get.SetAvatar(ref model, player.Avatar, GameData.DPlayers[player.ID].BodyType, EAnimatorType.AvatarControl);

        model.transform.parent = parent;
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
