using GameStruct;

public class UIEquipUtility
{
    public static UIValueItemData Convert(TItemData item)
    {
        UIValueItemData equipItem = new UIValueItemData
        {
            Name = item.Name,
            Icon = string.Format("Item_{0}", item.Icon),
            Frame = string.Format("Equipment_{0}", item.Quality),
            Desc = item.Explain
        };
        for(int i = 0; i < item.Bonus.Length; i++)
        {
            if (item.Bonus[i] == EBonus.None)
                continue;

            var data = new UIValueItemData.AttrKindData
            {
                Icon = string.Format("AttrKind_{0}", item.Bonus[i].GetHashCode()),
                Value = item.AttrValues[i]
            };
            equipItem.Values.Add(item.Bonus[i], data);
        }

        return equipItem;
    }
}
