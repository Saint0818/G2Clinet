using System.Collections.Generic;
using GameStruct;
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

    private Dictionary<EAttributeKind, float> mBasicAttr = new Dictionary<EAttributeKind, float>();
    private Item[] mItems;

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
    }

    public void Init(Dictionary<EAttributeKind, float> basicAttr, Item[] items)
    {
        CommonDelegateMethods.Action setAttr = delegate
        {
            mAttributes.SetVisible(true);

            var value = mBasicAttr[EAttributeKind.Strength] + mBasicAttr[EAttributeKind.Block];
            mAttributes.SetValue(UIAttributes.EGroup.StrBlk, value / GameConst.AttributeMax);

            value = mBasicAttr[EAttributeKind.Defence] + mBasicAttr[EAttributeKind.Steal];
            mAttributes.SetValue(UIAttributes.EGroup.DefStl, value / GameConst.AttributeMax);

            value = mBasicAttr[EAttributeKind.Dribble] + mBasicAttr[EAttributeKind.Pass];
            mAttributes.SetValue(UIAttributes.EGroup.DrbPass, value / GameConst.AttributeMax);

            value = mBasicAttr[EAttributeKind.Speed] + mBasicAttr[EAttributeKind.Stamina];
            mAttributes.SetValue(UIAttributes.EGroup.SpdSta, value / GameConst.AttributeMax);

            value = mBasicAttr[EAttributeKind.Point2] + mBasicAttr[EAttributeKind.Point3];
            mAttributes.SetValue(UIAttributes.EGroup.Pt2Pt3, value / GameConst.AttributeMax);

            value = mBasicAttr[EAttributeKind.Rebound] + mBasicAttr[EAttributeKind.Dunk];
            mAttributes.SetValue(UIAttributes.EGroup.RebDnk, value / GameConst.AttributeMax);
        };

        mBasicAttr = new Dictionary<EAttributeKind, float>(basicAttr);
        mItems = items;

        setAttr();
    }

    public void OnBackClick()
    {
        if(OnBackListener != null)
            OnBackListener();
    }
}
