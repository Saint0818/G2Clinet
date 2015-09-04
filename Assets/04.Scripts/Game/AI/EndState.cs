using AI;

public class EndState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.End; }
    }

    public override void Enter(object extraInfo)
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
    }
}