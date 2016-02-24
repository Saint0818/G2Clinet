using GameStruct;

public static class UIDailyLoginBuilder
{
    public static DailyLoginReward.Data BuildDailyReward(int day, TItemData itemData, 
                                                         DailyLoginReward.EStatus status)
    {
        return new DailyLoginReward.Data
        {
            Day = string.Format(TextConst.S(3811), day),
            ItemData = itemData,
            Name = itemData.Name,
            Status = status
        };
    }

    public static UILifetimeReward.Data BuildLifetimeReward(TLifetimeData data)
    {
        return new UILifetimeReward.Data
        {
            LoginNum = string.Format("{0}/{1}", GameData.Team.LifetimeRecord.LoginNum, data.LoginNum)
        };
    }
}
