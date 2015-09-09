using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    public enum EAISkillLv
    {
        AttackBasketDistance
    }

    public class AISkillLvFactory
    {
        [CanBeNull]
        public Condition Create(EAISkillLv lv, float value, AISkillJudger parent)
        {
            switch(lv)
            {
                case EAISkillLv.AttackBasketDistance:
                    return new AttackBasketDistanceCondition(parent, value);
            }

            Debug.LogWarningFormat("EAISkillLv:{0} is ignore!, value:{1}", lv, value);

            return null;
        }
    }
}