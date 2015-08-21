using AI;

public class CameraMovementState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.CameraMovement; }
    }

    public override void EnterImpl(object extraInfo)
    {
        CourtMgr.Get.ShowEnd(true);
        GameController.Get.InitIngameAnimator();
        GameController.Get.SetBornPositions();

        UISkip.UIShow(false, ESkipSituation.Game);
        CourtMgr.Get.ShowEnd();
        UIGame.UIShow(true);
        UIGame.Get.UIState(EUISituation.ShowTwo);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
