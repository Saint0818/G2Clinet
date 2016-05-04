
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;

/// <summary>
/// 關卡獲勝的獎勵.
/// </summary>
public class TStageReward
{
    /// <summary>
    /// 關卡給的金幣.
    /// </summary>
    [UsedImplicitly]
    public int AddMoney;

    /// <summary>
    /// 玩家帳號的體力.
    /// </summary>
    [UsedImplicitly]
    public int Power;

    /// <summary>
    /// 玩家帳號的金錢.
    /// </summary>
    [UsedImplicitly]
    public int Money;

    /// <summary>
    /// 獎勵給多少經驗值.
    /// </summary>
    [UsedImplicitly]
    public int AddExp;

    /// <summary>
    /// 獎勵給多少鑽石.
    /// </summary>
    [UsedImplicitly]
    public int AddDiamond;

    /// <summary>
    /// 玩家帳號目前的鑽石數量.
    /// </summary>
    [UsedImplicitly]
    public int Diamond;

    /// <summary>
    /// 必給的獎勵.(每個 element 都是 ItemID)
    /// </summary>
    [UsedImplicitly]
    public int[] SurelyItemIDs;

    /// <summary>
    /// 得到的亂數獎勵.
    /// </summary>
    [UsedImplicitly]
    public int RandomItemID;

    /// <summary>
    /// 玩家可能得到的亂數獎勵.
    /// </summary>
    [UsedImplicitly]
    public int[] CandidateItemIDs;

    /// <summary>
    /// 某位球員的資料.
    /// </summary>
    [UsedImplicitly]
    public TPlayer Player;

    /// <summary>
    /// 帳號的倉庫資料.
    /// </summary>
    [UsedImplicitly]
    public TItem[] Items;

    /// <summary>
    /// 帳號的數值裝.
    /// </summary>
    [UsedImplicitly]
    public TValueItem[] ValueItems;

    /// <summary>
    /// 帳號的材料.
    /// </summary>
    [UsedImplicitly]
    public TMaterialItem[] MaterialItems;

    /// <summary>
    /// 帳號的技能卡資料.
    /// </summary>
    [UsedImplicitly]
    public TSkill[] SkillCards;

    [UsedImplicitly]
    public Dictionary<int, int> GotItemCount;

    public override string ToString()
    {
        return string.Format("Power:{7}, Money:{8}, Diamond: {3}, AddMoney: {0}, AddExp: {1}, AddDiamond: {2}, RandomItemID: {4}, " +
                             "SurelyItemIDs:{5}, CandidateItemIDs:{6}", AddMoney, AddExp, AddDiamond, 
                             Diamond, RandomItemID, DebugerString.Convert(SurelyItemIDs), 
                             DebugerString.Convert(CandidateItemIDs), Power, Money);
    }
}
