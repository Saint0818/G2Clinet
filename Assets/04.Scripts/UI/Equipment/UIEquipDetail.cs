using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 裝備介面中間的詳細視窗.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Set() 設定顯示資訊. </item>
/// <item> (Optional)對 XXXListener 註冊事件. </item>
/// </list>
public class UIEquipDetail : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 畫面上方的道具被點擊. 參數(int slotIndex)
    /// </summary>
    public event Action<int> OnItemClickListener;

    /// <summary>
    /// 升級按鈕按下. 參數(int slotIndex)
    /// </summary>
    public event Action<int> OnUpgradeListener;

    /// <summary>
    /// 卸下按鈕按下. 參數(int slotIndex)
    /// </summary>
    public event Action<int> OnDemountListener;

    public Transform ItemParent;
    public UIButton UpgradeButton;
    public UIButton DemountButton;
    private const string ButtonNormal = "button_orange1";
    private const string ButtonDisable = "button_gray";
    public UILabel Desc;

    private UIEquipItem mEquipItem;

    private UIEquipDetailAttr[] mAttrs;

    /// <summary>
    /// 這是對應到資料表格的參數(UIEquipmentMain.Init 的參數), 目前 Detail 視窗顯示
    /// 哪一個道具的資訊.
    /// </summary>
    public int SlotIndex
    {
        get { return mSlotIndex; }
    }
    private int mSlotIndex;

    private UIEquipmentMain mMain;

    [UsedImplicitly]
	private void Awake()
    {
        mEquipItem = UIPrefabPath.LoadUI(UIPrefabPath.UIEquipItem, ItemParent).GetComponent<UIEquipItem>();
        mEquipItem.OnClickListener += onItemClick;

        mAttrs = GetComponentsInChildren<UIEquipDetailAttr>();
        mMain = GetComponent<UIEquipmentMain>();

        UpgradeButton.onClick.Add(new EventDelegate(onUpgradeClick));
        DemountButton.onClick.Add(new EventDelegate(onDemountClick));
    }

    public void Set(int slotIndex, UIValueItemData item)
    {
        mSlotIndex = slotIndex;

        mEquipItem.Set(item, !mMain.IsBestValueItem(mSlotIndex));

        Desc.text = item.Desc;

        updateButton(item);

        foreach(UIEquipDetailAttr attr in mAttrs)
        {
            attr.Clear();
        }

        int i = 0;
        foreach(KeyValuePair<EAttribute, UIValueItemData.BonusData> pair in item.Values)
        {
            mAttrs[i].Set(pair.Value.Icon, pair.Value.Value, item.GetInlayValue(pair.Key));
            ++i;
        }
    }

    private void updateButton(UIValueItemData item)
    {
        UpgradeButton.gameObject.SetActive(false);
        DemountButton.gameObject.SetActive(false);

        if(item.Status == UIValueItemData.EStatus.CannotUpgrade)
        {
            UpgradeButton.gameObject.SetActive(true);
            UpgradeButton.GetComponent<UISprite>().spriteName = ButtonDisable;
            UpgradeButton.normalSprite = ButtonDisable;
        }
        else if(item.Status == UIValueItemData.EStatus.Upgradeable)
        {
            UpgradeButton.gameObject.SetActive(true);
            UpgradeButton.GetComponent<UISprite>().spriteName = ButtonNormal;
            UpgradeButton.normalSprite = ButtonNormal;
        }
        else if(item.Status == UIValueItemData.EStatus.CannotDemount)
        {
            DemountButton.gameObject.SetActive(true);
            DemountButton.GetComponent<UISprite>().spriteName = ButtonDisable;
            DemountButton.normalSprite = ButtonDisable;
        }
        else if(item.Status == UIValueItemData.EStatus.Demount)
        {
            DemountButton.gameObject.SetActive(true);
            DemountButton.GetComponent<UISprite>().spriteName = ButtonNormal;
            DemountButton.normalSprite = ButtonNormal;
        }
        else
            Debug.LogErrorFormat("NotImplemented, Operation:{0}", item.Status);
    }

    private void onItemClick()
    {
        if(OnItemClickListener != null)
            OnItemClickListener(mSlotIndex);
    }

    private void onUpgradeClick()
    {
        if(OnUpgradeListener != null)
            OnUpgradeListener(mSlotIndex);
    }

    private void onDemountClick()
    {
        if(OnDemountListener != null)
            OnDemountListener(mSlotIndex);
    }
}
