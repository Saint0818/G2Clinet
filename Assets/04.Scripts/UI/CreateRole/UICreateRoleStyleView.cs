using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using Newtonsoft.Json;

[DisallowMultipleComponent]
public class UICreateRoleStyleView : MonoBehaviour
{
    private enum EPart
    {
        Hair,
        Cloth,
        Pants,
        Shoes
    }

	public int PlayerID;
	public string PlayerName;
	private int[] equipmentItems;

    public GameObject Window;
    public Transform ModelPreview;
    public UICreateRolePartButton HairButton;
    public UICreateRolePartButton ClothButton;
    public UICreateRolePartButton PantsButton;
    public UICreateRolePartButton ShoesButton;

    public UILabel[] ColorLabels;
    public UILabel HairLabel;
    public UILabel ClothLabel;
    public UILabel PantsLabel;
    public UILabel ShoesLabel;

    public UILabel[] PartLabels;

    private GameObject mModel;

    private EPart mCurrentPart = EPart.Hair;
    private EPlayerPostion mCurrentPos = EPlayerPostion.G;

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
        mCurrentPos = pos;

        Window.SetActive(true);

        if(mModel)
            Destroy(mModel);
        mModel = UICreateRole.CreateModel(pos, ModelPreview);

        updateUI(pos);
    }

    private void updateUI(EPlayerPostion pos)
    {
        int[] colorItems = CreateRoleDataMgr.Ins.GetBody(pos);
        for(int i = 0; i < ColorLabels.Length; i++)
        {
            ColorLabels[i].text = GameData.DItemData[colorItems[i]].NameTW;
        }

        int[] hairItemIDs = CreateRoleDataMgr.Ins.GetHairs(pos);
        HairLabel.text = GameData.DItemData[hairItemIDs[0]].NameTW;

        int[] clothItemIDs = CreateRoleDataMgr.Ins.GetCloths(pos);
        ClothLabel.text = GameData.DItemData[clothItemIDs[0]].NameTW;

        int[] pantsItemIDs = CreateRoleDataMgr.Ins.GetPants(pos);
        PantsLabel.text = GameData.DItemData[pantsItemIDs[0]].NameTW;

        int[] shoesItemIDs = CreateRoleDataMgr.Ins.GetShoes(pos);
        ShoesLabel.text = GameData.DItemData[shoesItemIDs[0]].NameTW;

        if (mCurrentPart == EPart.Hair)
            updateParts(hairItemIDs);
        else if(mCurrentPart == EPart.Cloth)
            updateParts(clothItemIDs);
        else if(mCurrentPart == EPart.Pants)
            updateParts(pantsItemIDs);
        else if(mCurrentPart == EPart.Shoes)
            updateParts(shoesItemIDs);
        else
            throw new InvalidEnumArgumentException(pos.ToString());
    }

    private void updateParts(int[] itemIDs)
    {
        for(int i = 0; i < PartLabels.Length; i++)
        {
            if(i >= itemIDs.Length)
                return;

            if(GameData.DItemData.ContainsKey(itemIDs[i]))
                PartLabels[i].text = GameData.DItemData[itemIDs[i]].NameTW;
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
//            Debug.Log("OnHairClicked");
            HairButton.Play();
            ClothButton.Hide();
            PantsButton.Hide();
            ShoesButton.Hide();

            mCurrentPart = EPart.Hair;
            updateUI(mCurrentPos);
        }
    }

    public void OnClothClicked()
    {
        if(UIToggle.current.value)
        {
//            Debug.Log("OnClothClicked");
            HairButton.Hide();
            ClothButton.Play();
            PantsButton.Hide();
            ShoesButton.Hide();

            mCurrentPart = EPart.Cloth;
            updateUI(mCurrentPos);
        }
    }

    public void OnPantsClicked()
    {
        if(UIToggle.current.value)
        {
//            Debug.Log("OnPantsClicked");
            HairButton.Hide();
            ClothButton.Hide();
            PantsButton.Play();
            ShoesButton.Hide();

            mCurrentPart = EPart.Pants;
            updateUI(mCurrentPos);
        }
    }

    public void OnShoesClicked()
    {
        if(UIToggle.current.value)
        {
//            Debug.Log("OnShoesClicked");
            HairButton.Hide();
            ClothButton.Hide();
            PantsButton.Hide();
            ShoesButton.Play();

            mCurrentPart = EPart.Shoes;
            updateUI(mCurrentPos);
        }
    }

    private UILabel getCurrentPartLabel()
    {
        if(mCurrentPart == EPart.Hair)
            return HairLabel;
        if(mCurrentPart == EPart.Cloth)
            return ClothLabel;
        if(mCurrentPart == EPart.Pants)
            return PantsLabel;
        if(mCurrentPart == EPart.Shoes)
            return ShoesLabel;
        return null;
    }

    public void OnPart1Clicked()
    {
        if(UIToggle.current.value)
            getCurrentPartLabel().text = PartLabels[0].text;
    }

    public void OnPart2Clicked()
    {
        if (UIToggle.current.value)
            getCurrentPartLabel().text = PartLabels[1].text;
    }

    public void OnPart3Clicked()
    {
        if (UIToggle.current.value)
            getCurrentPartLabel().text = PartLabels[2].text;
    }

    public void OnPart4Clicked()
    {
        if (UIToggle.current.value)
            getCurrentPartLabel().text = PartLabels[3].text;
    }

    public void OnPart5Clicked()
    {
        if (UIToggle.current.value)
            getCurrentPartLabel().text = PartLabels[4].text;
    }

    public void OnPart6Clicked()
    {
        if (UIToggle.current.value)
            getCurrentPartLabel().text = PartLabels[5].text;
    }

    public void OnBackClicked()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    public void OnNextClicked()
    {
		GameData.Team.Player.Items = new GameStruct.TItem[equipmentItems.Length];
		for (int i = 0; i < equipmentItems.Length; i++) {
			equipmentItems[i] = 1 + i*10;
			GameData.Team.Player.Items[i].ID = equipmentItems[i];
		}
		
		WWWForm form = new WWWForm();
		GameData.Team.Player.ID = PlayerID;
		GameData.Team.Player.Name = SystemInfo.deviceUniqueIdentifier;
		GameData.Team.Player.Init();

		form.AddField("PlayerID", GameData.Team.Player.ID);
		form.AddField("Name", GameData.Team.Player.Name);
		form.AddField("Items", JsonConvert.SerializeObject(equipmentItems));
		
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