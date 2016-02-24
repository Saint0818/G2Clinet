using System;
using System.Collections.Generic;
using JetBrains.Annotations;

/// <summary>
/// LifeTime 表格中的某一筆資料.
/// </summary>
public class TLifetimeData
{
    public struct Reward
    {
        public int ItemID;
        public int ItemNum;
    }

    [UsedImplicitly]
    public int LoginNum { get; private set; }

    public Reward[] Rewards
    {
        get { return mRewards ?? (mRewards = buildRewards()); }
    }
    private Reward[] mRewards;

    private Reward[] buildRewards()
    {
        Action<List<Reward>, int, int> validAndAdd = (ids, itemID, itemNum) =>
        {
            if(GameData.DItemData.ContainsKey(itemID) && itemNum > 0)
                ids.Add(new Reward
                {
                    ItemID = itemID,
                    ItemNum = itemNum
                });
        };

        List<Reward> rewards = new List<Reward>();
        validAndAdd(rewards, ItemID1, ItemNum1);
        validAndAdd(rewards, ItemID2, ItemNum2);
        validAndAdd(rewards, ItemID3, ItemNum3);

        return rewards.ToArray();
    }

    [UsedImplicitly] public int ItemID1 { get; private set; }
    [UsedImplicitly] public int ItemNum1 { get; private set; }
    [UsedImplicitly] public int ItemID2 { get; private set; }
    [UsedImplicitly] public int ItemNum2 { get; private set; }
    [UsedImplicitly] public int ItemID3 { get; private set; }
    [UsedImplicitly] public int ItemNum3 { get; private set; }
    
    public override string ToString()
    {
        return string.Format("LoginNum:{0}, RewardCount:{1}", LoginNum, Rewards.Length);
    }
}