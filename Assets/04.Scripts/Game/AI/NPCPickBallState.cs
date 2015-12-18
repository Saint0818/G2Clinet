namespace AI
{
    public class NPCPickBallState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.NPCPickBall; }
        }

        public override void Enter(object extraInfo)
        {
            CourtMgr.Get.Walls[0].SetActive(false);
            UIGame.Get.ChangeControl(false);
            CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc, true);

            GameController.Get.PickBallPlayer = null;
            for(int i = 0; i < GameController.Get.GamePlayers.Count; i++)
                GameController.Get.GamePlayers[i].IsCanCatchBall = true;

            GameController.Get.SetBall();
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
