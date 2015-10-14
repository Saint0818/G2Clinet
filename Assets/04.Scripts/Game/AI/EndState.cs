
namespace AI
{
    public class EndState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.End; }
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
    } // end of the class.
} // end of the namespace.

