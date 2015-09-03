using AI;

public class InitCourtState : State<EGameSituation>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.InitCourt; }
    }

    public override void Enter(object extraInfo)
    {
        if(GameController.Get.IsStart == false)
        {
            UIGame.Get.UIState(EUISituation.ShowTwo);

            GameController.Get.Situation = EGameSituation.Opening;
            GameController.Get.ChangeSituation(EGameSituation.Opening);
            Parent.ChangeState(EGameSituation.Opening);

            CourtMgr.Get.InitScoreboard(true);
        }
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}