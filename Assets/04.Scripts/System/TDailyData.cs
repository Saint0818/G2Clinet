using System;
using System.Collections.Generic;
using JetBrains.Annotations;

/// <summary>
/// Limit 表格中的某一筆資料.
/// </summary>
public class TDailyData
{
    public struct Reward
    {
        public int ItemID;
        public int ItemNum;
    }

    [UsedImplicitly]
    public int Year { get; private set; }

    [UsedImplicitly]
    public int Month { get; private set; }

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
        validAndAdd(rewards, ItemID4, ItemNum4);
        validAndAdd(rewards, ItemID5, ItemNum5);
        validAndAdd(rewards, ItemID6, ItemNum6);
        validAndAdd(rewards, ItemID7, ItemNum7);
        validAndAdd(rewards, ItemID8, ItemNum8);
        validAndAdd(rewards, ItemID9, ItemNum9);
        validAndAdd(rewards, ItemID10, ItemNum10);
        validAndAdd(rewards, ItemID11, ItemNum11);
        validAndAdd(rewards, ItemID12, ItemNum12);
        validAndAdd(rewards, ItemID13, ItemNum13);
        validAndAdd(rewards, ItemID14, ItemNum14);
        validAndAdd(rewards, ItemID15, ItemNum15);
        validAndAdd(rewards, ItemID16, ItemNum16);
        validAndAdd(rewards, ItemID17, ItemNum17);
        validAndAdd(rewards, ItemID18, ItemNum18);
        validAndAdd(rewards, ItemID19, ItemNum19);
        validAndAdd(rewards, ItemID20, ItemNum20);
        validAndAdd(rewards, ItemID21, ItemNum21);
        validAndAdd(rewards, ItemID22, ItemNum22);
        validAndAdd(rewards, ItemID23, ItemNum23);
        validAndAdd(rewards, ItemID24, ItemNum24);
        validAndAdd(rewards, ItemID25, ItemNum25);
        validAndAdd(rewards, ItemID26, ItemNum26);
        validAndAdd(rewards, ItemID27, ItemNum27);
        validAndAdd(rewards, ItemID28, ItemNum28);

        return rewards.ToArray();
    }

    [UsedImplicitly] public int ItemID1 { get; private set; }
    [UsedImplicitly] public int ItemNum1 { get; private set; }
    [UsedImplicitly] public int ItemID2 { get; private set; }
    [UsedImplicitly] public int ItemNum2 { get; private set; }
    [UsedImplicitly] public int ItemID3 { get; private set; }
    [UsedImplicitly] public int ItemNum3 { get; private set; }
    [UsedImplicitly] public int ItemID4 { get; private set; }
    [UsedImplicitly] public int ItemNum4 { get; private set; }
    [UsedImplicitly] public int ItemID5 { get; private set; }
    [UsedImplicitly] public int ItemNum5 { get; private set; }
    [UsedImplicitly] public int ItemID6 { get; private set; }
    [UsedImplicitly] public int ItemNum6 { get; private set; }
    [UsedImplicitly] public int ItemID7 { get; private set; }
    [UsedImplicitly] public int ItemNum7 { get; private set; }
    [UsedImplicitly] public int ItemID8 { get; private set; }
    [UsedImplicitly] public int ItemNum8 { get; private set; }
    [UsedImplicitly] public int ItemID9 { get; private set; }
    [UsedImplicitly] public int ItemNum9 { get; private set; }
    [UsedImplicitly] public int ItemID10 { get; private set; }
    [UsedImplicitly] public int ItemNum10 { get; private set; }
    [UsedImplicitly] public int ItemID11 { get; private set; }
    [UsedImplicitly] public int ItemNum11 { get; private set; }
    [UsedImplicitly] public int ItemID12 { get; private set; }
    [UsedImplicitly] public int ItemNum12 { get; private set; }
    [UsedImplicitly] public int ItemID13 { get; private set; }
    [UsedImplicitly] public int ItemNum13 { get; private set; }
    [UsedImplicitly] public int ItemID14 { get; private set; }
    [UsedImplicitly] public int ItemNum14 { get; private set; }
    [UsedImplicitly] public int ItemID15 { get; private set; }
    [UsedImplicitly] public int ItemNum15 { get; private set; }
    [UsedImplicitly] public int ItemID16 { get; private set; }
    [UsedImplicitly] public int ItemNum16 { get; private set; }
    [UsedImplicitly] public int ItemID17 { get; private set; }
    [UsedImplicitly] public int ItemNum17 { get; private set; }
    [UsedImplicitly] public int ItemID18 { get; private set; }
    [UsedImplicitly] public int ItemNum18 { get; private set; }
    [UsedImplicitly] public int ItemID19 { get; private set; }
    [UsedImplicitly] public int ItemNum19 { get; private set; }
    [UsedImplicitly] public int ItemID20 { get; private set; }
    [UsedImplicitly] public int ItemNum20 { get; private set; }
    [UsedImplicitly] public int ItemID21 { get; private set; }
    [UsedImplicitly] public int ItemNum21 { get; private set; }
    [UsedImplicitly] public int ItemID22 { get; private set; }
    [UsedImplicitly] public int ItemNum22 { get; private set; }
    [UsedImplicitly] public int ItemID23 { get; private set; }
    [UsedImplicitly] public int ItemNum23 { get; private set; }
    [UsedImplicitly] public int ItemID24 { get; private set; }
    [UsedImplicitly] public int ItemNum24 { get; private set; }
    [UsedImplicitly] public int ItemID25 { get; private set; }
    [UsedImplicitly] public int ItemNum25 { get; private set; }
    [UsedImplicitly] public int ItemID26 { get; private set; }
    [UsedImplicitly] public int ItemNum26 { get; private set; }
    [UsedImplicitly] public int ItemID27 { get; private set; }
    [UsedImplicitly] public int ItemNum27 { get; private set; }
    [UsedImplicitly] public int ItemID28 { get; private set; }
    [UsedImplicitly] public int ItemNum28 { get; private set; }

    public override string ToString()
    {
        return string.Format("{0}-{1}, RewardCount:{2}", Year, Month, Rewards.Length);
    }
}