public class DefenseCondition : Condition
{
    public DefenseCondition(AISkillJudger parent) : base(parent)
    {
    }

    public override bool IsValid()
    {
        return !Parent.IsAttack;
    }
}