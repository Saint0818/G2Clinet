

namespace AI
{
    public class AttackOrDefenseCondition : Condition
    {
        public AttackOrDefenseCondition(AISkillJudger parent) : base(parent)
        {
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
