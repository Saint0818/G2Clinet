
namespace AI
{
    public class NullAction : PlayerAIAction
    {
        public NullAction() : base(null, null)
        {
        }

        public override float Probability
        {
            get { return 0; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            return true;
        }

        public override void Do()
        {
        }
    }
}