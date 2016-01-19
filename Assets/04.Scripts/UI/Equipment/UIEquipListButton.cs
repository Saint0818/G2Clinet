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
    public UILabel AmountLabel;
    public UILabel NameLabel;

    public UISprite[] Attrs;
    public UILabel[] AttrValues;
    public GameObject RedPoint;

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

        RedPointVisible = false;
    }

    public void Set(UIValueItemData data)
    {
        NameLabel.text = data.Name;
        NameLabel.color = data.NameColor;

        Icon.atlas = data.Atlas;
        Icon.spriteName = data.Icon;
        Frame.spriteName = data.Frame;

        AmountLabel.gameObject.SetActive(data.Num >= 2);
        AmountLabel.text = data.Num.ToString();

        setValues(data.AllValues);
        setInlays(data.Inlay);
    }

    public bool RedPointVisible {set { RedPoint.SetActive(value); }}

    private void setValues(Dictionary<EAttribute, UIValueItemData.BonusData> allValues)
    {
        // clear.
        for(var i = 0; i < Attrs.Length; i++)
        {
            Attrs[i].gameObject.SetActive(false);
            AttrValues[i].gameObject.SetActive(false);
        }

        int index = 0;
        foreach(KeyValuePair<EAttribute, UIValueItemData.BonusData> pair in allValues)
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