using UnityEngine;
using JetBrains.Annotations;
using UI;

/// <summary>
/// 這是裝備介面中, 左邊的裝備欄位, 欄位可能是空的, 或是有資訊.
/// </summary>
public class UIEquipPartSlot : MonoBehaviour
{
    public Transform Parent;

    private UIEquipItem mItem;

    [UsedImplicitly]
	private void Awake()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(UIPrefabPath.ItemEquipmentBtn));
        obj.transform.parent = Parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        mItem = obj.GetComponent<UIEquipItem>();
    }

    public void Clear()
    {
        mItem.gameObject.SetActive(false);
    }

    public void SetItem(EquipItem item)
    {
        mItem.gameObject.SetActive(true);
        mItem.Set(item);
    }
}
