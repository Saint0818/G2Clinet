using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UI;
using UnityEngine;

[DisallowMultipleComponent]
public class UIEquipmentImpl : MonoBehaviour
{
    public event CommonDelegateMethods.Action OnBackListener;

    public Dictionary<EAttributeKind, float> BasicAttr
    {
        get { return mBasicAttr; }
    }

    public EquipItem[] Items { get; private set; }

    private Dictionary<EAttributeKind, float> mBasicAttr = new Dictionary<EAttributeKind, float>();

    private UIEquipPlayer mPlayerInfo;
    private UIEquipDetail mItemDetail;
    private UIEquipList mItemList;

    [UsedImplicitly]
    private void Awake()
    {
        mPlayerInfo = GetComponent<UIEquipPlayer>();
        mItemDetail = GetComponent<UIEquipDetail>();
        mItemList = GetComponent<UIEquipList>();
    }

    public void Init(Dictionary<EAttributeKind, float> basicAttr, EquipItem[] items)
    {
        mBasicAttr = new Dictionary<EAttributeKind, float>(basicAttr);
        Items = items;

        mPlayerInfo.UpdateUI();
        mItemDetail.Set(items[0]);
    }

    public void OnBackClick()
    {
        if (OnBackListener != null)
            OnBackListener();
    }
}
