
namespace AI
{
    /// <summary>
    /// A 隊(玩家)防守, B 隊(電腦)進攻.
    /// </summary>
    public class AttackNPCState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.AttackNPC; }
        }

        public override void Enter(object extraInfo)
        {
            CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc);

			if (GameController.Get.Joysticker && GameConst.AITime[GameData.Setting.AIChangeTimeLv] > 100)
                GameController.Get.Joysticker.SetManually();

            foreach (PlayerBehaviour player in GameController.Get.GamePlayers)
            {
                if (player.Team == ETeamKind.Self)
                    player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.Defense);
                else if (player.Team == ETeamKind.Npc)
                    player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.Attack);
            }

            if (AIController.Get.AIRemainTime > 0)
            {
                GameController.Get.Joysticker.SetAITime(AIController.Get.AIRemainTime);
                GameController.Get.Joysticker.AniState(EPlayerState.Idle);
                AIController.Get.AIRemainTime = 0;
            }
        }

        public override void UpdateAI()
        {
            if (GameController.Get.GamePlayers.Count <= 0)
                return;

            TTacticalData tactical;
            if (GameController.Get.BallOwner != null)
            {
                switch (GameController.Get.BallOwner.Postion)
                {
                    case EPlayerPostion.C:
                        AITools.RandomTactical(ETactical.Center, out tactical);
                        break;
                    case EPlayerPostion.F:
                        AITools.RandomTactical(ETactical.Forward, out tactical);
                        break;
                    case EPlayerPostion.G:
                        AITools.RandomTactical(ETactical.Guard, out tactical);
                        break;
                    default:
                        AITools.RandomTactical(ETactical.Attack, out tactical);
                        break;
                }
            }
            else
                AITools.RandomTactical(ETactical.Attack, out tactical);

            GameMsgDispatcher.Ins.SendMesssage(EGameMsg.CoachOrderAttackTactical, tactical);
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
