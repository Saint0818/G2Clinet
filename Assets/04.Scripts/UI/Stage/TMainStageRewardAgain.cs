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
            Diamond, RandomItemID, DebugerString.Convert(CandidateItemIDs), Money);
    }
}