﻿using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是 UIEquipList 下的道具. 
/// </summary>
public class UIEquipListButton : MonoBehaviour
{
    public UISprite Icon;
    public UILabel AmountLabel;
    public UILabel NameLabel;

    public UISprite[] Attrs;
    public UILabel[] AttrValues;

    private UIEquipList mParent;
    private int mIndex;

    [UsedImplicitly]
    private void Awake()
    {
	    
    }

    public void Init(UIEquipList parent, int index)
    {
        mParent = parent;
        mIndex = index;
    }

    public void Set(UIValueItemData item)
    {
        NameLabel.text = item.Name;
        Icon.spriteName = item.Icon;
        AmountLabel.text = item.Num.ToString();

        setValues(item.Values);
    }

    private void setValues(Dictionary<EAttributeKind, UIValueItemData.AttrKindData> itemValues)
    {
        // clear.
        for(var i = 0; i < Attrs.Length; i++)
        {
            Attrs[i].gameObject.SetActive(false);
            AttrValues[i].gameObject.SetActive(false);
        }

        int index = 0;
        foreach(KeyValuePair<EAttributeKind, UIValueItemData.AttrKindData> pair in itemValues)
        {
            Attrs[index].gameObject.SetActive(true);
            AttrValues[index].gameObject.SetActive(true);

            Attrs[index].spriteName = pair.Value.Icon;
            AttrValues[index].text = pair.Value.Value.ToString();
            ++index;
        }
    }

    /// <summary>
    /// NGUI Event.
    /// </summary>
    [UsedImplicitly]
    private void OnClick()
    {
        mParent.NotifyClick(mIndex);
    }
}