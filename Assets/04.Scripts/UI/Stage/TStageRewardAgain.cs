using GameStruct;

/// <summary>
/// 
/// </summary>
public class TStageRewardAgain
{
    /// <summary>
    /// 該帳號的鑽石.
    /// </summary>
    public int Diamond;

    /// <summary>
    /// 得到的亂數獎勵.
    /// </summary>
    public int RandomItemID;

    /// <summary>
    /// 玩家可能得到的亂數獎勵.
    /// </summary>
    public int[] CandidateItemIDs;

    /// <summary>
    /// 玩家的倉庫資料.
    /// </summary>
    public TItem[] Items;

    public override string ToString()
    {
        int candiateNum = 0;
        if(CandidateItemIDs != null)
            candiateNum = CandidateItemIDs.Length;
        return string.Format("Diamond:{0}, RandomItemID:{1}, CandidateItemNum:{2}", Diamond, RandomItemID, candiateNum);
    }
}