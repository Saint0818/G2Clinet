using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIEquipmentImpl : MonoBehaviour
{
    public event CommonDelegateMethods.Action OnBackListener;

    public Transform HexagonParent;

    public class Inlay
    {
        public string Icon;

        // 道具會影響哪些屬性的數值.
        public Dictionary<EAttributeKind, int> Values = new Dictionary<EAttributeKind, int>();

        public int GetValue(EAttributeKind kind)
        {
            if(Values.ContainsKey(kind))
                return Values[kind];
            return 0;
        }
    }

    public class Item
    {
        [NotNull] public string Name;
        [NotNull] public string Icon;
        [NotNull] public string Desc;

        // 道具會影響哪些屬性的數值.
        public Dictionary<EAttributeKind, int> Values = new Dictionary<EAttributeKind, int>();

        // 鑲嵌物品.
        [CanBeNull]public Inlay[] Inlays;

        public int Num; // 堆疊數量.

        public override string ToString()
        {
            return string.Format("Name: {0}, Icon: {1}, Desc: {2}", Name, Icon, Desc);
        }

        public int GetValue(EAttributeKind kind)
        {
            int sum = 0;
            if(Values.ContainsKey(kind))
                sum += Values[kind];

            if(Inlays != null)
            {
                foreach (Inlay inlay in Inlays)
                {
                    sum += inlay.GetValue(kind);
                }
            }

            return sum;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name);
        }
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
        mBasicAttr = new Dictionary<EAttributeKind, float>(basicAttr);
        mItems = items;

        updateAttributes();
    }

    private delegate int Action(EAttributeKind kind);

    private void updateAttributes()
    {
        Action getSumValue = delegate(EAttributeKind kind)
        {
            int sum = 0;
            for(int i = 0; i < mItems.Length; i++)
            {
                sum += mItems[i].GetValue(kind);
            }

//            Debug.LogFormat("{0}:{1}", kind, sum);

            return sum;
        };

        mAttributes.SetVisible(true);

        var value = mBasicAttr[EAttributeKind.Strength] + mBasicAttr[EAttributeKind.Block] +
                    getSumValue(EAttributeKind.Strength) + getSumValue(EAttributeKind.Block);
        mAttributes.SetValue(UIAttributes.EGroup.StrBlk, value / GameConst.AttributeMax);

        value = mBasicAttr[EAttributeKind.Defence] + mBasicAttr[EAttributeKind.Steal] +
                getSumValue(EAttributeKind.Defence) + getSumValue(EAttributeKind.Steal);
        mAttributes.SetValue(UIAttributes.EGroup.DefStl, value / GameConst.AttributeMax);

        value = mBasicAttr[EAttributeKind.Dribble] + mBasicAttr[EAttributeKind.Pass] +
                getSumValue(EAttributeKind.Dribble) + getSumValue(EAttributeKind.Pass);
        mAttributes.SetValue(UIAttributes.EGroup.DrbPass, value / GameConst.AttributeMax);

        value = mBasicAttr[EAttributeKind.Speed] + mBasicAttr[EAttributeKind.Stamina] +
                getSumValue(EAttributeKind.Speed) + getSumValue(EAttributeKind.Stamina);
        mAttributes.SetValue(UIAttributes.EGroup.SpdSta, value / GameConst.AttributeMax);

        value = mBasicAttr[EAttributeKind.Point2] + mBasicAttr[EAttributeKind.Point3] +
                getSumValue(EAttributeKind.Point2) + getSumValue(EAttributeKind.Point3);
        mAttributes.SetValue(UIAttributes.EGroup.Pt2Pt3, value / GameConst.AttributeMax);

        value = mBasicAttr[EAttributeKind.Rebound] + mBasicAttr[EAttributeKind.Dunk] +
                getSumValue(EAttributeKind.Rebound) + getSumValue(EAttributeKind.Dunk);
        mAttributes.SetValue(UIAttributes.EGroup.RebDnk, value / GameConst.AttributeMax);
    }

    public void OnBackClick()
    {
        if(OnBackListener != null)
            OnBackListener();
    }
}
