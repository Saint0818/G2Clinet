
namespace AI
{
    public class NoneState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.None; }
        }

        public override void Enter(object extraInfo)
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
    }
} // end of the namespace AI.


