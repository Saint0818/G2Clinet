using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UI;
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
    public UILabel Desc;
    public UIButton UpgradeButton;

    private UIEquipItem mEquipItem;

    private UIEquipDetailAttr[] mAttrs;

    /// <summary>
    /// 這是對應到資料表格的參數(UIEquipmentImpl.Init 的參數), 主要是用來確認什麼道具被點到.
    /// </summary>
    private int mIndex; 

    [UsedImplicitly]
	private void Awake()
    {
        mEquipItem = UIPrefabPath.LoadUI(UIPrefabPath.ItemEquipmentBtn, ItemParent).GetComponent<UIEquipItem>();
        mEquipItem.Clear();
        mEquipItem.OnClickListener += onItemClick;

        mAttrs = GetComponentsInChildren<UIEquipDetailAttr>();

        UpgradeButton.isEnabled = false;
    }

    public void Set(int index, EquipItem item)
    {
        mIndex = index;
        mEquipItem.Set(item);
        Desc.text = item.Desc;

        foreach(UIEquipDetailAttr attr in mAttrs)
        {
            attr.Clear();
        }

        int i = 0;
        foreach(KeyValuePair<EAttributeKind, EquipItem.AttrKindData> pair in item.Values)
        {
            mAttrs[i].Set(pair.Value.Icon, pair.Value.Value);
            ++i;
        }
    }

    private void onItemClick()
    {
        if(OnItemClickListener != null)
            OnItemClickListener(mIndex);
    }
}
