﻿using System;
using GameStruct;

public static class UIEquipChecker
{
    public static UIValueItemData.EStatus FindStatus(TItemData item, int[] playerInlayItemIDs)
    {
        if(11 <= item.Kind && item.Kind <= 16)
            return HasUpgradeItem(item) && IsInlayFull(item, playerInlayItemIDs) && 
                   HasUpgradeMoney(item) && IsLevelEnough(item)
                   ? UIValueItemData.EStatus.Upgradeable 
                   : UIValueItemData.EStatus.CannotUpgrade;

        if(item.Kind == 17 || item.Kind == 18)
            return UIValueItemData.EStatus.Demount;

        throw new NotImplementedException(string.Format("ItemID:{0}, Kind:{1}", item.ID, item.Kind));
    }

    public static bool HasUpgradeItem(TItemData item)
    {
        return GameData.DItemData.ContainsKey(item.UpgradeItem);
    }

    public static bool HasUpgradeMoney(TItemData item)
    {
        return GameData.Team.Money >= item.UpgradeMoney;
    }

    public static bool IsLevelEnough(TItemData item)
    {
        return GameData.Team.Player.Lv >= item.UpgradeLv;
    }

    /// <summary>
    /// 是否鑲嵌完畢?
    /// </summary>
    /// <param name="item"></param>
    /// <param name="playerInlayItemIDs"></param>
    /// <returns></returns>
    public static bool IsInlayFull(TItemData item, int[] playerInlayItemIDs)
    {
        for(var i = 0; i < item.ReviseMaterials.Length; i++)
        {
//            if(item.Materials[i].ItemID <= 0)
//                continue;

            bool found = false;
            if (playerInlayItemIDs != null) {
                for(var j = 0; j < playerInlayItemIDs.Length; j++)
                {
                    if(item.ReviseMaterials[i].ID == playerInlayItemIDs[j])
                    {
                        found = true;
                        break;
                    }
                }
            }

            if(!found)
                return false;
        }
        return true;
    }
}