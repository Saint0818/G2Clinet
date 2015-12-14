namespace AI
{
    /// <summary>
    /// 球員跑戰術.
    /// </summary>
    public class TacticalAction : PlayerAIAction
    {
        public TacticalAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return GameConst.AITacticalRate; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            return Player.CanMove;
        }

        public override void Do()
        {
        }
    }
}