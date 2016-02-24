using GameStruct;

public static class UIDailyLoginBuilder
{
    public static IDailyLoginReward.Data BuildDailyReward(int day, TItemData itemData, 
                                                          IDailyLoginReward.EStatus status)
    {
        return new IDailyLoginReward.Data
        {
            Day = string.Format(TextConst.S(3811), day),
            ItemData = itemData,
            Name = itemData.Name,
            Status = status
        };
    }
	
}
