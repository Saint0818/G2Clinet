
using GameStruct;

/// <summary>
/// 
/// </summary>
public class TStageReward
{
    /// <summary>
    /// 獎勵給的金幣.
    /// </summary>
    public int Money;

    /// <summary>
    /// 獎勵給多少經驗值.
    /// </summary>
    public int Exp;

    /// <summary>
    /// 必給的道具 ID.
    /// </summary>
    public int[] SurelyItemIDs;

    /// <summary>
    /// 得到的亂數獎勵.
    /// </summary>
    public int RandomItemID;

    /// <summary>
    /// 玩家可能得到的亂數獎勵.
    /// </summary>
    public int[] CandidateItemIDs;

    /// <summary>
    /// 某位球員的資料.
    /// </summary>
    public TPlayer Player;

    /// <summary>
    /// 玩家的倉庫資料.
    /// </summary>
    public TItem[] Items;

    public override string ToString()
    {
        return string.Format("Money: {0}, Exp: {1}, RandomItemID: {2}", Money, Exp, RandomItemID);
    }
}
