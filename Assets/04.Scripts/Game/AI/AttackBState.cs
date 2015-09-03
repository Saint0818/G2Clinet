using AI;
using GamePlayStruct;

/// <summary>
/// A 隊(玩家)防守, B 隊(電腦)進攻.
/// </summary>
public class AttackBState : State<EGameSituation>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.AttackB; }
    }

    public override void Enter(object extraInfo)
    {
        CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc);

        if(GameController.Get.Joysticker && GameData.Setting.AIChangeTime > 100)
            GameController.Get.Joysticker.SetNoAI();

        foreach (PlayerBehaviour player in GameController.Get.GamePlayers)
        {
            if(player.Team == ETeamKind.Self)
                player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.Defense);
            else if (player.Team == ETeamKind.Npc)
                player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.Attack);
        }
    }

    public override void Update()
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

        for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
        {
            PlayerBehaviour player = GameController.Get.GamePlayers[i];
            if (player.AIing && !GameController.Get.DoSkill(player))
            {
                if(player.Team == ETeamKind.Npc)
                {
                    if (!GameController.Get.IsShooting || !player.IsAllShoot)
                    {
                        GameController.Get.AIAttack(player);
                        GameController.Get.AIMove(player, ref tactical);
                    }
                    //                    else if()
                    //                    {
                    //                        GameController.Get.aiAttack(ref player);
                    //                        GameController.Get.aiMove(player, ref tactical);
                    //                    }
                }
                else
                    GameController.Get.AIDefend(player);
            }
        }
    }

    public override void Exit()
    {
    }
}