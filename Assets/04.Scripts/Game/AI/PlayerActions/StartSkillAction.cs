
using GameStruct;
using JetBrains.Annotations;

namespace AI
{
    /// <summary>
    /// 發動技能.
    /// </summary>
    /// 使用方法:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> Call Init() 初始化. </item>
    /// <item> Call Do() 嘗試放主動技. </item>
    /// </list>
    public class StartSkillAction
    {
        private readonly PlayerBehaviour mPlayer;

        /// <summary>
        /// 下一個要發的主動技.
        /// </summary>
        private int mNextSkillIndex;

        [CanBeNull]
        private AISkillJudger[] mSkillJudgers;

        public StartSkillAction(PlayerBehaviour player)
        {
            mPlayer = player;
        }

        public void Init([NotNull]PlayerBehaviour[] players, bool isAttack)
        {
            if(mPlayer.Attribute.ActiveSkills.Count == 0)
                return;

            mSkillJudgers = new AISkillJudger[mPlayer.Attribute.ActiveSkills.Count];
            for(int i = 0; i < mSkillJudgers.Length; i++)
            {
                mSkillJudgers[i] = new AISkillJudger(mPlayer, isAttack, players);
                if(GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkills[i].ID))
                {
                    TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkills[i].ID];
                    mSkillJudgers[i].SetNewCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
                }
            }
        }

        /// <summary>
        /// 嘗試放主動技.
        /// </summary>
        /// <returns> true: 主動技施放成功. </returns>
        public bool Do()
        {
            if(mPlayer.Attribute.ActiveSkills.Count > 0)
            {
                if(mSkillJudgers != null && mSkillJudgers[mNextSkillIndex].IsMatchCondition() &&
                   mPlayer.CanUseActiveSkill(mPlayer.Attribute.ActiveSkills[mNextSkillIndex]))
                {
                    if(GameController.Get.DoSkill(mPlayer, mPlayer.Attribute.ActiveSkills[mNextSkillIndex]))
                    {
                        // 現在規則是主動技會按順序發動, 第 1 招發完, 下一次會放第 2 招.
                        ++mNextSkillIndex;
                        if (mNextSkillIndex >= mPlayer.Attribute.ActiveSkills.Count)
                            mNextSkillIndex = 0;

                        return true; // 主動技真的放成功, 才真的會結束 AI 的判斷.
                    }
                }
            }

            return false;
        }
    }
}