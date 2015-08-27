using System.ComponentModel;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRole : UIBase
{
	private static UICreateRole instance;
	private const string UIName = "UICreateRole";

    private UICreateRoleFrameView mFrameView;
    private UICreateRolePositionView mPositionView;
    private UICreateRoleStyleView mStyleView;

	private int[] equipmentItems = new int[8];

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
		
		set {
			if (instance) {
				if (!value)
					RemoveUI(UIName);
				else
					instance.Show(value);
			} else
			if (value)
				Get.Show(value);
		}
	}

    public void ShowFrameView()
    {
        Show(true);

        mFrameView.Visible = true;
        mPositionView.Visible = false;
        mStyleView.Hide();
    }

    public void ShowPositionView()
    {
        Show(true);

        mFrameView.Visible = false;
        mPositionView.Visible = true;
        mStyleView.Hide();
    }

    public void ShowStyleView(EPlayerPostion pos)
    {
        Show(true);

        mFrameView.Visible = false;
        mPositionView.Visible = false;
        mStyleView.Show(pos);
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
        mFrameView.Visible = true;

        mPositionView = GetComponent<UICreateRolePositionView>();
        mPositionView.Visible = false;

        mStyleView = GetComponent<UICreateRoleStyleView>();
        mStyleView.Hide();

		GameData.Team.Player.ID = 1;
		GameData.Team.Player.Name = SystemInfo.deviceUniqueIdentifier;
		mStyleView.EquipmentItems[0] = CreateRoleDataMgr.Ins.Body(EPlayerPostion.G, 0);
		mStyleView.EquipmentItems[1] = CreateRoleDataMgr.Ins.Hair(EPlayerPostion.G, 0);
		mStyleView.EquipmentItems[3] = CreateRoleDataMgr.Ins.Cloth(EPlayerPostion.G, 0);
		mStyleView.EquipmentItems[4] = CreateRoleDataMgr.Ins.Pants(EPlayerPostion.G, 0);
		mStyleView.EquipmentItems[5] = CreateRoleDataMgr.Ins.Shoes(EPlayerPostion.G, 0);
    }

	protected override void InitCom()
	{
	    
    }

    public static GameObject CreateModel(Transform parent, string name, int playerID, 
        int skinItemID, int hairItemID, int clothItemID, int pantsItemID, int shoesItemID)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        player.Avatar.Body = GameData.DItemData[skinItemID].Avatar;
        player.Avatar.Hair = GameData.DItemData[hairItemID].Avatar;
        player.Avatar.Cloth = GameData.DItemData[clothItemID].Avatar;
        player.Avatar.Pants = GameData.DItemData[pantsItemID].Avatar;
        player.Avatar.Shoes = GameData.DItemData[shoesItemID].Avatar;

        return CreateModel(name, player, parent);
    }

    public static GameObject CreateModel(string name, TPlayer player, Transform parent)
    {
//        Debug.LogFormat("Player:{0}, Avatar:{1}", player, player.Avatar);

        GameObject model = new GameObject { name = name };
        ModelManager.Get.SetAvatar(ref model, player.Avatar, GameData.DPlayers[player.ID].BodyType, false);

        model.transform.parent = parent;
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;
        model.layer = LayerMask.NameToLayer("UI");
        foreach (Transform child in model.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
        }

        return model;
    }
}
