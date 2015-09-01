using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Avatar Setup View.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRoleStyleView : MonoBehaviour
{
    public enum EEquip
    {
        Body,
        Hair,
        Cloth,
        Pants,
        Shoes
    }

    public GameObject Window;
    public Transform ModelPreview;
    public UICreateRoleStyleViewGroup HairGroup;
    public UICreateRoleStyleViewGroup ClothGroup;
    public UICreateRoleStyleViewGroup PantsGroup;
    public UICreateRoleStyleViewGroup ShoesGroup;
    public UICreateRoleStyleViewGroup BodyGroup;

    private GameObject mModel;
    private int mPlayerID;

    private EEquip mCurrentEquip = EEquip.Hair;

    private readonly Dictionary<EEquip, UICreateRoleStyleViewGroup> mGroups = new Dictionary<EEquip, UICreateRoleStyleViewGroup>();
    private readonly Dictionary<EEquip, TItemData> mEquips = new Dictionary<EEquip, TItemData>();
        
    [UsedImplicitly]
    private void Awake()
    {
        mGroups.Add(EEquip.Hair, HairGroup);
        mGroups.Add(EEquip.Cloth, ClothGroup);
        mGroups.Add(EEquip.Pants, PantsGroup);
        mGroups.Add(EEquip.Shoes, ShoesGroup);
        mGroups.Add(EEquip.Body, BodyGroup);

        HairGroup.OnTitleClickListener += onGroupTitleClick;
        HairGroup.OnEquipClickListener += onEquipClick;

        ClothGroup.OnTitleClickListener += onGroupTitleClick;
        ClothGroup.OnEquipClickListener += onEquipClick;

        PantsGroup.OnTitleClickListener += onGroupTitleClick;
        PantsGroup.OnEquipClickListener += onEquipClick;

        ShoesGroup.OnTitleClickListener += onGroupTitleClick;
        ShoesGroup.OnEquipClickListener += onEquipClick;

        BodyGroup.OnTitleClickListener += onGroupTitleClick;
        BodyGroup.OnEquipClickListener += onEquipClick;
    }

    public void Show(EPlayerPostion pos)
    {
        Window.SetActive(true);

        initData(pos);

        updateModel();
    }

    private void initData(EPlayerPostion pos)
    {
        if (pos == EPlayerPostion.G)
            mPlayerID = 1;
        else if (pos == EPlayerPostion.F)
            mPlayerID = 2;
        else if (pos == EPlayerPostion.C)
            mPlayerID = 3;
        else
            Debug.LogErrorFormat("UnSupport Position:{0}", pos);

//        mBodyItemIDs = CreateRoleDataMgr.Ins.GetBody(pos);

        mEquips.Clear();

        //        TItemData[] items = findItems(mBodyItemIDs);
        //        mEquips.Add(EEquip.Body, items[0]);

        TItemData[] items = findItems(CreateRoleDataMgr.Ins.GetHairs(pos));
        mEquips.Add(EEquip.Hair, items[0]);
        HairGroup.Init(EEquip.Hair, items);
        HairGroup.Play();
        HairGroup.SetSelected();

        items = findItems(CreateRoleDataMgr.Ins.GetCloths(pos));
        mEquips.Add(EEquip.Cloth, items[0]);
        ClothGroup.Init(EEquip.Cloth, items);
        ClothGroup.Hide();

        items = findItems(CreateRoleDataMgr.Ins.GetPants(pos));
        mEquips.Add(EEquip.Pants, items[0]);
        PantsGroup.Init(EEquip.Pants, items);
        PantsGroup.Hide();

        items = findItems(CreateRoleDataMgr.Ins.GetShoes(pos));
        mEquips.Add(EEquip.Shoes, items[0]);
        ShoesGroup.Init(EEquip.Shoes, items);
        ShoesGroup.Hide();

        items = findItems(CreateRoleDataMgr.Ins.GetBody(pos));
        mEquips.Add(EEquip.Body, items[0]);
        BodyGroup.Init(EEquip.Body, items);
        BodyGroup.Hide();

        mCurrentEquip = EEquip.Hair;
    }

    private TItemData[] findItems(int[] itemIDs)
    {
        List<TItemData> data = new List<TItemData>();

        foreach(var itemID in itemIDs)
        {
            if(GameData.DItemData.ContainsKey(itemID))
                data.Add(GameData.DItemData[itemID]);
        }

        return data.ToArray();
    }

    private void onGroupTitleClick(EEquip equip)
    {
//        Debug.LogFormat("onGroupTitleClick, equip:{0}", equip);

        if(mCurrentEquip == equip)
            return;

        mCurrentEquip = equip;

        foreach(KeyValuePair<EEquip, UICreateRoleStyleViewGroup> pair in mGroups)
        {
            if(pair.Key == mCurrentEquip)
                pair.Value.Play();
            else
                pair.Value.Hide();
        }
    }

    private void onEquipClick(EEquip equip, TItemData item)
    {
//        Debug.LogFormat("onEquipClick, equip:{0}, item:{1}", equip, item);

        if(mEquips.ContainsKey(equip))
            mEquips[equip] = item;
        else
            mEquips.Add(equip, item);

        updateModel();
    }

    private void updateModel()
    {
        if (mEquips.Count < 5)
            return;

        if (mModel)
            Destroy(mModel);
    
        mModel = UICreateRole.CreateModel(ModelPreview, "StyleViewModel", mPlayerID,
            mEquips[EEquip.Body].ID,
            mEquips[EEquip.Hair].ID,
            mEquips[EEquip.Cloth].ID,
            mEquips[EEquip.Pants].ID,
            mEquips[EEquip.Shoes].ID);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

//    private void play(EEquip part)
//    {
//        switch(part)
//        {
//            case EEquip.Hair:
//                HairGroup.Play();
//                break;
//            case EEquip.Cloth:
//                HairGroup.Hide();
//                break;
//            case EEquip.Pants:
//                HairGroup.Hide();
//                break;
//            case EEquip.Shoes:
//                HairGroup.Hide();
//                break;
//
//            default:
//                throw new InvalidEnumArgumentException(part.ToString());
//        }
//    }
//
//    public void OnHairClicked()
//    {
//        if(UIToggle.current.value)
//        {
//            play(EEquip.Hair);
//
//            mCurrentEquip = EEquip.Hair;
//        }
//    }
//
//    public void OnClothClicked()
//    {
//        if(UIToggle.current.value)
//        {
//            play(EEquip.Cloth);
//
//            mCurrentEquip = EEquip.Cloth;
//        }
//    }
//
//    public void OnPantsClicked()
//    {
//        if(UIToggle.current.value)
//        {
//            play(EEquip.Pants);
//
//            mCurrentEquip = EEquip.Pants;
//        }
//    }
//
//    public void OnShoesClicked()
//    {
//        if(UIToggle.current.value)
//        {
//            play(EEquip.Shoes);
//
//            mCurrentEquip = EEquip.Shoes;
//        }
//    }

//    public void OnPart1Clicked()
//    {
//        if(UIToggle.current.value)
//        {
//            onPartClicked(0);
//        }
//    }
//
//    public void OnPart2Clicked()
//    {
//        if(UIToggle.current.value)
//        {
//            onPartClicked(1);
//        }
//    }
//
//    public void OnPart3Clicked()
//    {
//        if (UIToggle.current.value)
//        {
//            onPartClicked(2);
//        }
//    }
//
//    public void OnPart4Clicked()
//    {
//        if (UIToggle.current.value)
//        {
//            onPartClicked(3);
//        }
//    }
//
//    public void OnPart5Clicked()
//    {
//        if (UIToggle.current.value)
//        {
//            onPartClicked(4);
//        }
//    }
//
//    public void OnPart6Clicked()
//    {
//        if (UIToggle.current.value)
//        {
//            onPartClicked(5);
//        }
//    }

//    public void OnBody1Click()
//    {
//        if(UIToggle.current.value)
//        {
//            if(mEquips.ContainsKey(EEquip.Body))
//                mEquips[EEquip.Body] = GameData.DItemData[mBodyItemIDs[0]];
//            else
//                mEquips.Add(EEquip.Body, GameData.DItemData[mBodyItemIDs[0]]);
//
//            updateModel();
//        }
//    }
//
//    public void OnBody2Click()
//    {
//        if (UIToggle.current.value)
//        {
//            //            mCurrentSkinColorIndex = 1;
//
//            if (mEquips.ContainsKey(EEquip.Body))
//                mEquips[EEquip.Body] = GameData.DItemData[mBodyItemIDs[1]];
//            else
//                mEquips.Add(EEquip.Body, GameData.DItemData[mBodyItemIDs[1]]);
//
//            updateModel();
//        }
//    }
//
//    public void OnBody3Click()
//    {
//        if (UIToggle.current.value)
//        {
//            //            mCurrentSkinColorIndex = 2;
//
//            if (mEquips.ContainsKey(EEquip.Body))
//                mEquips[EEquip.Body] = GameData.DItemData[mBodyItemIDs[2]];
//            else
//                mEquips.Add(EEquip.Body, GameData.DItemData[mBodyItemIDs[2]]);
//
//            updateModel();
//        }
//    }

    public void OnBackClicked()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    public void OnNextClicked()
    {
        int[] equipmentItemIDs = new int[8];
        equipmentItemIDs[0] = mEquips[EEquip.Body].ID;
        equipmentItemIDs[1] = mEquips[EEquip.Hair].ID;
        equipmentItemIDs[3] = mEquips[EEquip.Cloth].ID;
        equipmentItemIDs[4] = mEquips[EEquip.Pants].ID;
        equipmentItemIDs[5] = mEquips[EEquip.Shoes].ID; ;

        GameData.Team.Player.ID = mPlayerID;
        GameData.Team.Player.Items = new GameStruct.TItem[equipmentItemIDs.Length];
		for (int i = 0; i < equipmentItemIDs.Length; i++) 
			GameData.Team.Player.Items[i].ID = equipmentItemIDs[i];
		GameData.Team.Player.Init();

		WWWForm form = new WWWForm();
        form.AddField("PlayerID", mPlayerID);
		form.AddField("Name", GameData.Team.Player.Name);
		form.AddField("Items", JsonConvert.SerializeObject(equipmentItemIDs));
		
		SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
    }

	private void waitCreateRole(bool ok, WWW www)
    {
		if(ok)
        {
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