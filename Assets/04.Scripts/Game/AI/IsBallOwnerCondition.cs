

namespace AI
{
    /// <summary>
    /// 是否球員有持球.
    /// </summary>
    public class IsBallOwnerCondition : Condition
    {
        private int mValue;

        public IsBallOwnerCondition(AISkillJudger parent) : base(parent)
        {
        }

        public override void Init(object value)
        {
            mValue = (int)value;
        }

        public override bool IsValid()
        {
            if(mValue == 1)
                return Parent.FocusPlayer.IsBallOwner;

            return true;
        }
    }
}
