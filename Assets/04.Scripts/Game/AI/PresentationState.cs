namespace AI
{
    public class PresentationState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.Presentation; }
        }

        public override void Enter(object extraInfo)
        {
            UISkip.UIShow(true, ESkipSituation.Game);
        }

        public override void Exit()
        {
        }

        public override void UpdateAI()
        {
            for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
            {
                if (GameController.Get.GamePlayers[i].ShowPos != -1)
                {
                    GameController.Get.GamePlayers[i].gameObject.transform.position = CameraMgr.Get.CharacterPos[GameController.Get.GamePlayers[i].ShowPos].transform.position;
                    GameController.Get.GamePlayers[i].gameObject.transform.eulerAngles = CameraMgr.Get.CharacterPos[GameController.Get.GamePlayers[i].ShowPos].transform.eulerAngles;
                }
            }
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
            if (msg.Msg == EGameMsg.UISkipClickOnGaming)
            {
				GameController.Get.ChangeSituation(EGameSituation.CameraMovement);
				AIController.Get.ChangeState(EGameSituation.CameraMovement);
            }
        }
    }
}
