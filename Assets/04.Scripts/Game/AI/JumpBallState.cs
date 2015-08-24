using AI;

public class JumpBallState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.JumpBall; }
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