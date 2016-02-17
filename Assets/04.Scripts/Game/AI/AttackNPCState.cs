using GameEnum;

namespace AI
{
    /// <summary>
    /// A 隊(玩家)防守, B 隊(電腦)進攻.
    /// </summary>
    public class AttackNPCState : AttackerState
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

            foreach(PlayerBehaviour player in GameController.Get.GamePlayers)
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

        public override void Update()
        {
        }

        public override void Exit()
        {
        }
    } // end of the class.
} // end of the namespace.
