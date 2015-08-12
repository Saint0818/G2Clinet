using AI;

public class ShowOneState : State<EGameSituation>
{
    public override void EnterImpl(object extraInfo)
    {
        GameController.Get.SkipShow();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
