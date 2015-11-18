using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是裝備介面中, 左邊的裝備欄位, 欄位可能是空的, 或是有資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Clear() or Set() 設定 Slot 顯示的資訊. </item>
/// </list>
public class UIEquipPartSlot : MonoBehaviour
{
    public Transform UIParent;

    public UIEquipPlayer Parent;
    public int Index; // 這是第幾個 Slot.

    private UIEquipItem mItem;

    [UsedImplicitly]
	private void Awake()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(UIPrefabPath.ItemEquipmentBtn));
        obj.transform.parent = UIParent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        mItem = obj.GetComponent<UIEquipItem>();
        mItem.OnClickListener += onItemClick;
    }

    public void Clear()
    {
        mItem.Clear();
    }

    public void Set(UIValueItemData item)
    {
        mItem.Set(item);
    }

    /// <summary>
    /// 呼叫時機: Slot 上的道具被點擊.
    /// </summary>
    private void onItemClick()
    {
        Parent.OnSlotClick(Index);
        GetComponent<UIToggle>().Set(true);
    }
}
