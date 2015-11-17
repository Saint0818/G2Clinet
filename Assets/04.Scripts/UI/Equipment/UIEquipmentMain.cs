using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是裝備介面主程式, 負責協調 UIEquipPlayer, UIEquipDetail, UIEquipList 之間的行為.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Init() 做整個介面的初始化. </item>
/// </list>
/// 
/// 實作細節:
/// <list type="number">
/// <item> 對 UI 的角度來說, 沒有 kind 之類的概念, 只有順序編號的概念, 所以 index 0 就是某個群組,
/// index 1 就是另外一個群組. 某個群組是什麼, 全部都交給外部使用的人決定. </item>
/// <item> Index 目前有 2 個: SlotIndex 和 ListIndex, SlotIndex 就是畫面左邊, 
/// 球員已裝備的數值裝; ListIndex 是倉庫項目的 Index. </item>
/// </list>
[DisallowMultipleComponent]
public class UIEquipmentMain : MonoBehaviour
{
    public event CommonDelegateMethods.Action OnBackListener;

    /// <summary>
    /// 玩家的基本能力數值.
    /// </summary>
    public Dictionary<EAttributeKind, float> BasicAttr
    {
        get { return mBasicAttr; }
    }
    private Dictionary<EAttributeKind, float> mBasicAttr = new Dictionary<EAttributeKind, float>();

    /// <summary>
    /// 顯示在左邊的裝備. Index 和 ListItems 互相對應, 也就是 EquipItems[0] 和 ListItems[0]
    /// 是同一個群組的裝備. [SlotIndex].
    /// </summary>
    public UIValueItemData[] ValueItems { get; private set; }

    /// <summary>
    /// 顯示在列表的裝備. [SlotIndex][ListIndex].
    /// </summary>
    public List<UIValueItemData[]> ListItems { get; private set; }

    private UIEquipPlayer mPlayerInfo;
    private UIEquipDetail mDetail;
    private UIEquipList mEquipList;

    [UsedImplicitly]
    private void Awake()
    {
        mPlayerInfo = GetComponent<UIEquipPlayer>();
        mPlayerInfo.OnSlotClickListener += onSlotClick;

        mDetail = GetComponent<UIEquipDetail>();
        mDetail.OnItemClickListener += onDetailItemClick;

        mEquipList = GetComponent<UIEquipList>();
        mEquipList.OnClickListener += onListItemClick;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="basicAttr"> 球員的基本數值. </param>
    /// <param name="valueItems"> 球員身上的裝備. </param>
    /// <param name="listItems"> 顯示在替換清單的裝備. </param>
    public void Init(Dictionary<EAttributeKind, float> basicAttr, UIValueItemData[] valueItems, 
                     List<UIValueItemData[]> listItems)
    {
        mBasicAttr = new Dictionary<EAttributeKind, float>(basicAttr);
        ValueItems = valueItems;
        ListItems = listItems;

        mPlayerInfo.UpdateUI();
        mDetail.Set(0, ValueItems[0]); // 預設顯示第一個群組的裝備.
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slotIndex"> 群組編號. </param>
    private void onSlotClick(int slotIndex)
    {
        mDetail.Set(slotIndex, ValueItems[slotIndex]);
        mEquipList.Hide();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slotIndex"> 群組編號. </param>
    private void onDetailItemClick(int slotIndex)
    {
        mEquipList.Show(ListItems[slotIndex], true);
    }

    private void onListItemClick(int listIndex)
    {
//        Debug.LogFormat("onListItemClick, index:{0}", index);

        // 道具交換.
        UIValueItemData item = ValueItems[mDetail.SlotIndex];
        ValueItems[mDetail.SlotIndex] = ListItems[mDetail.SlotIndex][listIndex];
        ListItems[mDetail.SlotIndex][listIndex] = item;

        // 介面刷新.
        mPlayerInfo.UpdateUI();
        mDetail.Set(mDetail.SlotIndex, ValueItems[mDetail.SlotIndex]);
        mEquipList.Show(ListItems[mDetail.SlotIndex], false);
    }

    public void OnBackClick()
    {
        if(OnBackListener != null)
            OnBackListener();
    }
}
