using System;
using System.Text;
using GameStruct;

public class TMainStageRewardAgain
{
    /// <summary>
    /// 帳號的錢.
    /// </summary>
    public int Money;

    /// <summary>
    /// 帳號的鑽石.
    /// </summary>
    public int Diamond;

    /// <summary>
    /// 球員的等級和經驗值.
    /// </summary>
    public int PlayerLv;
    public int PlayerExp;

    /// <summary>
    /// 得到的亂數獎勵.
    /// </summary>
    public int RandomItemID;

    /// <summary>
    /// 玩家可能得到的亂數獎勵.
    /// </summary>
    public int[] CandidateItemIDs;

    /// <summary>
    /// 帳號的倉庫資料.
    /// </summary>
    public TItem[] Items;

    /// <summary>
    /// 帳號的數值裝.
    /// </summary>
    public TValueItem[] ValueItems;

    /// <summary>
    /// 帳號的材料.
    /// </summary>
    public TMaterialItem[] MaterialItems;

    /// <summary>
    /// 帳號的技能卡資料.
    /// </summary>
    public TSkill[] SkillCards;

    public override string ToString()
    {
        return string.Format("Money:{3}, Diamond:{0}, RandomItemID:{1}, CandidateItem:{2}", 
            Diamond, RandomItemID, convert(CandidateItemIDs), Money);
    }

    private readonly StringBuilder mBuilder = new StringBuilder();
    private string convert(int[] data)
    {
        if (data == null)
            return String.Empty;

        mBuilder.Length = 0;

        for (var i = 0; i < data.Length; i++)
        {
            mBuilder.Append(data[i]);
            if (i + 1 < data.Length) // 不是最後一個
                mBuilder.Append(",");
        }

        return mBuilder.ToString();
    }
}