using AI;

public class OpeningState : State<EGameSituation>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.Opening; }
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