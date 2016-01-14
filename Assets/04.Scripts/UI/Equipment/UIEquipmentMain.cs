using System.Collections.Generic;
using DG.Tweening;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是裝備介面主程式, 負責協調 UIEquipPlayer, UIEquipDetail, UIEquipList 之間的行為.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call SetData() 做整個介面的初始化. </item>
/// <item> IsDataChanged() and GetExchangeData() 可以得知資料改變的狀態. </item>
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
    /// <summary>
    /// 返回按鈕按下.
    /// </summary>
    public event System.Action OnBackListener;

    public delegate void MaterialAction(UIEquipMaterialItem.EStatus status, int slotIndex, 
                                        int materialIndex, int storageIndex, int materialID);

    /// <summary>
    /// 材料按鈕按下. 參數: 
    /// [材料狀態, SlotIndex, MaterialIndex(第幾個材料 0 ~ 3), 材料在倉庫的哪個 Index(-1 表示找不到), MaterialItemID]
    /// </summary>
    public event MaterialAction OnMaterialListener;

    /// <summary>
    /// 玩家的基本能力數值.
    /// </summary>
    public Dictionary<EAttribute, float> BasicAttr
    {
        get { return mBasicAttr; }
    }
    private Dictionary<EAttribute, float> mBasicAttr = new Dictionary<EAttribute, float>();

    /// <summary>
    /// 顯示在左邊的裝備. Index 和 ListItems 互相對應, 也就是 PlayerValueItems[0] 和 ListItems[0]
    /// 是同一個群組的裝備. [SlotIndex].
    /// </summary>
    public UIValueItemData[] PlayerValueItems { get; private set; }

    /// <summary>
    /// <para> 記錄數值裝剛開始的狀態. 這個變數只要是防止脫下次數型數值裝的問題. </para>
    /// <para> true: 表示剛開始數值裝有東西. </para>
    /// </summary>
    private bool[] mIsInitPlayerValueItemValid;

    /// <summary>
    /// 顯示在列表的裝備. [SlotIndex][ListIndex].
    /// </summary>
    public List<List<UIValueItemData>> ListItems { get; private set; }

    private UIEquipPlayer mPlayerInfo;
    private UIEquipDetail mDetail;
    private UIEquipItemList mEquipList;

    public UIEquipMaterialList MaterialList
    {
        get { return mMaterialList; }
    }
    private UIEquipMaterialList mMaterialList;

    [UsedImplicitly]
    private void Awake()
    {
        mPlayerInfo = GetComponent<UIEquipPlayer>();
        mPlayerInfo.OnSlotClickListener += onSlotClick;

        mDetail = GetComponent<UIEquipDetail>();
        mDetail.OnItemClickListener += onDetailItemClick;
        mDetail.OnDemountListener += onDemountClick;

        mEquipList = GetComponent<UIEquipItemList>();
        mEquipList.OnClickListener += onItemExchange;

        mMaterialList = GetComponent<UIEquipMaterialList>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="basicAttr"> 球員的基本數值. </param>
    /// <param name="playerValueItems"> 球員身上的裝備. </param>
    /// <param name="listItems"> 顯示在替換清單的裝備. </param>
    public void SetData(Dictionary<EAttribute, float> basicAttr, UIValueItemData[] playerValueItems, 
                        List<List<UIValueItemData>> listItems)
    {
        mBasicAttr = new Dictionary<EAttribute, float>(basicAttr);
        PlayerValueItems = playerValueItems;
        ListItems = listItems;
        buildInitData();

        mPlayerInfo.UpdateUI();
        mDetail.Set(mDetail.SlotIndex, PlayerValueItems[mDetail.SlotIndex]); // 更新目前正在顯示的欄位.
        mMaterialList.Set(PlayerValueItems[mDetail.SlotIndex].Materials);
        mEquipList.Hide();
    }

    private void buildInitData()
    {
        mIsInitPlayerValueItemValid = new bool[PlayerValueItems.Length];
        for(var i = 0; i < PlayerValueItems.Length; i++)
        {
            mIsInitPlayerValueItemValid[i] = PlayerValueItems[i].IsValid();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slotIndex"> 群組編號. </param>
    private void onSlotClick(int slotIndex)
    {
        mDetail.Set(slotIndex, PlayerValueItems[slotIndex]);
        mMaterialList.Set(PlayerValueItems[slotIndex].Materials);
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

    private void onDemountClick(int slotIndex)
    {
//        Debug.LogFormat("onDemountClick, SlotIndex:{0}", slotIndex);

        if(PlayerValueItems[slotIndex].IsValid())
        {
            ListItems[slotIndex].Add(PlayerValueItems[slotIndex]);

            PlayerValueItems[slotIndex] = mIsInitPlayerValueItemValid[slotIndex] 
                ? UIValueItemDataBuilder.BuildDemount() : UIValueItemDataBuilder.BuildEmpty();

            mPlayerInfo.UpdateUI();
            mDetail.Set(slotIndex, PlayerValueItems[slotIndex]);
            if(mEquipList.Visible)
                mEquipList.Show(ListItems[slotIndex], true);
        }
    }

    private void onItemExchange(int listIndex)
    {
//        Debug.LogFormat("onItemExchange, index:{0}", index);

        UIValueItemData item = PlayerValueItems[mDetail.SlotIndex];
        PlayerValueItems[mDetail.SlotIndex] = ListItems[mDetail.SlotIndex][listIndex];
        if(item.IsValid())
            ListItems[mDetail.SlotIndex][listIndex] = item;
        else
            ListItems[mDetail.SlotIndex].RemoveAt(listIndex);

        // 介面刷新.
        mPlayerInfo.UpdateUI();
        mDetail.Set(mDetail.SlotIndex, PlayerValueItems[mDetail.SlotIndex]);
        mEquipList.Show(ListItems[mDetail.SlotIndex], false);
        mMaterialList.Set(PlayerValueItems[mDetail.SlotIndex].Materials);
    }

    /// <summary>
    /// (內部使用) slotIndex 上的數值裝備是最強的嗎?
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public bool IsBestValueItem(int slotIndex)
    {
        if(slotIndex < 0 || slotIndex >= PlayerValueItems.Length)
            return false;

        return PlayerValueItems[slotIndex].GetTotalPoints() >= getBestTotalPointsFromListItems(slotIndex);
    }

    /// <summary>
    /// 找出 slotIndex 上最強數值裝的數值總分(總和).
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    private int getBestTotalPointsFromListItems(int slotIndex)
    {
        int maxTotalPoints = int.MinValue;
        foreach(UIValueItemData itemData in ListItems[slotIndex])
        {
            if(maxTotalPoints < itemData.GetTotalPoints())
                maxTotalPoints = itemData.GetTotalPoints();
        }

        return maxTotalPoints;
    }

    public bool IsDataChanged()
    {
        for(var i = 0; i < PlayerValueItems.Length; i++)
        {
            if(PlayerValueItems[i].StorageIndex != UIValueItemData.StorageIndexNone)
                return true;
        }
        return false;
    }

    public int[] GetExchangeData()
    {
        int[] exchange = new int[8];
        for(int i = 0; i < exchange.Length; i++)
        {
            exchange[i] = PlayerValueItems[i].StorageIndex;
        }
        return exchange;
    }

    public void PlayInlayAnimation(int materialIndex, TweenCallback onComplete)
    {
        var startPos = mMaterialList.GetPosition(materialIndex);
        var destPos = mDetail.GetInlayPosition(materialIndex);

//        Debug.LogFormat("MaterialIndex:{0}, {1} -> {2}", materialIndex, startPos, destPos);

        var obj = UIPrefabPath.LoadUI(UIPrefabPath.UIFXAddValueItemInlay, transform);

        obj.transform.position = destPos;
        var localDestPos = obj.transform.localPosition;
        obj.transform.position = startPos;

        obj.AddComponent<DelayDestory>().AliveTimeInSeconds = 0.5f;
        obj.transform.DOLocalMove(localDestPos, 0.25f).OnComplete(onComplete);
    }

    public void OnBackClick()
    {
        if(OnBackListener != null)
            OnBackListener();
    }

    /// <summary>
    /// 內部使用...
    /// </summary>
    /// <param name="status"></param>
    /// <param name="materialIndex"></param>
    /// <param name="storageMaterialItemIndex"></param>
    /// <param name="materialItemID"></param>
    public void NotifyMaterialClick(UIEquipMaterialItem.EStatus status ,int materialIndex, 
                                    int storageMaterialItemIndex, int materialItemID)
    {
        if(OnMaterialListener != null)
            OnMaterialListener(status, mPlayerInfo.CurrentSlotIndex, materialIndex, 
                               storageMaterialItemIndex, materialItemID);
    }
}
