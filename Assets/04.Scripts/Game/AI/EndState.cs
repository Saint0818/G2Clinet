
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
            GameController.Get.IsStart = false;
            for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
                GameController.Get.GamePlayers[i].AniState(EPlayerState.Idle);

            CameraMgr.Get.SetCameraSituation(ECameraSituation.Finish);
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

