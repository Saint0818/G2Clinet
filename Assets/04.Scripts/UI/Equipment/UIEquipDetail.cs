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
    /// 呼叫時機: 畫面上方的道具被點擊. 參數(int index)
    /// </summary>
    public event Action<int> OnItemClickListener;

    /// <summary>
    /// 升級按鈕按下.
    /// </summary>
    public event Action<int> OnUpgradeListener;

    public Transform ItemParent;
    public UIButton UpgradeButton;
    private const string UpgradeButtonNormal = "button_orange1";
    private const string UpgradeButtonDisable = "button_gray";
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
        mEquipItem = UIPrefabPath.LoadUI(UIPrefabPath.ItemEquipmentBtn, ItemParent).GetComponent<UIEquipItem>();
        mEquipItem.OnClickListener += onItemClick;

        mAttrs = GetComponentsInChildren<UIEquipDetailAttr>();
        mMain = GetComponent<UIEquipmentMain>();

        UpgradeButton.onClick.Add(new EventDelegate(onUpgradeClick));
    }

    public void Set(int slotIndex, UIValueItemData item)
    {
        mSlotIndex = slotIndex;

        mEquipItem.Set(item, !mMain.IsBestValueItem(mSlotIndex));

        Desc.text = item.Desc;

        string upgradeSprite = item.IsUpgradeable ? UpgradeButtonNormal : UpgradeButtonDisable;
        UpgradeButton.GetComponent<UISprite>().spriteName = upgradeSprite;
        UpgradeButton.normalSprite = upgradeSprite;

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
}
