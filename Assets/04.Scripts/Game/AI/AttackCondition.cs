

namespace AI
{
    public class AttackCondition : Condition
    {
        public AttackCondition(AISkillJudger parent) : base(parent)
        {
        }

        public override bool IsValid()
        {
            return Parent.IsAttack;
        }
    }
}

