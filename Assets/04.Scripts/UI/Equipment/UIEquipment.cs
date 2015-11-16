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

    private UIEquipmentImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UIEquipmentImpl>();
        mImpl.OnBackListener += goToMainLobby;
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

//        Debug.LogFormat("Point2:{0}", GameData.Team.Player.GetSumValueItems(EAttributeKind.Point2));
//        Debug.LogFormat("Point3:{0}", GameData.Team.Player.GetSumValueItems(EAttributeKind.Point3));
//        Debug.LogFormat("Dunk:{0}", GameData.Team.Player.GetSumValueItems(EAttributeKind.Dunk));
//        Debug.LogFormat("Block:{0}", GameData.Team.Player.GetSumValueItems(EAttributeKind.Block));
//        Debug.LogFormat("Steal:{0}", GameData.Team.Player.GetSumValueItems(EAttributeKind.Steal));
//        Debug.LogFormat("Rebound:{0}", GameData.Team.Player.GetSumValueItems(EAttributeKind.Rebound));
    }

    private void initUI()
    {
        Dictionary<EAttributeKind, float> basicAttr = findBasicAttr();
        if(basicAttr == null)
            return;

        mImpl.Init(basicAttr, findPlayerValueItems(), findAllStorageValueItems());
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
                UIValueItemData uiItem = UIEquipUtility.Convert(GameData.DItemData[storageItem.ID]);
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
            item.StorageIndex = -1; // 表示這個裝備目前裝在球員身上, 不在倉庫內.
            items.Add(item);
        }
        return items.ToArray();
    }

    private UIValueItemData findPlayerValueItemByKind(int kind)
    {
        if(GameData.Team.Player.EquipItems.ContainsKey(kind) &&
           GameData.DItemData.ContainsKey(GameData.Team.Player.EquipItems[kind].ID))
        {
            TItemData item = GameData.DItemData[GameData.Team.Player.EquipItems[kind].ID];
            return UIEquipUtility.Convert(item);
        }
            
        return new UIValueItemData();
    }

    [CanBeNull]
    private Dictionary<EAttributeKind, float> findBasicAttr()
    {
        if(!GameData.DPlayers.ContainsKey(GameData.Team.Player.ID))
        {
            Debug.LogErrorFormat("Can't find GreatePlayer({0})", GameData.Team.Player.ID);
            return null;
        }

        TGreatPlayer basicPlayer = GameData.DPlayers[GameData.Team.Player.ID];
        return new Dictionary<EAttributeKind, float>
        {
            {EAttributeKind.Point2, basicPlayer.Point2},
            {EAttributeKind.Point3, basicPlayer.Point3},
            {EAttributeKind.Speed, basicPlayer.Speed},
            {EAttributeKind.Stamina, basicPlayer.Stamina},
            {EAttributeKind.Strength, basicPlayer.Strength},
            {EAttributeKind.Dunk, basicPlayer.Dunk},
            {EAttributeKind.Rebound, basicPlayer.Rebound},
            {EAttributeKind.Block, basicPlayer.Block},
            {EAttributeKind.Defence, basicPlayer.Defence},
            {EAttributeKind.Steal, basicPlayer.Steal},
            {EAttributeKind.Dribble, basicPlayer.Dribble},
            {EAttributeKind.Pass, basicPlayer.Pass},
        };
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

