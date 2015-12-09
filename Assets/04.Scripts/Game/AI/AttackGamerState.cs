
namespace AI
{
    /// <summary>
    /// A 隊(玩家)進攻, B 隊(電腦)防守.
    /// </summary>
    public class AttackGamerState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.AttackGamer; }
        }

        public override void Enter(object extraInfo)
        {
            CameraMgr.Get.SetCameraSituation(ECameraSituation.Self);

            if (GameController.Get.Joysticker && GameConst.AITime[GameData.Setting.AIChangeTimeLv] > 100)
                GameController.Get.Joysticker.SetManually();

            foreach (PlayerBehaviour player in GameController.Get.GamePlayers)
            {
                if (player.Team == ETeamKind.Self)
                    player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.Attack);
                else if (player.Team == ETeamKind.Npc)
                    player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.Defense);
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

            //        for(int i = 0; i < GameController.Get.GamePlayers.Count; i++)
            //        {
            //            PlayerBehaviour player = GameController.Get.GamePlayers[i];
            //            if(player.AIing && !GameController.Get.DoSkill(player))
            //            {
            //                if(player.Team == ETeamKind.Self)
            //                {
            //                    if(!GameController.Get.IsShooting || !player.IsAllShoot)
            //                    {
            //                        GameController.Get.AIAttack(player);
            //                        GameController.Get.AIMove(player, ref tactical);
            //                    }
            //                }
            //                else
            //                    GameController.Get.AIDefend(player);
            //            }
            //        }
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
