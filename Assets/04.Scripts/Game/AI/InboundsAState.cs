using AI;

public class InboundsAState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.InboundsA; }
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
