using System.Collections.Generic;
using System.Linq;
using GameStruct;

/// <summary>
/// 某一個鑲嵌物品.
/// </summary>
public class UIValueItemInlayData
{
    public string Icon;

    // 道具會影響哪些屬性的數值.
    public Dictionary<EAttribute, int> Values = new Dictionary<EAttribute, int>();

    public int GetValue(EAttribute kind)
    {
        if(Values.ContainsKey(kind))
            return Values[kind];
        return 0;
    }

    public int GetTotalPoints()
    {
        return Values.Sum(pair => pair.Value);

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