using AI;

public class NullState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.None; }
    }

    public override void EnterImpl(object extraInfo)
    {
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
