using GameStruct;

public static class UIEquipChecker
{
    public static bool IsUpgradeable(TItemData item, int[] playerInlayItemIDs)
    {
        return HasUpgradeItem(item) && IsInlayFull(item, playerInlayItemIDs);
    }

    public static bool HasUpgradeItem(TItemData item)
    {
        return GameData.DItemData.ContainsKey(item.UpgradeItem);
    }

    /// <summary>
    /// 是否鑲嵌完畢?
    /// </summary>
    /// <param name="item"></param>
    /// <param name="playerInlayItemIDs"></param>
    /// <returns></returns>
    public static bool IsInlayFull(TItemData item, int[] playerInlayItemIDs)
    {
        for(var i = 0; i < item.Materials.Length; i++)
        {
            if(item.Materials[i] <= 0)
                continue;

            bool found = false;
            for(var j = 0; j < playerInlayItemIDs.Length; j++)
            {
                if(item.Materials[i] == playerInlayItemIDs[j])
                {
                    found = true;
                    break;
                }
            }

            if(!found)
                return false;
        }
        return true;
    }
}