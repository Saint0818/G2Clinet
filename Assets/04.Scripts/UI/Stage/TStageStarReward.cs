using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;

/// <summary>
/// 關卡獲勝的獎勵.
/// </summary>
public class TStageStarReward
{
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
    /// 玩家帳號目前的鑽石數量.
    /// </summary>
    [UsedImplicitly]
    public int Diamond;

    [UsedImplicitly]
    public Dictionary<int, bool[]> MainStageStarReceived;

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

    [UsedImplicitly]
    public Dictionary<int, int> GotAvatar;

    public override string ToString()
    {
        return string.Format("Power: {0}, Diamond: {1}", Power, Diamond);
    }
}