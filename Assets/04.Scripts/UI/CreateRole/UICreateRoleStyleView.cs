using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using Newtonsoft.Json;

[DisallowMultipleComponent]
public class UICreateRoleStyleView : MonoBehaviour
{
    private enum EPart
    {
        SkinColor,
        Hair,
        Cloth,
        Pants,
        Shoes
    }

	public int[] EquipmentItems = new int[8];

    public GameObject Window;
    public Transform ModelPreview;
    public UICreateRolePartButton HairButton;
    public UICreateRolePartButton ClothButton;
    public UICreateRolePartButton PantsButton;
    public UICreateRolePartButton ShoesButton;

    public UILabel[] ColorLabels;
//    public UILabel HairLabel;
//    public UILabel ClothLabel;
//    public UILabel PantsLabel;
//    public UILabel ShoesLabel;

    public UILabel[] PartItemLabels;

    /// <summary>
    /// value: Item ID.
    /// </summary>
    private readonly Dictionary<EPart, int[]> mData = new Dictionary<EPart, int[]>();

    private GameObject mModel;

    private EPart mCurrentPart = EPart.Hair;
    private int mCurrentSkinColorIndex;
    private int mCurrentHairIndex;
    private int mCurrentClothIndex;
    private int mCurrentPantsIndex;
    private int mCurrentShoesIndex;

    [UsedImplicitly]
    private void Start()
    {
        HairButton.Play();
        ClothButton.Hide();
        PantsButton.Hide();
        ShoesButton.Hide();
    }

    public void Show(EPlayerPostion pos)
    {
        Window.SetActive(true);

        mData.Clear();
        mData.Add(EPart.SkinColor, CreateRoleDataMgr.Ins.GetBody(pos));
        mData.Add(EPart.Hair, CreateRoleDataMgr.Ins.GetHairs(pos));
        mData.Add(EPart.Cloth, CreateRoleDataMgr.Ins.GetCloths(pos));
        mData.Add(EPart.Pants, CreateRoleDataMgr.Ins.GetPants(pos));
        mData.Add(EPart.Shoes, CreateRoleDataMgr.Ins.GetShoes(pos));

        mCurrentSkinColorIndex = 0;
        mCurrentHairIndex = 0;
        mCurrentClothIndex = 0;
        mCurrentPantsIndex = 0;
        mCurrentShoesIndex = 0;
        mCurrentPart = EPart.Hair;

        updateUI();
    }

    private void updateUI()
    {
        for(int i = 0; i < ColorLabels.Length; i++)
        {
            ColorLabels[i].text = GameData.DItemData[mData[EPart.SkinColor][i]].NameTW;
        }

        HairButton.Name = GameData.DItemData[mData[EPart.Hair][mCurrentHairIndex]].NameTW;
        ClothButton.Name = GameData.DItemData[mData[EPart.Cloth][mCurrentClothIndex]].NameTW;
        PantsButton.Name = GameData.DItemData[mData[EPart.Pants][mCurrentPantsIndex]].NameTW;
        ShoesButton.Name = GameData.DItemData[mData[EPart.Shoes][mCurrentShoesIndex]].NameTW;

        updatePartItems(mData[mCurrentPart]);
    }

    private void updatePartItems(int[] itemIDs)
    {
        for(int i = 0; i < PartItemLabels.Length; i++)
        {
            if(i >= itemIDs.Length)
            {
                PartItemLabels[i].transform.parent.gameObject.SetActive(false);
                continue;
            }

            if(GameData.DItemData.ContainsKey(itemIDs[i]))
            {
                PartItemLabels[i].transform.parent.gameObject.SetActive(true);
                PartItemLabels[i].text = GameData.DItemData[itemIDs[i]].NameTW;
            }
            else
                Debug.LogErrorFormat("ItemID({0}) don't exist");
        }
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void OnHairClicked()
    {
        if(UIToggle.current.value)
        {
            HairButton.Play();
            ClothButton.Hide();
            PantsButton.Hide();
            ShoesButton.Hide();

            mCurrentPart = EPart.Hair;
            mCurrentHairIndex = 0;

            updateUI();
        }
    }

    public void OnClothClicked()
    {
        if(UIToggle.current.value)
        {
            HairButton.Hide();
            ClothButton.Play();
            PantsButton.Hide();
            ShoesButton.Hide();

            mCurrentPart = EPart.Cloth;
            mCurrentClothIndex = 0;

            updateUI();
        }
    }

    public void OnPantsClicked()
    {
        if(UIToggle.current.value)
        {
            HairButton.Hide();
            ClothButton.Hide();
            PantsButton.Play();
            ShoesButton.Hide();

            mCurrentPart = EPart.Pants;
            mCurrentPantsIndex = 0;

            updateUI();
        }
    }

    public void OnShoesClicked()
    {
        if(UIToggle.current.value)
        {
            HairButton.Hide();
            ClothButton.Hide();
            PantsButton.Hide();
            ShoesButton.Play();

            mCurrentPart = EPart.Shoes;
            mCurrentShoesIndex = 0;

            updateUI();
        }
    }

//    private UILabel getCurrentPartLabel()
//    {
//        if(mCurrentPart == EPart.Hair)
//            return HairLabel;
//        if(mCurrentPart == EPart.Cloth)
//            return ClothLabel;
//        if(mCurrentPart == EPart.Pants)
//            return PantsLabel;
//        if(mCurrentPart == EPart.Shoes)
//            return ShoesLabel;
//        return null;
//    }

    public void OnPart1Clicked()
    {
        if(UIToggle.current.value)
        {
            setCurrentIndex(0);
            updateUI();
        }
    }

    private void setCurrentIndex(int index)
    {
        switch(mCurrentPart)
        {
            case EPart.Hair:
                mCurrentHairIndex = index;
                break;
            case EPart.Cloth:
                mCurrentClothIndex = index;
                break;
            case EPart.Pants:
                mCurrentPantsIndex = index;
                break;
            case EPart.Shoes:
                mCurrentShoesIndex = index;
                break;
        }
    }

    public void OnPart2Clicked()
    {
        if(UIToggle.current.value)
        {
            setCurrentIndex(1);
            updateUI();
        }
    }

    public void OnPart3Clicked()
    {
        if (UIToggle.current.value)
        {
            setCurrentIndex(2);
            updateUI();
        }
    }

    public void OnPart4Clicked()
    {
        if (UIToggle.current.value)
        {
            setCurrentIndex(3);
            updateUI();
        }
    }

    public void OnPart5Clicked()
    {
        if (UIToggle.current.value)
        {
            setCurrentIndex(4);
            updateUI();
        }
    }

    public void OnPart6Clicked()
    {
        if (UIToggle.current.value)
        {
            setCurrentIndex(5);
            updateUI();
        }
    }

    public void OnBackClicked()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    public void OnNextClicked()
    {
		GameData.Team.Player.Items = new GameStruct.TItem[EquipmentItems.Length];
		for (int i = 0; i < EquipmentItems.Length; i++) 
			GameData.Team.Player.Items[i].ID = EquipmentItems[i];
		
		GameData.Team.Player.Init();

		WWWForm form = new WWWForm();
		form.AddField("PlayerID", GameData.Team.Player.ID);
		form.AddField("Name", GameData.Team.Player.Name);
		form.AddField("Items", JsonConvert.SerializeObject(EquipmentItems));
		
		SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
    }

	private void waitCreateRole(bool ok, WWW www) {
		if (ok) {
			GameData.Team.Player.Init();
			GameData.SaveTeam();
			UICreateRole.Visible = false;
			
			if (SceneMgr.Get.CurrentScene != SceneName.Lobby)
				SceneMgr.Get.ChangeLevel(SceneName.Lobby);
			else
				LobbyStart.Get.EnterLobby();
		}
	}
}