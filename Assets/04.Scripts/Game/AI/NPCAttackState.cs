using GameEnum;

namespace AI
{
    /// <summary>
    /// A 隊(玩家)防守, B 隊(電腦)進攻.
    /// </summary>
    public class NPCAttackState : AttackerState
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.NPCAttack; }
        }

        public override void Enter(object extraInfo)
        {
            base.Enter(extraInfo);

            if(Players.Count == 0)
            {
                foreach(PlayerBehaviour player in GameController.Get.GamePlayers)
                {
                    if(player.Team == ETeamKind.Npc)
                        Players.Add(player);
                }
            }

            // 電腦跑 Normal 戰術.
            Tactical = ETacticalAuto.AttackNormal;

            CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc);

			if (GameController.Get.Joysticker && GameConst.AITime[GameData.Setting.AIChangeTimeLv] > 100)
                GameController.Get.Joysticker.SetManually();

            setPlayerStates();

            if (AIController.Get.AIRemainTime > 0)
            {
                GameController.Get.Joysticker.SetAITime(AIController.Get.AIRemainTime);
                GameController.Get.Joysticker.AniState(EPlayerState.Idle);
                AIController.Get.AIRemainTime = 0;
            }
        }

        private static void setPlayerStates()
        {
            foreach(PlayerBehaviour player in GameController.Get.GamePlayers)
            {
                var playerAI = player.GetComponent<PlayerAI>();
                if(player.Team == ETeamKind.Self)
                {
//                    if(playerAI.IsInUpfield())
                        playerAI.ChangeState(EPlayerAIState.ReturnToHome, EPlayerAIState.Defense);
//                    else
//                        playerAI.ChangeState(EPlayerAIState.Defense);
                }
                else if(player.Team == ETeamKind.Npc)
                    playerAI.ChangeState(EPlayerAIState.Attack);
            }
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }
    } // end of the class.
} // end of the namespace.
