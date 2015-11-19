using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;

/// <summary>
/// 裝備介面所使用的裝備資料.
/// </summary>
public class UIValueItemData
{
    public class AttrKindData
    {
        public string Icon;
        public int Value;
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
    public Dictionary<EAttributeKind, AttrKindData> Values = new Dictionary<EAttributeKind, AttrKindData>();

    // 鑲嵌物品.
    [CanBeNull]
    public UIValueItemInlayData[] Inlays;

    public int Num; // 堆疊數量.

    public override string ToString()
    {
        return String.Format("Name: {0}, Icon: {1}, Desc: {2}", Name, Icon, Desc);
    }

    public int GetValue(EAttributeKind kind)
    {
        int sum = 0;
        if (Values.ContainsKey(kind))
            sum += Values[kind].Value;

        if (Inlays != null)
        {
            foreach (UIValueItemInlayData inlay in Inlays)
            {
                sum += inlay.GetValue(kind);
            }
        }

        return sum;
    }

    public bool IsValid()
    {
        return !String.IsNullOrEmpty(Name);
    }
}
