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
    private ActionQueue mActionQueue;

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIEquipmentMain>();
        mMain.OnBackListener += onBackClick;
        mMain.OnMaterialListener += onMaterialClick;

        var detail = GetComponent<UIEquipDetail>();
        detail.OnUpgradeListener += onUpgradeClick;

        mActionQueue = gameObject.AddComponent<ActionQueue>();
    }

    [UsedImplicitly]
    private void Start()
    {
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void Show()
    {
        Show(true);

        updateUI();

//        var protocol = new ValueItemExchangeProtocol();
//        protocol.Send(new [] {-2, -2, -2, -2, -2, -2, -1, -1}, new [] {-1, -1}, onChangeValueItem);
    }

    private void updateUI()
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
        for(int i = 0; i < GameData.Team.ValueItems.Length; i++)
        {
            TValueItem storageItem = GameData.Team.ValueItems[i];
            if(!GameData.DItemData.ContainsKey(storageItem.ID))
            {
                Debug.LogErrorFormat("Can't find ItemData, {0}", storageItem);
                continue;
            }

            if(GameData.DItemData[storageItem.ID].Kind == kind)
            {
                UIValueItemData uiItem = UIValueItemDataBuilder.Build(GameData.DItemData[storageItem.ID], 
                                                                      storageItem.RevisionInlayItemIDs);
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
            item.StorageIndex = -2; // -2 表示這個裝備目前裝在球員身上, 不在倉庫內.
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
            int[] inlayItemIDs = GameData.Team.Player.ValueItems[kind].RevisionInlayItemIDs;
            return UIValueItemDataBuilder.Build(item, inlayItemIDs);
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
            mActionQueue.Clear();
            mActionQueue.AddAction(new ValueItemChangeAction(getServerChangeData()));
            mActionQueue.Execute(onChangeValueItem);

//            var protocol = new ChangeValueItemProtocol();
//            protocol.Send(getServerChangeData(), onChangeValueItem);
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

    private void onChangeValueItem(bool ok)
    {
        goToMainLobby();
    }

    private void onMaterialClick(UIEquipMaterialItem.EStatus status, int slotIndex, 
                                 int storageMaterialItemIndex, int materialItemID)
    {
//        Debug.LogFormat("onMaterialClick, Status:{0}, slotIndex:{1}, storageMaterialItemIndex:{2}, MaterialItemID:{3}", 
//                        status, slotIndex, storageMaterialItemIndex, materialItemID);

        if(status == UIEquipMaterialItem.EStatus.Lack)
        {
            // 材料不足, 進入導引視窗.
            UIItemSource.Get.ShowMaterial(GameData.DItemData[materialItemID], enable => {if(enable) Hide();});
        }
        else if(status == UIEquipMaterialItem.EStatus.Inlayed)
        {
            Debug.Log("Alreay Inaly");
        }
        else if(status == UIEquipMaterialItem.EStatus.Enough)
        {
            mActionQueue.Clear();
            if (mMain.IsValueItemChanged())
                mActionQueue.AddAction(new ValueItemChangeAction(getServerChangeData()));

            // slot 0 對應到 kind 11, slot 1 對應到 kind 12, 以此類推.
            int valueItemKind = slotIndex + 11;
            mActionQueue.AddAction(new ValueItemAddInlayAction(valueItemKind, storageMaterialItemIndex));
            mActionQueue.Execute(onAddInlay);
        }
        else
            Debug.LogErrorFormat("Not Implemented. Status:{0}", status);
    }

    private void onAddInlay(bool ok)
    {
        Debug.LogFormat("onAddInlay, ok:{0}", ok);

        updateUI();
    }

    private void onUpgradeClick(int slotIndex)
    {
        Debug.LogFormat("onUpgradeClick, slotIndex:{0}", slotIndex);

        int valueItemKind = slotIndex + 11;
//        TValueItem valueItem = GameData.Team.Player.ValueItems[valueItemKind];
//        TItemData item = GameData.DItemData[valueItem.ID];

        mActionQueue.Clear();
        if(mMain.IsValueItemChanged())
            mActionQueue.AddAction(new ValueItemChangeAction(getServerChangeData()));
        mActionQueue.AddAction(new ValueItemUpgradeAction(valueItemKind));

        mActionQueue.Execute(onUpgrade);

//        if(UIEquipChecker.IsUpgradeable(item, valueItem.RevisionInlayItemIDs))
//        {
//            var upgradeCommand = new ValueItemUpgradeProtocol();
//            // 數值裝是從 11 開始. 所以只要加上 11, 就是對應的 kind.
//            upgradeCommand.Send(valueItemKind, onUpgrade);
//        }
//        else if(!UIEquipChecker.HasUpgradeItem(item))
//        {
//            // 是最高等級, 所以不能升級.
//            Debug.Log("Top Level Item.");
//        }
//        else if(!UIEquipChecker.IsInlayFull(item, valueItem.RevisionInlayItemIDs))
//        {
//            // 材料沒有鑲嵌完畢.
//            Debug.Log("Inlay not full.");
//        }
//        else if(!UIEquipChecker.HasUpgradeMoney(item))
//        {
//            // 沒錢.
//            Debug.Log("Money not enoguh.");
//        }
//        else
//            Debug.LogError("Not Implemented check...");
    }

    private void onUpgrade(bool ok)
    {
        Debug.LogFormat("onUpgrade, ok:{0}", ok);

        updateUI();
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

