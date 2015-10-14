namespace AI
{
    public class PlayerAttackNoneState : State<PlayerAttackState.EPlayerAttackState, EGameMsg>
    {
        public override PlayerAttackState.EPlayerAttackState ID
        {
            get { return PlayerAttackState.EPlayerAttackState.None;}
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

        public override void UpdateAI()
        {
        }

//        public override void Update()
//        {
//        }
    } // end of the class.
} // end of the namespace.