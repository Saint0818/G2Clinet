using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    public enum EAISkillLv
    {
        AttackBasketDistance
    }

    /// <summary>
    /// 如果有新狀態時, 必須要在 Controctor 新增新的 condition, 並檢查 Create 是否需要做對應的修改.
    /// </summary>
    public class AISkillLvFactory
    {
        private readonly Dictionary<EAISkillLv, Condition> mConditions = new Dictionary<EAISkillLv, Condition>();

        public AISkillLvFactory(AISkillJudger judger)
        {
            mConditions.Add(EAISkillLv.AttackBasketDistance, new AttackBasketDistanceCondition(judger));
        }

        [CanBeNull]
        public Condition Create(EAISkillLv lv, float value)
        {
            if(mConditions.ContainsKey(lv))
            {
                mConditions[lv].Init(value);
                return mConditions[lv];
            }

            Debug.LogWarningFormat("EAISkillLv:{0} is ignore!, value:{1}", lv, value);

            return null;
        }
    }
}