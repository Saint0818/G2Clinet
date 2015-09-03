using AI;
using GamePlayStruct;

/// <summary>
/// A 隊(玩家)進攻, B 隊(電腦)防守.
/// </summary>
public class AttackAState : State<EGameSituation>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.AttackA; }
    }

    public override void Enter(object extraInfo)
    {
        CameraMgr.Get.SetCameraSituation(ECameraSituation.Self);
//        judgeSkillUI();

        if(GameController.Get.Joysticker && GameData.Setting.AIChangeTime > 100)
            GameController.Get.Joysticker.SetNoAI();
    }

    public override void Update()
    {
//        if(GameController.Get.GamePlayers.Count <= 0)
//            return;
//
//        TTacticalData attackTactical;
//        if (GameController.Get.BallOwner != null)
//        {
//            switch(GameController.Get.BallOwner.Postion)
//            {
//                case EPlayerPostion.C:
//                    AITools.RandomTactical(ETactical.Center, out attackTactical);
//                    break;
//                case EPlayerPostion.F:
//                    AITools.RandomTactical(ETactical.Forward, out attackTactical);
//                    break;
//                case EPlayerPostion.G:
//                    AITools.RandomTactical(ETactical.Guard, out attackTactical);
//                    break;
//                default:
//                    AITools.RandomTactical(ETactical.Attack, out attackTactical);
//                    break;
//            }
//        }
//        else
//            AITools.RandomTactical(ETactical.Attack, out attackTactical);
//
//        for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
//        {
//            PlayerBehaviour npc = GameController.Get.GamePlayers[i];
//            if(npc.AIing && !GameController.Get.DoSkill(npc))
//            {
//                if(npc.Team == ETeamKind.Self)
//                {
//                    if(!GameController.Get.IsShooting)
//                    {
//                        GameController.Get.aiAttack(ref npc);
//                        GameController.Get.aiMove(npc, ref attackTactical);
//                    }
//                    else if (!npc.IsAllShoot)
//                    {
//                        GameController.Get.aiAttack(ref npc);
//                        GameController.Get.aiMove(npc, ref attackTactical);
//                    }
//                }
//                else
//                    GameController.Get.aiDefend(ref npc);
//            }
//        }
    }

    public override void Exit()
    {
    }
}