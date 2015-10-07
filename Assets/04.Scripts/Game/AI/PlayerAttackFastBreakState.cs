namespace AI
{
    public class PlayerAttackFastBreakState : State<PlayerAttackState.EPlayerAttackState, EGameMsg>
    {
        public override PlayerAttackState.EPlayerAttackState ID
        {
            get { return PlayerAttackState.EPlayerAttackState.FastBreak;}
        }

        public override void Enter(object extraInfo)
        {
        }

        public override void Exit()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }

        public override void Update()
        {
        }
    }
}