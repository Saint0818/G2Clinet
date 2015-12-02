
using System;
using System.Text;
using GameStruct;

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

    /// <summary>
    /// 帳號的技能卡資料.
    /// </summary>
    public TSkill[] SkillCards;

    public override string ToString()
    {
        return string.Format("AddMoney: {0}, AddExp: {1}, AddDiamond: {2}, Diamond: {3}, RandomItemID: {4}, " +
                             "SurelyItemIDs:{5}, CandidateItemIDs:{6}", AddMoney, AddExp, AddDiamond, 
                             Diamond, RandomItemID, convert(SurelyItemIDs), convert(CandidateItemIDs));
    }

    private readonly StringBuilder mBuilder = new StringBuilder();
    private string convert(int[] data)
    {
        if(data == null)
            return String.Empty;

        mBuilder.Length = 0;

        for(var i = 0; i < data.Length; i++)
        {
            mBuilder.Append(data[i]);
            if(i + 1 < data.Length) // 不是最後一個
                mBuilder.Append(",");
        }

        return mBuilder.ToString();
    }
}
