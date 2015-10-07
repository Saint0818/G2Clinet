using JetBrains.Annotations;

namespace AI
{
    public class PlayerAttackFastBreakState : State<PlayerAttackState.EPlayerAttackState, EGameMsg>
    {
        public override PlayerAttackState.EPlayerAttackState ID
        {
            get { return PlayerAttackState.EPlayerAttackState.FastBreak;}
        }

        private readonly PlayerAI mPlayerAI;
        private readonly PlayerBehaviour mPlayer;

        public PlayerAttackFastBreakState([NotNull] PlayerAI playerAI, [NotNull] PlayerBehaviour player)
        {
            mPlayerAI = playerAI;
            mPlayer = player;
        }

        public override void Enter(object extraInfo)
        {
            mPlayer.ResetMove();

//            TMoveData moveData = new TMoveData {Target = };
//            mPlayer.TargetPos = moveData;
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