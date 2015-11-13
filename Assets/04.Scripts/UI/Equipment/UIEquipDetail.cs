using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UI;
using UnityEngine;

/// <summary>
/// 裝備介面中間的詳細視窗.
/// </summary>
public class UIEquipDetail : MonoBehaviour
{
    public Transform ItemParent;
    public UILabel Desc;

    private UIEquipItem mEquipItem;

    private UIEquipDetailAttr[] mAttrs;

    [UsedImplicitly]
	private void Awake()
    {
        mEquipItem = UIPrefabPath.LoadUI(UIPrefabPath.ItemEquipmentBtn, ItemParent).GetComponent<UIEquipItem>();
        mEquipItem.Clear();

        mAttrs = GetComponentsInChildren<UIEquipDetailAttr>();
    }

    public void Hide()
    {
        mEquipItem.Clear();
    }

    public void Set(EquipItem item)
    {
        mEquipItem.Set(item);
        Desc.text = item.Desc;

        foreach(UIEquipDetailAttr attr in mAttrs)
        {
            attr.Clear();
        }

        int index = 0;
        foreach(KeyValuePair<EAttributeKind, EquipItem.AttrKindData> pair in item.Values)
        {
            mAttrs[index].Set(pair.Value.Icon, pair.Value.Value);
            ++index;
        }
    }
}
