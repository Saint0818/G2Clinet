using GameStruct;

public class UIEquipUtility
{
    public static UIValueItemData Convert(TItemData item)
    {
        UIValueItemData equipItem = new UIValueItemData
        {
            Name = item.Name,
            Icon = string.Format("Item_{0}", item.Icon),
            Desc = item.Explain
        };
        for(int i = 0; i < item.AttrKinds.Length; i++)
        {
            if (item.AttrKinds[i] == EAttributeKind.None)
                continue;

            var data = new UIValueItemData.AttrKindData
            {
                Icon = string.Format("AttrKind_{0}", item.AttrKinds[i].GetHashCode()),
                Value = item.AttrValues[i]
            };
            equipItem.Values.Add(item.AttrKinds[i], data);
        }

        return equipItem;
    }
}
