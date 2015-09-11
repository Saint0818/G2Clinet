using AI;

public class BPickBallAfterScoreState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.BPickBallAfterScore; }
    }

    public override void Enter(object extraInfo)
    {
        CourtMgr.Get.Walls[0].SetActive(false);
        UIGame.Get.ChangeControl(false);
        CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc, true);
//        pickBallPlayer = null;
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
}
