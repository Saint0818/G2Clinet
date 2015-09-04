using AI;

public class InboundsBState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.InboundsB; }
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

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
    }
}
