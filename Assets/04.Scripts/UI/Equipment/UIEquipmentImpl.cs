using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UI;
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
/// </list>
[DisallowMultipleComponent]
public class UIEquipmentImpl : MonoBehaviour
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
    /// 是同一個群組的裝備.
    /// </summary>
    public EquipItem[] EquipItems { get; private set; }

    /// <summary>
    /// 顯示在列表的裝備.
    /// </summary>
    public List<EquipItem[]> ListItems { get; private set; }

    private UIEquipPlayer mPlayerInfo;
    private UIEquipDetail mItemDetail;
    private UIEquipList mEquipList;

    [UsedImplicitly]
    private void Awake()
    {
        mPlayerInfo = GetComponent<UIEquipPlayer>();
        mPlayerInfo.OnSlotClickListener += onSlotClick;

        mItemDetail = GetComponent<UIEquipDetail>();
        mItemDetail.OnItemClickListener += onItemClick;

        mEquipList = GetComponent<UIEquipList>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="basicAttr"> 球員的基本數值. </param>
    /// <param name="equipItems"> 球員身上的裝備. </param>
    /// <param name="listItems"> 顯示在替換清單的裝備. </param>
    public void Init(Dictionary<EAttributeKind, float> basicAttr, EquipItem[] equipItems, 
                     List<EquipItem[]> listItems)
    {
        mBasicAttr = new Dictionary<EAttributeKind, float>(basicAttr);
        EquipItems = equipItems;
        ListItems = listItems;

        mPlayerInfo.UpdateUI();
        mItemDetail.Set(0, equipItems[0]); // 預設顯示第一個群組的裝備.
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"> 群組編號. </param>
    private void onSlotClick(int index)
    {
        mItemDetail.Set(index, EquipItems[index]);
        mEquipList.Hide();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"> 群組編號. </param>
    private void onItemClick(int index)
    {
        mEquipList.Show(ListItems[index]);
    }

    public void OnBackClick()
    {
        if (OnBackListener != null)
            OnBackListener();
    }
}
