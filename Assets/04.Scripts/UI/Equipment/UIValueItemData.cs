using System;
using System.Collections.Generic;
using System.Linq;
using GameStruct;

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

    public enum EStatus
    {
        CannotUpgrade, Upgradeable, Demount, CannotDemount
    }

    public const int StorageIndexNone = -2;
    public const int StorageIndexDemount = -1;
    /// <summary>
    /// UI 並不會使用這個數值, 這個只是給 UIEquipment 得知哪個道具被替換了, 而且也知道替換了
    /// 倉庫中的哪一個.
    /// </summary>
    public int StorageIndex = StorageIndexNone;
    /// <summary>
    /// UI 並不會使用這個數值, 這個只是給 UIEquipment 得知這是哪一個道具.
    /// </summary>
    public int ItemID;

    public string Name
    {
        set { mName = string.IsNullOrEmpty(value) ? string.Empty : value; }
        get { return mName; }
    }
    private string mName;

    public UIAtlas Atlas { get; set; }

    public string Icon
    {
        set { mIcon = string.IsNullOrEmpty(value) ? string.Empty : value; }
        get { return mIcon; }
    }
    private string mIcon;

    /// <summary>
    /// 道具的外框圖片.(會根據道具的 Quality 而顯示不同的外框)
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

    /// <summary>
    /// 鑲嵌孔狀態. true: 已鑲嵌, false: 未鑲嵌.
    /// </summary>
    public bool[] Inlay = new bool[0];

    /// <summary>
    /// 鑲嵌物品會影響的數值.
    /// </summary>
    public Dictionary<EAttribute, BonusData> InlayValues = new Dictionary<EAttribute, BonusData>();

    public int GetInlayValue(EAttribute attribute)
    {
        if(InlayValues.ContainsKey(attribute))
            return InlayValues[attribute].Value;
        return 0;
    }

    // 數值裝的升級材料.
    public List<UIEquipMaterialItem.Data> Materials = new List<UIEquipMaterialItem.Data>();

    public int Num; // 堆疊數量.

    public EStatus Status;

    public override string ToString()
    {
        return String.Format("Name: {0}, Icon: {1}, Desc: {2}", Name, Icon, Desc);
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Name);
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name);
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

        if(Materials != null)
        {
            sum += Materials.Sum(inlay => inlay.GetValue(kind));

            /*
            foreach(UIValueItemInlayData inlay in Materials)
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

        if (Materials != null)
        {
            totalPoints += Materials.Sum(inlay => inlay.GetTotalPoints());
            /*
            foreach (UIValueItemInlayData inlay in Materials)
            {
                totalPoints += inlay.GetTotalPoints();
            }
            */
        }

        return totalPoints;
    }
}
