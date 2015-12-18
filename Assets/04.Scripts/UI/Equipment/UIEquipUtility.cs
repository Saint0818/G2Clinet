using System;
using GameStruct;

public class UIEquipUtility
{
    public static UIValueItemData Convert(TItemData item)
    {
        UIValueItemData equipItem = new UIValueItemData
        {
            Name = item.Name,
			Atlas = GameData.DItemAtlas.ContainsKey(GameData.AtlasName(item.Atlas)) ? GameData.DItemAtlas[GameData.AtlasName(item.Atlas)] : null,
            Icon = string.Format("Item_{0}", item.Icon),
            Frame = string.Format("Equipment_{0}", item.Quality),
            Desc = item.Explain
        };

//        if(GameData.DItemAtlas.ContainsKey("AtlasItem_" + item.Atlas))
//            equipItem.Atlas = GameData.DItemAtlas["AtlasItem_" + item.Atlas];

        for (int i = 0; i < item.Bonus.Length; i++)
        {
            if (item.Bonus[i] == EBonus.None)
                continue;

            var data = new UIValueItemData.BonusData
            {
                Icon = string.Format("AttrKind_{0}", item.Bonus[i].GetHashCode()),
                Value = item.BonusValues[i]
            };
            equipItem.Values.Add(convert(item.Bonus[i]), data);
        }

        return equipItem;
    }

    private static EAttribute convert(EBonus bonus)
    {
        switch(bonus)
        {
            case EBonus.Point2: return EAttribute.Point2;
            case EBonus.Point3: return EAttribute.Point3;
            case EBonus.Speed: return EAttribute.Speed;
            case EBonus.Stamina: return EAttribute.Stamina;
            case EBonus.Strength: return EAttribute.Strength;
            case EBonus.Dunk: return EAttribute.Dunk;
            case EBonus.Rebound: return EAttribute.Rebound;
            case EBonus.Block: return EAttribute.Block;
            case EBonus.Defence: return EAttribute.Defence;
            case EBonus.Steal: return EAttribute.Steal;
            case EBonus.Dribble: return EAttribute.Dribble;
            case EBonus.Pass: return EAttribute.Pass;
        }

        throw new NotImplementedException(string.Format("Bouns:{0}", bonus));
    }
}
