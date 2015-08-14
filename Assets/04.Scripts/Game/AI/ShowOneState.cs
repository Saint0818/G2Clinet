using AI;

public class ShowOneState : State<EGameSituation, EGameMsg>
{
    public override void EnterImpl(object extraInfo)
    {
        CourtMgr.Get.ShowEnd(true);
        GameController.Get.InitIngameAnimator();
        GameController.Get.SetBornPositions();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
