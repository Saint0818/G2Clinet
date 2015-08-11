using AI;

public class ShowOneState : State<EGameSituation>
{
    public override void EnterImpl()
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
