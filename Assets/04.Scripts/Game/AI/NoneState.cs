using AI;

public class NoneState : State<EGameSituation>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.None; }
    }

    public override void Enter(object extraInfo)
    {
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
