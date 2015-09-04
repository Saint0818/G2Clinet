using AI;

public class JumpBallState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.JumpBall; }
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