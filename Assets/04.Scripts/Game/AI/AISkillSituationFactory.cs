using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 如果有新狀態時, 必須要在 Controctor 新增新的 condition, 並檢查 Create 是否需要做對應的修改.
    /// </summary>
    public class AISkillSituationFactory
    {
        private readonly AttackOrDefenseCondition mAttackOrDefense;
        private readonly AttackCondition mAttack;
        private readonly DefenseCondition mDefense;

        private readonly Dictionary<int, Condition> mConditions = new Dictionary<int, Condition>();

        public AISkillSituationFactory(AISkillJudger judger)
        {
            // bit 0.
            mAttackOrDefense = new AttackOrDefenseCondition(judger);
            mAttack = new AttackCondition(judger);
            mDefense = new DefenseCondition(judger);

            mConditions.Add(1, new HasNearbyOpponentCondition(judger)); // bit 1.
            mConditions.Add(2, new IsBallOwnerCondition(judger)); // bit 2.
        }

        [CanBeNull]
        public Condition Create(int bitNum, int value)
        {
            if(mConditions.ContainsKey(bitNum))
            {
                mConditions[bitNum].Init(value);
                return mConditions[bitNum];
            }

            if(bitNum == 0)
            {
                switch(value)
                {
                    case 0: return mAttackOrDefense;
                    case 1: return mAttack;
                    case 2: return mDefense;
                }
            }

            Debug.LogWarningFormat("bitNum:{0} is ignore!, value:{1}", bitNum, value);

            return null;
        }
        

    } // end of the class AISkillSituationFactory.
} // end of the namespace AI.
