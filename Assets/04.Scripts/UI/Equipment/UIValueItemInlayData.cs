using System.Collections.Generic;
using GameStruct;

/// <summary>
/// 某一個鑲嵌物品.
/// </summary>
public class UIValueItemInlayData
{
    public string Icon;

    // 道具會影響哪些屬性的數值.
    public Dictionary<EAttributeKind, int> Values = new Dictionary<EAttributeKind, int>();

    public int GetValue(EAttributeKind kind)
    {
        if (Values.ContainsKey(kind))
            return Values[kind];
        return 0;
    }
}