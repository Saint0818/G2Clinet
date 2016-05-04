using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;

public class TStageRewardAgain
{
    /// <summary>
    /// 帳號的體力.
    /// </summary>
    [UsedImplicitly]
    public int Power;

    /// <summary>
    /// 帳號的錢.
    /// </summary>
    [UsedImplicitly]
    public int Money;

    /// <summary>
    /// 帳號的鑽石.
    /// </summary>
    [UsedImplicitly]
    public int Diamond;

    /// <summary>
    /// 球員的等級和經驗值.
    /// </summary>
    [UsedImplicitly]
    public int PlayerLv;
    [UsedImplicitly]
    public int PlayerExp;

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
        return string.Format("Money:{3}, Diamond:{0}, RandomItemID:{1}, CandidateItem:{2}", 
            Diamond, RandomItemID, DebugerString.Convert(CandidateItemIDs), Money);
    }
}