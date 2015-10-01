using AI;
using GamePlayEnum;
using JetBrains.Annotations;

public class JumpBallState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.JumpBall; }
    }

    public override void Enter(object extraInfo)
    {
        GameController.Get.IsStart = true;
        CourtMgr.Get.InitScoreboard(true);
//        GameController.Get.setPassIcon(true);

        PlayerBehaviour npc = findJumpBallPlayer(ETeamKind.Self);
        if(npc)
            npc.DoPassiveSkill(ESkillSituation.JumpBall, CourtMgr.Get.RealBall.transform.position);

        npc = findJumpBallPlayer(ETeamKind.Npc);
        if(npc)
            npc.DoPassiveSkill(ESkillSituation.JumpBall, CourtMgr.Get.RealBall.transform.position);
    }

    public override void Update()
    {
        if(GameController.Get.BallOwner == null)
        {
            for(int i = 0; i < GameController.Get.GamePlayers.Count; i++)
            {
                GameController.Get.DoPickBall(GameController.Get.GamePlayers[i]);
            }
        }
    }

    public override void Exit()
    {
    }

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
    }

    [CanBeNull]
    private PlayerBehaviour findJumpBallPlayer(ETeamKind team)
    {
        int findIndex = team == 0 ? 0 : 3;
        PlayerBehaviour npc = null;
        for(int i = 0; i < GameController.Get.GamePlayers.Count; i++)
        {
            if(GameController.Get.GamePlayers[i].gameObject.activeInHierarchy && 
               GameController.Get.GamePlayers[i].Team == team && 
               GameController.Get.GamePlayers[i].IsJumpBallPlayer)
            {
                findIndex = i;
            }
        }

        if(findIndex < GameController.Get.GamePlayers.Count)
            npc = GameController.Get.GamePlayers[findIndex];

        return npc;
    }
}