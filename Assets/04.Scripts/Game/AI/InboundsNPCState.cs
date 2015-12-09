
namespace AI
{
    public class InboundsNPCState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.InboundsNPC; }
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
    }
}


