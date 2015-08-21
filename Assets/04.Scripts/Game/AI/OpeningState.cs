using AI;

public class OpeningState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.Opening; }
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