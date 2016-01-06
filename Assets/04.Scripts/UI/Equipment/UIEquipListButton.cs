using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是 UIEquipList 下的道具. 
/// </summary>
public class UIEquipListButton : MonoBehaviour
{
    public UISprite Icon;
    public UISprite Frame;
//    public UILabel AmountLabel;
    public UILabel NameLabel;

    public UISprite[] Attrs;
    public UILabel[] AttrValues;

    public GameObject[] InlaySlots;
    public GameObject[] Inlays;

    private UIEquipItemList mParent;
    private int mIndex;

    [UsedImplicitly]
    private void Awake()
    {
	    
    }

    public void Init(UIEquipItemList parent, int index)
    {
        mParent = parent;
        mIndex = index;
    }

    public void Set(UIValueItemData item)
    {
        NameLabel.text = item.Name;
        Icon.atlas = item.Atlas;
        Icon.spriteName = item.Icon;
        Frame.spriteName = item.Frame;
//        AmountLabel.text = item.Num.ToString();

        setValues(item.Values);
        setInlays(item.Inlay);
    }

    private void setValues(Dictionary<EAttribute, UIValueItemData.BonusData> itemValues)
    {
        // clear.
        for(var i = 0; i < Attrs.Length; i++)
        {
            Attrs[i].gameObject.SetActive(false);
            AttrValues[i].gameObject.SetActive(false);
        }

        int index = 0;
        foreach(KeyValuePair<EAttribute, UIValueItemData.BonusData> pair in itemValues)
        {
            Attrs[index].gameObject.SetActive(true);
            AttrValues[index].gameObject.SetActive(true);

            Attrs[index].spriteName = pair.Value.Icon;
            AttrValues[index].text = pair.Value.Value.ToString();
            ++index;
        }
    }

    private void setInlays(bool[] inlays)
    {
        for(int i = 0; i < Inlays.Length; i++)
        {
            Inlays[i].SetActive(false);
            InlaySlots[i].SetActive(false);
        }

        for(var i = 0; i < inlays.Length; i++)
        {
            InlaySlots[i].SetActive(true);
            Inlays[i].SetActive(inlays[i]);
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