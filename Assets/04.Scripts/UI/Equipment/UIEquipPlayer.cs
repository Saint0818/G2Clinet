using System.Collections.Generic;
using GameStruct;
using UnityEngine;
using JetBrains.Annotations;

/// <summary>
/// 負責裝備介面左邊的玩家裝備資訊.
/// </summary>
public class UIEquipPlayer : MonoBehaviour
{
    public Transform HexagonParent;
    public Transform[] SlotParents;

    private UIAttributes mAttributes;
    private readonly List<UIEquipPartSlot> mPartSlots = new List<UIEquipPartSlot>();

    private UIEquipmentImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UIEquipmentImpl>();

        GameObject obj = Instantiate(Resources.Load<GameObject>(UIPrefabPath.AttriuteHexagon));
        obj.transform.parent = HexagonParent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        mAttributes = obj.GetComponent<UIAttributes>();

        foreach(Transform parent in SlotParents)
        {
            obj = Instantiate(Resources.Load<GameObject>(UIPrefabPath.EquipPartSlot));
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            mPartSlots.Add(obj.GetComponent<UIEquipPartSlot>());
        }
    }

    public void UpdateUI()
    {
        updateAttributes();
        updateSlots();
    }

    private void updateSlots()
    {
        for(int i = 0; i < mPartSlots.Count; i++)
        {
            if(mImpl.Items.Length > i && mImpl.Items[i].IsValid())
                mPartSlots[i].SetItem(mImpl.Items[i]);
            else
                mPartSlots[i].Clear();
        }
    }

    private void updateAttributes()
    {
        mAttributes.SetVisible(true);

        var value = mImpl.BasicAttr[EAttributeKind.Block] + getSumValue(EAttributeKind.Block);
        mAttributes.SetValue(UIAttributes.EGroup.Block, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Steal] + getSumValue(EAttributeKind.Steal);
        mAttributes.SetValue(UIAttributes.EGroup.Steal, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Point2] + getSumValue(EAttributeKind.Point2);
        mAttributes.SetValue(UIAttributes.EGroup.Point2, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Dunk] + getSumValue(EAttributeKind.Dunk);
        mAttributes.SetValue(UIAttributes.EGroup.Dunk, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Point3] + getSumValue(EAttributeKind.Point3);
        mAttributes.SetValue(UIAttributes.EGroup.Point3, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Rebound] + getSumValue(EAttributeKind.Rebound);
        mAttributes.SetValue(UIAttributes.EGroup.Rebound, value / GameConst.AttributeMax);
    }

    private int getSumValue(EAttributeKind kind)
    {
        int sum = 0;
        for (int i = 0; i < mImpl.Items.Length; i++)
        {
            sum += mImpl.Items[i].GetValue(kind);
        }

//            Debug.LogFormat("{0}:{1}", kind, sum);

        return sum;
    }
} // end of the class.


