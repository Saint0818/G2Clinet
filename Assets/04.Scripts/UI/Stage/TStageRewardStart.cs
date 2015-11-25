
using GameStruct;

/// <summary>
/// 
/// </summary>
public class TStageRewardStart
{
    /// <summary>
    /// 獎勵給的金幣.
    /// </summary>
    public int AddMoney;

    /// <summary>
    /// 玩家帳號的金錢.
    /// </summary>
    public int Money;

    /// <summary>
    /// 獎勵給多少經驗值.
    /// </summary>
    public int AddExp;

    /// <summary>
    /// 獎勵給多少鑽石.
    /// </summary>
    public int AddDiamond;

    /// <summary>
    /// 玩家帳號目前的鑽石數量.
    /// </summary>
    public int Diamond;

    /// <summary>
    /// 必給的獎勵.(每個 element 都是 ItemID)
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
        int surelyNum = 0;
        if(SurelyItemIDs != null)
            surelyNum = SurelyItemIDs.Length;
        int candidateNum = 0;
        if(CandidateItemIDs != null)
            candidateNum = CandidateItemIDs.Length;
        return string.Format("AddMoney: {0}, AddExp: {1}, AddDiamond: {2}, Diamond: {3}, RandomItemID: {4}, SurelyNums:{5}, CandidateNum:{6}", AddMoney, AddExp, AddDiamond, Diamond, RandomItemID, surelyNum, candidateNum);
    }
}
