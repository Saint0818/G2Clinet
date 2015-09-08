using System.Collections.Generic;
using AI;
using JetBrains.Annotations;

/// <summary>
/// 用來判斷主動技是否可以施放的判斷類別.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> new instance. </item>
/// <item> call SetCondition() 設定要判斷的條件. </item>
/// <item> call IsMatchCondition() 檢查條件是否符合. </item>
/// </list>
/// 
/// 增加判斷條件(Skill Table 的 Situation):
/// <list type="number">
/// <item> 繼承 Condition. </item>
/// <item> 修改 AISkillConditionFactory. </item>
/// </list>
/// </remarks>
public class AISkillJudger
{
    public PlayerBehaviour[] Players
    {
        get { return mPlayers; }
    }
    private readonly PlayerBehaviour[] mPlayers;

    private readonly List<Condition> mConditions = new List<Condition>();

    private readonly AISkillConditionFactory mFactory = new AISkillConditionFactory();

    /// <summary>
    /// 是否是進攻方.
    /// </summary>
    public bool IsAttack
    {
        get { return mIsAttack; }
    }
    private readonly bool mIsAttack;

    /// <summary>
    /// 以哪位球員本身的狀態來判斷主動技是否可以施放.
    /// </summary>
    public PlayerBehaviour FocusPlayer
    {
        get { return mFocusPlayer; }
    }
    private readonly PlayerBehaviour mFocusPlayer;

    public AISkillJudger(PlayerBehaviour focusPlayer, PlayerBehaviour[] players, bool isAttack)
    {
        mFocusPlayer = focusPlayer;
        mPlayers = players;
        mIsAttack = isAttack;
    }

    public void SetCondition([NotNull]string situation, int aiSkillLvID)
    {
        int[] bits = BitConverter.Convert(situation);
        if(bits == null)
            return;

        mConditions.Clear();
        for(int bitNum = 0; bitNum < bits.Length; bitNum++)
        {
            var condition = mFactory.Create(bitNum, bits[bitNum], this);
            if(condition != null)
                mConditions.Add(condition);
        }
    }

    public bool IsMatchCondition()
    {
        if(mConditions.Count == 0)
            return false;

        for(int i = 0; i < mConditions.Count; i++)
        {
            if(!mConditions[i].IsValid())
                return false;
        }
        return true;

//        return mConditions.All(condition => condition.IsValid());
    }
}
