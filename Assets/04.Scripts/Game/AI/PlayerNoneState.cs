namespace AI
{
    public class PlayerNoneState : State<EPlayerAIState, EGameMsg>
    {
        public override EPlayerAIState ID
        {
            get { return EPlayerAIState.None; }
        }

        public override void Enter(object extraInfo)
        {
        }

        public override void Exit()
        {
        }

        public override void UpdateAI()
        {
        }

//        public override void Update()
//        {
//        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    }
}
