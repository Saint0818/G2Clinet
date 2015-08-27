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
    private int mPlayerID;

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

        if(pos == EPlayerPostion.G)
            mPlayerID = 1;
        else if(pos == EPlayerPostion.F)
            mPlayerID = 2;
        else if(pos == EPlayerPostion.C)
            mPlayerID = 3;
        else
            Debug.LogErrorFormat("UnSupport Position:{0}", pos);

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
        updateModel();
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
            if(i >= itemIDs.Length || !GameData.DItemData.ContainsKey(itemIDs[i]))
            {
                PartItemLabels[i].transform.parent.gameObject.SetActive(false);
                continue;
            }

            PartItemLabels[i].transform.parent.gameObject.SetActive(true);
            PartItemLabels[i].text = GameData.DItemData[itemIDs[i]].NameTW;
        }
    }

    private void updateModel()
    {
        if(mModel)
            Destroy(mModel);

        int skinColorAvatar = GameData.DItemData[mData[EPart.SkinColor][mCurrentSkinColorIndex]].Avatar;
        int hairAvatar = GameData.DItemData[mData[EPart.Hair][mCurrentHairIndex]].Avatar;
        int clothAvatar = GameData.DItemData[mData[EPart.Cloth][mCurrentClothIndex]].Avatar;
        int pantsAvatar = GameData.DItemData[mData[EPart.Pants][mCurrentPantsIndex]].Avatar;
        int shoesAvatar = GameData.DItemData[mData[EPart.Shoes][mCurrentShoesIndex]].Avatar;
        mModel = UICreateRole.CreateModel(ModelPreview, "StyleViewModel", mPlayerID, skinColorAvatar, 
            hairAvatar, clothAvatar, pantsAvatar, shoesAvatar);
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
            updateModel();
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
            updateModel();
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
            updateModel();
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
            updateModel();
        }
    }

    public void OnPart1Clicked()
    {
        if(UIToggle.current.value)
        {
            setCurrentIndex(0);

            updateUI();
            updateModel();
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
            updateModel();
        }
    }

    public void OnPart3Clicked()
    {
        if (UIToggle.current.value)
        {
            setCurrentIndex(2);

            updateUI();
            updateModel();
        }
    }

    public void OnPart4Clicked()
    {
        if (UIToggle.current.value)
        {
            setCurrentIndex(3);

            updateUI();
            updateModel();
        }
    }

    public void OnPart5Clicked()
    {
        if (UIToggle.current.value)
        {
            setCurrentIndex(4);

            updateUI();
            updateModel();
        }
    }

    public void OnPart6Clicked()
    {
        if (UIToggle.current.value)
        {
            setCurrentIndex(5);

            updateUI();
            updateModel();
        }
    }

    public void OnSkinColor1Clicked()
    {
        if(UIToggle.current.value)
        {
            mCurrentSkinColorIndex = 0;

            updateModel();
        }
    }

    public void OnSkinColor2Clicked()
    {
        if (UIToggle.current.value)
        {
            mCurrentSkinColorIndex = 1;

            updateModel();
        }
    }

    public void OnSkinColor3Clicked()
    {
        if (UIToggle.current.value)
        {
            mCurrentSkinColorIndex = 2;

            updateModel();
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