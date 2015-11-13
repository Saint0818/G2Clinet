using GameStruct;
using UI;

public class UIEquipUtility
{
    public static EquipItem Convert(TItemData item)
    {
        EquipItem equipItem = new EquipItem();
        equipItem.Name = item.Name;
        equipItem.Icon = string.Format("Item_{0}", item.Icon);
        equipItem.Desc = item.Explain;
        for (int i = 0; i < item.AttrKinds.Length; i++)
        {
            if (item.AttrKinds[i] == EAttributeKind.None)
                continue;

            var data = new EquipItem.AttrKindData
            {
                Icon = string.Format("AttrKind_{0}", item.AttrKinds[i].GetHashCode()),
                Value = item.AttrValues[i]
            };
            equipItem.Values.Add(item.AttrKinds[i], data);
        }

        return equipItem;
    }
}
