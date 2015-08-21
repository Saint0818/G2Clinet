using AI;

public class InboundsBState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.InboundsB; }
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
