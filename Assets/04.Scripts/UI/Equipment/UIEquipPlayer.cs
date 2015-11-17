using System.Collections.Generic;
using GameStruct;
using UnityEngine;
using JetBrains.Annotations;

/// <summary>
/// 負責裝備介面左邊的玩家裝備資訊.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call UpdateUI() 更新介面. </item>
/// <item> OnSlotClickListener 註冊 Slot 點擊事件. </item>
/// </list>
public class UIEquipPlayer : MonoBehaviour
{
    /// <summary>
    /// Slot 被點擊. 參數(int index).
    /// </summary>
    public event CommonDelegateMethods.Int1 OnSlotClickListener;

    public Transform HexagonParent;
    public Transform[] SlotParents;

    private UIAttributes mAttributes;
    private readonly List<UIEquipPartSlot> mPartSlots = new List<UIEquipPartSlot>();

    private UIEquipmentImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UIEquipmentImpl>();

        GameObject obj = UIPrefabPath.LoadUI(UIPrefabPath.AttriuteHexagon, HexagonParent);
        mAttributes = obj.GetComponent<UIAttributes>();

        for(int i = 0; i < SlotParents.Length; i++)
        {
            Transform parent = SlotParents[i];
            obj = Instantiate(Resources.Load<GameObject>(UIPrefabPath.EquipPartSlot));
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            var slot = obj.GetComponent<UIEquipPartSlot>();
            mPartSlots.Add(slot);
            slot.Index = i;
            slot.Parent = this;
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
            if(mImpl.ValueItems.Length > i && mImpl.ValueItems[i].IsValid())
                mPartSlots[i].Set(mImpl.ValueItems[i]);
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
        for (int i = 0; i < mImpl.ValueItems.Length; i++)
        {
            sum += mImpl.ValueItems[i].GetValue(kind);
        }

//            Debug.LogFormat("{0}:{1}", kind, sum);

        return sum;
    }

    public void OnSlotClick(int index)
    {
//        Debug.LogFormat("OnSlotClick, index:{0}", index);

        if(OnSlotClickListener != null)
            OnSlotClickListener(index);
    }
} 


