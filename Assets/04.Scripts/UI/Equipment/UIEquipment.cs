using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UI;
using UnityEngine;

/// <summary>
/// 關卡頁面, 會顯示很多的小關卡.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示關卡. </item>
/// <item> Call Hide() 關閉關卡. </item>
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

        updateUI();
    }

    private void updateUI()
    {
        if (!GameData.DPlayers.ContainsKey(GameData.Team.Player.ID))
        {
            Debug.LogErrorFormat("Can't find GreatePlayer({0})", GameData.Team.Player.ID);
            return;
        }

        TGreatPlayer basicPlayer = GameData.DPlayers[GameData.Team.Player.ID];
        Dictionary<EAttributeKind, float> basicAttr = new Dictionary<EAttributeKind, float>
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

        // 和企劃約定, 道具要從 kind = 11 開始逆時針放.
        List<EquipItem> items = new List<EquipItem>();
        for (int kind = 11; kind <= 18; kind++) // 11 ~ 18 是數值裝的種類.
        {
            EquipItem uiItem = new EquipItem();
            items.Add(uiItem);

            if (GameData.Team.Player.EquipItems.ContainsKey(kind) &&
                GameData.DItemData.ContainsKey(GameData.Team.Player.EquipItems[kind].ID))
            {
                TItemData item = GameData.DItemData[GameData.Team.Player.EquipItems[kind].ID];
                uiItem.Name = item.Name;
                uiItem.Icon = item.Icon;
                uiItem.Desc = item.Explain;
                for (int i = 0; i < item.AttrKinds.Length; i++)
                {
                    if (item.AttrKinds[i] == EAttributeKind.None)
                        continue;

                    uiItem.Values.Add(item.AttrKinds[i], item.AttrValues[i]);
                }
            }
        }
        mImpl.Init(basicAttr, items.ToArray());
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

