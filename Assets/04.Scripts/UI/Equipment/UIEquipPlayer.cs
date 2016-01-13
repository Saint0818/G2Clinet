using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

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
    public event Action<int> OnSlotClickListener;

    public Transform HexagonParent;
    public Transform[] SlotParents;

    /// <summary>
    /// 現在玩家選擇的數值裝.
    /// </summary>
    public int CurrentSlotIndex { get; private set; }

    private UIAttributes mAttributes;
    private readonly List<UIEquipPartSlot> mPartSlots = new List<UIEquipPartSlot>();

    private UIEquipmentMain mMain;

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIEquipmentMain>();

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
            obj.name = string.Format("{0}({1})", obj.name, i);
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
        for(int slotIndex = 0; slotIndex < mPartSlots.Count; slotIndex++)
        {
            if(mMain.PlayerValueItems.Length > slotIndex)
                mPartSlots[slotIndex].Set(mMain.PlayerValueItems[slotIndex], !mMain.IsBestValueItem(slotIndex));
        }
    }

    private void updateAttributes()
    {
        mAttributes.SetVisible(true);

        var value = mMain.BasicAttr[EAttribute.Block] + getSumValue(EAttribute.Block);
        mAttributes.SetValue(UIAttributes.EGroup.Block, value / GameConst.AttributeMax);

        value = mMain.BasicAttr[EAttribute.Steal] + getSumValue(EAttribute.Steal);
        mAttributes.SetValue(UIAttributes.EGroup.Steal, value / GameConst.AttributeMax);

        value = mMain.BasicAttr[EAttribute.Point2] + getSumValue(EAttribute.Point2);
        mAttributes.SetValue(UIAttributes.EGroup.Point2, value / GameConst.AttributeMax);

        value = mMain.BasicAttr[EAttribute.Dunk] + getSumValue(EAttribute.Dunk);
        mAttributes.SetValue(UIAttributes.EGroup.Dunk, value / GameConst.AttributeMax);

        value = mMain.BasicAttr[EAttribute.Point3] + getSumValue(EAttribute.Point3);
        mAttributes.SetValue(UIAttributes.EGroup.Point3, value / GameConst.AttributeMax);

        value = mMain.BasicAttr[EAttribute.Rebound] + getSumValue(EAttribute.Rebound);
        mAttributes.SetValue(UIAttributes.EGroup.Rebound, value / GameConst.AttributeMax);
    }

    private int getSumValue(EAttribute kind)
    {
        int sum = 0;
        for (int i = 0; i < mMain.PlayerValueItems.Length; i++)
        {
            sum += mMain.PlayerValueItems[i].GetSumValue(kind);
        }

//            Debug.LogFormat("{0}:{1}", kind, sum);

        return sum;
    }

    public void OnSlotClick(int slotIndex)
    {
//        Debug.LogFormat("OnSlotClick, index:{0}", index);

        CurrentSlotIndex = slotIndex;

        if(OnSlotClickListener != null)
            OnSlotClickListener(CurrentSlotIndex);
    }
} 


