using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    public class AISkillSituationFactory
    {
        [CanBeNull]
        public Condition Create(int bitNum, int value, AISkillJudger parent)
        {
            switch (bitNum)
            {
                case 0: // bit 0.
                    switch (value)
                    {
                        case 0: return new AttackOrDefenseCondition(parent);
                        case 1: return new AttackCondition(parent);
                        case 2: return new DefenseCondition(parent);
                    }
                    break;
                case 1: return new HasNearbyOpponentCondition(parent, value); // bit 1.
                case 2: return new IsBallOwnerCondition(parent, value); // bit 2.
            }

            Debug.LogWarningFormat("bitNum:{0} is ignore!, value:{1}", bitNum, value);

            return null;
        }
    } // end of the class AISkillSituationFactory.
} // end of the namespace AI.
