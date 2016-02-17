using GameEnum;

namespace AI
{
    /// <summary>
    /// 玩家進攻, 電腦防守.
    /// </summary>
    public class AttackGamerState : AttackerState
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.GamerAttack; }
        }

        public override void Enter(object extraInfo)
        {
            base.Enter(extraInfo);

            if(Players.Count == 0)
            {
                foreach(PlayerBehaviour player in GameController.Get.GamePlayers)
                {
                    if(player.Team == ETeamKind.Self)
                        Players.Add(player);
                }
            }

            Tactical = AIController.Get.PlayerAttackTactical;

            CameraMgr.Get.SetCameraSituation(ECameraSituation.Self);

            if(GameController.Get.Joysticker && GameConst.AITime[GameData.Setting.AIChangeTimeLv] > 100)
                GameController.Get.Joysticker.SetManually();

            foreach(PlayerBehaviour player in GameController.Get.GamePlayers)
            {
                if (player.Team == ETeamKind.Self)
                    player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.Attack);
                else if (player.Team == ETeamKind.Npc)
                    player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.Defense);
            }

            if(AIController.Get.AIRemainTime > 0)
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
