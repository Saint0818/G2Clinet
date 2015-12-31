using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 裝備介面, 此類別主要是負責將玩家的資料轉換成介面程式使用的資料.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show(). </item>
/// <item> Call Hide(). </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIEquipment : UIBase
{
    private static UIEquipment instance;
    private const string UIName = "UIEquipment";

    private UIEquipmentMain mMain;

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIEquipmentMain>();
        mMain.OnBackListener += onBackClick;
        mMain.OnMaterialListener += onMaterialClick;
    }

    [UsedImplicitly]
    private void Start()
    {
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void Show()
    {
        Show(true);

        initUI();

//        var addValueItemInlay = new AddValueItemInlayProtocol();
//        addValueItemInlay.Send(13, 0, onAddValueItemInlay);

//        var valueItemUpgrade = new ValueItemUpgradeProtocol();
//        valueItemUpgrade.Send(11, onValueItemUpgrade);
    }

//    private void onAddValueItemInlay(bool ok)
//    {
//        Debug.Log("onAddValueItemInlay");
//    }
//
//    private void onValueItemUpgrade(bool ok)
//    {
//        Debug.Log("onValueItemUpgrade");
//    }

    private void initUI()
    {
        Dictionary<EAttribute, float> basicAttr = findBasicAttr();
        if(basicAttr == null)
            return;

        mMain.Init(basicAttr, findPlayerValueItems(), findAllStorageValueItems());
    }

    /// <summary>
    /// 找出倉庫的全部數值裝備.
    /// </summary>
    /// <returns></returns>
    private List<UIValueItemData[]> findAllStorageValueItems()
    {
        List<UIValueItemData[]> sumItems = new List<UIValueItemData[]>();
        for(int kind = 11; kind <= 18; kind++)
        {
            var equipItems = findStorageItemsByKind(kind);

//            EquipItem equipItem = findPlayerValueItemByKind(kind);
//            if(equipItem.IsValid())
//                equipItems.Add(equipItem);

            sumItems.Add(equipItems.ToArray());
        }

        return sumItems;
    }

    /// <summary>
    /// 根據 TItemData.Kind, 從倉庫找出全部的數值裝備.
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    private List<UIValueItemData> findStorageItemsByKind(int kind)
    {
        List<UIValueItemData> items = new List<UIValueItemData>();
        for(int i = 0; i < GameData.Team.Items.Length; i++)
        {
            TItem storageItem = GameData.Team.Items[i];
            if(!GameData.DItemData.ContainsKey(storageItem.ID))
            {
                Debug.LogErrorFormat("Can't find ItemData, {0}", storageItem);
                continue;
            }

            if(GameData.DItemData[storageItem.ID].Kind == kind)
            {
                UIValueItemData uiItem = UIEquipUtility.Build(GameData.DItemData[storageItem.ID], storageItem.InlayItemIDs ?? new int[0]);
                uiItem.StorageIndex = i;
                items.Add(uiItem);
            }
        }

        return items;
    }

    /// <summary>
    /// 找出玩家身上裝備的數值裝備. 
    /// 和企劃約定, 道具要從 kind = 11 開始逆時針放.
    /// </summary>
    /// <returns></returns>
    private UIValueItemData[] findPlayerValueItems()
    {
        List<UIValueItemData> items = new List<UIValueItemData>();
        for(int kind = 11; kind <= 18; kind++) // 11 ~ 18 是數值裝的種類.
        {
            UIValueItemData item = findPlayerValueItemByKind(kind);
            item.StorageIndex = -1; // -1 表示這個裝備目前裝在球員身上, 不在倉庫內.
            items.Add(item);
        }
        return items.ToArray();
    }

    private UIValueItemData findPlayerValueItemByKind(int kind)
    {
        if(GameData.Team.Player.ValueItems.ContainsKey(kind) &&
           GameData.DItemData.ContainsKey(GameData.Team.Player.ValueItems[kind].ID))
        {
            TItemData item = GameData.DItemData[GameData.Team.Player.ValueItems[kind].ID];
            int[] inlayItemIDs = GameData.Team.Player.ValueItems[kind].InlayItemIDs;
            return UIEquipUtility.Build(item, inlayItemIDs);
        }
            
        return new UIValueItemData();
    }

    [CanBeNull]
    private Dictionary<EAttribute, float> findBasicAttr()
    {
        if(!GameData.DPlayers.ContainsKey(GameData.Team.Player.ID))
        {
            Debug.LogErrorFormat("Can't find GreatePlayer({0})", GameData.Team.Player.ID);
            return null;
        }

        TGreatPlayer basicPlayer = GameData.DPlayers[GameData.Team.Player.ID];
        return new Dictionary<EAttribute, float>
        {
            {EAttribute.Point2, basicPlayer.Point2 + GameData.Team.Player.GetPotentialValue(EAttribute.Point2)},
            {EAttribute.Point3, basicPlayer.Point3 + GameData.Team.Player.GetPotentialValue(EAttribute.Point3)},
            {EAttribute.Speed, basicPlayer.Speed + GameData.Team.Player.GetPotentialValue(EAttribute.Speed)},
            {EAttribute.Stamina, basicPlayer.Stamina + GameData.Team.Player.GetPotentialValue(EAttribute.Stamina)},
            {EAttribute.Strength, basicPlayer.Strength + GameData.Team.Player.GetPotentialValue(EAttribute.Strength)},
            {EAttribute.Dunk, basicPlayer.Dunk + GameData.Team.Player.GetPotentialValue(EAttribute.Dunk)},
            {EAttribute.Rebound, basicPlayer.Rebound + GameData.Team.Player.GetPotentialValue(EAttribute.Rebound)},
            {EAttribute.Block, basicPlayer.Block + GameData.Team.Player.GetPotentialValue(EAttribute.Block)},
            {EAttribute.Defence, basicPlayer.Defence + GameData.Team.Player.GetPotentialValue(EAttribute.Defence)},
            {EAttribute.Steal, basicPlayer.Steal + GameData.Team.Player.GetPotentialValue(EAttribute.Steal)},
            {EAttribute.Dribble, basicPlayer.Dribble + GameData.Team.Player.GetPotentialValue(EAttribute.Dribble)},
            {EAttribute.Pass, basicPlayer.Pass + GameData.Team.Player.GetPotentialValue(EAttribute.Pass)}
        };
    }

    private void onBackClick()
    {
        if(mMain.IsValueItemChanged())
        {
            var protocol = new ChangeValueItemProtocol();
            protocol.Send(getServerChangeData(), goToMainLobby);
        }
        else
            goToMainLobby();
    }

    private int[] getServerChangeData()
    {
        int[] changeValueItems = new int[8];
        for(int i = 0; i < changeValueItems.Length; i++)
        {
            changeValueItems[i] = mMain.ValueItems[i].StorageIndex;
        }
        return changeValueItems;
    }

    private void onMaterialClick(int slotIndex, int storageMaterialItemIndex)
    {
        Debug.LogFormat("onMaterialClick, slotIndex:{0}, storageMaterialItemIndex:{1}", slotIndex, storageMaterialItemIndex);

        int valueItemKind = slotIndex + 11;
        if(!GameData.Team.Player.ValueItems.ContainsKey(valueItemKind))
        {
            Debug.LogErrorFormat("Can't find ValueItem, kind:{0}", valueItemKind);
            return;
        }

        TValueItem valueItem = GameData.Team.Player.ValueItems[valueItemKind];
        if(!GameData.DItemData.ContainsKey(valueItem.ID))
        {
            Debug.LogErrorFormat("Can't find ItemData, ItemID:{0}", valueItem.ID);
            return;
        }


    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    private void goToMainLobby()
    {
        UIMainLobby.Get.Show();
        Hide();
    }

    public static UIEquipment Get
    {
        get
        {
            if (!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIEquipment;
            }

            return instance;
        }
    }
} // end of the class.

