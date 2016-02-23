using GameStruct;

public static class UIDailyLoginBuilder
{
    public static UIDailyLoginReward.Data BuildDailyReward(int day, TItemData itemData, bool showClear)
    {
        return new UIDailyLoginReward.Data
        {
            Day = string.Format(TextConst.S(3811), day),
            ItemData = itemData,
            Name = itemData.Name,
            ShowClear = showClear
        };
    }
	
}
