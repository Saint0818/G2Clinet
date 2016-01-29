﻿using System;
using System.Collections.Generic;
using GameStruct;
using UnityEngine;

/// <summary>
/// 這是數值裝的某個材料.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Init() 做初始化. </item>
/// <item> Call Set() 更新顯示的資料. </item>
/// </list>
public class UIEquipMaterialItem : MonoBehaviour
{
    public enum EStatus
    {
        Enough, Lack, Inlayed
    }

    public class Data
    {
        public string Name;
        public Color32 NameColor;
        public string Icon;
        public Color32 IconBGColor;
        public string Frame;

        /// <summary>
        /// 材料需要多少才可以鑲嵌.
        /// </summary>
        public int NeedValue;

        /// <summary>
        /// 玩家身上擁有的材料數量.
        /// </summary>
        public int RealValue;

        public EStatus Status;

        /// <summary>
        /// 這不是介面資料, 這是倉庫的索引.
        /// </summary>
        public int StorageIndex;

        /// <summary>
        /// 這不是介面資料, 這是材料的編號.
        /// </summary>
        public int ItemID;

        // 道具會影響哪些屬性的數值.
        public Dictionary<EAttribute, UIValueItemData.BonusData> Values = new Dictionary<EAttribute, UIValueItemData.BonusData>();

        public int GetValue(EAttribute kind)
        {
            if(Values.ContainsKey(kind))
                return Values[kind].Value;
            return 0;
        }

        public int GetTotalPoints()
        {
//            return Values.Sum(pair => pair.Value.Value);

            int totalPoints = 0;
            foreach(KeyValuePair<EAttribute, UIValueItemData.BonusData> pair in Values)
            {
                totalPoints += pair.Value.Value;
            }

            return totalPoints;
        }
    }

    public UISprite Icon;
    public UISprite IconBG;
    public UISprite Frame;
    public UILabel Name;

    public UIButton[] AttrButtons;
    public UISprite[] AttrSprites;
    public UILabel[] AttrValues;

    public UILabel Amount;
    public GameObject LackIcon;
    public GameObject EnoughIcon;
    public GameObject InlayIcon;

    public delegate void Action(EStatus status, int materialIndex, int storageIndex, int materialItemID);
    public event Action ClickListener;

    /// <summary>
    /// 這是第幾個材料.
    /// </summary>
    private int mIndex;
    private EStatus mStatus;

    private int mStorageIndex;
    private int mMaterialItemID;
    private UIButton mButton;

    private readonly EBonus[] mBonus = new EBonus[3];

    private void Awake()
    {
        mButton = GetComponent<UIButton>();

        AttrButtons[0].onClick.Add(new EventDelegate(() => UIAttributeHint.Get.UpdateView((int)mBonus[0])));
        AttrButtons[1].onClick.Add(new EventDelegate(() => UIAttributeHint.Get.UpdateView((int)mBonus[1])));
        AttrButtons[2].onClick.Add(new EventDelegate(() => UIAttributeHint.Get.UpdateView((int)mBonus[2])));
    }

    public void Init(int index)
    {
        mIndex = index;
    }

    public void Set(Data data)
    {
        Name.text = data.Name;
        Name.color = data.NameColor;

        Icon.spriteName = data.Icon;
        IconBG.color = data.IconBGColor;
        Frame.spriteName = data.Frame;
        mStatus = data.Status;

        mButton.normalSprite = data.Frame;
        mButton.hoverSprite = data.Frame;
        mButton.pressedSprite = data.Frame;

        setBonus(data);
        setAmount(data);
        setStatusIcon(data.Status);

        mStorageIndex = data.StorageIndex;
        mMaterialItemID = data.ItemID;
    }

    private void setBonus(Data data)
    {
        for(int i = 0; i < AttrSprites.Length; i++)
        {
            AttrSprites[i].gameObject.SetActive(false);
            AttrValues[i].gameObject.SetActive(false);
        }

        int j = 0;
        foreach(KeyValuePair<EAttribute, UIValueItemData.BonusData> pair in data.Values)
        {
            AttrSprites[j].gameObject.SetActive(true);
            AttrValues[j].gameObject.SetActive(true);

            AttrSprites[j].spriteName = pair.Value.Icon;
            AttrValues[j].text = string.Format("+{0}", pair.Value.Value);

            mBonus[j] = pair.Value.Bonus;

            ++j;
        }
    }

    private void setAmount(Data data)
    {
        if(data.Status == EStatus.Lack)
            // 前面的數字顯示紅色.
            Amount.text = string.Format("[FF0000]{0}[FFFFFF]/{1}", data.RealValue, data.NeedValue);
        else if(data.Status == EStatus.Enough)
            // 前面的數字顯示綠色.
            Amount.text = string.Format("[00FF00]{0}[FFFFFF]/{1}", data.RealValue, data.NeedValue);
        else if(data.Status == EStatus.Inlayed)
            Amount.text = string.Format("{0}/{1}", data.RealValue, data.NeedValue);
        else
            throw new NotImplementedException(data.Status.ToString());
    }

    private void setStatusIcon(EStatus status)
    {
        LackIcon.SetActive(false);
        EnoughIcon.SetActive(false);
        InlayIcon.SetActive(false);

        if(status == EStatus.Lack)
            LackIcon.SetActive(true);
        else if(status == EStatus.Enough)
            EnoughIcon.SetActive(true);
        else if(status == EStatus.Inlayed)
            InlayIcon.SetActive(true);
        else
            throw new NotImplementedException(status.ToString());
    }

    public void NotifyClick()
    {
        if(ClickListener != null)
            ClickListener(mStatus, mIndex, mStorageIndex, mMaterialItemID);
    }
}
