
namespace AI
{
    public class CameraMovementState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.CameraMovement; }
        }

        public override void Enter(object extraInfo)
        {
            CourtMgr.Get.ShowEnd(true);
            GameController.Get.InitIngameAnimator();
            GameController.Get.SetBornPositions();

            UISkip.UIShow(false, ESkipSituation.Game);
            CourtMgr.Get.ShowEnd();
            UIGame.UIShow(true);
            UIGame.Get.UIState(EUISituation.ShowTwo);
        }

        public override void UpdateAI()
        {
        }

//        public override void Update()
//        {
//        }

        public override void Exit()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    } // end of the class.
} // end of the namespace.
