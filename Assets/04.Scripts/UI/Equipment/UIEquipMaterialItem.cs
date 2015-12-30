using System;
using System.Collections.Generic;
using System.Linq;
using GameStruct;
using UnityEngine;

/// <summary>
/// 這是數值裝的某個材料.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Init() 做初始化. </item>
/// <item> Call UpdateUI() 更新顯示的資料. </item>
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
        public string Icon;

        /// <summary>
        /// 材料需要多少才可以鑲嵌.
        /// </summary>
        public int NeedValue;

        /// <summary>
        /// 玩家身上擁有的材料數量.
        /// </summary>
        public int RealValue;

        public EStatus Status;

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
            return Values.Sum(pair => pair.Value.Value);

            /*
            int totalPoints = 0;
            foreach(KeyValuePair<EAttribute, int> pair in Values)
            {
                totalPoints += pair.Value;
            }

            return totalPoints;
            */
        }
    }

    public UISprite Icon;
    public UILabel Name;
    public UISprite[] AttrSprites;
    public UILabel[] AttrValues;
    public UILabel Amount;
    public GameObject LackIcon;
    public GameObject EnoughIcon;
    public GameObject InlayIcon;

    public event CommonDelegateMethods.Int1 Listener;

    private int mIndex;

    public void Init(int index)
    {
        mIndex = index;
    }

    public void UpdateUI(Data data)
    {
        Name.text = data.Name;
        Icon.spriteName = data.Icon;

        updateBonus(data);

        updateAmount(data);
        updateStatusIcon(data.Status);
    }

    private void updateBonus(Data data)
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

            ++j;
        }
    }

    private void updateAmount(Data data)
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

    private void updateStatusIcon(EStatus status)
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

    public void FireClickEvent()
    {
        if(Listener != null)
            Listener(mIndex);
    }
}
