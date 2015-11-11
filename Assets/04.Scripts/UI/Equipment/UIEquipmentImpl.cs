using System.Collections.Generic;
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
    }

    private delegate void Action(Dictionary<UIAttributes.EGroup, float> attributes);

    public void Init(Dictionary<UIAttributes.EGroup, float> attributes, Item[] items)
    {
        Action setAttr = delegate
        {
            mAttributes.SetVisible(true);

            foreach(KeyValuePair<UIAttributes.EGroup, float> pair in attributes)
            {
                mAttributes.SetValue(pair.Key, pair.Value);
            }
        };

        setAttr(attributes);
    }

    public void OnBackClick()
    {
        if(OnBackListener != null)
            OnBackListener();
    }
}
