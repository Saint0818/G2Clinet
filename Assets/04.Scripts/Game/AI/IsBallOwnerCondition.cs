

namespace AI
{
    public class IsBallOwnerCondition : Condition
    {
        private readonly int mValue;

        public IsBallOwnerCondition(AISkillJudger parent, int value) : base(parent)
        {
            mValue = value;
        }

        public override bool IsValid()
        {
            if(mValue == 1)
                return Parent.FocusPlayer.IsBallOwner;

            return true;
        }
    }
}
