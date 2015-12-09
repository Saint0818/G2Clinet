
namespace AI
{
    /// <summary>
    /// 對手得分, 玩家隊伍執行撿球.
    /// </summary>
    public class GamerPickBallState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.GamerPickBall; }
        }

        public override void Enter(object extraInfo)
        {
            CourtMgr.Get.Walls[1].SetActive(false);
            UIGame.Get.ChangeControl(true);
            CameraMgr.Get.SetCameraSituation(ECameraSituation.Self, true);

            GameController.Get.PickBallPlayer = null;
            for(int i = 0; i < GameController.Get.GamePlayers.Count; i++)
                GameController.Get.GamePlayers[i].IsCanCatchBall = true;
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
