

namespace AI
{
    public class DefenseCondition : Condition
    {
        public DefenseCondition(AISkillJudger parent) : base(parent)
        {
        }

        public override void Init(object value)
        {
        }

        public override bool IsValid()
        {
            return !Parent.IsAttack;
        }
    }
}
