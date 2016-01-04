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
/// </list>
public class UIEquipDetail : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 畫面上方的道具被點擊. 參數(int index)
    /// </summary>
    public event CommonDelegateMethods.Int1 OnItemClickListener;

    public Transform ItemParent;
    public UIButton UpgradeButton;
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
        mEquipItem.Clear();
        mEquipItem.OnClickListener += onItemClick;

        mAttrs = GetComponentsInChildren<UIEquipDetailAttr>();
        mMain = GetComponent<UIEquipmentMain>();

        UpgradeButton.isEnabled = false;
    }

    public void Set(int slotIndex, UIValueItemData item)
    {
        mSlotIndex = slotIndex;

        mEquipItem.Set(item, !mMain.IsBestValueItem(mSlotIndex));

        Desc.text = item.Desc;

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
}
