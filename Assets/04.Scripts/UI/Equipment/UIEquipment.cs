using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UI;
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
    private List<EquipItem[]> findAllStorageValueItems()
    {
        List<EquipItem[]> sumItems = new List<EquipItem[]>();
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
    /// 根據 item.kind, 從倉庫找出全部的數值裝備.
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    private List<EquipItem> findStorageItemsByKind(int kind)
    {
        List<EquipItem> items = new List<EquipItem>();
        foreach(TItem item in GameData.Team.Items)
        {
            if(!GameData.DItemData.ContainsKey(item.ID))
            {
                Debug.LogErrorFormat("Can't find ItemData, {0}", item);
                continue;
            }

            if(GameData.DItemData[item.ID].Kind == kind)
            {
                items.Add(UIEquipUtility.Convert(GameData.DItemData[item.ID]));
            }
        }

        return items;
    }

    /// <summary>
    /// 找出玩家身上裝備的數值裝備. 
    /// 和企劃約定, 道具要從 kind = 11 開始逆時針放.
    /// </summary>
    /// <returns></returns>
    private EquipItem[] findPlayerValueItems()
    {
        List<EquipItem> items = new List<EquipItem>();
        for(int kind = 11; kind <= 18; kind++) // 11 ~ 18 是數值裝的種類.
        {
//            if(GameData.Team.Player.EquipItems.ContainsKey(kind) &&
//               GameData.DItemData.ContainsKey(GameData.Team.Player.EquipItems[kind].ID))
//            {
//                TItemData item = GameData.DItemData[GameData.Team.Player.EquipItems[kind].ID];
//                items.Add(UIEquipUtility.Convert(item));
//            }
//            else
//                items.Add(new EquipItem());

            items.Add(findPlayerValueItemByKind(kind));
        }
        return items.ToArray();
    }

    private EquipItem findPlayerValueItemByKind(int kind)
    {
        if(GameData.Team.Player.EquipItems.ContainsKey(kind) &&
           GameData.DItemData.ContainsKey(GameData.Team.Player.EquipItems[kind].ID))
        {
            TItemData item = GameData.DItemData[GameData.Team.Player.EquipItems[kind].ID];
            return UIEquipUtility.Convert(item);
        }
            
        return new EquipItem();
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

