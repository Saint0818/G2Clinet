using System;
using System.Collections.Generic;
using System.Linq;
using GameStruct;
using JetBrains.Annotations;

/// <summary>
/// 裝備介面所使用的裝備資料.
/// </summary>
public class UIValueItemData
{
    public class BonusData
    {
        public string Icon; // 數值裝某個屬性的圖片.
        public int Value; // 數值裝某個屬性的數值.
    }

    /// <summary>
    /// UI 並不會使用這個數值, 這個只是給 UIEquipment 用來得知哪個道具被替換了, 而且也知道替換了
    /// 倉庫中的哪一個.
    /// </summary>
    public int StorageIndex { set; get; }

    public string Name
    {
        set { mName = string.IsNullOrEmpty(value) ? string.Empty : value; }
        get { return mName; }
    }
    private string mName;

    public string Icon
    {
        set { mIcon = string.IsNullOrEmpty(value) ? string.Empty : value; }
        get { return mIcon; }
    }
    private string mIcon;

    /// <summary>
    /// 道具的背景圖片.
    /// </summary>
    public string Frame
    {
        get { return mFrame; }
        set { mFrame = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }
    private string mFrame;

    public string Desc
    {
        set { mDesc = string.IsNullOrEmpty(value) ? string.Empty : value; }
        get { return mDesc; }
    }
    private string mDesc;

    // 道具會影響哪些屬性的數值.
    public Dictionary<EAttribute, BonusData> Values = new Dictionary<EAttribute, BonusData>();

    // 鑲嵌物品.
    [CanBeNull]
    public UIValueItemInlayData[] Inlays;

    public int Num; // 堆疊數量.

    public override string ToString()
    {
        return String.Format("Name: {0}, Icon: {1}, Desc: {2}", Name, Icon, Desc);
    }

    /// <summary>
    /// 某個屬性的總和數值.
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public int GetSumValue(EAttribute kind)
    {
        int sum = 0;
        if(Values.ContainsKey(kind))
            sum += Values[kind].Value;

        if(Inlays != null)
        {
            sum += Inlays.Sum(inlay => inlay.GetValue(kind));

            /*
            foreach(UIValueItemInlayData inlay in Inlays)
            {
                sum += inlay.GetValue(kind);
            }
            */
        }

        return sum;
    }

    /// <summary>
    /// 數值裝總分. (內部會用總分來區別裝備的強弱)
    /// </summary>
    /// <returns></returns>
    public int GetTotalPoints()
    {
        int totalPoints = Values.Sum(pair => pair.Value.Value);
        /*
        foreach(KeyValuePair<EAttribute, BonusData> pair in Values)
        {
            totalPoints += pair.Value.Value;
        }
        */

        if (Inlays != null)
        {
            totalPoints += Inlays.Sum(inlay => inlay.GetTotalPoints());
            /*
            foreach (UIValueItemInlayData inlay in Inlays)
            {
                totalPoints += inlay.GetTotalPoints();
            }
            */
        }

        return totalPoints;
    }

//    public bool IsValid()
//    {
//        return !String.IsNullOrEmpty(Name);
//    }
}
