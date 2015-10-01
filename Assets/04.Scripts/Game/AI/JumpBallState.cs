using AI;
using GamePlayEnum;
using JetBrains.Annotations;

/// <summary>
/// 球飛向誰的程式碼在 CourtMgr.SetBallState(EPlayerState.JumpBall).
/// </summary>
public class JumpBallState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.JumpBall; }
    }

    /// <summary>
    /// 這是要接球的球員.
    /// </summary>
    [CanBeNull]private PlayerBehaviour mReceiveBallPlayer;

    public override void Enter(object extraInfo)
    {
        GameController.Get.IsStart = true;
        CourtMgr.Get.InitScoreboard(true);
//        GameController.Get.setPassIcon(true);

        // 找出 2 位要跳球的球員.
        PlayerBehaviour npc = findJumpBallPlayer(ETeamKind.Self);
        if(npc)
            npc.DoPassiveSkill(ESkillSituation.JumpBall, CourtMgr.Get.RealBall.transform.position);

        npc = findJumpBallPlayer(ETeamKind.Npc);
        if(npc)
            npc.DoPassiveSkill(ESkillSituation.JumpBall, CourtMgr.Get.RealBall.transform.position);
    }

    public override void Update()
    {
        if(GameController.Get.BallOwner == null && mReceiveBallPlayer != null)
        {
            //            for(int i = 0; i < GameController.Get.GamePlayers.Count; i++)
            //            {
            //                GameController.Get.DoPickBall(GameController.Get.GamePlayers[i]);
            //            }

            GameController.Get.DoPickBall(mReceiveBallPlayer);
        }
    }

    public override void Exit()
    {
    }

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
        if(msg.Msg == EGameMsg.PlayerTouchBallWhenJumpBall)
        {
            PlayerBehaviour touchPlayer = (PlayerBehaviour)msg.ExtraInfo;
            mReceiveBallPlayer = randomReceiveBallPlayer(touchPlayer);

            // 要求籃球飛向 ReceiveBallPlayer.
            CourtMgr.Get.SetBallState(EPlayerState.JumpBall, mReceiveBallPlayer);
        }
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

    [CanBeNull]
    private PlayerBehaviour randomReceiveBallPlayer(PlayerBehaviour exceptPlayer)
    {
        var team = AIController.Get.GeTeam(exceptPlayer.Team);
        PlayerAI receivalBallPlayer = team.RandomSameTeamPlayer(exceptPlayer.GetComponent<PlayerAI>());

        if(receivalBallPlayer != null)
            return receivalBallPlayer.GetComponent<PlayerBehaviour>();
        return null;
    }
}