using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIEquipmentImpl : MonoBehaviour
{
    public event CommonDelegateMethods.Action OnBackListener;

    public Transform HexagonParent;
    

    public class Attribute
    {
        public string Icon;
        public int Value;
    }

    public class Item
    {
        public string Name;
        public string Icon;
        public string Desc;
        public Attribute[] Attributes;
        public int Num; // 堆疊數量.
    }

    private UIAttributes mAttributes;

    [UsedImplicitly]
    private void Awake()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(UIPrefabPath.AttriuteHexagon));
        obj.transform.parent = HexagonParent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        mAttributes = obj.GetComponent<UIAttributes>();

        mAttributes.SetVisible(true);
        mAttributes.SetValue(UIAttributes.EAttribute.StrBlk, 0.5f);
        mAttributes.SetValue(UIAttributes.EAttribute.DefStl, 0.5f);
        mAttributes.SetValue(UIAttributes.EAttribute.DrbPass, 0.5f);
        mAttributes.SetValue(UIAttributes.EAttribute.SpdSta, 0.5f);
        mAttributes.SetValue(UIAttributes.EAttribute.Pt2Pt3, 0.5f);
        mAttributes.SetValue(UIAttributes.EAttribute.RebDnk, 0.5f);
    }

    public void Init(int[] playerValues, Item[] items)
    {

    }

    public void OnBackClick()
    {
        if(OnBackListener != null)
            OnBackListener();
    }
}
