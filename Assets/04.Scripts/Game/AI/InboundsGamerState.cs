
namespace AI
{
    public class InboundsGamerState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.InboundsGamer; }
        }

        public override void Enter(object extraInfo)
        {
        }

        public override void UpdateAI()
        {
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    } // end of the class.
} // end of the namespace.


