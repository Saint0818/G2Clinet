using AI;

/// <summary>
/// A 隊(玩家)進攻, B 隊(電腦)防守.
/// </summary>
public class AttackBState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.AttackB; }
    }

    public override void EnterImpl(object extraInfo)
    {
        CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc);

        if(GameController.Get.Joysticker && GameData.Setting.AIChangeTime > 100)
            GameController.Get.Joysticker.SetNoAI();
    }

    public override void Update()
    {
//        if (PlayerList.Count > 0)
//        {
//            if (BallOwner != null)
//            {
//                switch (BallOwner.Postion)
//                {
//                    case EPlayerPostion.C:
//                        AITools.RandomTactical(ETactical.Center, out attackTactical);
//                        break;
//                    case EPlayerPostion.F:
//                        AITools.RandomTactical(ETactical.Forward, out attackTactical);
//                        break;
//                    case EPlayerPostion.G:
//                        AITools.RandomTactical(ETactical.Guard, out attackTactical);
//                        break;
//                    default:
//                        AITools.RandomTactical(ETactical.Attack, out attackTactical);
//                        break;
//                }
//            }
//            else
//                AITools.RandomTactical(ETactical.Attack, out attackTactical);
//
//            for (int i = 0; i < PlayerList.Count; i++)
//            {
//                PlayerBehaviour npc = PlayerList[i];
//                if (npc.AIing && !DoSkill(npc))
//                {
//                    if (npc.Team == team)
//                    {
//                        if (!IsShooting)
//                        {
//                            aiAttack(ref npc);
//                            aiMove(npc, ref attackTactical);
//                        }
//                        else if (!npc.IsAllShoot)
//                        {
//                            aiAttack(ref npc);
//                            aiMove(npc, ref attackTactical);
//                        }
//                    }
//                    else
//                        aiDefend(ref npc);
//                }
//            }
//        }
    }

    public override void Exit()
    {
    }
}