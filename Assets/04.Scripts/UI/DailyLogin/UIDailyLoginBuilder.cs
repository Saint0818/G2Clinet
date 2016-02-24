using System.Collections.Generic;
using GameStruct;

public static class UIDailyLoginBuilder
{
    public static DailyLoginReward.Data BuildDailyReward(int day, TItemData itemData, 
                                                         UIDailyLoginMain.EStatus status)
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
        List<TItemData> items = new List<TItemData>();
        for(var i = 0; i < data.Rewards.Length; i++)
        {
            if(GameData.DItemData.ContainsKey(data.Rewards[i].ItemID))
                items.Add(GameData.DItemData[data.Rewards[i].ItemID]);
        }

        UIDailyLoginMain.EStatus status;
        int receiveLoginNum = UIDailyLoginHelper.GetLifetimeReceiveLoginNum();
        int currentLoginNum = GameData.Team.LifetimeRecord.LoginNum;
        if(receiveLoginNum >= data.LoginNum)
            status = UIDailyLoginMain.EStatus.Received;
        else
            status = currentLoginNum >= data.LoginNum ? UIDailyLoginMain.EStatus.Receivable : UIDailyLoginMain.EStatus.NoReceive;

        return new UILifetimeReward.Data
        {
            LoginNum = string.Format("{0}/{1}", GameData.Team.LifetimeRecord.LoginNum, data.LoginNum),
            Items = items.ToArray(),
            Status = status
        };
    }
}
